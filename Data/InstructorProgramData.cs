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
    /// Repository encargado de la gestión de la entidad InstructorProgram en la base de datos
    /// </summary>
    public class InstructorProgramData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de eventos</param>
        public InstructorProgramData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las relaciones instructor-programa almacenadas en la base de datos
        /// </summary>
        /// <returns>Lista de relaciones instructor-programa</returns>
        public async Task<IEnumerable<InstructorProgram>> GetAllAsync()
        {
            return await _context.Set<InstructorProgram>().ToListAsync();
        }

        /// <summary>
        /// Obtiene una relación instructor-programa por su ID
        /// </summary>
        /// <param name="id">ID de la relación a buscar</param>
        /// <returns>La relación encontrada o null si no existe</returns>
        public async Task<InstructorProgram?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<InstructorProgram>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener relación instructor-programa con ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva relación instructor-programa en la base de datos
        /// </summary>
        /// <param name="instructorProgram">Instancia de la relación a crear</param>
        /// <returns>La relación creada</returns>
        public async Task<InstructorProgram> CreateAsync(InstructorProgram instructorProgram)
        {
            try
            {
                await _context.Set<InstructorProgram>().AddAsync(instructorProgram);
                await _context.SaveChangesAsync();
                return instructorProgram;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la relación instructor-programa: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una relación instructor-programa existente en la base de datos
        /// </summary>
        /// <param name="instructorProgram">Objeto con la información actualizada</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario</returns>
        public async Task<bool> UpdateAsync(InstructorProgram instructorProgram)
        {
            try
            {
                _context.Set<InstructorProgram>().Update(instructorProgram);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la relación instructor-programa: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una relación instructor-programa de la base de datos
        /// </summary>
        /// <param name="id">Identificador único de la relación a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var instructorProgram = await _context.Set<InstructorProgram>().FindAsync(id);
                if (instructorProgram == null)
                    return false;

                _context.Set<InstructorProgram>().Remove(instructorProgram);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la relación instructor-programa: {ex.Message}");
                return false;
            }
        }
    }
} 