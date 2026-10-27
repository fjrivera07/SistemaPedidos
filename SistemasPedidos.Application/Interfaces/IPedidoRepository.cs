using SistemaPedidos.Domain.Entities;

namespace SistemaPedidos.Application.Interfaces;

public interface IPedidoRepository
{
    Task AgregarAsync(Pedido pedido);
    Task<Pedido?> ObtenerPorIdAsync(int id);
    Task GuardarCambiosAsync();
}