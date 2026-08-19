using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using HiSubmit.Infrastructure.Contexts;

#nullable disable

namespace HiSubmit.Infrastructure.Migrations;

[DbContext(typeof(BlazorHeroContext))]
[Migration("20260819190000_AddSettlementStatementsAndAdvertisingInvoices")]
public partial class AddSettlementStatementsAndAdvertisingInvoices : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "FestivalSettlementStatements",
            schema: "hisubmi1_user",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FestivalId = table.Column<int>(type: "int", nullable: false),
                PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                GrossIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                SiteCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                AdvertisingCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                PaymentsToFestival = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                DisputeReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ApprovalNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConfirmedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                PaidOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FestivalSettlementStatements", x => x.Id);
                table.ForeignKey(
                    name: "FK_FestivalSettlementStatements_Festivals_FestivalId",
                    column: x => x.FestivalId,
                    principalSchema: "hisubmi1_user",
                    principalTable: "Festivals",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AdvertisingInvoices",
            schema: "hisubmi1_user",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FestivalId = table.Column<int>(type: "int", nullable: false),
                AdvertiseRequestId = table.Column<int>(type: "int", nullable: true),
                FestivalSettlementStatementId = table.Column<int>(type: "int", nullable: true),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                DueOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                PaidOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AdvertisingInvoices", x => x.Id);
                table.ForeignKey(
                    name: "FK_AdvertisingInvoices_AdvertiseRequests_AdvertiseRequestId",
                    column: x => x.AdvertiseRequestId,
                    principalSchema: "hisubmi1_user",
                    principalTable: "AdvertiseRequests",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_AdvertisingInvoices_FestivalSettlementStatements_FestivalSettlementStatementId",
                    column: x => x.FestivalSettlementStatementId,
                    principalSchema: "hisubmi1_user",
                    principalTable: "FestivalSettlementStatements",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_AdvertisingInvoices_Festivals_FestivalId",
                    column: x => x.FestivalId,
                    principalSchema: "hisubmi1_user",
                    principalTable: "Festivals",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "SettlementAdjustments",
            schema: "hisubmi1_user",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FestivalSettlementStatementId = table.Column<int>(type: "int", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                EvidenceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SettlementAdjustments", x => x.Id);
                table.ForeignKey(
                    name: "FK_SettlementAdjustments_FestivalSettlementStatements_FestivalSettlementStatementId",
                    column: x => x.FestivalSettlementStatementId,
                    principalSchema: "hisubmi1_user",
                    principalTable: "FestivalSettlementStatements",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_FestivalSettlementStatements_FestivalId_PeriodStart_PeriodEnd",
            schema: "hisubmi1_user",
            table: "FestivalSettlementStatements",
            columns: new[] { "FestivalId", "PeriodStart", "PeriodEnd" },
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_AdvertisingInvoices_AdvertiseRequestId",
            schema: "hisubmi1_user",
            table: "AdvertisingInvoices",
            column: "AdvertiseRequestId");
        migrationBuilder.CreateIndex(
            name: "IX_AdvertisingInvoices_FestivalSettlementStatementId",
            schema: "hisubmi1_user",
            table: "AdvertisingInvoices",
            column: "FestivalSettlementStatementId");
        migrationBuilder.CreateIndex(
            name: "IX_AdvertisingInvoices_FestivalId",
            schema: "hisubmi1_user",
            table: "AdvertisingInvoices",
            column: "FestivalId");
        migrationBuilder.CreateIndex(
            name: "IX_SettlementAdjustments_FestivalSettlementStatementId",
            schema: "hisubmi1_user",
            table: "SettlementAdjustments",
            column: "FestivalSettlementStatementId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AdvertisingInvoices", schema: "hisubmi1_user");
        migrationBuilder.DropTable(name: "SettlementAdjustments", schema: "hisubmi1_user");
        migrationBuilder.DropTable(name: "FestivalSettlementStatements", schema: "hisubmi1_user");
    }
}
