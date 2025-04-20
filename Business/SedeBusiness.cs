using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.EntityFrameworkCore; // Para DbUpdateException
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Para validación Email
using System.Linq;
using System.Text.RegularExpressions; // Si se usa Regex para Email/Phone
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las sedes en el sistema.
    /// </summary>
    public class SedeBusiness
    {
        private readonly SedeData _sedeData;
        private readonly ILogger<SedeBusiness> _logger;

        // Helper para validar email (movido aquí para disponibilidad)
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true; // Email es opcional, pero si se da, debe ser válido
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public SedeBusiness(SedeData sedeData, ILogger<SedeBusiness> logger)
        {
            _sedeData = sedeData;
            _logger = logger;
        }

        // Método para obtener todas las sedes como DTOs
        public async Task<IEnumerable<SedeDto>> GetAllSedesAsync()
        {
            try
            {
                var sedes = await _sedeData.GetAllAsync();
                return MapToDTOList(sedes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las sedes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de sedes", ex);
            }
        }

        // Método para obtener una sede por ID como DTO
        public async Task<SedeDto> GetSedeByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una sede con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede debe ser mayor que cero");
            }

            try
            {
                var sede = await _sedeData.GetByIdAsync(id);
                if (sede == null)
                {
                    _logger.LogInformation("No se encontró ninguna sede con ID: {Id}", id);
                    throw new EntityNotFoundException("Sede", id);
                }

                return MapToDTO(sede);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sede con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la sede con ID {id}", ex);
            }
        }

        // Método para crear una sede desde un DTO
        public async Task<SedeDto> CreateSedeAsync(SedeDto sedeDto)
        {
            try
            {
                ValidateSede(sedeDto); // Validar datos
                var sede = MapToEntity(sedeDto);
                sede.CreateDate = DateTime.UtcNow; // Establecer fecha creación
                sede.Active = true; // Activa por defecto al crear

                var sedeCreada = await _sedeData.CreateAsync(sede);
                return MapToDTO(sedeCreada);
            }
            catch (DbUpdateException dbEx) // Capturar posible error de FK o UNIQUE
            {
                 _logger.LogError(dbEx, "Error de base de datos al crear sede. ¿CenterId válido? ¿CodeSede duplicado?");
                 throw new ExternalServiceException("Base de datos", "Error al crear la sede. Verifique que el CenterId sea válido y que el CodeSede no esté duplicado.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al crear nueva sede: {Name}", sedeDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la sede", ex);
            }
        }

        // Método para actualizar una sede existente (PUT)
        public async Task<SedeDto> UpdateSedeAsync(int id, SedeDto sedeDto)
        {
            if (id <= 0 || id != sedeDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una sede con un ID inválido o no coincidente: {SedeId}, DTO ID: {DtoId}", id, sedeDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateSede(sedeDto); // Validar datos completos

            try
            {
                var existingSede = await _sedeData.GetByIdAsync(id);
                if (existingSede == null)
                {
                    _logger.LogInformation("No se encontró la sede con ID {SedeId} para actualizar", id);
                    throw new EntityNotFoundException("Sede", id);
                }

                // Mapea los cambios del DTO a la entidad existente
                existingSede = MapToEntity(sedeDto, existingSede);
                existingSede.UpdateDate = DateTime.UtcNow; // Actualizar fecha modificación

                await _sedeData.UpdateAsync(existingSede); // Asume que UpdateAsync existe
                return MapToDTO(existingSede);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx) // Posible FK inválida (CenterId) o UNIQUE (CodeSede)
            {
                _logger.LogError(dbEx, "Error de base de datos al actualizar sede {SedeId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar la sede con ID {id}. Verifique CenterId y CodeSede.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar sede {SedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la sede con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente una sede (PATCH)
        public async Task<SedeDto> PatchSedeAsync(int id, SedeDto sedeDto)
        {
             if (id <= 0 || (sedeDto.Id != 0 && id != sedeDto.Id))
            {
                _logger.LogWarning("Se intentó aplicar patch a una sede con un ID inválido o no coincidente: {SedeId}, DTO ID: {DtoId}", id, sedeDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado en la URL es inválido o no coincide con el ID del DTO (si se proporcionó) para PATCH.");
            }

            try
            {
                var existingSede = await _sedeData.GetByIdAsync(id);
                if (existingSede == null)
                {
                    _logger.LogInformation("No se encontró la sede con ID {SedeId} para aplicar patch", id);
                    throw new EntityNotFoundException("Sede", id);
                }

                bool changed = false;

                // Aplicar cambios parciales si los valores del DTO son diferentes y válidos
                if (sedeDto.Name != null && existingSede.Name != sedeDto.Name)
                {
                    if (string.IsNullOrWhiteSpace(sedeDto.Name)) throw new Utilities.Exceptions.ValidationException("Name","El Name no puede ser vacío en PATCH.");
                    existingSede.Name = sedeDto.Name; changed = true;
                }
                if (sedeDto.CodeSede != null && existingSede.CodeSede != sedeDto.CodeSede)
                {
                     // Podría validar formato/longitud de CodeSede
                    existingSede.CodeSede = sedeDto.CodeSede; changed = true;
                }
                if (sedeDto.Address != null && existingSede.Address != sedeDto.Address)
                {
                     existingSede.Address = sedeDto.Address; changed = true;
                }
                 if (sedeDto.PhoneSede != null && existingSede.PhoneSede != sedeDto.PhoneSede)
                {
                    // Podría validar formato de PhoneSede
                    existingSede.PhoneSede = sedeDto.PhoneSede; changed = true;
                }
                if (sedeDto.EmailContact != null && existingSede.EmailContact != sedeDto.EmailContact)
                {
                    if (!IsValidEmail(sedeDto.EmailContact)) throw new Utilities.Exceptions.ValidationException("EmailContact", "EmailContact no válido en PATCH.");
                    existingSede.EmailContact = sedeDto.EmailContact; changed = true;
                }
                if (sedeDto.CenterId > 0 && existingSede.CenterId != sedeDto.CenterId)
                {
                     // Validar existencia de CenterId podría hacerse aquí o confiar en la BD
                    existingSede.CenterId = sedeDto.CenterId; changed = true;
                }
                // Para Active, si viene en el DTO (que siempre lo hará con SedeDto), aplicamos si es diferente.
                if (existingSede.Active != sedeDto.Active)
                {
                    existingSede.Active = sedeDto.Active;
                    existingSede.DeleteDate = sedeDto.Active ? (DateTime?)null : DateTime.UtcNow; // Actualizar DeleteDate
                    changed = true;
                }

                if (changed)
                {
                    existingSede.UpdateDate = DateTime.UtcNow;
                    await _sedeData.UpdateAsync(existingSede);
                    _logger.LogInformation("Aplicado patch a la sede con ID {SedeId}", id);
                }
                else
                {
                    _logger.LogInformation("No se detectaron cambios en la solicitud PATCH para la sede con ID {SedeId}", id);
                }

                return MapToDTO(existingSede);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al aplicar patch a sede {SedeId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al aplicar patch a la sede con ID {id}. Verifique FKs y UNIQUE constraints.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a sede {SedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch a la sede con ID {id}", ex);
            }
        }

        // Método para eliminar una sede (DELETE persistente)
        // ADVERTENCIA: Puede fallar si existen UserSedes asociados.
        public async Task DeleteSedeAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una sede con un ID inválido: {SedeId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede debe ser mayor a 0");
            }
            try
            {
                var existingSede = await _sedeData.GetByIdAsync(id);
                if (existingSede == null)
                {
                    _logger.LogInformation("No se encontró la sede con ID {SedeId} para eliminar (persistente)", id);
                    throw new EntityNotFoundException("Sede", id);
                }

                // ADVERTENCIA: Fallará si existen UserSedes asociados.
                bool deleted = await _sedeData.DeleteAsync(id); // Asume DeleteAsync existe en SedeData
                if (deleted)
                {
                    _logger.LogInformation("Sede con ID {SedeId} eliminada exitosamente (persistente)", id);
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar (persistente) la sede con ID {SedeId}. Posiblemente no encontrada por Data.", id);
                    throw new EntityNotFoundException("Sede", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx) // Captura FK violation
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar la sede {SedeId}. Posible FK con UserSede.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la sede con ID {id}. Verifique si tiene usuarios asociados.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al eliminar (persistente) la sede {SedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la sede con ID {id}", ex);
            }
        }

        // Método para eliminar lógicamente una sede (soft delete)
        public async Task SoftDeleteSedeAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar soft-delete a una sede con un ID inválido: {SedeId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede debe ser mayor a 0");
            }
            try
            {
                var sedeToDeactivate = await _sedeData.GetByIdAsync(id);
                if (sedeToDeactivate == null)
                {
                    _logger.LogInformation("No se encontró la sede con ID {SedeId} para desactivar (soft-delete)", id);
                    throw new EntityNotFoundException("Sede", id);
                }

                if (!sedeToDeactivate.Active)
                {
                    _logger.LogInformation("La sede con ID {SedeId} ya está inactiva.", id);
                    return; // No hacer nada si ya está inactivo
                }

                sedeToDeactivate.Active = false;
                sedeToDeactivate.DeleteDate = DateTime.UtcNow;
                
                await _sedeData.UpdateAsync(sedeToDeactivate);

                _logger.LogInformation("Sede con ID {SedeId} marcada como inactiva (soft-delete)", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx) // Poco probable aquí, pero posible
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete de la sede {SedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la sede con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al realizar soft-delete de la sede {SedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la sede con ID {id}", ex);
            }
        }

        // Método para validar el DTO (modificado)
        private void ValidateSede(SedeDto sedeDto)
        {
            if (sedeDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Sede no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(sedeDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una sede con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la sede es obligatorio");
            }
            if (sedeDto.CenterId <= 0)
            {
                 _logger.LogWarning("Se intentó crear/actualizar una sede con CenterId inválido: {CenterId}", sedeDto.CenterId);
                 throw new Utilities.Exceptions.ValidationException("CenterId", "El CenterId asociado a la sede es obligatorio y debe ser válido.");
            }
            // Validar EmailContact solo si no está vacío
            if (!string.IsNullOrWhiteSpace(sedeDto.EmailContact) && !IsValidEmail(sedeDto.EmailContact))
            {
                 _logger.LogWarning("Se intentó crear/actualizar una sede con EmailContact inválido: {EmailContact}", sedeDto.EmailContact);
                 throw new Utilities.Exceptions.ValidationException("EmailContact", "El EmailContact proporcionado no tiene un formato válido.");
            }
             // Podrían añadirse validaciones para CodeSede (si es obligatorio), Address, PhoneSede (formato)
             // if (string.IsNullOrWhiteSpace(sedeDto.CodeSede)) throw new Utilities.Exceptions.ValidationException("CodeSede", "El CodeSede es obligatorio.");
             _logger.LogDebug("Validación de Sede DTO exitosa para: {SedeName}", sedeDto.Name);
        }

        //Funciones de mapeos 
        // Método para mapear de Sede a SedeDto
        private SedeDto MapToDTO(Sede sede)
        {
            return new SedeDto
            {
                Id = sede.Id,
                Name = sede.Name,
                CodeSede = sede.CodeSede,
                Address = sede.Address,
                PhoneSede = sede.PhoneSede,
                EmailContact = sede.EmailContact,
                CenterId = sede.CenterId,
                Active = sede.Active
            };
        }

        // Método para mapear de SedeDto a Sede (para creación)
        private Sede MapToEntity(SedeDto sedeDto)
        {
            return new Sede
            {
                // Id = sedeDto.Id, // No en creación
                Name = sedeDto.Name,
                CodeSede = sedeDto.CodeSede, // Podría ser null/vacío si se permite
                Address = sedeDto.Address, // Podría ser null/vacío
                PhoneSede = sedeDto.PhoneSede, // Podría ser null/vacío
                EmailContact = sedeDto.EmailContact, // Podría ser null/vacío
                CenterId = sedeDto.CenterId,
                Active = sedeDto.Active // Usualmente true al crear
                // CreateDate se establece fuera. Update/DeleteDate son null inicialmente.
            };
        }

         // Método para mapear de DTO a una entidad existente (para actualización PUT)
        private Sede MapToEntity(SedeDto sedeDto, Sede existingSede)
        {
            existingSede.Name = sedeDto.Name;
            existingSede.CodeSede = sedeDto.CodeSede;
            existingSede.Address = sedeDto.Address;
            existingSede.PhoneSede = sedeDto.PhoneSede;
            existingSede.EmailContact = sedeDto.EmailContact;
            existingSede.CenterId = sedeDto.CenterId;
            existingSede.Active = sedeDto.Active;

            // Actualizar DeleteDate basado en Active
            if (existingSede.Active && existingSede.DeleteDate.HasValue)
            {
                existingSede.DeleteDate = null; // Reactivando
            }
            else if (!existingSede.Active && !existingSede.DeleteDate.HasValue)
            {
                 // Podría asignarse aquí o dejarse solo para SoftDelete
                 existingSede.DeleteDate = DateTime.UtcNow;
            }
            // UpdateDate se maneja fuera. CreateDate no se toca.
            return existingSede;
        }

        // Método para mapear una lista de Sede a una lista de SedeDto
        private IEnumerable<SedeDto> MapToDTOList(IEnumerable<Sede> sedes)
        {
            return sedes.Select(MapToDTO).ToList(); // Usar LINQ Select
        }
    }
}

