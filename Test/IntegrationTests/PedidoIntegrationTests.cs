using Application.Commands;
using Application.DTOs;
using Domain.Entities;
using Domain.Entities.Enum;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using System.Text;
using Xunit;

namespace Test.IntegrationTests
{
    public class PedidoIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public PedidoIntegrationTests(CustomWebApplicationFactory<Program> factory, DatabaseFixture fixture)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _fixture = fixture;
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
            var codigoPedido = "TEST1234";

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
    }
}
