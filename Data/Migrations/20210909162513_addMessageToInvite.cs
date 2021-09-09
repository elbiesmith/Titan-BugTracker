using Microsoft.EntityFrameworkCore.Migrations;

namespace Titan_BugTracker.Data.Migrations
{
    public partial class addMessageToInvite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Invites",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "Invites");
        }
    }
}
