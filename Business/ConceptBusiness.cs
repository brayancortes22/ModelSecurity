using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los conceptos en el sistema.
    /// </summary>
    public class ConceptBusiness
    {
        private readonly ConceptData _conceptData;
        private readonly ILogger<ConceptBusiness> _logger;

        public ConceptBusiness(ConceptData conceptData, ILogger<ConceptBusiness> logger)
        {
            _conceptData = conceptData;
            _logger = logger;
        }

        // Método para obtener todos los conceptos como DTOs
        public async Task<IEnumerable<ConceptDto>> GetAllConceptsAsync()
        {
            try
            {
                var concepts = await _conceptData.GetAllAsync();
                return MapToDTOList(concepts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los conceptos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de conceptos", ex);
            }
        }

        // Método para obtener un concepto por ID como DTO
        public async Task<ConceptDto> GetConceptByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un concepto con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del concepto debe ser mayor que cero");
            }

            try
            {
                var concept = await _conceptData.GetByIdAsync(id);
                if (concept == null)
                {
                    _logger.LogInformation("No se encontró ningún concepto con ID: {Id}", id);
                    throw new EntityNotFoundException("concept", id);
                }

                return MapToDTO(concept);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el concepto con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el concepto con ID {id}", ex);
            }
        }

        // Método para crear un concepto desde un DTO
        public async Task<ConceptDto> CreateConceptAsync(ConceptDto conceptDto)
        {
            try
            {
                ValidateConcept(conceptDto);
                var concept = MapToEntity(conceptDto);
                concept.CreateDate = DateTime.UtcNow;
                var conceptCreado = await _conceptData.CreateAsync(concept);
                return MapToDTO(conceptCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo concepto: {Name}", conceptDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el concepto", ex);
            }
        }

        // Método para actualizar un concepto existente (PUT)
        public async Task UpdateConceptAsync(int id, ConceptDto conceptDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un concepto con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del concepto debe ser mayor que cero");
            }

            if (conceptDto == null || id != conceptDto.Id)
            {
                _logger.LogWarning("Datos inválidos para actualizar el concepto con ID: {Id}. DTO: {@ConceptDto}", id, conceptDto);
                throw new Utilities.Exceptions.ValidationException("Los datos proporcionados son inválidos para la actualización.");
            }

            ValidateConcept(conceptDto); // Reutilizar validación básica

            try
            {
                var existingConcept = await _conceptData.GetByIdAsync(id);
                if (existingConcept == null)
                {
                    _logger.LogInformation("No se encontró ningún concepto para actualizar con ID: {Id}", id);
                    throw new EntityNotFoundException("concept", id);
                }

                // Mapear todo el DTO a la entidad existente
                existingConcept.UpdateDate = DateTime.UtcNow;
                existingConcept = MapToEntity(conceptDto, existingConcept);

                await _conceptData.UpdateAsync(existingConcept);
                _logger.LogInformation("Concepto actualizado con ID: {Id}", id);
            }
            catch (EntityNotFoundException)
            {
                throw; // Re-lanzar para que el controlador lo maneje
            }
            catch (DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar el concepto con ID {Id}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar el concepto con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar el concepto con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el concepto con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente un concepto (PATCH)
        public async Task PatchConceptAsync(int id, ConceptDto conceptDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a un concepto con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del concepto debe ser mayor que cero");
            }

            if (conceptDto == null)
            {
                _logger.LogWarning("Patch DTO nulo para el concepto con ID: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("El objeto de patch no puede ser nulo.");
            }
            
            if (conceptDto.Id != 0 && id != conceptDto.Id)
            {
                 _logger.LogWarning("ID de ruta {RouteId} no coincide con ID de cuerpo {BodyId} en PATCH para Concepto", id, conceptDto.Id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID de la ruta no coincide con el ID del cuerpo.");
            }

            try
            {
                var existingConcept = await _conceptData.GetByIdAsync(id);
                if (existingConcept == null)
                {
                    _logger.LogInformation("No se encontró ningún concepto para aplicar patch con ID: {Id}", id);
                    throw new EntityNotFoundException("concept", id);
                }

                bool updated = false;
                existingConcept.UpdateDate = DateTime.UtcNow;

                // Aplicar cambios si los valores del DTO son diferentes a los existentes
                if (conceptDto.Name != null && existingConcept.Name != conceptDto.Name)
                {
                    existingConcept.Name = conceptDto.Name;
                    updated = true;
                }
                 if (conceptDto.Observation != null && existingConcept.Observation != conceptDto.Observation)
                {
                    existingConcept.Observation = conceptDto.Observation;
                    updated = true;
                }
                if (existingConcept.Active != conceptDto.Active)
                {
                    existingConcept.Active = conceptDto.Active;
                    updated = true;
                }

                if (updated)
                {
                    // Validar la entidad resultante después del patch
                    ValidateConcept(MapToDTO(existingConcept));
                    await _conceptData.UpdateAsync(existingConcept);
                    _logger.LogInformation("Concepto actualizado parcialmente (patch) con ID: {Id}", id);
                }
                else
                {
                     _logger.LogInformation("No se realizaron cambios en el concepto con ID {Id} durante el patch.", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw; 
            }
            catch (Utilities.Exceptions.ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al aplicar patch al concepto con ID: {Id}", id);
                 throw; 
            }
            catch (DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch al concepto con ID {Id}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el concepto con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch al concepto con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al concepto con ID {id}", ex);
            }
        }

        // Método para eliminar un concepto por ID (DELETE persistente)
        public async Task DeleteConceptAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un concepto con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del concepto debe ser mayor que cero");
            }

            try
            {
                var existingConcept = await _conceptData.GetByIdAsync(id);
                if (existingConcept == null)
                {
                    _logger.LogInformation("No se encontró ningún concepto para eliminar con ID: {Id}", id);
                    throw new EntityNotFoundException("concept", id);
                }

                bool deleted = await _conceptData.DeleteAsync(id);
                 if (!deleted)
                 {
                     // Esto no debería ocurrir si GetByIdAsync encontró la entidad, pero por si acaso.
                     _logger.LogWarning("No se pudo eliminar el concepto con ID {Id} desde la capa de datos.", id);
                     throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el concepto con ID {id}");
                 }
                _logger.LogInformation("Concepto eliminado con ID: {Id}", id);
            }
            catch (EntityNotFoundException)
            {
                throw; 
            }
             catch (DbUpdateException dbEx) // Capturar error si hay FKs
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar el concepto con ID {Id}. Posible violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el concepto con ID {id}. Verifique dependencias.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al eliminar el concepto con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el concepto con ID {id}", ex);
            }
        }

        // Método para realizar un borrado lógico de un concepto por ID (SOFT DELETE)
        public async Task SoftDeleteConceptAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar borrado lógico de un concepto con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del concepto debe ser mayor que cero");
            }

            try
            {
                 var existingConcept = await _conceptData.GetByIdAsync(id);
                if (existingConcept == null)
                {
                    _logger.LogInformation("No se encontró ningún concepto para borrado lógico con ID: {Id}", id);
                    throw new EntityNotFoundException("concept", id);
                }

                if (!existingConcept.Active)
                {
                    _logger.LogInformation("El concepto con ID {Id} ya está inactivo.", id);
                    // Considerar si lanzar excepción o no hacer nada
                    // throw new ValidationException($"El concepto con ID {id} ya está inactivo.");
                    return; 
                }

                existingConcept.DeleteDate = DateTime.UtcNow;
                existingConcept.Active = false; 
                await _conceptData.UpdateAsync(existingConcept); 
                _logger.LogInformation("Borrado lógico realizado para el concepto con ID: {Id}", id);
            }
            catch (EntityNotFoundException)
            {
                throw; 
            }
             catch (Utilities.Exceptions.ValidationException ex) // Captura si la lógica interna lanza validación
            {
                _logger.LogWarning(ex, "Validación fallida durante borrado lógico del concepto con ID: {Id}", id);
                throw;
            }
             catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar borrado lógico del concepto con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar borrado lógico del concepto con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al realizar borrado lógico del concepto con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar borrado lógico del concepto con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateConcept(ConceptDto conceptDto)
        {
            if (conceptDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Concept no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(conceptDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un concepto con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del concepto es obligatorio");
            }
        }

        //Funciones de mapeos 
        // Método para mapear de Concept a ConceptDto
        private ConceptDto MapToDTO(Concept concept)
        {
            return new ConceptDto
            {
                Id = concept.Id,
                Name = concept.Name,
                Observation = concept.Observation,
                Active = concept.Active
            };
        }

        // Método para mapear de ConceptDto a Concept
        private Concept MapToEntity(ConceptDto conceptDto)
        {
            return new Concept
            {
                Id = conceptDto.Id,
                Name = conceptDto.Name,
                Observation = conceptDto.Observation,
                Active = conceptDto.Active
            };
        }

        // Método para mapear una lista de Concept a una lista de ConceptDto
        private IEnumerable<ConceptDto> MapToDTOList(IEnumerable<Concept> concepts)
        {
            var conceptsDTO = new List<ConceptDto>();
            foreach (var concept in concepts)
            {
                conceptsDTO.Add(MapToDTO(concept));
            }
            return conceptsDTO;
        }

        // Método para mapear de DTO a una entidad existente (para actualización)
        private Concept MapToEntity(ConceptDto dto, Concept existingConcept)
        {
            existingConcept.Name = dto.Name;
            existingConcept.Observation = dto.Observation;
            existingConcept.Active = dto.Active;
            // No actualizamos el Id
            return existingConcept;
        }
    }
}
