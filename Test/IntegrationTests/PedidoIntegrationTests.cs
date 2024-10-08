﻿using Application.Commands;
using Application.DTOs;
using Domain.Entities.Enum;
using Newtonsoft.Json;
using Shouldly;
using System.Text;

namespace Test.IntegrationTests
{
    public class PedidoIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly DatabaseFixture _fixture;

        public PedidoIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            _fixture =  new DatabaseFixture(_factory.GetDataBaseNome());
        }

        [Fact]
        public async Task CriarPedido_DeveRetornarPedidoCriado_ComSucesso_ViaApi()
        {
            // Arrange
            var command = new CriarPedidoCommand("usuario123", new List<CriarItemPedidoCommand>
            {
                new CriarItemPedidoCommand("PROD01", 2)
            });

            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Pedidos/CriarPedido", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PedidoResponseDto>(jsonResponse);

            result.ShouldNotBeNull();
            result.CodigoPedido.ShouldNotBeNullOrEmpty();
            result.UsuarioNome.ShouldBe("Usuário Teste");
            result.Itens.Count.ShouldBe(1);
            result.ValorTotal.ShouldBe(200);
        }

        [Fact]
        public async Task CancelarPedido_DeveRetornarSucesso_ViaApi()
        {
            // Arrange: Obtém o código de um pedido que foi inserido na fixture
            var codigoPedido = "PEDIDO2";

            // Atualiza o status do pedido para 'Pagamento Concluído' para permitir a separação
            await _fixture.AlterarStatusPedido(codigoPedido, StatusPedido.AguardandoEstoque);


            // Act
            var response = await _client.PutAsync($"/api/Pedidos/CancelarPedido/{codigoPedido}", null);

            // Assert
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            Assert.Equal("Pedido cancelado com sucesso", (string)result.mensagem);
        }

        [Fact]
        public async Task BuscarPedido_DeveRetornarPedidoPorCodigo_ComSucesso()
        {
            // Arrange
            var codigoPedido = "TEST1234";

            // Act
            var response = await _client.GetAsync($"/api/Pedidos/BuscarPorCodigo/{codigoPedido}");

            // Assert 
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PedidoResponseDto>(jsonResponse);

            Assert.NotNull(result);
            Assert.Equal(codigoPedido, result.CodigoPedido);
        }

        [Fact]
        public async Task SepararPedido_DeveConcluirComSucesso_ViaApi()
        {
            // Arrange: Obtém o código de um pedido que foi inserido na fixture
            var codigoPedido = "TEST1234";

            // Atualiza o status do pedido para 'Pagamento Concluído' para permitir a separação
            await _fixture.AlterarStatusPedido(codigoPedido, StatusPedido.PagamentoConcluido);


            // Act: Envia a requisição POST para separar o pedido
            var response = await _client.PostAsync($"/api/Pedidos/{codigoPedido}/separar", null);

            // Assert: Verifica se o status de retorno é 200 OK
            response.EnsureSuccessStatusCode();

            // Verifica o conteúdo da resposta
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse); // Converte a resposta para um objeto dinâmico

            // Verifica se a mensagem de sucesso foi retornada
            string mensagem = result.mensagem;
            mensagem.ShouldBe("Pedido separado com sucesso!");
        }
    }
}
