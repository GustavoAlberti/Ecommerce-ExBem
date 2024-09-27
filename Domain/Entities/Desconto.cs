namespace Domain.Entities
{
    public class Desconto
    {
        public int Id { get; private set; }
        public int? ProdutoId { get; private set; } 
        public decimal ValorDesconto { get; private set; } 
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }

        private Desconto() { }

        public Desconto(int? produtoId, decimal valorDesconto, DateTime dataInicio, DateTime dataFim)
        {
            ProdutoId = produtoId;
            ValorDesconto = valorDesconto;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }

        public bool IsValido(DateTime dataPedido)
        {
            return DataInicio <= dataPedido && dataPedido <= DataFim;
        }
    }
}
