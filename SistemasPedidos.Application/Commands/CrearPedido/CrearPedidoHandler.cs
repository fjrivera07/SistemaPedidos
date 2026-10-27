using SistemaPedidos.Application.Interfaces;
using SistemaPedidos.Domain.Entities;
using SistemaPedidos.Domain.Exceptions;

namespace SistemaPedidos.Application.Commands.CrearPedido;

public class CrearPedidoHandler
{
    private readonly IPedidoRepository _repository;

    public CrearPedidoHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> HandleAsync(
        CrearPedidoCommand command)
    {
        if (command.Detalles is null ||
            command.Detalles.Count == 0)
        {
            throw new ReglaNegocioException(
                "El pedido debe contener al menos un producto.");
        }

        var pedido = new Pedido(command.ClienteId);

        foreach (var detalle in command.Detalles)
        {
            pedido.AgregarProducto(
                detalle.ProductoId,
                detalle.Cantidad,
                detalle.PrecioUnitario);
        }

        await _repository.AgregarAsync(pedido);

        await _repository.GuardarCambiosAsync();

        return pedido.Id;
    }
}