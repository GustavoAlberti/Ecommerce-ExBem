using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class PedidoService : IPedidoService
    {

        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IDescontoRepository _descontoRepository;


        public PedidoService(IPedidoRepository pedidoRepository, IUsuarioRepository usuarioRepository, IProdutoRepository produtoRepository, IDescontoRepository descontoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _usuarioRepository = usuarioRepository;
            _produtoRepository = produtoRepository;
            _descontoRepository = descontoRepository;
        }

        public async Task<PedidoResponseDto> BuscarPorCodigoPedidoAsync(string codigoPedido)
        {
            // Obter o pedido pelo repositório
            var pedido = await _pedidoRepository.ObterPorCodigoPedidoAsync(codigoPedido);

            if (pedido == null)
            {
                throw new Exception("Pedido não encontrado.");
            }

            // Retornar os dados do pedido
            return new PedidoResponseDto(
                codigoPedido: pedido.CodigoPedido,
                usuarioNome: pedido.Usuario.Nome,
                itens: pedido.Itens.Select(i => new ItemPedidoResponseDto(
                    i.Produto.CodigoProduto,
                    i.Preco,
                    i.Quantidade
                )).ToList(),
                pedido.ValorTotal,
                pedido.Status.ToString(),
                pedido.DataCriacao.ToString("dd-MM-yyyy")
            );
        }

        public async Task CancelarPedidoAsync(string codigoPedido)
        {
            // Obter o pedido pelo repositório
            var pedido = await _pedidoRepository.ObterPorCodigoPedidoAsync(codigoPedido);

            if (pedido == null)
            {
                throw new Exception("Pedido não encontrado.");
            }

            // Verificar se o pedido pode ser cancelado
            if (pedido.Status != StatusPedido.AguardandoPagamento)
            {
                throw new InvalidOperationException("O pedido não pode ser cancelado.");
            }

            // Cancelar o pedido e excluir o carrinho usando o repositório
            await _pedidoRepository.CancelarAsync(pedido);
        }

        public async Task<PedidoResponseDto> CriarPedidoAsync(CriarPedidoDto dto)
        {
            var usuario = await _usuarioRepository.ObterPorLoginAsync(dto.UsuarioLogin);

            if (usuario is null)
            {
                throw new Exception("Usuário não encontrado");
                //Problem detail - erro esperado.. Não usar exception
            }

            // Criar o pedido
            var pedido = new Pedido(usuario.Id);

            // Buscar os produtos de uma vez para os códigos fornecidos no DTO
            var produtos = await _produtoRepository.ObterPorCodigosAsync(dto.Itens.Select(i => i.CodigoProduto));

            if (produtos.Count() != dto.Itens.Count())
            {
                throw new Exception("Alguns produtos não foram encontrados.");
            }

            // Adicionar os itens ao pedido
            foreach (var item in dto.Itens)
            {
                var produto = produtos.First(p => p.CodigoProduto == item.CodigoProduto);

                // Adicionar item ao pedido
                pedido.AdicionarItem(new ItemPedido(produto, item.Quantidade, produto.Preco));

            }

            // Buscar e aplicar os descontos através do repositório
            var descontos = await _descontoRepository.ObterDescontosValidosAsync(pedido.Itens.Select(i => i.ProdutoId));

            descontos.ForEach(pedido.AplicarDescontoParaProduto);

            // Atualizar o valor total do pedido
            pedido.AtualizarValorTotal();

            // Definir o status do pedido
            pedido.AlterarStatus(StatusPedido.AguardandoPagamento);

            // Adicionar o pedido através do repositório
            await _pedidoRepository.AdicionarAsync(pedido);

            // Retorno direto com o new no construtor da DTO
            return new PedidoResponseDto(
                codigoPedido: pedido.CodigoPedido,
                usuarioNome: usuario.Nome,
                itens: pedido.Itens.Select(i => new ItemPedidoResponseDto(
                    i.Produto.CodigoProduto,
                    i.Preco,
                    i.Quantidade
                )).ToList(),
                pedido.ValorTotal,
                pedido.Status.ToString(),
                pedido.DataCriacao.ToString("yyyy-MM-dd")
            );

        }
    }

}
