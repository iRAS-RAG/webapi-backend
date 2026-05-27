using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class DocumentSeed
    {
        private static readonly DateTime SeedTimestamp = new(
            2024,
            01,
            01,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        public static readonly Guid Doc1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001901");

        public static readonly Guid Doc2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001902");

        public static readonly Guid Doc3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001903");

        public static List<Document> Documents =>
            [
                new Document
                {
                    Id = Doc1Id,
                    Title = "Kĩ thuật nuôi cua biển",
                    FileUrl =
                        "https://res.cloudinary.com/dhelnz7sw/raw/upload/v1779455970/IRAS-RAG/documents/KT_nu%C3%B4i_cua_bi%E1%BB%83n_lcsego.pdf",
                    UploadedByUserId = UserSeed.AdminId,
                    UploadedAt = new DateTime(2023, 12, 15, 10, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                    RagStatus = DocumentRagStatus.Pending,
                },
                new Document
                {
                    Id = Doc2Id,
                    Title = "Cẩm nang nuôi cá nước tĩnh Mè Trôi Trắm Chép",
                    FileUrl =
                        "https://res.cloudinary.com/dhelnz7sw/raw/upload/v1779455830/IRAS-RAG/documents/C%E1%BA%A9m_nang_nu%C3%B4i_c%C3%A1_n%C6%B0%E1%BB%9Bc_t%C4%A9nh_M%C3%A8_Tr%C3%B4i_Tr%E1%BA%AFm_Ch%C3%A9p_lfmxrd.pdf",
                    UploadedByUserId = UserSeed.SupervisorId,
                    UploadedAt = new DateTime(2023, 12, 20, 14, 30, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                    RagStatus = DocumentRagStatus.Pending,
                },
                new Document
                {
                    Id = Doc3Id,
                    Title = "Kĩ thuật nuôi cá rô",
                    FileUrl =
                        "https://res.cloudinary.com/dhelnz7sw/raw/upload/v1778650465/IRAS-RAG/documents/KT_nu%C3%B4i_c%C3%A1_r%C3%B4_siwemh.pdf",
                    UploadedByUserId = UserSeed.AdminId,
                    UploadedAt = new DateTime(2024, 01, 10, 9, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                    RagStatus = DocumentRagStatus.Pending,
                },
            ];
    }
}
