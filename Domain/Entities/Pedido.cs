using Domain.Entities.Enum;

namespace Domain.Entities
{
    public class Pedido
    {
        public int Id { get; private set; }
        public string CodigoPedido { get; private set; }
        public int UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; }
        public List<ItemPedido> Itens { get; private set; } = new List<ItemPedido>();
        public Pagamento Pagamento { get; private set; }
        public decimal ValorTotal { get; private set; }
        public StatusPedido Status { get; private set; }
        public DateTime DataCriacao { get; private set; }


        private Pedido () 
        { 
            Itens = new List<ItemPedido> ();
        }

        public Pedido(int usuarioId)
        {
            UsuarioId = usuarioId;
            Status = StatusPedido.AguardandoPagamento;
            CodigoPedido = GerarCodigoPedido();
            DataCriacao = DateTime.UtcNow;
        }

        public void AdicionarItem(ItemPedido item)
        {
            Itens.Add(item);
            AtualizarValorTotal();
        }

        public void AplicarDesconto(decimal valorDesconto)
        {
            if (valorDesconto < 0)
            {
                throw new ArgumentException("O valor de desconto não pode ser negativo.");
            }

            ValorTotal -= valorDesconto;

            if (ValorTotal < 0)
                ValorTotal = 0;
        }

        public void AtualizarValorTotal()
        {
            ValorTotal = Itens.Sum(i => i.Preco * i.Quantidade);
        }

        public void AlterarStatus(StatusPedido novoStatus)
        {
            Status = novoStatus;
        }

        public void AplicarDescontoParaProduto(Desconto desconto)
        {
            if (desconto.ProdutoId.HasValue && Itens.Any(i => i.Produto.Id == desconto.ProdutoId.Value))
            {
                ValorTotal -= desconto.ValorDesconto;
            }
            else if (desconto.DataInicio <= DataCriacao && DataCriacao <= desconto.DataFim)
            {
                ValorTotal -= ValorTotal * (desconto.ValorDesconto / 100);
            }
        }

        private string GerarCodigoPedido()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(); 
        }
    }

}
