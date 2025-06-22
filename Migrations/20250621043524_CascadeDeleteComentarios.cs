using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaoSolidaria.Migrations
{
    public partial class CascadeDeleteComentarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Postagens_PostagemId",
                table: "Comentarios");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Postagens_PostagemId",
                table: "Comentarios",
                column: "PostagemId",
                principalTable: "Postagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Postagens_PostagemId",
                table: "Comentarios");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Postagens_PostagemId",
                table: "Comentarios",
                column: "PostagemId",
                principalTable: "Postagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
