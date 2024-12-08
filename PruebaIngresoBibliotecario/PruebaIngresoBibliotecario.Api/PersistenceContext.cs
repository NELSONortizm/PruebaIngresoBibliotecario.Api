using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PruebaIngresoBibliotecario.Api.Models;
using System.Threading.Tasks;

namespace PruebaIngresoBibliotecario.Infrastructure
{
    public class PersistenceContext : DbContext
    {

        private readonly IConfiguration Config;

       
        public PersistenceContext(DbContextOptions<PersistenceContext> options, IConfiguration config) : base(options)
        {
            Config = config;
        }

        // DbSet para la entidad Prestamo
        public DbSet<Prestamo> Prestamos { get; set; } = null!;

        // Método para confirmar cambios en la base de datos
        public async Task CommitAsync()
        {
            await SaveChangesAsync().ConfigureAwait(false);
        }

        // Configuración del modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Config.GetValue<string>("SchemaName"));

            // Configuración de esquema predeterminado si se proporciona en la configuración
            var schemaName = Config.GetValue<string>("SchemaName");
            if (!string.IsNullOrEmpty(schemaName))
            {
                modelBuilder.HasDefaultSchema(schemaName);
            }

            // Configurar propiedades de Prestamo (ejemplo)
            modelBuilder.Entity<Prestamo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Isbn).IsRequired();
                entity.Property(e => e.IdentificacionUsuario)
                      .IsRequired()
                      .HasMaxLength(10);
                entity.Property(e => e.TipoUsuario).IsRequired();
                entity.Property(e => e.FechaMaximaDevolucion).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
