using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTrackingToManualLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_mortality_logs_batch_id",
                table: "mortality_logs");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "mortality_logs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "feeding_logs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001601"),
                column: "user_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000003"));

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001602"),
                column: "user_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000003"));

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001603"),
                column: "user_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000003"));

            migrationBuilder.UpdateData(
                table: "mortality_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001701"),
                column: "user_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000003"));

            migrationBuilder.UpdateData(
                table: "mortality_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001702"),
                column: "user_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000003"));

            migrationBuilder.CreateIndex(
                name: "ix_mortality_logs_batch_id_date",
                table: "mortality_logs",
                columns: new[] { "batch_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_mortality_logs_user_id",
                table: "mortality_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_feeding_logs_user_id",
                table: "feeding_logs",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_feeding_logs_users_user_id",
                table: "feeding_logs",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_mortality_logs_users_user_id",
                table: "mortality_logs",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_feeding_logs_users_user_id",
                table: "feeding_logs");

            migrationBuilder.DropForeignKey(
                name: "fk_mortality_logs_users_user_id",
                table: "mortality_logs");

            migrationBuilder.DropIndex(
                name: "ix_mortality_logs_batch_id_date",
                table: "mortality_logs");

            migrationBuilder.DropIndex(
                name: "ix_mortality_logs_user_id",
                table: "mortality_logs");

            migrationBuilder.DropIndex(
                name: "ix_feeding_logs_user_id",
                table: "feeding_logs");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "mortality_logs");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "feeding_logs");

            migrationBuilder.CreateIndex(
                name: "ix_mortality_logs_batch_id",
                table: "mortality_logs",
                column: "batch_id");
        }
    }
}
