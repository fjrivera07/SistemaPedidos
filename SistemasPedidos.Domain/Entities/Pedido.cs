using SistemaPedidos.Domain.Enums;
using SistemaPedidos.Domain.Exceptions;

namespace SistemaPedidos.Domain.Entities;

public class Pedido
{
    private readonly List<DetallePedido> _detalles = new();

    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public decimal Total { get; private set; }
    public EstadoPedido Estado { get; private set; }

    public IReadOnlyCollection<DetallePedido> Detalles =>
        _detalles.AsReadOnly();

    private Pedido() { } // EF Core

    public Pedido(int clienteId)
    {
        if (clienteId <= 0)
            throw new ArgumentException(
                "El cliente no es válido.");

        ClienteId = clienteId;
        Estado = EstadoPedido.Pendiente;
        Total = 0;
    }

    public void AgregarProducto(
        int productoId,
        int cantidad,
        decimal precioUnitario)
    {
        if (Estado != EstadoPedido.Pendiente)
            throw new ReglaNegocioException(
                "Solo se pueden modificar pedidos pendientes.");

        var detalle = new DetallePedido(
            productoId,
            cantidad,
            precioUnitario);

        _detalles.Add(detalle);

        CalcularTotal();
    }

    public void Cancelar()
    {
        if (Estado == EstadoPedido.Entregado)
            throw new ReglaNegocioException(
                "No se puede cancelar un pedido entregado.");

        if (Estado == EstadoPedido.Cancelado)
            throw new ReglaNegocioException(
                "El pedido ya está cancelado.");

        Estado = EstadoPedido.Cancelado;
    }

    private void CalcularTotal()
    {
        Total = _detalles.Sum(d => d.Subtotal);
    }
}