using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SegundaPracticaAzure.Models;
using SegundaPracticaAzure.Repositories;
using System.Security.Claims;

namespace SegundaPracticaAzure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraCubosController : ControllerBase
    {
        private RepositoryCubos repo;

        public CompraCubosController(RepositoryCubos repo)
        {
            this.repo = repo;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<CompraCubo>>> Get()
        {
            Claim claimUser = HttpContext.User.Claims
                .SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claimUser.Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUser);
            int idUsuario = usuario.IdUsuario;
            List<CompraCubo> pedidos = await this.repo.GetPedidosAsync(idUsuario);
            return pedidos;
        }

    }
}
