namespace Application.DTOs
{
    public record PagamentoResponseDto(
        string Mensagem,
        decimal ValorTotal,
        decimal? ValorComDesconto, 
        int? NumeroParcelas, 
        string TipoPagamento,
        string Status
    );


}
