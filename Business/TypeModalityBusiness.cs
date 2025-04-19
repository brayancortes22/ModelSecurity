using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los tipos de modalidades en el sistema.
    /// </summary>
    public class TypeModalityBusiness
    {
        private readonly TypeModalityData _typeModalityData;
        private readonly ILogger<TypeModalityBusiness> _logger;

        public TypeModalityBusiness(TypeModalityData typeModalityData, ILogger<TypeModalityBusiness> logger)
        {
            _typeModalityData = typeModalityData;
            _logger = logger;
        }

        // Método para obtener todas las modalidades como DTOs
        public async Task<IEnumerable<TypeModalityDto>> GetAllTypeModalitiesAsync()
        {
            try
            {
                var typeModalities = await _typeModalityData.GetAllAsync();
                return MapToDTOList(typeModalities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las modalidades");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de modalidades", ex);
            }
        }

        // Método para obtener una modalidad por ID como DTO
        public async Task<TypeModalityDto> GetTypeModalityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una modalidad con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la modalidad debe ser mayor que cero");
            }

            try
            {
                var typeModality = await _typeModalityData.GetByIdAsync(id);
                if (typeModality == null)
                {
                    _logger.LogInformation("No se encontró ninguna modalidad con ID: {Id}", id);
                    throw new EntityNotFoundException("typeModality", id);
                }

                return MapToDTO(typeModality);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la modalidad con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la modalidad con ID {id}", ex);
            }
        }

        // Método para crear una modalidad desde un DTO
        public async Task<TypeModalityDto> CreateTypeModalityAsync(TypeModalityDto typeModalityDto)
        {
            try
            {
                ValidateTypeModality(typeModalityDto);
                var typeModality = MapToEntity(typeModalityDto);
                var typeModalityCreado = await _typeModalityData.CreateAsync(typeModality);
                return MapToDTO(typeModalityCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva modalidad: {Name}", typeModalityDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la modalidad", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateTypeModality(TypeModalityDto typeModalityDto)
        {
            if (typeModalityDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto TypeModality no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(typeModalityDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una modalidad con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la modalidad es obligatorio");
            }
        }

        // Método para actualizar una modalidad existente (reemplazo completo)
        public async Task<TypeModalityDto> UpdateTypeModalityAsync(int id, TypeModalityDto typeModalityDto)
        {
            if (id <= 0 || id != typeModalityDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una modalidad con ID inválido o no coincidente: {ModalityId}, DTO ID: {DtoId}", id, typeModalityDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateTypeModality(typeModalityDto); // Reutilizamos la validación

            try
            {
                var existingModality = await _typeModalityData.GetByIdAsync(id);
                if (existingModality == null)
                {
                    _logger.LogInformation("No se encontró la modalidad con ID {ModalityId} para actualizar", id);
                    throw new EntityNotFoundException("TypeModality", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingModality.Name = typeModalityDto.Name;
                existingModality.Description = typeModalityDto.Description;
                existingModality.Active = typeModalityDto.Active; // Asegurarse de que Active se actualice

                await _typeModalityData.UpdateAsync(existingModality);
                return MapToDTO(existingModality);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar para que el controlador la maneje
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar la modalidad con ID {ModalityId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar la modalidad con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la modalidad con ID {ModalityId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la modalidad con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente una modalidad existente (PATCH)
        public async Task<TypeModalityDto> PatchTypeModalityAsync(int id, TypeModalityDto typeModalityDto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a una modalidad con ID inválido: {ModalityId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la modalidad debe ser mayor que cero.");
            }

            try
            {
                var existingModality = await _typeModalityData.GetByIdAsync(id);
                if (existingModality == null)
                {
                    _logger.LogInformation("No se encontró la modalidad con ID {ModalityId} para aplicar patch", id);
                    throw new EntityNotFoundException("TypeModality", id);
                }

                // Aplicar cambios parciales desde el DTO si los valores no son nulos/vacíos
                if (!string.IsNullOrWhiteSpace(typeModalityDto.Name))
                {
                    existingModality.Name = typeModalityDto.Name;
                }
                // Description puede ser nulo o vacío, así que actualizamos si se proporciona explícitamente
                if (typeModalityDto.Description != null)
                {
                     existingModality.Description = typeModalityDto.Description;
                }
                 // No actualizamos Active en PATCH usualmente, a menos que sea explícito.
                 // Si se necesita actualizar Active con PATCH, se podría añadir una lógica específica o requerir que el DTO incluya el campo.
                 // Por ahora, lo omitimos en la actualización parcial.

                await _typeModalityData.UpdateAsync(existingModality);
                return MapToDTO(existingModality);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch a la modalidad con ID {ModalityId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la modalidad con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a la modalidad con ID {ModalityId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la modalidad con ID {id}", ex);
            }
        }

        // Método para eliminar una modalidad (DELETE persistente)
        public async Task DeleteTypeModalityAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar una modalidad con ID inválido: {ModalityId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID de la modalidad debe ser mayor a 0");
            }
            try
            {
                 var existingModality = await _typeModalityData.GetByIdAsync(id);
                if (existingModality == null)
                {
                     _logger.LogInformation("No se encontró la modalidad con ID {ModalityId} para eliminar", id);
                    throw new EntityNotFoundException("TypeModality", id);
                }

                bool deleted = await _typeModalityData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Modalidad con ID {ModalityId} eliminada exitosamente", id);
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar la modalidad con ID {ModalityId}. Posiblemente no encontrada por la capa de datos.", id);
                     // Podríamos lanzar EntityNotFoundException aquí también si DeleteAsync devuelve false consistentemente cuando no encuentra la entidad.
                    throw new EntityNotFoundException("TypeModality", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Captura errores de FK
            {
                 _logger.LogError(dbEx, "Error de base de datos al eliminar la modalidad con ID {ModalityId}. Posible violación de FK.", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar la modalidad con ID {id}. Verifique dependencias.", dbEx);
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar la modalidad con ID {ModalityId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar la modalidad con ID {id}", ex);
            }
        }

         // Método para desactivar (eliminar lógicamente) una modalidad
        public async Task SoftDeleteTypeModalityAsync(int id)
        {
             if (id <= 0)
            {
                 _logger.LogWarning("Se intentó realizar soft-delete a una modalidad con ID inválido: {ModalityId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID de la modalidad debe ser mayor a 0");
            }

             try
            {
                var existingModality = await _typeModalityData.GetByIdAsync(id);
                if (existingModality == null)
                {
                    _logger.LogInformation("No se encontró la modalidad con ID {ModalityId} para soft-delete", id);
                    throw new EntityNotFoundException("TypeModality", id);
                }

                 if (!existingModality.Active) // Si ya está inactivo, no hacer nada o loggear
                {
                     _logger.LogInformation("La modalidad con ID {ModalityId} ya se encuentra inactiva.", id);
                     return; // O podrías lanzar una excepción si prefieres indicar que la operación no cambió el estado.
                }

                existingModality.Active = false;
                await _typeModalityData.UpdateAsync(existingModality); // Usar Update para cambiar el estado Active
                 _logger.LogInformation("Modalidad con ID {ModalityId} desactivada (soft-delete) exitosamente", id);
            }
             catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete de la modalidad con ID {ModalityId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar la modalidad con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error general al realizar soft-delete de la modalidad con ID {ModalityId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar la modalidad con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de TypeModality a TypeModalityDto
        private TypeModalityDto MapToDTO(TypeModality typeModality)
        {
            return new TypeModalityDto
            {
                Id = typeModality.Id,
                Name = typeModality.Name,
                Description = typeModality.Description,
                Active = typeModality.Active // si existe la entidad
            };
        }

        // Método para mapear de TypeModalityDto a TypeModality
        private TypeModality MapToEntity(TypeModalityDto typeModalityDto)
        {
            return new TypeModality
            {
                Id = typeModalityDto.Id,
                Name = typeModalityDto.Name,
                Description = typeModalityDto.Description,
                Active = typeModalityDto.Active // si existe la entidad
            };
        }

        // Método para mapear una lista de TypeModality a una lista de TypeModalityDto
        private IEnumerable<TypeModalityDto> MapToDTOList(IEnumerable<TypeModality> typeModalities)
        {
            var typeModalitiesDTO = new List<TypeModalityDto>();
            foreach (var typeModality in typeModalities)
            {
                typeModalitiesDTO.Add(MapToDTO(typeModality));
            }
            return typeModalitiesDTO;
        }
    }
}
