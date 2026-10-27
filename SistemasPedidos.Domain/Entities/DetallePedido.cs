namespace SistemaPedidos.Domain.Entities;

public class DetallePedido
{
    public int Id { get; private set; }
    public int ProductoId { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }

    public decimal Subtotal => Cantidad * PrecioUnitario;

    private DetallePedido() { } // EF Core

    internal DetallePedido(
        int productoId,
        int cantidad,
        decimal precioUnitario)
    {
        if (productoId <= 0)
            throw new ArgumentException(
                "El producto no es válido.");

        if (cantidad <= 0)
            throw new ArgumentException(
                "La cantidad debe ser mayor que cero.");

        if (precioUnitario <= 0)
            throw new ArgumentException(
                "El precio debe ser mayor que cero.");

        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }
}