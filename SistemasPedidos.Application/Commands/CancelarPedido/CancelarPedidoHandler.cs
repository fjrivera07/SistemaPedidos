using SistemaPedidos.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPedidos.Application.Commands.CancelarPedido
{
    public class CancelarPedidoHandler
    {
        private readonly IPedidoRepository repository;

        public CancelarPedidoHandler(IPedidoRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> HandleAsync(CancelarPedidoCommand command)
        {

            var pedido = await repository.ObtenerPorIdAsync(command.PedidoId);
            if (pedido == null)
            {
                return false;
            }
            pedido.Cancelar();
            await repository.GuardarCambiosAsync();
            return true;
        }
    }
}
