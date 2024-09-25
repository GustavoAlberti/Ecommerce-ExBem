using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ajusteDeleteCarrinho1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Carrinhos_CarrinhoId",
                table: "Pedidos");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Carrinhos_CarrinhoId",
                table: "Pedidos",
                column: "CarrinhoId",
                principalTable: "Carrinhos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Carrinhos_CarrinhoId",
                table: "Pedidos");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Carrinhos_CarrinhoId",
                table: "Pedidos",
                column: "CarrinhoId",
                principalTable: "Carrinhos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
