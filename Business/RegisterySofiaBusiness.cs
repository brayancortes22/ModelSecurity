using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los registros de Sofia en el sistema.
    /// </summary>
    public class RegisterySofiaBusiness
    {
        private readonly RegisterySofiaData _registerySofiaData;
        private readonly ILogger<RegisterySofiaBusiness> _logger;

        public RegisterySofiaBusiness(RegisterySofiaData registerySofiaData, ILogger<RegisterySofiaBusiness> logger)
        {
            _registerySofiaData = registerySofiaData;
            _logger = logger;
        }

        // Método para obtener todos los registros de Sofia como DTOs
        public async Task<IEnumerable<RegisterySofiaDto>> GetAllRegisterySofiasAsync()
        {
            try
            {
                var registerySofias = await _registerySofiaData.GetAllAsync();
                return MapToDTOList(registerySofias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de Sofia");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de registros de Sofia", ex);
            }
        }

        // Método para obtener un registro de Sofia por ID como DTO
        public async Task<RegisterySofiaDto> GetRegisterySofiaByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro de Sofia con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del registro de Sofia debe ser mayor que cero");
            }

            try
            {
                var registerySofia = await _registerySofiaData.GetByIdAsync(id);
                if (registerySofia == null)
                {
                    _logger.LogInformation("No se encontró ningún registro de Sofia con ID: {Id}", id);
                    throw new EntityNotFoundException("registerySofia", id);
                }

                return MapToDTO(registerySofia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de Sofia con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el registro de Sofia con ID {id}", ex);
            }
        }

        // Método para crear un registro de Sofia desde un DTO
        public async Task<RegisterySofiaDto> CreateRegisterySofiaAsync(RegisterySofiaDto registerySofiaDto)
        {
            try
            {
                ValidateRegisterySofia(registerySofiaDto);
                var registerySofia = MapToEntity(registerySofiaDto);
                var registerySofiaCreado = await _registerySofiaData.CreateAsync(registerySofia);
                return MapToDTO(registerySofiaCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo registro de Sofia: {Name}", registerySofiaDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el registro de Sofia", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRegisterySofia(RegisterySofiaDto registerySofiaDto)
        {
            if (registerySofiaDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto RegisterySofia no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(registerySofiaDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un registro de Sofia con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del registro de Sofia es obligatorio");
            }
        }

        // Método para actualizar un registro de Sofia existente (reemplazo completo)
        public async Task<RegisterySofiaDto> UpdateRegisterySofiaAsync(int id, RegisterySofiaDto registerySofiaDto)
        {
            if (id <= 0 || id != registerySofiaDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar un registro de Sofia con ID inválido o no coincidente: {RegisterySofiaId}, DTO ID: {DtoId}", id, registerySofiaDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateRegisterySofia(registerySofiaDto); // Reutilizamos la validación

            try
            {
                var existingRegistery = await _registerySofiaData.GetByIdAsync(id);
                if (existingRegistery == null)
                {
                    _logger.LogInformation("No se encontró el registro de Sofia con ID {RegisterySofiaId} para actualizar", id);
                    throw new EntityNotFoundException("RegisterySofia", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingRegistery.Name = registerySofiaDto.Name;
                existingRegistery.Description = registerySofiaDto.Description;
                existingRegistery.Document = registerySofiaDto.Document;
                existingRegistery.Active = registerySofiaDto.Active;

                await _registerySofiaData.UpdateAsync(existingRegistery);
                return MapToDTO(existingRegistery);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar el registro de Sofia con ID {RegisterySofiaId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar el registro de Sofia con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar el registro de Sofia con ID {RegisterySofiaId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el registro de Sofia con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente un registro de Sofia (PATCH)
        public async Task<RegisterySofiaDto> PatchRegisterySofiaAsync(int id, RegisterySofiaDto registerySofiaDto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a un registro de Sofia con ID inválido: {RegisterySofiaId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del registro de Sofia debe ser mayor que cero.");
            }

            try
            {
                var existingRegistery = await _registerySofiaData.GetByIdAsync(id);
                if (existingRegistery == null)
                {
                    _logger.LogInformation("No se encontró el registro de Sofia con ID {RegisterySofiaId} para aplicar patch", id);
                    throw new EntityNotFoundException("RegisterySofia", id);
                }

                bool updated = false;

                // Actualizar Name si se proporciona y es diferente
                if (!string.IsNullOrWhiteSpace(registerySofiaDto.Name) && registerySofiaDto.Name != existingRegistery.Name)
                {
                    existingRegistery.Name = registerySofiaDto.Name;
                    updated = true;
                }
                // Actualizar Description si se proporciona y es diferente (puede ser null)
                if (registerySofiaDto.Description != null && registerySofiaDto.Description != existingRegistery.Description)
                {
                     existingRegistery.Description = registerySofiaDto.Description;
                     updated = true;
                }
                // Actualizar Document si se proporciona y es diferente (puede ser null)
                 if (registerySofiaDto.Document != null && registerySofiaDto.Document != existingRegistery.Document)
                {
                     existingRegistery.Document = registerySofiaDto.Document;
                     updated = true;
                }
                // No actualizamos Active en PATCH

                if (updated)
                {
                    await _registerySofiaData.UpdateAsync(existingRegistery);
                     _logger.LogInformation("Patch aplicado al registro de Sofia con ID: {RegisterySofiaId}", id);
                }
                else
                {
                    _logger.LogInformation("No se realizaron cambios en el registro de Sofia con ID {RegisterySofiaId} durante el patch.", id);
                }

                return MapToDTO(existingRegistery);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch al registro de Sofia con ID {RegisterySofiaId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el registro de Sofia con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch al registro de Sofia con ID {RegisterySofiaId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el registro de Sofia con ID {id}", ex);
            }
        }

        // Método para eliminar un registro de Sofia (DELETE persistente)
        public async Task DeleteRegisterySofiaAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar un registro de Sofia con ID inválido: {RegisterySofiaId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del registro de Sofia debe ser mayor a 0");
            }
            try
            {
                 var existingRegistery = await _registerySofiaData.GetByIdAsync(id); // Verificar existencia
                if (existingRegistery == null)
                {
                     _logger.LogInformation("No se encontró el registro de Sofia con ID {RegisterySofiaId} para eliminar", id);
                    throw new EntityNotFoundException("RegisterySofia", id);
                }

                bool deleted = await _registerySofiaData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Registro de Sofia con ID {RegisterySofiaId} eliminado exitosamente", id);
                }
                else
                {
                     _logger.LogWarning("No se pudo eliminar el registro de Sofia con ID {RegisterySofiaId}.", id);
                    throw new EntityNotFoundException("RegisterySofia", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Capturar error si hay FKs apuntando a este registro
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar el registro de Sofia con ID {RegisterySofiaId}. Posible violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el registro de Sofia con ID {id}. Verifique dependencias.", dbEx);
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar el registro de Sofia con ID {RegisterySofiaId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar el registro de Sofia con ID {id}", ex);
            }
        }

        // Método para desactivar (eliminar lógicamente) un registro de Sofia
        public async Task SoftDeleteRegisterySofiaAsync(int id)
        {
             if (id <= 0)
            {
                 _logger.LogWarning("Se intentó realizar soft-delete a un registro de Sofia con ID inválido: {RegisterySofiaId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del registro de Sofia debe ser mayor a 0");
            }

             try
            {
                var existingRegistery = await _registerySofiaData.GetByIdAsync(id);
                if (existingRegistery == null)
                {
                    _logger.LogInformation("No se encontró el registro de Sofia con ID {RegisterySofiaId} para soft-delete", id);
                    throw new EntityNotFoundException("RegisterySofia", id);
                }

                 if (!existingRegistery.Active)
                {
                     _logger.LogInformation("El registro de Sofia con ID {RegisterySofiaId} ya se encuentra inactivo.", id);
                     return; 
                }

                existingRegistery.Active = false;
                await _registerySofiaData.UpdateAsync(existingRegistery); 
                 _logger.LogInformation("Registro de Sofia con ID {RegisterySofiaId} desactivado (soft-delete) exitosamente", id);
            }
             catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete del registro de Sofia con ID {RegisterySofiaId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar el registro de Sofia con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error general al realizar soft-delete del registro de Sofia con ID {RegisterySofiaId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar el registro de Sofia con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de RegisterySofia a RegisterySofiaDto
        private RegisterySofiaDto MapToDTO(RegisterySofia registerySofia)
        {
            return new RegisterySofiaDto
            {
                Id = registerySofia.Id,
                Name = registerySofia.Name,
                Description = registerySofia.Description,
                Document = registerySofia.Document,
                Active = registerySofia.Active // si existe la entidad
            };
        }

        // Método para mapear de RegisterySofiaDto a RegisterySofia
        private RegisterySofia MapToEntity(RegisterySofiaDto registerySofiaDto)
        {
            return new RegisterySofia
            {
                Id = registerySofiaDto.Id,
                Name = registerySofiaDto.Name,
                Description = registerySofiaDto.Description,
                Document = registerySofiaDto.Document,
                Active = registerySofiaDto.Active // si existe la entidad
            };
        }

        // Método para mapear una lista de RegisterySofia a una lista de RegisterySofiaDto
        private IEnumerable<RegisterySofiaDto> MapToDTOList(IEnumerable<RegisterySofia> registerySofias)
        {
            var registerySofiasDTO = new List<RegisterySofiaDto>();
            foreach (var registerySofia in registerySofias)
            {
                registerySofiasDTO.Add(MapToDTO(registerySofia));
            }
            return registerySofiasDTO;
        }
    }
}
