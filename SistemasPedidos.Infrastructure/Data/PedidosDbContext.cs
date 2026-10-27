using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Domain.Entities;

namespace SistemaPedidos.Infrastructure.Data;

public class PedidosDbContext : DbContext
{
    public PedidosDbContext(
        DbContextOptions<PedidosDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pedido> Pedidos => Set<Pedido>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Pedido
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("Pedidos");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            entity.Property(p => p.Total)
                .HasPrecision(18, 2);

            entity.Property(p => p.Estado)
                .HasConversion<int>();

            entity.Property(p => p.ClienteId)
                .IsRequired();

            // Relación con DetallePedido
            entity.HasMany(p => p.Detalles)
                .WithOne()
                .HasForeignKey("PedidoId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(p => p.Detalles)
                .UsePropertyAccessMode(
                    PropertyAccessMode.Field);
        });

        // Configuración de DetallePedido
        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.ToTable("DetallesPedido");

            entity.HasKey(d => d.Id);

            entity.Property(d => d.Id)
                .ValueGeneratedOnAdd();

            entity.Property(d => d.ProductoId)
                .IsRequired();

            entity.Property(d => d.Cantidad)
                .IsRequired();

            entity.Property(d => d.PrecioUnitario)
                .HasPrecision(18, 2);

            // Subtotal es calculado, no se guarda
            entity.Ignore(d => d.Subtotal);
        });
    }
}