using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SegundaPracticaAzure.Helpers;
using SegundaPracticaAzure.Models;
using SegundaPracticaAzure.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SegundaPracticaAzure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryCubos repo;
        private HelperToken helper;

        public AuthController(RepositoryCubos repo, HelperToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(string email, string password)
        {
            Usuario usuario = await
                this.repo.LoginUsuarioAsync(email, password);
            if (usuario == null)
            {
                return Unauthorized();
            }
            else
            {
                SigningCredentials credentials =
                    new SigningCredentials(this.helper.GetKeyToken()
                    , SecurityAlgorithms.HmacSha256);
                string jsonUser = JsonConvert.SerializeObject(usuario);
                Claim[] infoUser = new[]
                {
                    new Claim("UserData", jsonUser)
                };
                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: infoUser,
                        issuer: this.helper.Issuer.Value,
                        audience: this.helper.Audience.Value,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );
                return Ok(
                    new
                    {
                        response =
                        new JwtSecurityTokenHandler().WriteToken(token),
                    });
            }
        }
    }
}
