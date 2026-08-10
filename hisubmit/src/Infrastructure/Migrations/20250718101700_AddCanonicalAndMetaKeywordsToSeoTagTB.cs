using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiSubmit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCanonicalAndMetaKeywordsToSeoTagTB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CanonicalUrl",
                table: "MetaTags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywords",
                table: "MetaTags",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanonicalUrl",
                table: "MetaTags");

            migrationBuilder.DropColumn(
                name: "MetaKeywords",
                table: "MetaTags");
        }
    }
}
