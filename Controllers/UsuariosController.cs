using Azure.Security.KeyVault.Secrets;
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
    public class UsuariosController : ControllerBase
    {
        private RepositoryCubos repo;
        private SecretClient secretClient;
        private KeyVaultSecret ContainerUrl;

        public UsuariosController(RepositoryCubos repo, SecretClient secretClient)
        {
            this.repo = repo;
            this.secretClient = secretClient;
            this.ContainerUrl = this.secretClient.GetSecret("ContainerUrl");
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Usuario>> PerfilUsuario()
        {
            Claim claimUser = HttpContext.User.Claims
                .SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claimUser.Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUser);
            int idUsuario = usuario.IdUsuario;
            Usuario usuarioValid = await this.repo.FindUsuarioAsync(idUsuario);
            usuarioValid.Imagen = this.ContainerUrl.Value + "/" + usuarioValid.Imagen;
            return usuarioValid;
        }
    }
}
