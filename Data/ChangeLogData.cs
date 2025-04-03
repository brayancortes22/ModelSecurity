using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repository encargado de la gestión de la entidad ChangeLog en la base de datos
    /// </summary>
    public class ChangeLogData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de eventos</param>
        public ChangeLogData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de cambios almacenados en la base de datos
        /// </summary>
        /// <returns>Lista de registros de cambios</returns>
        public async Task<IEnumerable<ChangeLog>> GetAllAsync()
        {
            return await _context.Set<ChangeLog>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro de cambio por su ID
        /// </summary>
        /// <param name="id">ID del registro a buscar</param>
        /// <returns>El registro encontrado o null si no existe</returns>
        public async Task<ChangeLog?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<ChangeLog>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener registros de cambio: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo registro de cambio en la base de datos
        /// </summary>
        /// <param name="changeLog">Instancia del registro a crear</param>
        /// <returns>El registro creado</returns>
        public async Task<ChangeLog> CreateAsync(ChangeLog changeLog)
        {
            try
            {
                await _context.Set<ChangeLog>().AddAsync(changeLog);
                await _context.SaveChangesAsync();
                return changeLog;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el registro de cambio: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var changeLog = await _context.Set<ChangeLog>().FindAsync(id);
                if (changeLog == null)
                    return false;

                _context.Set<ChangeLog>().Remove(changeLog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar registros de cambio antiguos: {ex.Message}");
                throw;
            }
        }
    }
}
