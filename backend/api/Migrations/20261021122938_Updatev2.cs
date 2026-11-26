using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Updatev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_size_byte",
                table: "files");

            migrationBuilder.AddColumn<long>(
                name: "content_length",
                table: "files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "files",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_files_user_id",
                table: "files",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_files_users_user_id",
                table: "files",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_files_users_user_id",
                table: "files");

            migrationBuilder.DropIndex(
                name: "ix_files_user_id",
                table: "files");

            migrationBuilder.DropColumn(
                name: "content_length",
                table: "files");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "files");

            migrationBuilder.AddColumn<decimal>(
                name: "file_size_byte",
                table: "files",
                type: "numeric",
                nullable: true);
        }
    }
}
