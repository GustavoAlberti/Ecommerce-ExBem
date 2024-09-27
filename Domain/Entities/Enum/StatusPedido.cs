namespace Domain.Entities.Enum
{
    public enum StatusPedido
    {
        AguardandoPagamento = 1,
        ProcessandoPagamento = 2,
        PagamentoConcluido = 3,
        SeparandoPedido = 4,
        Concluido = 5,
        AguardandoEstoque = 6,
        Cancelado = 7
    }
}
