using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las verificaciones del sistema.
    /// </summary>
    public class VerificationBusiness
    {
        private readonly VerificationData _verificationData;
        private readonly ILogger<VerificationBusiness> _logger;

        public VerificationBusiness(VerificationData verificationData, ILogger<VerificationBusiness> logger)
        {
            _verificationData = verificationData;
            _logger = logger;
        }

        // Método para obtener todos las verificaciones como DTOs
        public async Task<IEnumerable<VerificationDto>> GetAllVerificationsAsync()
        {
            try
            {
                var verifications = await _verificationData.GetAllAsync();
                return MapToDTOList(verifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos las verificaciones ");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de verificaciones", ex);
            }
        }

        // Método para obtener una verificacion por ID como DTO
        public async Task<VerificationDto> GetVerificationByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una verificacion con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la verificacion debe ser mayor que cero");
            }

            try
            {
                var verification = await _verificationData.GetByIdAsync(id);
                if (verification == null)
                {
                    _logger.LogInformation("No se encontró ninguna verificacion con ID: {Id}", id);
                    throw new EntityNotFoundException("verification", id);
                }

                return MapToDTO(verification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la verificacion con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la verificacion con ID {id}", ex);
            }
        }

        // Método para crear una verificacion desde un DTO
        public async Task<VerificationDto> CreateVerificationAsync(VerificationDto verificationDto)
        {
            try
            {
                ValidateVerification(verificationDto);
                var verification = MapToEntity(verificationDto);
                verification.CreateDate = DateTime.UtcNow;
                var verificationCreado = await _verificationData.CreateAsync(verification);
                return MapToDTO(verificationCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva verificacion: {Name}", verificationDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la verificacion", ex);
            }
        }

        // Método para actualizar una verificacion
        // Modificado para aceptar ID y DTO, y devolver DTO
        public async Task<VerificationDto> UpdateVerificationAsync(int id, VerificationDto verificationDto)
        {
            if (id <= 0 || id != verificationDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una verificación con ID inválido o no coincidente: {VerificationId}, DTO ID: {DtoId}", id, verificationDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            try
            {
                ValidateVerification(verificationDto);
                var verification = await _verificationData.GetByIdAsync(id); // Usar id del parámetro
                if (verification == null)
                {
                    _logger.LogInformation("No se encontró ninguna verificacion para actualizar con ID: {Id}", id);
                    throw new EntityNotFoundException("verification", id);
                }

                // Mapear DTO a entidad existente
                verification.Name = verificationDto.Name;
                verification.Observation = verificationDto.Observation;
                verification.Active = verificationDto.Active; // Actualizar Active en PUT

                verification.UpdateDate = DateTime.UtcNow;
                await _verificationData.UpdateAsync(verification);
                _logger.LogInformation("Verificacion actualizada con ID: {Id}", id);
                return MapToDTO(verification); // Devolver el DTO actualizado
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al actualizar la verificacion con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la verificacion con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la verificacion con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la verificacion con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente una verificación (PATCH)
        public async Task<VerificationDto> PatchVerificationAsync(int id, VerificationDto verificationDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a una verificación con ID inválido: {VerificationId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la verificación debe ser mayor que cero.");
            }

            try
            {
                var existingVerification = await _verificationData.GetByIdAsync(id);
                if (existingVerification == null)
                {
                    _logger.LogInformation("No se encontró la verificación con ID {VerificationId} para aplicar patch", id);
                    throw new EntityNotFoundException("Verification", id);
                }

                existingVerification.UpdateDate = DateTime.UtcNow;
                bool updated = false;
                

                // Actualizar Name si se proporciona y es diferente
                if (!string.IsNullOrWhiteSpace(verificationDto.Name) && verificationDto.Name != existingVerification.Name)
                {
                    existingVerification.Name = verificationDto.Name;
                    updated = true;
                }

                // Actualizar Observation si se proporciona y es diferente
                if (verificationDto.Observation != null && verificationDto.Observation != existingVerification.Observation)
                {
                    existingVerification.Observation = verificationDto.Observation;
                    updated = true;
                }
                // No actualizamos 'Active' en PATCH. Para eso está SoftDelete.

                if (updated)
                {
                    await _verificationData.UpdateAsync(existingVerification);
                    _logger.LogInformation("Patch aplicado a verificación con ID: {VerificationId}", id);
                }
                else
                {
                    _logger.LogInformation("No se realizaron cambios en la verificación con ID {VerificationId} durante el patch.", id);
                }

                return MapToDTO(existingVerification);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch a la verificacion con ID {VerificationId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la verificacion con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a la verificacion con ID {VerificationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la verificacion con ID {id}", ex);
            }
        }

        // Método para eliminar una verificacion permanentemente
        public async Task DeleteVerificationAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una verificacion con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la verificacion debe ser mayor que cero");
            }
            try
            {
                var verification = await _verificationData.GetByIdAsync(id);
                if (verification == null)
                {
                    _logger.LogInformation("No se encontró ninguna verificacion para eliminar con ID: {Id}", id);
                    throw new EntityNotFoundException("verification", id);
                }
                await _verificationData.DeleteAsync(id);
                _logger.LogInformation("Verificacion eliminada permanentemente con ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la verificacion con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la verificacion con ID {id}", ex);
            }
        }

        // Método para realizar borrado lógico de una verificacion
        public async Task LogicalDeleteVerificationAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar borrado lógico a una verificacion con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la verificacion debe ser mayor que cero");
            }
            try
            {
                var verification = await _verificationData.GetByIdAsync(id);
                if (verification == null)
                {
                    _logger.LogInformation("No se encontró ninguna verificacion para borrado lógico con ID: {Id}", id);
                    throw new EntityNotFoundException("verification", id);
                }

                // Marcar como inactivo (o lógica de borrado lógico)
                verification.DeleteDate = DateTime.UtcNow;
                verification.Active = false; // Asume que la entidad tiene una propiedad 'Active'
                await _verificationData.UpdateAsync(verification); // Reutiliza UpdateAsync para marcar como inactivo
                _logger.LogInformation("Borrado lógico realizado para verificacion con ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar borrado lógico de la verificacion con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar borrado lógico de la verificacion con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateVerification(VerificationDto verificationDto)
        {
            if (verificationDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto verificacion no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(verificationDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una verificacion con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la verificacion es obligatorio");
            }
        }

        //Funciones de mapeos 
        // Método para mapear de Verification a VerificationDto
        private VerificationDto MapToDTO(Verification verification)
        {
            return new VerificationDto
            {
                Id = verification.Id,
                Name = verification.Name,
                Observation = verification.Observation,
                Active = verification.Active //si existe la entidad
            };
        }

        // Método para mapear de VerificationDto a Verification
        private Verification MapToEntity(VerificationDto verificationDto)
        {
            return new Verification
            {
                Id = verificationDto.Id,
                Name = verificationDto.Name,
                Observation = verificationDto.Observation,
                Active = verificationDto.Active //si existe la entidad
            };
        }

        // Método para mapear una lista de Verification a una lista de VerificationDto
        private IEnumerable<VerificationDto> MapToDTOList(IEnumerable<Verification> verifications)
        {
            var verificationsDTO = new List<VerificationDto>();
            foreach (var verification in verifications)
            {
                verificationsDTO.Add(MapToDTO(verification));
            }
            return verificationsDTO;
        }
    }
}
