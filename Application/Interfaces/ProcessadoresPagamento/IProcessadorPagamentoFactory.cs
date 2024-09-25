using Application.DTOs;
using Application.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ProcessadoresPagamento
{
    public interface IProcessadorPagamentoFactory
    {
        IProcessadorPagamento CriarProcessadorPagamento(TipoPagamento tipoPagamento);
    }
}
