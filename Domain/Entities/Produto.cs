namespace Domain.Entities
{
    public class Produto
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string CodigoProduto { get; private set; }
        public decimal Preco { get; private set; }
        public int QuantidadeEmEstoque { get; private set; }

        private Produto() { }

        
        public Produto(string nome, string codigoProduto, decimal preco, int quantidadeEmEstoque)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            CodigoProduto = codigoProduto ?? throw new ArgumentNullException(nameof(codigoProduto));
            Preco = preco;
            QuantidadeEmEstoque = quantidadeEmEstoque;
        }
        
        public void AjustarEstoque(int quantidade)
        {
            if (QuantidadeEmEstoque + quantidade < 0)
                throw new InvalidOperationException("Quantidade em estoque insuficiente.");

            QuantidadeEmEstoque -= quantidade; 
        }

    }

}
