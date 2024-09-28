using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Test.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _dbName;

        public CustomWebApplicationFactory()
        {
            _dbName = Guid.NewGuid().ToString();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ECommerceDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ECommerceDbContext>(options =>
                    options.UseInMemoryDatabase(_dbName));

                Console.WriteLine("Banco de dados em memória configurado para testes.");
            });
        }

        public string GetDataBaseNome() => _dbName;
    }

}
