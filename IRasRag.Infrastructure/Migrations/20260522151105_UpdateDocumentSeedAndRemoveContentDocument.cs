using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDocumentSeedAndRemoveContentDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "content", table: "documents");

            migrationBuilder.UpdateData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001901"),
                columns: new[] { "file_url", "title" },
                values: new object[]
                {
                    "https://res.cloudinary.com/dhelnz7sw/raw/upload/v1779455970/IRAS-RAG/documents/KT_nu%C3%B4i_cua_bi%E1%BB%83n_lcsego.pdf",
                    "Kĩ thuật nuôi cua biển",
                }
            );

            migrationBuilder.UpdateData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001902"),
                columns: new[] { "file_url", "title" },
                values: new object[]
                {
                    "https://res.cloudinary.com/dhelnz7sw/raw/upload/v1779455830/IRAS-RAG/documents/C%E1%BA%A9m_nang_nu%C3%B4i_c%C3%A1_n%C6%B0%E1%BB%9Bc_t%C4%A9nh_M%C3%A8_Tr%C3%B4i_Tr%E1%BA%AFm_Ch%C3%A9p_lfmxrd.pdf",
                    "Cẩm nang nuôi cá nước tĩnh Mè Trôi Trắm Chép",
                }
            );

            migrationBuilder.UpdateData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001903"),
                columns: new[] { "file_url", "title" },
                values: new object[]
                {
                    "https://res.cloudinary.com/dhelnz7sw/raw/upload/v1778650465/IRAS-RAG/documents/KT_nu%C3%B4i_c%C3%A1_r%C3%B4_siwemh.pdf",
                    "Kĩ thuật nuôi cá rô",
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "documents",
                type: "text",
                nullable: true
            );

            migrationBuilder.UpdateData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001901"),
                columns: new[] { "content", "file_url", "title" },
                values: new object[]
                {
                    "Khi nhiệt độ nước vượt quá 30°C:\n1. Tăng lưu lượng nước tuần hoàn\n2. Bật hệ thống làm mát\n3. Giảm mật độ thả nuôi nếu cần\n4. Kiểm tra hàm lượng oxy hòa tan\n5. Theo dõi hành vi của cá",
                    "https://res.cloudinary.com/seed/documents/doc1-huong-dan-xu-ly-nhiet-do-cao.pdf",
                    "Hướng dẫn xử lý nhiệt độ cao trong bể nuôi",
                }
            );

            migrationBuilder.UpdateData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001902"),
                columns: new[] { "content", "file_url", "title" },
                values: new object[]
                {
                    "Để duy trì độ pH ổn định:\n1. Kiểm tra độ kiềm của nước\n2. Sử dụng vôi nông nghiệp để tăng pH\n3. Sử dụng axit citric để giảm pH\n4. Theo dõi pH hàng ngày\n5. Đảm bảo hệ thống lọc sinh học hoạt động tốt",
                    "https://res.cloudinary.com/seed/documents/doc2-quy-trinh-dieu-chinh-do-ph.pdf",
                    "Quy trình điều chỉnh độ pH trong hệ thống RAS",
                }
            );

            migrationBuilder.UpdateData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001903"),
                columns: new[] { "content", "file_url", "title" },
                values: new object[]
                {
                    "Các thông số quan trọng cần theo dõi:\n1. Nhiệt độ: 25-30°C\n2. pH: 6.5-8.5\n3. Oxy hòa tan: >5 mg/L\n4. Ammonia: <0.1 mg/L\n5. Nitrite: <0.2 mg/L\n6. Nitrate: <50 mg/L\nThực hiện kiểm tra hàng ngày và ghi chép đầy đủ.",
                    "https://res.cloudinary.com/seed/documents/doc3-quan-ly-chat-luong-nuoc.pdf",
                    "Hướng dẫn quản lý chất lượng nước trong nuôi trồng thủy sản",
                }
            );
        }
    }
}
