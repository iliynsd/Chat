using Microsoft.EntityFrameworkCore.Migrations;

namespace Chat.Migrations
{
    public partial class init04 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ChatActions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ChatActions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
