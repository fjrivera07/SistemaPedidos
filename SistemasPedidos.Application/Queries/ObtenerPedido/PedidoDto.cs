namespace SistemaPedidos.Application.Queries.ObtenerPedido;

public record DetallePedidoDto(
    int ProductoId,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);

public record PedidoDto(
    int Id,
    int ClienteId,
    decimal Total,
    string Estado,
    List<DetallePedidoDto> Detalles);