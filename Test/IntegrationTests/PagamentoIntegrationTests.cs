using Application.Commands;
using Application.DTOs;
using Domain.Entities.Enum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using Shouldly;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.IntegrationTests
{
    public class PagamentoIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PagamentoIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing"); 
            }).CreateClient(); // Cria o cliente HTTP para simular requisições
        }

        [Fact]
        public async Task ProcessarPagamento_DeveConcluirComSucesso()
        {
            // Arrange
            var command = new ProcessarPagamentoCommand("PEDIDO123", TipoPagamento.CartaoDeCredito, 3);
            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/pagamentos/processar", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagamentoResponseDto>(jsonResponse);

            result.Status.ShouldBe("Pagamento Concluído");
            result.Mensagem.ShouldBe("Pagamento realizado com sucesso.");
            result.NumeroParcelas.ShouldBe(3);
        }
    }
}
