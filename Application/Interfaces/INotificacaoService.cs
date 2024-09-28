using Domain.Entities;

namespace Application.Interfaces
{
    public interface INotificacaoService
    {
        Task EnviarNotificacaoStatusPedidoAsync(Pedido pedido, string mensagem);
        Task EnviarNotificacaoEstoqueInsuficienteAsync(Pedido pedido);
        Task EnviarNotificacaoErroAsync(string mensagem);
    }
}
