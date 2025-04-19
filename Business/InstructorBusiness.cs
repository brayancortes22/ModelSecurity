using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los instructores en el sistema.
    /// </summary>
    public class InstructorBusiness
    {
        private readonly InstructorData _instructorData;
        private readonly ILogger<InstructorBusiness> _logger;

        public InstructorBusiness(InstructorData instructorData, ILogger<InstructorBusiness> logger)
        {
            _instructorData = instructorData;
            _logger = logger;
        }

        // Método para obtener todos los instructores como DTOs
        public async Task<IEnumerable<InstructorDto>> GetAllInstructorsAsync()
        {
            try
            {
                var instructors = await _instructorData.GetAllAsync();
                return MapToDTOList(instructors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los instructores");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de instructores", ex);
            }
        }

        // Método para obtener un instructor por ID como DTO
        public async Task<InstructorDto> GetInstructorByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un instructor con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del instructor debe ser mayor que cero");
            }

            try
            {
                var instructor = await _instructorData.GetByIdAsync(id);
                if (instructor == null)
                {
                    _logger.LogInformation("No se encontró ningún instructor con ID: {Id}", id);
                    throw new EntityNotFoundException("instructor", id);
                }

                return MapToDTO(instructor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el instructor con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el instructor con ID {id}", ex);
            }
        }

        // Método para crear un instructor desde un DTO
        public async Task<InstructorDto> CreateInstructorAsync(InstructorDto instructorDto)
        {
            try
            {
                ValidateInstructor(instructorDto);
                var instructor = MapToEntity(instructorDto);
                var instructorCreado = await _instructorData.CreateAsync(instructor);
                return MapToDTO(instructorCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo instructor");
                throw new ExternalServiceException("Base de datos", "Error al crear el instructor", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateInstructor(InstructorDto instructorDto)
        {
            if (instructorDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Instructor no puede ser nulo");
            }

            if (instructorDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un instructor con UserId inválido");
                throw new Utilities.Exceptions.ValidationException("UserId", "El UserId del instructor es obligatorio y debe ser mayor que cero");
            }
        }

        // Método para actualizar un instructor existente (reemplazo completo)
        public async Task<InstructorDto> UpdateInstructorAsync(int id, InstructorDto instructorDto)
        {
            if (id <= 0 || id != instructorDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar un instructor con ID inválido o no coincidente: {InstructorId}, DTO ID: {DtoId}", id, instructorDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateInstructor(instructorDto); // Reutilizamos la validación

            try
            {
                var existingInstructor = await _instructorData.GetByIdAsync(id);
                if (existingInstructor == null)
                {
                    _logger.LogInformation("No se encontró el instructor con ID {InstructorId} para actualizar", id);
                    throw new EntityNotFoundException("Instructor", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingInstructor.UserId = instructorDto.UserId;
                existingInstructor.Active = instructorDto.Active;

                await _instructorData.UpdateAsync(existingInstructor);
                return MapToDTO(existingInstructor);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Podría ser violación de FK si UserId no existe
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar el instructor con ID {InstructorId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar el instructor con ID {id}. Verifique la existencia del Usuario.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar el instructor con ID {InstructorId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el instructor con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente un instructor (PATCH)
        // Permite cambiar el UserId asociado o el estado Active.
        public async Task<InstructorDto> PatchInstructorAsync(int id, InstructorDto instructorDto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a un instructor con ID inválido: {InstructorId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del instructor debe ser mayor que cero.");
            }

            try
            {
                var existingInstructor = await _instructorData.GetByIdAsync(id);
                if (existingInstructor == null)
                {
                    _logger.LogInformation("No se encontró el instructor con ID {InstructorId} para aplicar patch", id);
                    throw new EntityNotFoundException("Instructor", id);
                }

                bool updated = false;

                // Actualizar UserId si se proporciona y es diferente
                if (instructorDto.UserId > 0 && instructorDto.UserId != existingInstructor.UserId)
                {
                    existingInstructor.UserId = instructorDto.UserId;
                    updated = true;
                }
                
                // Actualizar Active si se proporciona explícitamente en el DTO y es diferente
                // Nota: Comparar directamente booleanos.
                if (instructorDto.Active != existingInstructor.Active) 
                {
                    // Si se incluye explícitamente en el DTO para PATCH, actualizamos Active.
                    // Podríamos tener una validación adicional si solo queremos permitir ciertos cambios vía PATCH.
                    existingInstructor.Active = instructorDto.Active;
                    updated = true;
                }

                if (updated)
                {
                    await _instructorData.UpdateAsync(existingInstructor);
                    _logger.LogInformation("Patch aplicado al instructor con ID: {InstructorId}", id);
                }
                else
                {
                    _logger.LogInformation("No se realizaron cambios en el instructor con ID {InstructorId} durante el patch.", id);
                }

                return MapToDTO(existingInstructor);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch al instructor con ID {InstructorId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el instructor con ID {id}. Verifique la existencia del Usuario.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch al instructor con ID {InstructorId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el instructor con ID {id}", ex);
            }
        }

        // Método para eliminar un instructor (DELETE persistente)
        public async Task DeleteInstructorAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar un instructor con ID inválido: {InstructorId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del instructor debe ser mayor a 0");
            }
            try
            {
                 var existingInstructor = await _instructorData.GetByIdAsync(id); // Verificar existencia
                if (existingInstructor == null)
                {
                     _logger.LogInformation("No se encontró el instructor con ID {InstructorId} para eliminar", id);
                    throw new EntityNotFoundException("Instructor", id);
                }

                bool deleted = await _instructorData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Instructor con ID {InstructorId} eliminado exitosamente", id);
                }
                else
                {
                     _logger.LogWarning("No se pudo eliminar el instructor con ID {InstructorId}.", id);
                    throw new EntityNotFoundException("Instructor", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Capturar error si hay FKs
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar el instructor con ID {InstructorId}. Posible violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el instructor con ID {id}. Verifique dependencias.", dbEx);
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar el instructor con ID {InstructorId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar el instructor con ID {id}", ex);
            }
        }

        // Método para desactivar (eliminar lógicamente) un instructor
        public async Task SoftDeleteInstructorAsync(int id)
        {
             if (id <= 0)
            {
                 _logger.LogWarning("Se intentó realizar soft-delete a un instructor con ID inválido: {InstructorId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del instructor debe ser mayor a 0");
            }

             try
            {
                var existingInstructor = await _instructorData.GetByIdAsync(id);
                if (existingInstructor == null)
                {
                    _logger.LogInformation("No se encontró el instructor con ID {InstructorId} para soft-delete", id);
                    throw new EntityNotFoundException("Instructor", id);
                }

                 if (!existingInstructor.Active)
                {
                     _logger.LogInformation("El instructor con ID {InstructorId} ya se encuentra inactivo.", id);
                     return; 
                }

                existingInstructor.Active = false;
                await _instructorData.UpdateAsync(existingInstructor); 
                 _logger.LogInformation("Instructor con ID {InstructorId} desactivado (soft-delete) exitosamente", id);
            }
             catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete del instructor con ID {InstructorId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar el instructor con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error general al realizar soft-delete del instructor con ID {InstructorId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar el instructor con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de Instructor a InstructorDto
        private InstructorDto MapToDTO(Instructor instructor)
        {
            return new InstructorDto
            {
                Id = instructor.Id,
                Active = instructor.Active,
                UserId = instructor.UserId // Relación con la entidad User
            };
        }

        // Método para mapear de InstructorDto a Instructor
        private Instructor MapToEntity(InstructorDto instructorDto)
        {
            return new Instructor
            {
                Id = instructorDto.Id,
                Active = instructorDto.Active,
                UserId = instructorDto.UserId // Relación con la entidad User
            };
        }

        // Método para mapear una lista de Instructor a una lista de InstructorDto
        private IEnumerable<InstructorDto> MapToDTOList(IEnumerable<Instructor> instructors)
        {
            var instructorsDTO = new List<InstructorDto>();
            foreach (var instructor in instructors)
            {
                instructorsDTO.Add(MapToDTO(instructor));
            }
            return instructorsDTO;
        }
    }
}
