using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SegundaPracticaAzure.Models;
using SegundaPracticaAzure.Repositories;

namespace SegundaPracticaAzure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private RepositoryCubos repo;
        private SecretClient secretClient;
        private KeyVaultSecret ContainerUrl;

        public CubosController(RepositoryCubos repo, SecretClient secretClient)
        {
            this.repo = repo;
            this.secretClient = secretClient;
            this.ContainerUrl = this.secretClient.GetSecret("ContainerUrl");
        }

        [HttpGet]
        public async Task<ActionResult<List<Cubo>>> Get()
        {
            List<Cubo> cubos = await this.repo.GetCubosAsync();
            foreach (Cubo cubo in cubos)
            {
                cubo.Imagen = this.ContainerUrl.Value + "/" + cubo.Imagen;
            }

            return cubos;
        }

        [HttpGet]
        [Route("[action]/{marca}")]
        public async Task<ActionResult<List<Cubo>>> CubosMarca(string marca)
        {
            return await this.repo.FindCubosByMarcaAsync(marca);
        }
    }
}
