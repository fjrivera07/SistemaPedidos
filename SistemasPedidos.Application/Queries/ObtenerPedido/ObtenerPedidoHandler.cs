using SistemaPedidos.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPedidos.Application.Queries.ObtenerPedido
{
    public class ObtenerPedidoHandler
    {
        private readonly IPedidoRepository repository;

        public ObtenerPedidoHandler(IPedidoRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PedidoDto?> HandleAsync(
            ObtenerPedidoQuery query)
        {
            var pedido = await repository.ObtenerPorIdAsync(query.Id);

            if (pedido is null)
                return null;

            return new PedidoDto(
                pedido.Id,
                pedido.ClienteId,
                pedido.Total,
                pedido.Estado.ToString(),
                pedido.Detalles.Select(d => new DetallePedidoDto(
                    d.ProductoId,
                    d.Cantidad,
                    d.PrecioUnitario,
                    d.Subtotal
                )).ToList()
            );
        }
    }
}
