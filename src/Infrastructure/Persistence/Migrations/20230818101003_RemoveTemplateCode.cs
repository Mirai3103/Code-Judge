using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Code_Judge.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTemplateCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateCode",
                table: "Problems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TemplateCode",
                table: "Problems",
                type: "text",
                unicode: false,
                nullable: false);
        }
    }
}
