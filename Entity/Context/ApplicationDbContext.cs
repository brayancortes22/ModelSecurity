using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;
namespace Entity.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ILogger<ApplicationDbContext> _logger;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger)
            : base(options)
        {
            _logger = logger;
        }

        public DbSet<Rol> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuraciones adicionales del modelo
        }

        /// <summary>
        /// Actualiza un rol existente en la base de datos.
        /// </summary>
        /// <param name="rol">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Rol rol)
        {
            try
            {
                Set<Rol>().Update(rol);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Crea un nuevo rol en la base de datos.
        /// </summary>
        /// <param name="rol">Instancia del rol a crear.</param>
        /// <returns>El rol creado.</returns>
        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            {
                await Set<Rol>().AddAsync(rol);
                await SaveChangesAsync();
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el rol: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los roles almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de roles.</returns>
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await Set<Rol>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un rol específico por su identificador.
        /// </summary>
        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                return await Set<Rol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener rol con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Elimina un rol de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del rol a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rol = await Set<Rol>().FindAsync(id);
                if (rol == null)
                    return false;

                Set<Rol>().Remove(rol);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el rol: {ex.Message}");
                return false;
            }
        }
    }
}
