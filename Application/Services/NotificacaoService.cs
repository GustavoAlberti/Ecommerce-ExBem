using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class NotificacaoService : INotificacaoService
    {
        public async Task EnviarNotificacaoStatusPedidoAsync(Pedido pedido, string mensagem)
        {
            var notificacaoMensagem = $"Pedido {pedido.CodigoPedido}: {mensagem}";
            await EnviarNotificacaoAsync("notificacoes@empresa.com", "Atualização de Status do Pedido", notificacaoMensagem);
        }

        public async Task EnviarNotificacaoEstoqueInsuficienteAsync(Pedido pedido)
        {
            var mensagem = $"O produto do pedido {pedido.CodigoPedido} está com estoque insuficiente.";
            await EnviarNotificacaoAsync("vendas@empresa.com", "Estoque Insuficiente", mensagem);
        }

        public async Task EnviarNotificacaoErroAsync(string mensagem)
        {
            await EnviarNotificacaoAsync("suporte@empresa.com", "Erro no Sistema", mensagem);
        }

        private Task EnviarNotificacaoAsync(string destinatario, string assunto, string mensagem)
        {
            Console.WriteLine($"Enviando notificação para {destinatario}:\nAssunto: {assunto}\nMensagem: {mensagem}");
            return Task.CompletedTask;
        }
    }
}
