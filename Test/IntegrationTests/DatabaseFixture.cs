namespace Test.IntegrationTests
{
    using Domain.Entities;
    using Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;

    public class DatabaseFixture : IDisposable
    {
        public ECommerceDbContext Context { get; private set; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<ECommerceDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            Context = new ECommerceDbContext(options);

            if (!Context.Usuarios.Any())
            {
                var usuario = new Usuario("Usuário Teste", "teste@email.com", "usuario123");
                var produto = new Produto("Produto Teste", "PROD01", 100, 50);

                Context.Usuarios.Add(usuario);
                Context.Produtos.Add(produto);

                // Criar um pedido com o código "TEST1234"
                var pedido = new Pedido(usuario.Id);
                pedido.AdicionarItem(new ItemPedido(produto, 2, produto.Preco));
                typeof(Pedido).GetProperty("CodigoPedido").SetValue(pedido, "TEST1234"); // Define o código fixo
                pedido.AlterarStatus(Domain.Entities.Enum.StatusPedido.AguardandoPagamento);

                Context.Pedidos.Add(pedido);

                Context.SaveChanges();
            }
        }

        public void Dispose()
        {
            // Limpar ou fechar a conexão quando terminar
            Context.Dispose();
        }
    }

}
