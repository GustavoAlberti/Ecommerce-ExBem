namespace Application.DTOs
{
    public record CriarPedidoDto
    {
        public required string UsuarioLogin { get; init; }
        public IEnumerable<CriarItemPedidoDto> Itens { get; init; }
    }

    public record CriarItemPedidoDto
    {
        public string CodigoProduto { get; init; }
        public int Quantidade { get; init; }
    }
}
