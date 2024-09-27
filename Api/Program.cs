using Application.Handlers;
using Application.Interfaces;
using Application.Interfaces.PagamentoStrategy;
using Application.Services;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Strategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configurando a string de conex�o (exemplo para SQL Server)
// A string de conex�o agora ser� diferenciada com base no ambiente para suportar testes de integra��o
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Verifica se o ambiente � de teste (dentro dos testes, voc� configuraria "Testing" como o nome do ambiente)
if (builder.Environment.IsEnvironment("Testing"))
{
    // Se for ambiente de testes, utiliza o banco de dados em mem�ria
    builder.Services.AddDbContext<ECommerceDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    // Caso contr�rio, usa a conex�o normal (SQL Server no exemplo)
    builder.Services.AddDbContext<ECommerceDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Repositorios
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IDescontoRepository, DescontoRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();

// Servi�os da Aplica��o
builder.Services.AddScoped<INotificacaoService, NotificacaoService>();

// Adicionando as estrat�gias de pagamento
builder.Services.AddTransient<PagamentoPixStrategy>();
builder.Services.AddTransient<PagamentoCartaoCreditoStrategy>();

// Configurando o dicion�rio de estrat�gias de pagamento para ser injetado no PagamentoService
builder.Services.AddScoped<IDictionary<TipoPagamento, IPagamentoStrategy>>(provider => new Dictionary<TipoPagamento, IPagamentoStrategy>
{
    { TipoPagamento.Pix, provider.GetRequiredService<PagamentoPixStrategy>() },
    { TipoPagamento.CartaoDeCredito, provider.GetRequiredService<PagamentoCartaoCreditoStrategy>() }
});

// Adicionando o MediatR e registrando os Handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProcessarPagamentoCommandHandler).Assembly));

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// M�todo est�tico necess�rio para rodar o WebApplicationFactory nos testes de integra��o
public partial class Program { }
