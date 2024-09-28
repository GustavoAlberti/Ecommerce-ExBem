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
                .UseInMemoryDatabase(dbName)
                .Options;

            Context = new ECommerceDbContext(options);

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
                Context.Pedidos.Add(pedido2);
                var pagamento = new Pagamento(pedido2.Id, TipoPagamento.Pix, 200);
                pagamento.ConcluirPagamento();
                Context.Pagamentos.Add(pagamento);
                pedido2.AlterarStatus(StatusPedido.PagamentoConcluido);
                Context.Pedidos.Update(pedido2);

                Context.Pedidos.Add(pedido);
                Context.Pedidos.Add(pedido2);
                Context.SaveChanges();
            }
        }

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
