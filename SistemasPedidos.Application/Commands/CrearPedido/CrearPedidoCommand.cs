namespace SistemaPedidos.Application.Commands.CrearPedido;

public record CrearPedidoCommand(
    int ClienteId,
    List<CrearDetallePedidoDto> Detalles);

public record CrearDetallePedidoDto(
    int ProductoId,
    int Cantidad,
    decimal PrecioUnitario); 