using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class GrowthStageSeed
    {
        // Stage 1: Fry (hatch → 21 days, 0.5–2 g)
        public static readonly Guid FryStageId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000401");

        // Stage 2: Fingerling (22 → 50 days, 2–20 g)
        public static readonly Guid FingerlingStageId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000403"
        );

        // Stage 3: Juvenile (51 → 110 days, 20–150 g)
        // GUID preserved from the original seed for backward compatibility.
        public static readonly Guid JuvenileStageId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000402"
        );

        // Stage 4: Grow-out (111 → harvest ~200 days, 150–500+ g)
        public static readonly Guid GrowOutStageId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000404"
        );

        // ── Mud crab stages ───────────────────────────────────────────

        // Stage 1: Crab megalopa/fry (settlement → 20 days, 0.02–0.5 g)
        public static readonly Guid CrabFryStageId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000405"
        );

        // Stage 2: Crab juvenile (21 → 60 days, 0.5–50 g)
        public static readonly Guid CrabJuvenileStageId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000406"
        );

        // Stage 3: Crab grow-out (61 → ~150 days, 50–350 g)
        public static readonly Guid CrabGrowOutStageId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000407"
        );

        // ── Reef squid stages ─────────────────────────────────────────

        // Stage 1: Paralarva (hatch → 30 days, 0.001–0.5 g)
        public static readonly Guid SquidParalarvaStageId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000408"
        );

        // Stage 2: Juvenile (31 → 90 days, 0.5–50 g)
        public static readonly Guid SquidJuvenileStageId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000409"
        );

        // Stage 3: Sub-adult / Grow-out (91 → ~180 days, 50–400 g)
        public static readonly Guid SquidGrowOutStageId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000410"
        );

        public static List<GrowthStage> GrowthStages =>
            [
                new GrowthStage
                {
                    Id = FryStageId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    Name = "Cá bột",
                    Description =
                        "Giai đoạn từ khi nở đến 21 ngày tuổi. Cá có kích thước nhỏ (0.5–2 g), "
                        + "cần thức ăn giàu đạm (≥40%) dạng bột mịn, cho ăn 6–8 lần/ngày.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                new GrowthStage
                {
                    Id = FingerlingStageId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    Name = "Cá hương",
                    Description =
                        "Giai đoạn từ 22 đến 50 ngày tuổi. Cá đạt 2–20 g, "
                        + "chuyển sang thức ăn viên nhỏ (35–40% đạm), cho ăn 4–6 lần/ngày.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                new GrowthStage
                {
                    Id = JuvenileStageId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    Name = "Cá giống",
                    Description =
                        "Giai đoạn từ 51 đến 110 ngày tuổi. Cá đạt 20–150 g, "
                        + "thức ăn viên vừa (30–35% đạm), cho ăn 3–4 lần/ngày.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                new GrowthStage
                {
                    Id = GrowOutStageId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    Name = "Cá thương phẩm",
                    Description =
                        "Giai đoạn từ 111 ngày tuổi đến khi thu hoạch (~200 ngày). Cá đạt 150–500+ g, "
                        + "thức ăn viên lớn (28–32% đạm), cho ăn 2–3 lần/ngày.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                // ── Crab: Fry ──────────────────────────────────────────
                new GrowthStage
                {
                    Id = CrabFryStageId,
                    SpeciesId = SpeciesSeed.MudCrabId,
                    Name = "Cua bột",
                    Description =
                        "Giai đoạn từ khi ấu trùng megalopa bám vào giá thể đến 20 ngày tuổi. "
                        + "Cua đạt 0.02–0.5 g, cần thức ăn giàu đạm (≥48%) dạng bột mịn, "
                        + "cho ăn 5–6 lần/ngày. Tỷ lệ sống thấp do tập tính ăn thịt lẫn nhau.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                // ── Crab: Juvenile ─────────────────────────────────────
                new GrowthStage
                {
                    Id = CrabJuvenileStageId,
                    SpeciesId = SpeciesSeed.MudCrabId,
                    Name = "Cua giống",
                    Description =
                        "Giai đoạn từ 21 đến 60 ngày tuổi. Cua đạt 0.5–50 g, "
                        + "thức ăn viên nhỏ (40–45% đạm), cho ăn 3–4 lần/ngày. "
                        + "Cần giá thể (lưới, ống nhựa) để giảm tỷ lệ ăn thịt đồng loại.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                // ── Crab: Grow-out ─────────────────────────────────────
                new GrowthStage
                {
                    Id = CrabGrowOutStageId,
                    SpeciesId = SpeciesSeed.MudCrabId,
                    Name = "Cua thịt",
                    Description =
                        "Giai đoạn từ 61 ngày tuổi đến khi thu hoạch (~150 ngày). Cua đạt 50–350+ g, "
                        + "thức ăn viên vừa (38–42% đạm) kết hợp cá tạp tươi, cho ăn 2–3 lần/ngày. "
                        + "Nuôi trong lồng riêng hoặc ao có giá thể để tránh đánh nhau.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                // ── Squid: Paralarva ──────────────────────────────────
                new GrowthStage
                {
                    Id = SquidParalarvaStageId,
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    Name = "Mực ấu trùng",
                    Description =
                        "Giai đoạn từ khi nở đến 30 ngày tuổi. Mực đạt 0.001–0.5 g, "
                        + "sống trôi nổi (planktonic), cần thức ăn sống (Artemia, mysid) "
                        + "6–8 lần/ngày. Rất nhạy cảm với ánh sáng và chất lượng nước. "
                        + "Bể nuôi phải dạng tròn, không góc cạnh để tránh mực va đập.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                // ── Squid: Juvenile ────────────────────────────────────
                new GrowthStage
                {
                    Id = SquidJuvenileStageId,
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    Name = "Mực non",
                    Description =
                        "Giai đoạn từ 31 đến 90 ngày tuổi. Mực đạt 0.5–50 g, "
                        + "bắt đầu bám đáy, chuyển dần từ thức ăn sống sang thức ăn chết "
                        + "(tôm/cá băm nhỏ), cho ăn 4–5 lần/ngày. "
                        + "Tỷ lệ sống phụ thuộc vào chất lượng thức ăn chuyển đổi.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                // ── Squid: Sub-adult / Grow-out ───────────────────────
                new GrowthStage
                {
                    Id = SquidGrowOutStageId,
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    Name = "Mực thương phẩm",
                    Description =
                        "Giai đoạn từ 91 ngày tuổi đến khi thu hoạch (~180 ngày). Mực đạt 50–400 g, "
                        + "ăn cá/tôm cắt miếng, cho ăn 2–3 lần/ngày. "
                        + "Mực trưởng thành có tập tính săn mồi mạnh, cần cho ăn đầy đủ "
                        + "để tránh ăn thịt lẫn nhau.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
            ];
    }
}
