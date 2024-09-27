using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Test.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove o DbContext existente
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ECommerceDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona o banco de dados em memória
                services.AddDbContext<ECommerceDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));

                Console.WriteLine("Banco de dados em memória configurado para testes.");
            });
        }
    }

}
