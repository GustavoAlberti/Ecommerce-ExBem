using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
            : base(options)
        {
        }

        // DbSets para as entidades
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<ItemPedido> ItensPedidos { get; set; }
        public DbSet<Desconto> Descontos { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Auto-increment
                entity.Property(e => e.CodigoPedido).IsRequired().HasMaxLength(8);
                entity.Property(e => e.ValorTotal).HasColumnType("decimal(18,2)");

                // Configuração alterada para evitar múltiplos caminhos de exclusão em cascata
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict); // Alterado de Cascade para Restrict

                entity.HasMany(e => e.Itens)
                      .WithOne(i => i.Pedido)
                      .HasForeignKey(i => i.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade); // Mantém o Cascade nos itens

                entity.HasOne(e => e.Pagamento)
                      .WithOne()
                      .HasForeignKey<Pagamento>(p => p.PedidoId);
            });

            // Configurações da tabela Produto
            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Auto-increment
                entity.Property(e => e.Nome).IsRequired();
                entity.Property(e => e.CodigoProduto).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.CodigoProduto).IsUnique(); // Código do produto deve ser único
                entity.Property(e => e.Preco).HasColumnType("decimal(18,2)");
            });

            // Configurações da tabela Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Auto-increment
                entity.Property(e => e.Nome).IsRequired();
                entity.Property(e => e.UsuarioLogin).IsRequired();
                entity.HasIndex(e => e.UsuarioLogin).IsUnique(); // Login deve ser único
            });

            modelBuilder.Entity<Pagamento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Valor).HasColumnType("decimal(18,2)");
            });


            // Configurações da tabela ItemPedido
            modelBuilder.Entity<ItemPedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Auto-increment
                entity.Property(e => e.Preco).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Produto)
                      .WithMany()
                      .HasForeignKey(e => e.ProdutoId);
                entity.HasOne(e => e.Pedido)
                      .WithMany(p => p.Itens) // Um Pedido pode ter muitos ItensPedido
                      .HasForeignKey(e => e.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            // Configurações da tabela Desconto
            modelBuilder.Entity<Desconto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Auto-increment
                entity.Property(e => e.ValorDesconto).HasColumnType("decimal(18,2)");
            });

            // Configurações da tabela Notificacao
            modelBuilder.Entity<Notificacao>(entity =>
            {
                entity.HasKey(e => e.NotificacaoId);
            });

        }
    }
}
