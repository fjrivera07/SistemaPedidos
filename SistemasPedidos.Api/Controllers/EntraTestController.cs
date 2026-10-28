using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace SistemaPedidos.Api.Controllers;

[ApiController]
[Route("api/entra-test")]
public class EntraTestController : ControllerBase
{
    [HttpGet("pedidos")]
    [Authorize(AuthenticationSchemes = "EntraID")]
    [RequiredScope("Pedidos.Read")]
    public IActionResult ConsultarPedidos()
    {
        return Ok(new
        {
            Mensaje = "Acceso autorizado mediante Microsoft Entra ID",
            Usuario = User.Identity?.Name
        });
    }
}