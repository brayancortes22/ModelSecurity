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
    /// Repository encargado de la gestión de la entidad Person en la base de datos
    /// </summary>
    public class PersonData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de eventos</param>
        public PersonData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las personas almacenadas en la base de datos
        /// </summary>
        /// <returns>Lista de personas</returns>
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.Set<Person>().ToListAsync();
        }

        /// <summary>
        /// Obtiene una persona por su ID
        /// </summary>
        /// <param name="id">ID de la persona a buscar</param>
        /// <returns>La persona encontrada o null si no existe</returns>
        public async Task<Person?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Person>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener persona con ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva persona en la base de datos
        /// </summary>
        /// <param name="person">Instancia de la persona a crear</param>
        /// <returns>La persona creada</returns>
        public async Task<Person> CreateAsync(Person person)
        {
            try
            {
                await _context.Set<Person>().AddAsync(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la persona: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una persona existente en la base de datos
        /// </summary>
        /// <param name="person">Objeto con la información actualizada</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario</returns>
        public async Task<bool> UpdateAsync(Person person)
        {
            try
            {
                _context.Set<Person>().Update(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la persona: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una persona de la base de datos
        /// </summary>
        /// <param name="id">Identificador único de la persona a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var person = await _context.Set<Person>().FindAsync(id);
                if (person == null)
                    return false;

                _context.Set<Person>().Remove(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la persona: {ex.Message}");
                return false;
            }
        }
    }
}

