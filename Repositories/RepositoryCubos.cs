using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SegundaPracticaAzure.Data;
using SegundaPracticaAzure.Models;
using System.Security.Claims;

namespace SegundaPracticaAzure.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext context;

        public RepositoryCubos(CubosContext context)
        {
            this.context = context;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            return await this.context.Cubos.ToListAsync();
        }

        public async Task<List<Cubo>> FindCubosByMarcaAsync(string marca)
        {
            return await this.context.Cubos
                .Where(x => x.Marca == marca).ToListAsync();
        }

        public async Task<Usuario> FindUsuarioAsync(int idUsuario)
        {
            return await this.context.Usuarios
                .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
        }

        private async Task<int> GetMaxIdUsuarioAsync()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await
                    this.context.Usuarios.MaxAsync(x => x.IdUsuario) + 1;
            }
        }

        public async Task<Usuario> InsertUsuarioAsync(Usuario user)
        {
            Usuario usuario = new Usuario();
            usuario.IdUsuario = await this.GetMaxIdUsuarioAsync();
            usuario.Nombre = user.Nombre;
            usuario.Email = user.Email;
            usuario.Pass = user.Pass;
            usuario.Imagen = user.Imagen;

            this.context.Usuarios.Add(user);

            await this.context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> LoginUsuarioAsync(string email, string password)
        {
            Usuario usuario = await this.context.Usuarios
                .FirstOrDefaultAsync(x => x.Email == email && x.Pass == password);
            return usuario;
        }

        public async Task<List<CompraCubo>> GetPedidosAsync(int idUsuario)
        {
            return await this.context.ComprasCubos
                .Where(x => x.IdUsuario == idUsuario).ToListAsync();
        }

    }
}
