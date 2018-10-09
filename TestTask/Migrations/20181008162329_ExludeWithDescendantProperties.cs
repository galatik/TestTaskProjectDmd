using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestTask.Migrations
{
    public partial class ExludeWithDescendantProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionTimeWithDescendant",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PlannedLaboriousnessWithDescendant",
                table: "Tasks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "CompletionTimeWithDescendant",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlannedLaboriousnessWithDescendant",
                table: "Tasks",
                nullable: true);
        }
    }
}
