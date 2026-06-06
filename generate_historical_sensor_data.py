#!/usr/bin/env python3
"""
Generate historical sensor_log data for the past month and output an SQL script.

This script mirrors the data that the MQTT sensor simulator would have produced:
  - Reads PIN_RANGES directly from mqtt_sensor_simulator.py (same directory)
  - Uses the same aggregation logic as TelemetryLogBatchWriter (1-minute windows, ~30 samples each)
  - Outputs INSERT statements for the sensor_logs table

Usage:
    python generate_historical_sensor_data.py > load_1month.sql
    python generate_historical_sensor_data.py --days 7 --output last_week.sql
    python generate_historical_sensor_data.py --start 2026-05-01 --end 2026-06-01 --output may.sql
    python generate_historical_sensor_data.py --macs "68:FE:71:16:A5:18,AA:BB:CC:DD:EE:FF"
"""

import argparse
import ast
import datetime
import os
import random
import re
import sys
import uuid
from typing import Dict, List, Tuple

# ---------------------------------------------------------------------------
# Pin-to-sensor mapping (mirrors SensorSeed.cs).
# These are database-specific — they don't change with simulator ranges.
# ---------------------------------------------------------------------------
SENSOR_UUIDS: Dict[int, str] = {
    1: "aaaaaaaa-0000-0000-0000-000000001301",  # Temperature
    2: "aaaaaaaa-0000-0000-0000-000000001302",  # pH
    3: "aaaaaaaa-0000-0000-0000-000000001303",  # TDS
    4: "aaaaaaaa-0000-0000-0000-000000001304",  # Water Flow
    5: "aaaaaaaa-0000-0000-0000-000000001305",  # Water Level
}

# Default MAC addresses (the one from seed data, plus you can add more).
DEFAULT_MACS = ["68:FE:71:16:A5:18"]

# How many raw readings go into each 1-minute aggregation window.
# MQTT simulator publishes ~every 2 seconds → ~30 samples per minute.
SAMPLES_PER_WINDOW = 30


def find_simulator_file() -> str:
    """Find mqtt_sensor_simulator.py in this script's directory."""
    script_dir = os.path.dirname(os.path.abspath(__file__))
    candidate = os.path.join(script_dir, "mqtt_sensor_simulator.py")
    if not os.path.isfile(candidate):
        print(
            f"Error: mqtt_sensor_simulator.py not found at {candidate}",
            file=sys.stderr,
        )
        sys.exit(1)
    return candidate


def load_pin_ranges(simulator_path: str) -> Dict[int, Tuple[float, float]]:
    """Read PIN_RANGES from the MQTT simulator file using AST parsing.

    This safely extracts the dictionary literal without executing the file.
    """
    with open(simulator_path, "r", encoding="utf-8") as f:
        source = f.read()

    # Locate the PIN_RANGES assignment block.
    # Pattern: PIN_RANGES = { 1: (low, high), ... }
    match = re.search(
        r"PIN_RANGES\s*=\s*(\{.*?\})\s*\n(?!\s*(?:#|$))",
        source,
        re.DOTALL,
    )
    if not match:
        # Fallback: try a simpler end-of-block heuristic — look for the dict
        # followed by a blank line or a top-level definition.
        match = re.search(
            r"PIN_RANGES\s*=\s*(\{.*?\})\s*\n(?=\n|def |class |\S)", source, re.DOTALL
        )

    if not match:
        print(
            "Error: could not locate PIN_RANGES in mqtt_sensor_simulator.py",
            file=sys.stderr,
        )
        sys.exit(1)

    dict_literal = match.group(1)

    try:
        parsed = ast.literal_eval(dict_literal)
    except (ValueError, SyntaxError) as e:
        print(f"Error parsing PIN_RANGES: {e}", file=sys.stderr)
        sys.exit(1)

    # Validate shape: dict[int -> tuple[float, float]]
    for pin, pair in parsed.items():
        if not isinstance(pair, (list, tuple)) or len(pair) != 2:
            print(
                f"Error: pin {pin} range {pair!r} is not a 2-tuple",
                file=sys.stderr,
            )
            sys.exit(1)

    # Normalise all values to float
    result: Dict[int, Tuple[float, float]] = {
        int(pin): (float(pair[0]), float(pair[1])) for pin, pair in parsed.items()
    }

    # Determine which pins are boolean (low == high == 1).
    # We detect this heuristically; pins whose full range is (1, 1) are
    # treated as always-1 (no randomness).
    print(
        f"  Loaded {len(result)} pin ranges from {os.path.basename(simulator_path)}",
        file=sys.stderr,
    )
    for pin, (lo, hi) in sorted(result.items()):
        tag = "  (boolean)" if lo == hi == 1.0 else ""
        print(f"    Pin {pin}: {lo} – {hi}{tag}", file=sys.stderr)

    return result


