using Domain.Entities;
using Domain.Entities.Enum;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Test.IntegrationTests
{
    public class DatabaseFixture : IDisposable
    {
        public ECommerceDbContext Context { get; private set; }

        public DatabaseFixture(string dbName)
        {
            var options = new DbContextOptionsBuilder<ECommerceDbContext>()
                .UseInMemoryDatabase(dbName) // dinamico
                .Options;

            Context = new ECommerceDbContext(options);

            // Popula o banco de dados inicial apenas se estiver vazio
            if (!Context.Usuarios.Any())
            {
                var usuario = new Usuario("Usuário Teste", "teste@email.com", "usuario123");
                var produto = new Produto("Produto Teste", "PROD01", 100, 50);

                Context.Usuarios.Add(usuario);
                Context.Produtos.Add(produto);

                var pedido = new Pedido(usuario.Id);
                pedido.AdicionarItem(new ItemPedido(produto, 2, produto.Preco));
                typeof(Pedido).GetProperty("CodigoPedido").SetValue(pedido, "TEST1234");

                // Pedido 2
                var pedido2 = new Pedido(usuario.Id);
                pedido2.AdicionarItem(new ItemPedido(produto, 2, produto.Preco));
                typeof(Pedido).GetProperty("CodigoPedido").SetValue(pedido2, "PEDIDO2");

                var pagamento = new Pagamento(pedido2.Id, TipoPagamento.Pix, 200);
                // Usa reflection para definir o pagamento no Pedido 2
                typeof(Pedido)
                    .GetProperty(nameof(Pedido.Pagamento), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.SetValue(pedido2, pagamento);

                Context.Pedidos.Add(pedido);
                Context.Pedidos.Add(pedido2);
                Context.SaveChanges();
            }
        }

        // Método auxiliar para alterar o status de um pedido
        public async Task AlterarStatusPedido(string codigoPedido, StatusPedido novoStatus)
        {
            var pedido = Context.Pedidos.FirstOrDefault(p => p.CodigoPedido == codigoPedido);

            if (pedido != null)
            {
                pedido.AlterarStatus(novoStatus);
                Context.Pedidos.Update(pedido);
                await Context.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }

}
