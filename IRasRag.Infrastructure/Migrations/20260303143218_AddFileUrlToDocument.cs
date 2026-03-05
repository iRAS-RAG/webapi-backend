using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFileUrlToDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "file_url",
                table: "documents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.UpdateData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001901"),
                column: "file_url",
                value: "https://res.cloudinary.com/seed/documents/doc1-huong-dan-xu-ly-nhiet-do-cao.pdf"
            );

            migrationBuilder.UpdateData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001902"),
                column: "file_url",
                value: "https://res.cloudinary.com/seed/documents/doc2-quy-trinh-dieu-chinh-do-ph.pdf"
            );

            migrationBuilder.UpdateData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001903"),
                column: "file_url",
                value: "https://res.cloudinary.com/seed/documents/doc3-quan-ly-chat-luong-nuoc.pdf"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "file_url", table: "documents");
        }
    }
}
