#!/usr/bin/env python3
"""
MQTT Sensor Data Simulator – Fish Farm Edition

Publishes realistic sensor readings every second for pins 1–5:
    pin1: temperature (°C)       – normal range 26.5–27.5
    pin2: pH                     – normal range 7.0–7.5
    pin3: TDS (ppm)              – normal range 300–400
    pin4: water flow (L/min)     – normal range 40–50
    pin5: water level (boolean)  – normal = 1 (always 1 for normal operation)

Values are randomly generated within these narrow bands each second to simulate
steady, healthy farm conditions. The script publishes to:
    topic = "iras-rag/telemetry/{mac}"

Values are randomly generated within these narrow bands each second to simulate
steady, healthy farm conditions. The script publishes to:
    topic = "iras-rag/telemetry/{mac}"

Usage: see --help
"""

import json
import random
import time
import signal
import sys
import argparse
from typing import List, Dict, Any

import paho.mqtt.client as mqtt

# Default configuration
DEFAULT_BROKER = "129.212.228.41"
DEFAULT_PORT = 1883
DEFAULT_QOS = 0
DEFAULT_INTERVAL = 2  # seconds
DEFAULT_MAC_FILE = "mac.txt"

# Use modern Callback API if available (paho-mqtt >=2.0)
try:
    MQTT_CALLBACK_VERSION = mqtt.CallbackAPIVersion.VERSION2
except AttributeError:
    MQTT_CALLBACK_VERSION = mqtt.CallbackAPIVersion.VERSION1  # fallback

# Normal ranges for each pin (min, max)
# For boolean pins, normal value is 1.
PIN_RANGES = {
    1: (28.5, 29.0),      # temperature (°C)
    2: (6.27, 6.44),        # pH
    3: (1681.0, 1749.0),    # TDS (ppm)
    4: (2.5, 2.7),      # water flow (L/min)
    5: (1, 1),            # water level (boolean) – always 1 (normal)
}


def load_macs_from_file(filepath: str) -> List[str]:
    """Read MAC addresses from a file, one per line (empty lines ignored)."""
    macs = []
    try:
        with open(filepath, 'r') as f:
            for line in f:
                mac = line.strip()
                if mac:
                    macs.append(mac)
    except FileNotFoundError:
        print(f"Error: MAC file '{filepath}' not found.")
        sys.exit(1)
    if not macs:
        print(f"Error: No MAC addresses found in '{filepath}'.")
        sys.exit(1)
    return macs


def generate_readings() -> List[Dict[str, Any]]:
    """Generate realistic normal sensor readings for pins 1-5."""
    readings = []
    for pin in range(1, 6):
        low, high = PIN_RANGES[pin]
        if pin == 5:                     # boolean (water level)
            # Always 1 for normal operation
            value = 1
        else:
            # Generate a random float within the normal range, rounded to 2 decimals
            value = round(random.uniform(low, high), 2)
        readings.append({"pin": pin, "val": value})
    return readings


def create_payload(mac: str) -> str:
    """Create JSON payload for the given MAC address."""
    payload = {"mac": mac, "readings": generate_readings()}
    return json.dumps(payload)


def on_connect(client, userdata, flags, reason_code, properties=None):
    """Callback when connected to MQTT broker."""
    rc = reason_code.value if hasattr(reason_code, 'value') else reason_code
    if rc == 0:
        print(f"Connected to MQTT broker at {userdata['broker']}:{userdata['port']}")
    elif rc == 5:
        print("Connection refused – not authorised. Did you forget -u / -P ?")
        sys.exit(1)
    else:
        print(f"Failed to connect, return code {rc}")
        sys.exit(1)


def on_publish(client, userdata, mid, reason_code=None, properties=None):
    """Callback when a message is published."""
    pass


def signal_handler(sig, frame):
    """Handle Ctrl+C to gracefully exit."""
    print("\nStopping sensor simulator...")
    client.loop_stop()
    client.disconnect()
    sys.exit(0)


def parse_arguments():
    """Parse command line arguments."""
    parser = argparse.ArgumentParser(
        description="Simulate fish farm sensor data and publish to MQTT."
    )
    parser.add_argument("-b", "--broker", default=DEFAULT_BROKER)
    parser.add_argument("-p", "--port", type=int, default=DEFAULT_PORT)
    parser.add_argument("-i", "--interval", type=float, default=DEFAULT_INTERVAL)
    parser.add_argument("-u", "--username", default=None, help="MQTT username")
    parser.add_argument("-P", "--password", default=None, help="MQTT password")
    parser.add_argument("-q", "--qos", type=int, default=DEFAULT_QOS, choices=[0,1,2])
    parser.add_argument("-m", "--mac", default=None,
                        help="Single MAC address. If not given, MACs are read from mac.txt")
    parser.add_argument("--mac-file", default=DEFAULT_MAC_FILE,
                        help=f"File with MAC addresses, one per line (default: {DEFAULT_MAC_FILE})")
    return parser.parse_args()


if __name__ == "__main__":
    args = parse_arguments()

    # Determine MAC source
    if args.mac:
        mac_source = "fixed"
        mac = args.mac
        print(f"Using fixed MAC: {mac}")
    else:
        mac_source = "file"
        mac_list = load_macs_from_file(args.mac_file)
        print(f"Loaded {len(mac_list)} MAC(s) from {args.mac_file}")

    # Set up MQTT client
    client = mqtt.Client(callback_api_version=MQTT_CALLBACK_VERSION)
    client.user_data_set({"broker": args.broker, "port": args.port})
    client.on_connect = on_connect
    client.on_publish = on_publish

    if args.username and args.password:
        client.username_pw_set(args.username, args.password)

    # Connect to broker
    try:
        client.connect(args.broker, args.port, keepalive=60)
    except Exception as e:
        print(f"Could not connect to broker: {e}")
        sys.exit(1)

    client.loop_start()
    signal.signal(signal.SIGINT, signal_handler)

    print(f"Publishing to dynamic topics: iras-rag/telemetry/<mac>")
    print(f"Interval: {args.interval} seconds")
    print("Press Ctrl+C to stop.\n")

    try:
        while True:
            current_mac = mac if mac_source == "fixed" else random.choice(mac_list)
            topic = f"iras-rag/telemetry/{current_mac}"
            payload = create_payload(current_mac)
            result = client.publish(topic, payload, qos=args.qos)
            if result.rc != mqtt.MQTT_ERR_SUCCESS:
                print(f"Failed to publish: {mqtt.error_string(result.rc)}")
            else:
                print(f"Published to {topic}: {payload}")
            time.sleep(args.interval)
    except KeyboardInterrupt:
        signal_handler(None, None)
