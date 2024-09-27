using Application.Commands;
using Application.DTOs;
using Domain.Entities;
using Domain.Entities.Enum;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using System.Text;

namespace Test.IntegrationTests
{
    public class PagamentoIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly DatabaseFixture _fixture;

        public PagamentoIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            _fixture = new DatabaseFixture(_factory.GetDataBaseNome());
        }

        [Fact]
        public async Task ProcessarPagamento_DeveConcluirComSucesso_ViaPix()
        {
            using var fixture = new DatabaseFixture("TestDb_PagamentoPix");

            var command = new ProcessarPagamentoCommand("TEST1234", TipoPagamento.Pix, null);
            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Pagamento/pagar", content);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagamentoResponseDto>(jsonResponse);

            result.Status.ShouldBe("Pagamento Concluído");
            result.ValorComDesconto.ShouldBe(190); // Considerando 5% de desconto
        }

        [Fact]
        public async Task ProcessarPagamento_DeveConcluirComSucesso_ViaCartao()
        {
            using var fixture = new DatabaseFixture("TestDb_PagamentoCartao");

            var command = new ProcessarPagamentoCommand("TEST1234", TipoPagamento.CartaoDeCredito, 2);
            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Pagamento/pagar", content);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagamentoResponseDto>(jsonResponse);

            result.Status.ShouldBe("Pagamento Concluído");
            result.ValorTotal.ShouldBe(200); // Sem desconto
        }
    }
}
