using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaoSolidaria.Migrations
{
    public partial class AddCaminhoImagemToUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaminhoImagem",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaminhoImagem",
                table: "AspNetUsers");
        }
    }
}
