using Application.Interfaces;
using Application.Interfaces.PagamentoStrategy;
using Application.Services;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Strategies;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// Configurando a string de conexão (exemplo para SQL Server)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// Adicionando o DbContext ao container de injeção de dependência
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseSqlServer(connectionString));


// Repositorios
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IDescontoRepository, DescontoRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();



// Serviços da Aplicação
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<INotificacaoService, NotificacaoService>();
builder.Services.AddScoped<IPagamentoService, PagamentoService>();

// Adicionando as estratégias de pagamento
builder.Services.AddTransient<PagamentoPixStrategy>();
builder.Services.AddTransient<PagamentoCartaoCreditoStrategy>();

// Configurando o dicionário de estratégias de pagamento para ser injetado no PagamentoService
builder.Services.AddScoped<IDictionary<TipoPagamento, IPagamentoStrategy>>(provider => new Dictionary<TipoPagamento, IPagamentoStrategy>
{
    { TipoPagamento.Pix, provider.GetRequiredService<PagamentoPixStrategy>() },
    { TipoPagamento.CartaoDeCredito, provider.GetRequiredService<PagamentoCartaoCreditoStrategy>() }
});


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
