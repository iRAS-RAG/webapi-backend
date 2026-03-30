using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedTypeReferenceToFeedingLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_feeding_logs_farming_batch_id_created_date",
                table: "feeding_logs"
            );

            migrationBuilder.AddColumn<Guid>(
                name: "feed_type_id",
                table: "feeding_logs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001601"),
                column: "feed_type_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000201")
            );

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001602"),
                column: "feed_type_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000201")
            );

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001603"),
                column: "feed_type_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000202")
            );

            migrationBuilder.CreateIndex(
                name: "ix_feeding_logs_farming_batch_id_feed_type_id_created_date",
                table: "feeding_logs",
                columns: new[] { "farming_batch_id", "feed_type_id", "created_date" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_feeding_logs_feed_type_id",
                table: "feeding_logs",
                column: "feed_type_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_feeding_logs_feed_types_feed_type_id",
                table: "feeding_logs",
                column: "feed_type_id",
                principalTable: "feed_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_feeding_logs_feed_types_feed_type_id",
                table: "feeding_logs"
            );

            migrationBuilder.DropIndex(
                name: "ix_feeding_logs_farming_batch_id_feed_type_id_created_date",
                table: "feeding_logs"
            );

            migrationBuilder.DropIndex(name: "ix_feeding_logs_feed_type_id", table: "feeding_logs");

            migrationBuilder.DropColumn(name: "feed_type_id", table: "feeding_logs");

            migrationBuilder.CreateIndex(
                name: "ix_feeding_logs_farming_batch_id_created_date",
                table: "feeding_logs",
                columns: new[] { "farming_batch_id", "created_date" }
            );
        }
    }
}
