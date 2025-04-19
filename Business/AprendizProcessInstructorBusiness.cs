using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con la relación entre Aprendiz, Proceso e Instructor en el sistema.
    /// </summary>
    public class AprendizProcessInstructorBusiness
    {
        private readonly AprendizProcessInstructorData _aprendizProcessInstructorData;
        private readonly ILogger<AprendizProcessInstructorBusiness> _logger;

        public AprendizProcessInstructorBusiness(AprendizProcessInstructorData aprendizProcessInstructorData, ILogger<AprendizProcessInstructorBusiness> logger)
        {
            _aprendizProcessInstructorData = aprendizProcessInstructorData;
            _logger = logger;
        }

        // Método para obtener todas las relaciones Aprendiz-Proceso-Instructor como DTOs
        public async Task<IEnumerable<AprendizProcessInstructorDto>> GetAllAprendizProcessInstructorsAsync()
        {
            try
            {
                var relaciones = await _aprendizProcessInstructorData.GetAllAsync();
                return MapToDTOList(relaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones Aprendiz-Proceso-Instructor");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de relaciones", ex);
            }
        }

        // Método para obtener una relación Aprendiz-Proceso-Instructor por ID como DTO
        public async Task<AprendizProcessInstructorDto> GetAprendizProcessInstructorByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una relación con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación debe ser mayor que cero");
            }

            try
            {
                var relacion = await _aprendizProcessInstructorData.GetByIdAsync(id);
                if (relacion == null)
                {
                    _logger.LogInformation("No se encontró ninguna relación con ID: {Id}", id);
                    throw new EntityNotFoundException("AprendizProcessInstructor", id);
                }

                return MapToDTO(relacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la relación con ID {id}", ex);
            }
        }

        // Método para crear una relación Aprendiz-Proceso-Instructor desde un DTO
        public async Task<AprendizProcessInstructorDto> CreateAprendizProcessInstructorAsync(AprendizProcessInstructorDto dto)
        {
            try
            {
                ValidateAprendizProcessInstructor(dto);
                var relacion = MapToEntity(dto);
                var creada = await _aprendizProcessInstructorData.CreateAsync(relacion);
                return MapToDTO(creada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva relación Aprendiz-Proceso-Instructor");
                throw new ExternalServiceException("Base de datos", "Error al crear la relación", ex);
            }
        }

        // Método para actualizar una relación existente (PUT)
        public async Task<AprendizProcessInstructorDto> UpdateAprendizProcessInstructorAsync(int id, AprendizProcessInstructorDto dto)
        {
            if (id <= 0 || id != dto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una relación con un ID invalido o no coincidente: {RelationId}, DTO ID: {DtoId}", id, dto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateAprendizProcessInstructor(dto);

            try
            {
                var existingRelacion = await _aprendizProcessInstructorData.GetByIdAsync(id);
                if (existingRelacion == null)
                {
                    _logger.LogInformation("No se encontró la relación con ID {RelationId} para actualizar", id);
                    throw new EntityNotFoundException("AprendizProcessInstructor", id);
                }

                existingRelacion = MapToEntity(dto, existingRelacion);

                await _aprendizProcessInstructorData.UpdateAsync(existingRelacion);
                return MapToDTO(existingRelacion);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al actualizar la relación con ID {RelationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la relación con ID {RelationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación con ID {id}", ex);
            }
        }

        // Método para eliminar una relación (DELETE persistente)
        public async Task DeleteAprendizProcessInstructorAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una relación con un ID invalido: {RelationId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación debe ser mayor a 0");
            }
            try
            {
                var existingRelacion = await _aprendizProcessInstructorData.GetByIdAsync(id);
                if (existingRelacion == null)
                {
                    _logger.LogInformation("No se encontró la relación con ID {RelationId} para eliminar", id);
                    throw new EntityNotFoundException("AprendizProcessInstructor", id);
                }

                bool deleted = await _aprendizProcessInstructorData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Relación AprendizProcessInstructor con ID {RelationId} eliminada exitosamente", id);
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar la relación con ID {RelationId}. Posiblemente no encontrada por la capa de datos.", id);
                    throw new EntityNotFoundException("AprendizProcessInstructor", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar la relación con ID {RelationId}. Posible violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la relación con ID {id}. Verifique dependencias.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error general al eliminar la relación con ID {RelationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la relación con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateAprendizProcessInstructor(AprendizProcessInstructorDto dto)
        {
            if (dto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto AprendizProcessInstructor no puede ser nulo");
            }

            if (dto.AprendizId <= 0 || dto.InstructorId <= 0 || dto.ProcessId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación con IDs principales inválidos");
                throw new Utilities.Exceptions.ValidationException("IDs", "Los IDs de Aprendiz, Instructor y Proceso deben ser mayores que cero");
            }
        }

        //Funciones de mapeos
        // Método para mapear de AprendizProcessInstructor a AprendizProcessInstructorDto
        private AprendizProcessInstructorDto MapToDTO(AprendizProcessInstructor relacion)
        {
            return new AprendizProcessInstructorDto
            {
                Id = relacion.Id,
                AprendizId = relacion.AprendizId,
                InstructorId = relacion.InstructorId,
                RegisterySofiaId = relacion.RegisterySofiaId,
                ConceptId = relacion.ConceptId,
                EnterpriseId = relacion.EnterpriseId,
                ProcessId = relacion.ProcessId,
                TypeModalityId = relacion.TypeModalityId,
                StateId = relacion.StateId,
                VerificationId = relacion.VerificationId
            };
        }

        // Método para mapear de AprendizProcessInstructorDto a AprendizProcessInstructor (para creación)
        private AprendizProcessInstructor MapToEntity(AprendizProcessInstructorDto dto)
        {
            return new AprendizProcessInstructor
            {
                Id = dto.Id,
                AprendizId = dto.AprendizId,
                InstructorId = dto.InstructorId,
                RegisterySofiaId = dto.RegisterySofiaId,
                ConceptId = dto.ConceptId,
                EnterpriseId = dto.EnterpriseId,
                ProcessId = dto.ProcessId,
                TypeModalityId = dto.TypeModalityId,
                StateId = dto.StateId,
                VerificationId = dto.VerificationId
            };
        }

        // Método para mapear de DTO a una entidad existente (para actualización)
        private AprendizProcessInstructor MapToEntity(AprendizProcessInstructorDto dto, AprendizProcessInstructor existingRelacion)
        {
            existingRelacion.AprendizId = dto.AprendizId;
            existingRelacion.InstructorId = dto.InstructorId;
            existingRelacion.RegisterySofiaId = dto.RegisterySofiaId;
            existingRelacion.ConceptId = dto.ConceptId;
            existingRelacion.EnterpriseId = dto.EnterpriseId;
            existingRelacion.ProcessId = dto.ProcessId;
            existingRelacion.TypeModalityId = dto.TypeModalityId;
            existingRelacion.StateId = dto.StateId;
            existingRelacion.VerificationId = dto.VerificationId;
            return existingRelacion;
        }

        // Método para mapear una lista de AprendizProcessInstructor a una lista de AprendizProcessInstructorDto
        private IEnumerable<AprendizProcessInstructorDto> MapToDTOList(IEnumerable<AprendizProcessInstructor> relaciones)
        {
            var relacionesDTO = new List<AprendizProcessInstructorDto>();
            foreach (var relacion in relaciones)
            {
                relacionesDTO.Add(MapToDTO(relacion));
            }
            return relacionesDTO;
        }
    }
}
