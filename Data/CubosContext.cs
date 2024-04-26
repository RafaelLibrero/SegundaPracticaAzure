using Microsoft.EntityFrameworkCore;
using SegundaPracticaAzure.Models;

namespace SegundaPracticaAzure.Data
{
    public class CubosContext: DbContext
    {
        public CubosContext(DbContextOptions<CubosContext> options) : base(options) { }

        public DbSet<Cubo> Cubos { get; set; }
        public DbSet<CompraCubo> ComprasCubos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
