using Data;
using Entity.DTOautogestion;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los programas de instructores en el sistema.
    /// </summary>
    public class InstructorProgramBusiness
    {
        private readonly InstructorProgramData _instructorProgramData;
        private readonly ILogger<InstructorProgramBusiness> _logger;

        public InstructorProgramBusiness(InstructorProgramData instructorProgramData, ILogger<InstructorProgramBusiness> logger)
        {
            _instructorProgramData = instructorProgramData;
            _logger = logger;
        }

        // Método para obtener todos los programas de instructores como DTOs
        public async Task<IEnumerable<InstructorProgramDto>> GetAllInstructorProgramsAsync()
        {
            try
            {
                var instructorPrograms = await _instructorProgramData.GetAllAsync();
                return MapToDTOList(instructorPrograms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los programas de instructores");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de programas de instructores", ex);
            }
        }

        // Método para obtener un programa de instructor por ID como DTO
        public async Task<InstructorProgramDto> GetInstructorProgramByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un programa de instructor con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa de instructor debe ser mayor que cero");
            }

            try
            {
                var instructorProgram = await _instructorProgramData.GetByIdAsync(id);
                if (instructorProgram == null)
                {
                    _logger.LogInformation("No se encontró ningún programa de instructor con ID: {Id}", id);
                    throw new EntityNotFoundException("instructorProgram", id);
                }

                return MapToDTO(instructorProgram);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el programa de instructor con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el programa de instructor con ID {id}", ex);
            }
        }

        // Método para crear un programa de instructor desde un DTO
        public async Task<InstructorProgramDto> CreateInstructorProgramAsync(InstructorProgramDto instructorProgramDto)
        {
            try
            {
                ValidateInstructorProgram(instructorProgramDto);
                var instructorProgram = MapToEntity(instructorProgramDto);
                var instructorProgramCreado = await _instructorProgramData.CreateAsync(instructorProgram);
                return MapToDTO(instructorProgramCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo programa de instructor");
                throw new ExternalServiceException("Base de datos", "Error al crear el programa de instructor", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateInstructorProgram(InstructorProgramDto instructorProgramDto)
        {
            if (instructorProgramDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto InstructorProgram no puede ser nulo");
            }

            if (instructorProgramDto.InstructorId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa de instructor con InstructorId inválido");
                throw new Utilities.Exceptions.ValidationException("InstructorId", "El InstructorId es obligatorio y debe ser mayor que cero");
            }

            if (instructorProgramDto.ProgramId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa de instructor con ProgramId inválido");
                throw new Utilities.Exceptions.ValidationException("ProgramId", "El ProgramId es obligatorio y debe ser mayor que cero");
            }
        }

        // Método para actualizar una relación instructor-programa existente (reemplazo completo)
        public async Task<InstructorProgramDto> UpdateInstructorProgramAsync(int id, InstructorProgramDto instructorProgramDto)
        {
            if (id <= 0 || id != instructorProgramDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una relación instructor-programa con ID inválido o no coincidente: {InstructorProgramId}, DTO ID: {DtoId}", id, instructorProgramDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateInstructorProgram(instructorProgramDto); // Reutilizamos la validación

            try
            {
                var existingRelation = await _instructorProgramData.GetByIdAsync(id);
                if (existingRelation == null)
                {
                    _logger.LogInformation("No se encontró la relación instructor-programa con ID {InstructorProgramId} para actualizar", id);
                    throw new EntityNotFoundException("InstructorProgram", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingRelation.InstructorId = instructorProgramDto.InstructorId;
                existingRelation.ProgramId = instructorProgramDto.ProgramId;

                await _instructorProgramData.UpdateAsync(existingRelation);
                return MapToDTO(existingRelation);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Podría ser violación de FK 
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar la relación instructor-programa con ID {InstructorProgramId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación instructor-programa con ID {id}. Verifique la existencia de Instructor y Programa.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la relación instructor-programa con ID {InstructorProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación instructor-programa con ID {id}", ex);
            }
        }

        // Método para eliminar una relación instructor-programa (DELETE persistente)
        public async Task DeleteInstructorProgramAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar una relación instructor-programa con ID inválido: {InstructorProgramId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación instructor-programa debe ser mayor a 0");
            }
            try
            {
                 var existingRelation = await _instructorProgramData.GetByIdAsync(id); // Verificar existencia
                if (existingRelation == null)
                {
                     _logger.LogInformation("No se encontró la relación instructor-programa con ID {InstructorProgramId} para eliminar", id);
                    throw new EntityNotFoundException("InstructorProgram", id);
                }

                bool deleted = await _instructorProgramData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Relación instructor-programa con ID {InstructorProgramId} eliminada exitosamente", id);
                }
                else
                {
                     _logger.LogWarning("No se pudo eliminar la relación instructor-programa con ID {InstructorProgramId}.", id);
                    throw new EntityNotFoundException("InstructorProgram", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar la relación instructor-programa con ID {InstructorProgramId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar la relación instructor-programa con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de InstructorProgram a InstructorProgramDto
        private InstructorProgramDto MapToDTO(InstructorProgram instructorProgram)
        {
            return new InstructorProgramDto
            {
                Id = instructorProgram.Id,
                InstructorId = instructorProgram.InstructorId,
                ProgramId = instructorProgram.ProgramId,
            };
        }

        // Método para mapear de InstructorProgramDto a InstructorProgram
        private InstructorProgram MapToEntity(InstructorProgramDto instructorProgramDto)
        {
            return new InstructorProgram
            {
                Id = instructorProgramDto.Id,
                InstructorId = instructorProgramDto.InstructorId,
                ProgramId = instructorProgramDto.ProgramId
            };
        }

        // Método para mapear una lista de InstructorProgram a una lista de InstructorProgramDto
        private IEnumerable<InstructorProgramDto> MapToDTOList(IEnumerable<InstructorProgram> instructorPrograms)
        {
            var instructorProgramsDTO = new List<InstructorProgramDto>();
            foreach (var instructorProgram in instructorPrograms)
            {
                instructorProgramsDTO.Add(MapToDTO(instructorProgram));
            }
            return instructorProgramsDTO;
        }
    }
}
