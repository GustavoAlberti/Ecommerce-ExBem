using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificacaoService : INotificacaoService
    {
        public async Task EnviarNotificacaoStatusPedidoAsync(Pedido pedido, string mensagem)
        {
            // Lógica para enviar notificação sobre o status do pedido
            var notificacaoMensagem = $"Pedido {pedido.CodigoPedido}: {mensagem}";
            await EnviarNotificacaoAsync("notificacoes@empresa.com", "Atualização de Status do Pedido", notificacaoMensagem);
        }

        public async Task EnviarNotificacaoEstoqueInsuficienteAsync(Pedido pedido)
        {
            // Lógica para enviar notificação sobre estoque insuficiente
            var mensagem = $"O produto do pedido {pedido.CodigoPedido} está com estoque insuficiente.";
            await EnviarNotificacaoAsync("vendas@empresa.com", "Estoque Insuficiente", mensagem);
        }

        public async Task EnviarNotificacaoErroAsync(string mensagem)
        {
            // Lógica para enviar notificação de erro
            await EnviarNotificacaoAsync("suporte@empresa.com", "Erro no Sistema", mensagem);
        }

        private Task EnviarNotificacaoAsync(string destinatario, string assunto, string mensagem)
        {
            // Simulação de envio de notificação (e-mail ou outro tipo de mensagem)
            Console.WriteLine($"Enviando notificação para {destinatario}:\nAssunto: {assunto}\nMensagem: {mensagem}");
            return Task.CompletedTask;
        }
    }
}
