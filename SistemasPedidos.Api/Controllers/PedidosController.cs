using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPedidos.Application.Commands.CancelarPedido;
using SistemaPedidos.Application.Commands.CrearPedido;
using SistemaPedidos.Application.Queries.ObtenerPedido;
using SistemaPedidos.Domain.Exceptions;

namespace SistemaPedidos.Api.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly CrearPedidoHandler crearHandler;
        private readonly ObtenerPedidoHandler obtenerHandler;
        private readonly CancelarPedidoHandler cancelarHandler;

        public PedidosController(CrearPedidoHandler crearHandler, ObtenerPedidoHandler obtenerHandler, CancelarPedidoHandler cancelarHandler)
        {
            this.crearHandler = crearHandler;
            this.obtenerHandler = obtenerHandler;
            this.cancelarHandler = cancelarHandler;
        }

        [Authorize(Roles = "Usuario,Administrador")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearPedidoCommand command)
        {
            var id = await crearHandler.HandleAsync(command);

            return Ok(new { Id = id });
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var pedidoDto = await obtenerHandler.HandleAsync(new ObtenerPedidoQuery(id));

            if (pedidoDto == null)
            {
                return NotFound();
            }
            return Ok(pedidoDto);
        }

        [Authorize(Roles = "Administrador")]

        [HttpPut("{id:int}/cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var cancelado = await cancelarHandler.HandleAsync(
                    new CancelarPedidoCommand(id));

                if (!cancelado)
                    return NotFound(new
                    {
                        Mensaje = "El pedido no existe."
                    });

                return NoContent();
            }
            catch (ReglaNegocioException ex)
            {
                return Conflict(new
                {
                    Mensaje = ex.Message
                });
            }
        }
    }
}
