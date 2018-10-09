using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestTask.Migrations
{
    public partial class AddedDefaultValuesAndNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Tasks",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "PlannedLaboriousnessWithDescendant",
                table: "Tasks",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "CompletionTimeWithDescendant",
                table: "Tasks",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActualCompletionDate",
                table: "Tasks",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Tasks",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "PlannedLaboriousnessWithDescendant",
                table: "Tasks",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "CompletionTimeWithDescendant",
                table: "Tasks",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActualCompletionDate",
                table: "Tasks",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