def generate_window_values(
    pin: int,
    pin_ranges: Dict[int, Tuple[float, float]],
    samples: int = SAMPLES_PER_WINDOW,
) -> Tuple[float, float, float]:
    """Generate `samples` random readings within the pin's normal range
    and return (average, min, max)."""
    lo, hi = pin_ranges[pin]

    if lo == hi:
        # Constant / boolean sensor — no randomness needed
        vals = [lo] * samples
    else:
        vals = [round(random.uniform(lo, hi), 2) for _ in range(samples)]

    return (
        round(sum(vals) / len(vals), 2),
        min(vals),
        max(vals),
    )


def make_sql_inserts(
    sensor_id: str,
    pin: int,
    period_start: datetime.datetime,
    pin_ranges: Dict[int, Tuple[float, float]],
    mac_index: int = 0,
) -> str:
    """Generate one INSERT row for a single sensor × single 1-minute window.

    The row UUID is deterministic so re-running with the same seed produces
    the same result — the MAC index disambiguates across multiple boards.
    """
    avg, mn, mx = generate_window_values(pin, pin_ranges)

    ts_str = period_start.strftime("%Y-%m-%d %H:%M:%S+00")
    now_str = period_start.strftime("%Y-%m-%d %H:%M:%S+00")

    # Deterministic row UUID for idempotency
    row_seed = f"{sensor_id}-{ts_str}-mac{mac_index}"
    row_uuid = uuid.uuid5(uuid.NAMESPACE_DNS, row_seed)
    has_warning = "false"

    return (
        f"    ('{row_uuid}', '{sensor_id}', '{ts_str}', "
        f"{avg}, {mn}, {mx}, {SAMPLES_PER_WINDOW}, {has_warning}, "
        f"'{now_str}', '{now_str}')"
    )


