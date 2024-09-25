using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INotificacaoService
    {
        Task EnviarNotificacaoStatusPedidoAsync(Pedido pedido, string mensagem);
        Task EnviarNotificacaoEstoqueInsuficienteAsync(Pedido pedido);
        Task EnviarNotificacaoErroAsync(string mensagem);
    }
}
