using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class OptimisedClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsUnread",
                table: "Notifications",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PictureId",
                table: "Likes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_PictureId",
                table: "Likes",
                column: "PictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Pictures_PictureId",
                table: "Likes",
                column: "PictureId",
                principalTable: "Pictures",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Pictures_PictureId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_PictureId",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "Likes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnread",
                table: "Notifications",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }
    }
}