def generate_sql(
    start: datetime.datetime,
    end: datetime.datetime,
    mac_addresses: List[str],
    pin_ranges: Dict[int, Tuple[float, float]],
) -> str:
    """Generate the full SQL script for the given time range and MACs."""
    lines: List[str] = []
    lines.append("-- ============================================================")
    lines.append(f"-- Historical sensor_log data generated by {sys.argv[0]}")
    lines.append(f"-- Period: {start.isoformat()}  →  {end.isoformat()}")
    lines.append(f"-- MACs:   {', '.join(mac_addresses)}")
    lines.append(f"-- Sensors: {len(SENSOR_UUIDS)} per board")
    lines.append("-- ============================================================")
    lines.append("")
    lines.append("-- Wrap in a transaction for speed and atomicity")
    lines.append("BEGIN;")
    lines.append("")

    batch_size = 500
    rows: List[str] = []

    total_minutes = int((end - start).total_seconds() // 60)
    current = start
    progress_interval = max(1, total_minutes // 100)  # ~1% increments

    for minute_idx in range(total_minutes):
        period = current

        if minute_idx % progress_interval == 0:
            pct = (minute_idx / total_minutes) * 100
            print(f"  Progress: {pct:.0f}%  ({period.isoformat()})", file=sys.stderr)

        for mac_idx, mac in enumerate(mac_addresses):
            for pin in sorted(pin_ranges):
                sensor_id = SENSOR_UUIDS.get(pin)
                if sensor_id is None:
                    continue
                row = make_sql_inserts(sensor_id, pin, period, pin_ranges, mac_idx)
                rows.append(row)

                if len(rows) >= batch_size:
                    lines.append("INSERT INTO sensor_logs (")
                    lines.append("    id, sensor_id, period_start,")
                    lines.append("    average, min, max, sample_count, has_warning,")
                    lines.append("    created_at, modified_at")
                    lines.append(") VALUES")
                    lines.append(",\n".join(rows))
                    lines.append(";")
                    lines.append("")
                    rows = []

        current += datetime.timedelta(minutes=1)

    # Flush remaining rows
    if rows:
        lines.append("INSERT INTO sensor_logs (")
        lines.append("    id, sensor_id, period_start,")
        lines.append("    average, min, max, sample_count, has_warning,")
        lines.append("    created_at, modified_at")
        lines.append(") VALUES")
        lines.append(",\n".join(rows))
        lines.append(";")
        lines.append("")

    lines.append("COMMIT;")
    lines.append("")
    lines.append("-- Done.")
    return "\n".join(lines)


def parse_args() -> argparse.Namespace:
    """Parse command-line arguments."""
    parser = argparse.ArgumentParser(
        description=(
            "Generate an SQL script with historical sensor_log data for an "
            "aquatic fish farm. PIN_RANGES are read from mqtt_sensor_simulator.py "
            "in the same directory."
        )
    )
    parser.add_argument(
        "--days",
        type=int,
        default=None,
        help="Number of days of history to generate (default: 30). "
        "Ignored if --start/--end are given.",
    )
    parser.add_argument(
        "--start",
        type=str,
        default=None,
        help="Start date (YYYY-MM-DD or ISO format). Default: 30 days ago.",
    )
    parser.add_argument(
        "--end",
        type=str,
        default=None,
        help="End date (YYYY-MM-DD or ISO format). Default: now.",
    )
    parser.add_argument(
        "--macs",
        type=str,
        default=None,
        help="Comma-separated MAC addresses. Default: 68:FE:71:16:A5:18",
    )
    parser.add_argument(
        "--simulator",
        type=str,
        default=None,
        help="Path to mqtt_sensor_simulator.py (default: same directory).",
    )
    parser.add_argument(
        "--output",
        "-o",
        type=str,
        default=None,
        help="Output file path (default: stdout).",
    )
    parser.add_argument(
        "--seed",
        type=int,
        default=None,
        help="Random seed for reproducibility.",
    )
    return parser.parse_args()


def main() -> None:
    args = parse_args()

    if args.seed is not None:
        random.seed(args.seed)

    # ---- Load pin ranges from the MQTT simulator file ----
    simulator_path = args.simulator if args.simulator else find_simulator_file()
    print(f"Reading ranges from: {simulator_path}", file=sys.stderr)
    pin_ranges = load_pin_ranges(simulator_path)

    # Validate that all loaded pins have a sensor UUID mapping
    for pin in pin_ranges:
        if pin not in SENSOR_UUIDS:
            print(
                f"Warning: pin {pin} has no sensor UUID mapping, skipping",
                file=sys.stderr,
            )

    # ---- Determine time range ----
    now = datetime.datetime.now(datetime.timezone.utc).replace(second=0, microsecond=0)

    if args.start:
        start = datetime.datetime.fromisoformat(args.start)
        if start.tzinfo is None:
            start = start.replace(tzinfo=datetime.timezone.utc)
    else:
        days = args.days if args.days is not None else 30
        start = now - datetime.timedelta(days=days)

    if args.end:
        end = datetime.datetime.fromisoformat(args.end)
        if end.tzinfo is None:
            end = end.replace(tzinfo=datetime.timezone.utc)
    else:
        end = now

    start = start.replace(second=0, microsecond=0)
    end = end.replace(second=0, microsecond=0)

    if end <= start:
        print("Error: --end must be after --start.", file=sys.stderr)
        sys.exit(1)

    # ---- Determine MACs ----
    if args.macs:
        macs = [m.strip() for m in args.macs.split(",") if m.strip()]
    else:
        macs = DEFAULT_MACS

    # ---- Overview ----
    total_windows = int((end - start).total_seconds() // 60)
    total_rows = total_windows * len(macs) * 8
    total_mb_est = total_rows * 0.00025

    print(
        f"Generating {total_rows:,} sensor_log rows "
        f"({total_windows:,} minutes × {len(macs)} board(s) × 8 sensors)",
        file=sys.stderr,
    )
    print(f"  Estimated SQL size: ~{total_mb_est:.0f} MB", file=sys.stderr)
    print(f"  Time range: {start.isoformat()}  →  {end.isoformat()}", file=sys.stderr)
    print(f"  MACs: {', '.join(macs)}", file=sys.stderr)
    print("", file=sys.stderr)

    sql = generate_sql(start, end, macs, pin_ranges)

    if args.output:
        with open(args.output, "w", encoding="utf-8") as f:
            f.write(sql)
        print(f"\nWritten to {args.output}", file=sys.stderr)
    else:
        sys.stdout.write(sql)


if __name__ == "__main__":
    main()
