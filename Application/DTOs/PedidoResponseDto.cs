namespace Application.DTOs
{
    public record PedidoResponseDto
    {
        public string CodigoPedido { get; init; }
        public string UsuarioNome { get; init; }
        public List<ItemPedidoResponseDto> Itens { get; init; }
        public decimal ValorTotal { get; init; }
        public string Status { get; init; } 
        public string DataPedido { get; init; } 

        
        public PedidoResponseDto(string codigoPedido, string usuarioNome, List<ItemPedidoResponseDto> itens, decimal valorTotal, string status, string dataPedido)
        {
            CodigoPedido = codigoPedido;
            UsuarioNome = usuarioNome;
            Itens = itens;
            ValorTotal = valorTotal;
            Status = status;
            DataPedido = dataPedido;
        }
    }

    public record ItemPedidoResponseDto
    {
        public string CodigoProduto { get; init; }
        public decimal Preco { get; init; }
        public int Quantidade { get; init; }

        public ItemPedidoResponseDto(string codigoProduto, decimal preco, int quantidade)
        {
            CodigoProduto = codigoProduto;
            Preco = preco;
            Quantidade = quantidade;
        }
    }
}
