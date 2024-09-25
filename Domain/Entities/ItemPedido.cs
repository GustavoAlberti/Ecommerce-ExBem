using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ItemPedido
    {
        public int Id { get; private set; }
        public Pedido Pedido { get; private set; }
        public int PedidoId { get; private set; }
        public Produto Produto { get; private set; }
        public int ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal Preco { get; private set; }


        // Construtor vazio exigido pelo EF
        private ItemPedido() { }

        // Construtor para inicialização de um item de pedido
        public ItemPedido(Produto produto, int quantidade, decimal preco)
        {
            Produto = produto ?? throw new ArgumentNullException(nameof(produto));
            ProdutoId = produto.Id;
            Quantidade = quantidade;
            Preco = preco;
        }

        // Método para atualizar a quantidade
        public void AtualizarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));

            Quantidade = quantidade;
        }

        // Método para atualizar o preço
        public void AtualizarPreco(decimal preco)
        {
            if (preco <= 0)
                throw new ArgumentException("O preço deve ser maior que zero.", nameof(preco));

            Preco = preco;
        }
    }

}
