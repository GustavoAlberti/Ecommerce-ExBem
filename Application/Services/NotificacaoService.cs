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
        //public Task EnviarNotificacaoStatusPedido(Pedido pedido, string mensagem)
        //{
        //    // Enviar e-mail com o novo status do pedido
        //    return Task.CompletedTask;
        //}

        //public Task EnviarNotificacaoEstoqueInsuficiente(Pedido pedido)
        //{
        //    // Notificar sobre falta de estoque
        //    return Task.CompletedTask;
        //}

        public Task EnviarNotificacaoStatusPedidoAsync(Pedido pedido, string mensagem)
        {
            //Implementar lógica de envio de e - mail para o cliente
            return Task.CompletedTask;
        }

        public Task EnviarNotificacaoEstoqueInsuficienteAsync(Pedido pedido)
        {
            // Implementar lógica de envio de e-mail para o departamento de vendas
            return Task.CompletedTask;
        }

        public Task EnviarNotificacaoErroAsync(string mensagem)
        {
            return Task.CompletedTask;
        }
    }
}
