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
    /// Clase de negocio encargada de la lógica relacionada con las regionales en el sistema.
    /// </summary>
    public class RegionalBusiness
    {
        private readonly RegionalData _regionalData;
        private readonly ILogger<RegionalBusiness> _logger;

        public RegionalBusiness(RegionalData regionalData, ILogger<RegionalBusiness> logger)
        {
            _regionalData = regionalData;
            _logger = logger;
        }

        // Método para obtener todas las regionales como DTOs
        public async Task<IEnumerable<RegionalDto>> GetAllRegionalsAsync()
        {
            try
            {
                var regionals = await _regionalData.GetAllAsync();
                return MapToDTOList(regionals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las regionales");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de regionales", ex);
            }
        }

        // Método para obtener una regional por ID como DTO
        public async Task<RegionalDto> GetRegionalByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una regional con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la regional debe ser mayor que cero");
            }

            try
            {
                var regional = await _regionalData.GetByIdAsync(id);
                if (regional == null)
                {
                    _logger.LogInformation("No se encontró ninguna regional con ID: {Id}", id);
                    throw new EntityNotFoundException("Regional", id);
                }

                return MapToDTO(regional);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la regional con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la regional con ID {id}", ex);
            }
        }

        // Método para crear una regional desde un DTO
        public async Task<RegionalDto> CreateRegionalAsync(RegionalDto regionalDto)
        {
            try
            {
                ValidateRegional(regionalDto);
                var regional = MapToEntity(regionalDto);
                regional.CreateDate = DateTime.UtcNow;
                var regionalCreado = await _regionalData.CreateAsync(regional);
                return MapToDTO(regionalCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva regional: {Name}", regionalDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la regional", ex);
            }
        }

        // Método para actualizar una regional existente (PUT)
        public async Task<RegionalDto> UpdateRegionalAsync(int id, RegionalDto regionalDto)
        {
            if (id <= 0 || id != regionalDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una regional con un ID invalido o no coincidente: {RegionalId}, DTO ID: {DtoId}", id, regionalDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateRegional(regionalDto);

            try
            {
                var existingRegional = await _regionalData.GetByIdAsync(id);
                if (existingRegional == null)
                {
                    _logger.LogInformation("No se encontró la regional con ID {RegionalId} para actualizar", id);
                    throw new EntityNotFoundException("Regional", id);
                }

                existingRegional = MapToEntity(regionalDto, existingRegional);
                existingRegional.UpdateDate = DateTime.UtcNow;

                await _regionalData.UpdateAsync(existingRegional);
                return MapToDTO(existingRegional);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al actualizar la regional con ID {RegionalId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la regional con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la regional con ID {RegionalId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la regional con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente una regional (PATCH)
        public async Task<RegionalDto> PatchRegionalAsync(int id, RegionalDto regionalDto)
        {
            if (id <= 0 || (regionalDto.Id != 0 && id != regionalDto.Id))
            {
                _logger.LogWarning("Se intentó aplicar patch a una regional con un ID invalido o no coincidente: {RegionalId}, DTO ID: {DtoId}", id, regionalDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado en la URL es inválido o no coincide con el ID del DTO (si se proporcionó) para PATCH.");
            }

            try
            {
                var existingRegional = await _regionalData.GetByIdAsync(id);
                if (existingRegional == null)
                {
                    _logger.LogInformation("No se encontró la regional con ID {RegionalId} para aplicar patch", id);
                    throw new EntityNotFoundException("Regional", id);
                }

                bool changed = false;
                if (regionalDto.Name != null && existingRegional.Name != regionalDto.Name)
                {
                    if (string.IsNullOrWhiteSpace(regionalDto.Name))
                        throw new Utilities.Exceptions.ValidationException("Name", "El Name no puede estar vacío en PATCH.");
                    existingRegional.Name = regionalDto.Name;
                    changed = true;
                }
                if (regionalDto.CodeRegional != null && existingRegional.CodeRegional != regionalDto.CodeRegional)
                {
                    if (string.IsNullOrWhiteSpace(regionalDto.CodeRegional))
                        throw new Utilities.Exceptions.ValidationException("CodeRegional", "El CodeRegional no puede estar vacío en PATCH.");
                    existingRegional.CodeRegional = regionalDto.CodeRegional;
                    changed = true;
                }
                if (regionalDto.Description != null && existingRegional.Description != regionalDto.Description)
                {
                    existingRegional.Description = regionalDto.Description;
                    changed = true;
                }
                if (regionalDto.Address != null && existingRegional.Address != regionalDto.Address)
                {
                    existingRegional.Address = regionalDto.Address;
                    changed = true;
                }
                if (existingRegional.Active != regionalDto.Active)
                {
                    existingRegional.Active = regionalDto.Active;
                    if (!regionalDto.Active)
                    {
                        existingRegional.DeleteDate = DateTime.UtcNow;
                    }
                    else
                    {
                        existingRegional.DeleteDate = null;
                    }
                    changed = true;
                }

                if (changed)
                {
                    existingRegional.UpdateDate = DateTime.UtcNow;
                    await _regionalData.UpdateAsync(existingRegional);
                    _logger.LogInformation("Aplicado patch a la regional con ID {RegionalId}", id);
                }
                else
                {
                    _logger.LogInformation("No se detectaron cambios en la solicitud PATCH para la regional con ID {RegionalId}", id);
                }

                return MapToDTO(existingRegional);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al aplicar patch a la regional con ID {RegionalId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch a la regional con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a la regional con ID {RegionalId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch a la regional con ID {id}", ex);
            }
        }

        // Método para eliminar una regional (DELETE persistente)
        public async Task DeleteRegionalAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una regional con un ID invalido: {RegionalId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la regional debe ser mayor a 0");
            }
            try
            {
                var existingRegional = await _regionalData.GetByIdAsync(id);
                if (existingRegional == null)
                {
                    _logger.LogInformation("No se encontró la regional con ID {RegionalId} para eliminar (persistente)", id);
                    throw new EntityNotFoundException("Regional", id);
                }

                bool deleted = await _regionalData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Regional con ID {RegionalId} eliminada exitosamente (persistente)", id);
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar (persistente) la regional con ID {RegionalId}. Posiblemente no encontrada por la capa de datos.", id);
                    throw new EntityNotFoundException("Regional", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar la regional con ID {RegionalId}. Posible violación de FK por Centers dependientes.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la regional con ID {id}. Verifique que no tenga Centros asociados.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al eliminar (persistente) la regional con ID {RegionalId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la regional con ID {id}", ex);
            }
        }

        // Método para eliminar lógicamente una regional (soft delete)
        public async Task SoftDeleteRegionalAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar soft-delete a una regional con un ID invalido: {RegionalId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la regional debe ser mayor a 0");
            }
            try
            {
                var regionalToDeactivate = await _regionalData.GetByIdAsync(id);
                if (regionalToDeactivate == null)
                {
                    _logger.LogInformation("No se encontró la regional con ID {RegionalId} para desactivar (soft-delete)", id);
                    throw new EntityNotFoundException("Regional", id);
                }

                if (!regionalToDeactivate.Active)
                {
                    _logger.LogInformation("La regional con ID {RegionalId} ya está inactiva.", id);
                    return;
                }

                regionalToDeactivate.Active = false;
                regionalToDeactivate.DeleteDate = DateTime.UtcNow;
               
                await _regionalData.UpdateAsync(regionalToDeactivate);

                _logger.LogInformation("Regional con ID {RegionalId} marcada como inactiva (soft-delete)", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete de la regional con ID {RegionalId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la regional con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al realizar soft-delete de la regional con ID {RegionalId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la regional con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRegional(RegionalDto regionalDto)
        {
            if (regionalDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Regional no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(regionalDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una regional con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la regional es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(regionalDto.CodeRegional))
            {
                _logger.LogWarning("Se intentó crear/actualizar una regional con CodeRegional vacío");
                throw new Utilities.Exceptions.ValidationException("CodeRegional", "El CodeRegional de la regional es obligatorio");
            }
        }

        //Funciones de mapeos
        // Método para mapear de Regional a RegionalDto
        private RegionalDto MapToDTO(Regional regional)
        {
            return new RegionalDto
            {
                Id = regional.Id,
                Name = regional.Name,
                Description = regional.Description,
                CodeRegional = regional.CodeRegional,
                Address = regional.Address,
                Active = regional.Active
            };
        }

        // Método para mapear de RegionalDto a Regional (para creación)
        private Regional MapToEntity(RegionalDto regionalDto)
        {
            return new Regional
            {
                Name = regionalDto.Name,
                Description = regionalDto.Description,
                CodeRegional = regionalDto.CodeRegional,
                Address = regionalDto.Address,
                Active = regionalDto.Active
            };
        }

        // Método para mapear de DTO a una entidad existente (para actualización PUT)
        private Regional MapToEntity(RegionalDto regionalDto, Regional existingRegional)
        {
            existingRegional.Name = regionalDto.Name;
            existingRegional.CodeRegional = regionalDto.CodeRegional;
            existingRegional.Description = regionalDto.Description;
            existingRegional.Address = regionalDto.Address;
            existingRegional.Active = regionalDto.Active;

            if (existingRegional.Active && existingRegional.DeleteDate.HasValue)
            {
                existingRegional.DeleteDate = null;
            }
            else if (!existingRegional.Active && !existingRegional.DeleteDate.HasValue)
            {
                existingRegional.DeleteDate = DateTime.UtcNow;
            }

            return existingRegional;
        }

        // Método para mapear una lista de Regional a una lista de RegionalDto
        private IEnumerable<RegionalDto> MapToDTOList(IEnumerable<Regional> regionals)
        {
            return regionals.Select(MapToDTO).ToList();
        }
    }
}
