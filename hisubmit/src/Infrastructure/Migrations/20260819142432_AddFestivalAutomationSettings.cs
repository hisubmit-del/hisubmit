using Microsoft.EntityFrameworkCore.Migrations;
using HiSubmit.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace HiSubmit.Infrastructure.Migrations;

/// <summary>
/// Adds opt-in controls for festival automation.
/// This migration intentionally changes only the Festivals table.
/// </summary>
[Migration("20260819142432_AddFestivalAutomationSettings")]
[DbContext(typeof(BlazorHeroContext))]
public partial class AddFestivalAutomationSettings : Migration
{
    // This migration is intentionally kept small because the existing model
    // snapshot contains historical schema drift unrelated to these settings.
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "EnableAutomaticPeriodCreation",
            table: "Festivals",
            schema: "hisubmi1_user",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "EnableAutomaticSelectionNews",
            table: "Festivals",
            schema: "hisubmi1_user",
            type: "bit",
            nullable: false,
            defaultValue: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "EnableAutomaticPeriodCreation",
            table: "Festivals",
            schema: "hisubmi1_user");

        migrationBuilder.DropColumn(
            name: "EnableAutomaticSelectionNews",
            table: "Festivals",
            schema: "hisubmi1_user");
    }
}
