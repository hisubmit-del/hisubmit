using HiSubmit.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiSubmit.Infrastructure.Migrations;

/// <summary>
/// Compatibility marker for the fee-type migration already recorded by the
/// restored databases. The schema change was applied there already; keeping
/// this identifier in the source prevents EF from replaying the initial
/// migration chain against an existing database.
/// </summary>
[DbContext(typeof(BlazorHeroContext))]
[Migration("20250726170647_Change_FeePropTypeInCategoryDeadlineFeeTB")]
public partial class Change_FeePropTypeInCategoryDeadlineFeeTB : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
