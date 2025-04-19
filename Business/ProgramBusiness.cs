using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los programas en el sistema.
    /// </summary>
    public class ProgramBusiness
    {
        private readonly ProgramData _programData;
        private readonly ILogger<ProgramBusiness> _logger;

        public ProgramBusiness(ProgramData programData, ILogger<ProgramBusiness> logger)
        {
            _programData = programData;
            _logger = logger;
        }

        // Método para obtener todos los programas como DTOs
        public async Task<IEnumerable<ProgramDto>> GetAllProgramsAsync()
        {
            try
            {
                var programs = await _programData.GetAllAsync();
                return MapToDTOList(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los programas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de programas", ex);
            }
        }

        // Método para obtener un programa por ID como DTO
        public async Task<ProgramDto> GetProgramByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un programa con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa debe ser mayor que cero");
            }

            try
            {
                var program = await _programData.GetByIdAsync(id);
                if (program == null)
                {
                    _logger.LogInformation("No se encontró ningún programa con ID: {Id}", id);
                    throw new EntityNotFoundException("program", id);
                }

                return MapToDTO(program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el programa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el programa con ID {id}", ex);
            }
        }

        // Método para crear un programa desde un DTO
        public async Task<ProgramDto> CreateProgramAsync(ProgramDto programDto)
        {
            try
            {
                ValidateProgram(programDto);
                var program = MapToEntity(programDto);
                program.CreateDate = DateTime.UtcNow;
                var programCreado = await _programData.CreateAsync(program);
                return MapToDTO(programCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo programa: {Name}", programDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el programa", ex);
            }
        }

        // Método para actualizar un programa existente (PUT)
        public async Task<ProgramDto> UpdateProgramAsync(int id, ProgramDto programDto)
        {
            if (id <= 0 || id != programDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar un programa con un ID invalido o no coincidente: {ProgramId}, DTO ID: {DtoId}", id, programDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateProgram(programDto);

            try
            {
                var existingProgram = await _programData.GetByIdAsync(id);
                if (existingProgram == null)
                {
                    _logger.LogInformation("No se encontró el programa con ID {ProgramId} para actualizar", id);
                    throw new EntityNotFoundException("Program", id);
                }

                existingProgram = MapToEntity(programDto, existingProgram);
                existingProgram.UpdateDate = DateTime.UtcNow;

                await _programData.UpdateAsync(existingProgram);
                return MapToDTO(existingProgram);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al actualizar el programa con ID {ProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el programa con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar el programa con ID {ProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el programa con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente un programa (PATCH)
        public async Task<ProgramDto> PatchProgramAsync(int id, ProgramDto programDto)
        {
            if (id <= 0 || (programDto.Id != 0 && id != programDto.Id))
            {
                _logger.LogWarning("Se intentó aplicar patch a un programa con un ID invalido o no coincidente: {ProgramId}, DTO ID: {DtoId}", id, programDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado en la URL es inválido o no coincide con el ID del DTO (si se proporcionó) para PATCH.");
            }

            try
            {
                var existingProgram = await _programData.GetByIdAsync(id);
                if (existingProgram == null)
                {
                    _logger.LogInformation("No se encontró el programa con ID {ProgramId} para aplicar patch", id);
                    throw new EntityNotFoundException("Program", id);
                }

                bool changed = false;
                if (programDto.Name != null && existingProgram.Name != programDto.Name)
                {
                    if (string.IsNullOrWhiteSpace(programDto.Name))
                        throw new Utilities.Exceptions.ValidationException("Name", "El Name no puede estar vacío en PATCH.");
                    existingProgram.Name = programDto.Name;
                    changed = true;
                }
                if (programDto.CodeProgram != default(decimal) && existingProgram.CodeProgram != programDto.CodeProgram)
                {
                    if (programDto.CodeProgram <= 0)
                        throw new Utilities.Exceptions.ValidationException("CodeProgram", "El CodeProgram debe ser mayor que cero.");
                    existingProgram.CodeProgram = programDto.CodeProgram;
                    changed = true;
                }
                if (programDto.TypeProgram != null && existingProgram.TypeProgram != programDto.TypeProgram)
                {
                    if (string.IsNullOrWhiteSpace(programDto.TypeProgram))
                        throw new Utilities.Exceptions.ValidationException("TypeProgram", "El TypeProgram no puede estar vacío en PATCH.");
                    existingProgram.TypeProgram = programDto.TypeProgram;
                    changed = true;
                }
                if (programDto.Description != null && existingProgram.Description != programDto.Description)
                {
                    existingProgram.Description = programDto.Description;
                    changed = true;
                }
                if (existingProgram.Active != programDto.Active)
                {
                    existingProgram.Active = programDto.Active;
                    if (!programDto.Active)
                    {
                        existingProgram.DeleteDate = DateTime.UtcNow;
                    }
                    else
                    {
                        existingProgram.DeleteDate = null;
                    }
                    changed = true;
                }

                if (changed)
                {
                    existingProgram.UpdateDate = DateTime.UtcNow;
                    await _programData.UpdateAsync(existingProgram);
                    _logger.LogInformation("Aplicado patch al programa con ID {ProgramId}", id);
                }
                else
                {
                    _logger.LogInformation("No se detectaron cambios en la solicitud PATCH para el programa con ID {ProgramId}", id);
                }

                return MapToDTO(existingProgram);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al aplicar patch al programa con ID {ProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al programa con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch al programa con ID {ProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al programa con ID {id}", ex);
            }
        }

        // Método para eliminar un programa (DELETE persistente)
        public async Task DeleteProgramAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un programa con un ID invalido: {ProgramId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa debe ser mayor a 0");
            }
            try
            {
                var existingProgram = await _programData.GetByIdAsync(id);
                if (existingProgram == null)
                {
                    _logger.LogInformation("No se encontró el programa con ID {ProgramId} para eliminar (persistente)", id);
                    throw new EntityNotFoundException("Program", id);
                }

                bool deleted = await _programData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Programa con ID {ProgramId} eliminado exitosamente (persistente)", id);
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar (persistente) el programa con ID {ProgramId}.", id);
                    throw new EntityNotFoundException("Program", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar el programa con ID {ProgramId}. Posible violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el programa con ID {id}. Verifique dependencias (Aprendices/Instructores).", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error general al eliminar (persistente) el programa con ID {ProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el programa con ID {id}", ex);
            }
        }

        // Método para eliminar lógicamente un programa (soft delete)
        public async Task SoftDeleteProgramAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar soft-delete a un programa con un ID invalido: {ProgramId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa debe ser mayor a 0");
            }
            try
            {
                var programToDeactivate = await _programData.GetByIdAsync(id);
                if (programToDeactivate == null)
                {
                    _logger.LogInformation("No se encontró el programa con ID {ProgramId} para desactivar (soft-delete)", id);
                    throw new EntityNotFoundException("Program", id);
                }

                if (!programToDeactivate.Active)
                {
                    _logger.LogInformation("El programa con ID {ProgramId} ya está inactivo.", id);
                    return;
                }

                programToDeactivate.Active = false;
                programToDeactivate.DeleteDate = DateTime.UtcNow;
                programToDeactivate.UpdateDate = DateTime.UtcNow;
                await _programData.UpdateAsync(programToDeactivate);

                _logger.LogInformation("Programa con ID {ProgramId} marcado como inactivo (soft-delete)", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete del programa con ID {ProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar el programa con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al realizar soft-delete del programa con ID {ProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar el programa con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateProgram(ProgramDto programDto)
        {
            if (programDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Program no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(programDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del programa es obligatorio");
            }
            if (programDto.CodeProgram <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa con CodeProgram inválido: {CodeProgram}", programDto.CodeProgram);
                throw new Utilities.Exceptions.ValidationException("CodeProgram", "El CodeProgram debe ser mayor que cero.");
            }
            if (string.IsNullOrWhiteSpace(programDto.TypeProgram))
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa con TypeProgram vacío");
                throw new Utilities.Exceptions.ValidationException("TypeProgram", "El TypeProgram es obligatorio");
            }
        }

        //Funciones de mapeos
        // Método para mapear de Program a ProgramDto
        private ProgramDto MapToDTO(Program program)
        {
            return new ProgramDto
            {
                Id = program.Id,
                Name = program.Name,
                Description = program.Description,
                CodeProgram = program.CodeProgram,
                TypeProgram = program.TypeProgram,
                Active = program.Active
            };
        }

        // Método para mapear de ProgramDto a Program (para creación)
        private Program MapToEntity(ProgramDto programDto)
        {
            return new Program
            {
                Name = programDto.Name,
                Description = programDto.Description,
                CodeProgram = programDto.CodeProgram,
                TypeProgram = programDto.TypeProgram,
                Active = programDto.Active
            };
        }

        // Método para mapear de DTO a una entidad existente (para actualización PUT)
        private Program MapToEntity(ProgramDto programDto, Program existingProgram)
        {
            existingProgram.Name = programDto.Name;
            existingProgram.CodeProgram = programDto.CodeProgram;
            existingProgram.TypeProgram = programDto.TypeProgram;
            existingProgram.Description = programDto.Description;
            existingProgram.Active = programDto.Active;

            if (existingProgram.Active && existingProgram.DeleteDate.HasValue)
            {
                existingProgram.DeleteDate = null;
            }
            else if (!existingProgram.Active && !existingProgram.DeleteDate.HasValue)
            {
                existingProgram.DeleteDate = DateTime.UtcNow;
            }

            return existingProgram;
        }

        // Método para mapear una lista de Program a una lista de ProgramDto
        private IEnumerable<ProgramDto> MapToDTOList(IEnumerable<Program> programs)
        {
            return programs.Select(MapToDTO).ToList();
        }
    }
}
