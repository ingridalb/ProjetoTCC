using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaoSolidaria.Migrations
{
    public partial class Primeira : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_AspNetUsers_UsuarioId1",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Postagens_AspNetUsers_UsuarioId1",
                table: "Postagens");

            migrationBuilder.DropIndex(
                name: "IX_Postagens_UsuarioId1",
                table: "Postagens");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_UsuarioId1",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Postagens");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Comentarios");

            migrationBuilder.RenameColumn(
                name: "Conteudo",
                table: "Chats",
                newName: "ConteudoMensagem");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConteudoMensagem",
                table: "Chats",
                newName: "Conteudo");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId1",
                table: "Postagens",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId1",
                table: "Comentarios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Postagens_UsuarioId1",
                table: "Postagens",
                column: "UsuarioId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioId1",
                table: "Comentarios",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_AspNetUsers_UsuarioId1",
                table: "Comentarios",
                column: "UsuarioId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Postagens_AspNetUsers_UsuarioId1",
                table: "Postagens",
                column: "UsuarioId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
