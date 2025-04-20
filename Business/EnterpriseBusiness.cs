using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las empresas en el sistema.
    /// </summary>
    public class EnterpriseBusiness
    {
        private readonly EnterpriseData _enterpriseData;
        private readonly ILogger<EnterpriseBusiness> _logger;

        public EnterpriseBusiness(EnterpriseData enterpriseData, ILogger<EnterpriseBusiness> logger)
        {
            _enterpriseData = enterpriseData;
            _logger = logger;
        }

        // Método para obtener todas las empresas como DTOs
        public async Task<IEnumerable<EnterpriseDto>> GetAllEnterprisesAsync()
        {
            try
            {
                var enterprises = await _enterpriseData.GetAllAsync();
                return MapToDTOList(enterprises);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las empresas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de empresas", ex);
            }
        }

        // Método para obtener una empresa por ID como DTO
        public async Task<EnterpriseDto> GetEnterpriseByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una empresa con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la empresa debe ser mayor que cero");
            }

            try
            {
                var enterprise = await _enterpriseData.GetByIdAsync(id);
                if (enterprise == null)
                {
                    _logger.LogInformation("No se encontró ninguna empresa con ID: {Id}", id);
                    throw new EntityNotFoundException("enterprise", id);
                }

                return MapToDTO(enterprise);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la empresa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la empresa con ID {id}", ex);
            }
        }

        // Método para crear una empresa desde un DTO
        public async Task<EnterpriseDto> CreateEnterpriseAsync(EnterpriseDto enterpriseDto)
        {
            try
            {
                ValidateEnterprise(enterpriseDto);
                var enterprise = MapToEntity(enterpriseDto);
                enterprise.CreateDate = DateTime.UtcNow;
                var enterpriseCreado = await _enterpriseData.CreateAsync(enterprise);
                return MapToDTO(enterpriseCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva empresa: {Name}", enterpriseDto?.NameEnterprise ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la empresa", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateEnterprise(EnterpriseDto enterpriseDto)
        {
            if (enterpriseDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Enterprise no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(enterpriseDto.NameEnterprise))
            {
                _logger.LogWarning("Se intentó crear/actualizar una empresa con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la empresa es obligatorio");
            }
        }

        // Método para actualizar una empresa existente (PUT)
        public async Task UpdateEnterpriseAsync(int id, EnterpriseDto enterpriseDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar una empresa con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la empresa debe ser mayor que cero");
            }

            if (enterpriseDto == null || id != enterpriseDto.Id)
            {
                _logger.LogWarning("Datos inválidos para actualizar la empresa con ID: {Id}. DTO: {@EnterpriseDto}", id, enterpriseDto);
                throw new Utilities.Exceptions.ValidationException("Los datos proporcionados son inválidos para la actualización.");
            }

            ValidateEnterprise(enterpriseDto); // Reutilizar validación básica

            try
            {
                var existingEnterprise = await _enterpriseData.GetByIdAsync(id);
                if (existingEnterprise == null)
                {
                    _logger.LogInformation("No se encontró ninguna empresa para actualizar con ID: {Id}", id);
                    throw new EntityNotFoundException("enterprise", id);
                }

                existingEnterprise.UpdateDate = DateTime.UtcNow;
                // Mapear todo el DTO a la entidad existente
                existingEnterprise = MapToEntity(enterpriseDto, existingEnterprise);

                await _enterpriseData.UpdateAsync(existingEnterprise);
                _logger.LogInformation("Empresa actualizada con ID: {Id}", id);
            }
            catch (EntityNotFoundException)
            {
                throw; 
            }
            catch (DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar la empresa con ID {Id}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar la empresa con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la empresa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la empresa con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente una empresa (PATCH)
        public async Task PatchEnterpriseAsync(int id, EnterpriseDto enterpriseDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a una empresa con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la empresa debe ser mayor que cero");
            }

            if (enterpriseDto == null)
            {
                _logger.LogWarning("Patch DTO nulo para la empresa con ID: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("El objeto de patch no puede ser nulo.");
            }
            
            if (enterpriseDto.Id != 0 && id != enterpriseDto.Id)
            {
                 _logger.LogWarning("ID de ruta {RouteId} no coincide con ID de cuerpo {BodyId} en PATCH para Empresa", id, enterpriseDto.Id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID de la ruta no coincide con el ID del cuerpo.");
            }

            try
            {
                var existingEnterprise = await _enterpriseData.GetByIdAsync(id);
                if (existingEnterprise == null)
                {
                    _logger.LogInformation("No se encontró ninguna empresa para aplicar patch con ID: {Id}", id);
                    throw new EntityNotFoundException("enterprise", id);
                }

                bool updated = false;
                existingEnterprise.UpdateDate = DateTime.UtcNow;

                // Aplicar cambios si los valores del DTO son diferentes a los existentes
                if (enterpriseDto.NameEnterprise != null && existingEnterprise.NameEnterprise != enterpriseDto.NameEnterprise)
                {
                    existingEnterprise.NameEnterprise = enterpriseDto.NameEnterprise;
                    updated = true;
                }
                 if (enterpriseDto.NitEnterprise != null && existingEnterprise.NitEnterprise != enterpriseDto.NitEnterprise)
                {
                    existingEnterprise.NitEnterprise = enterpriseDto.NitEnterprise;
                    updated = true;
                }
                 if (enterpriseDto.Locate != null && existingEnterprise.Locate != enterpriseDto.Locate)
                {
                    existingEnterprise.Locate = enterpriseDto.Locate;
                    updated = true;
                }
                 if (enterpriseDto.Observation != null && existingEnterprise.Observation != enterpriseDto.Observation)
                {
                    existingEnterprise.Observation = enterpriseDto.Observation;
                    updated = true;
                }
                if (enterpriseDto.PhoneEnterprise != null && existingEnterprise.PhoneEnterprise != enterpriseDto.PhoneEnterprise)
                {
                    existingEnterprise.PhoneEnterprise = enterpriseDto.PhoneEnterprise;
                    updated = true;
                }
                 if (enterpriseDto.EmailEnterprise != null && existingEnterprise.EmailEnterprise != enterpriseDto.EmailEnterprise)
                {
                    existingEnterprise.EmailEnterprise = enterpriseDto.EmailEnterprise;
                    updated = true;
                }
                if (existingEnterprise.Active != enterpriseDto.Active)
                {
                    existingEnterprise.Active = enterpriseDto.Active;
                    updated = true;
                }

                if (updated)
                {
                    // Validar la entidad resultante después del patch
                    ValidateEnterprise(MapToDTO(existingEnterprise)); 
                    await _enterpriseData.UpdateAsync(existingEnterprise);
                    _logger.LogInformation("Empresa actualizada parcialmente (patch) con ID: {Id}", id);
                }
                else
                {
                     _logger.LogInformation("No se realizaron cambios en la empresa con ID {Id} durante el patch.", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw; 
            }
            catch (Utilities.Exceptions.ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al aplicar patch a la empresa con ID: {Id}", id);
                 throw; 
            }
             catch (DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch a la empresa con ID {Id}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la empresa con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a la empresa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch a la empresa con ID {id}", ex);
            }
        }

        // Método para eliminar una empresa por ID (DELETE persistente)
        public async Task DeleteEnterpriseAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una empresa con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la empresa debe ser mayor que cero");
            }

            try
            {
                var existingEnterprise = await _enterpriseData.GetByIdAsync(id);
                if (existingEnterprise == null)
                {
                    _logger.LogInformation("No se encontró ninguna empresa para eliminar con ID: {Id}", id);
                    throw new EntityNotFoundException("enterprise", id);
                }

                bool deleted = await _enterpriseData.DeleteAsync(id);
                 if (!deleted)
                 {
                     _logger.LogWarning("No se pudo eliminar la empresa con ID {Id} desde la capa de datos.", id);
                     throw new ExternalServiceException("Base de datos", $"No se pudo eliminar la empresa con ID {id}");
                 }
                _logger.LogInformation("Empresa eliminada con ID: {Id}", id);
            }
            catch (EntityNotFoundException)
            {
                throw; 
            }
             catch (DbUpdateException dbEx) 
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar la empresa con ID {Id}. Posible violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la empresa con ID {id}. Verifique dependencias.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al eliminar la empresa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la empresa con ID {id}", ex);
            }
        }

        // Método para realizar un borrado lógico de una empresa por ID (SOFT DELETE)
        public async Task SoftDeleteEnterpriseAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar borrado lógico de una empresa con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la empresa debe ser mayor que cero");
            }

            try
            {
                 var existingEnterprise = await _enterpriseData.GetByIdAsync(id);
                if (existingEnterprise == null)
                {
                    _logger.LogInformation("No se encontró ninguna empresa para borrado lógico con ID: {Id}", id);
                    throw new EntityNotFoundException("enterprise", id);
                }

                if (!existingEnterprise.Active)
                {
                    _logger.LogInformation("La empresa con ID {Id} ya está inactiva.", id);
                    return; 
                }

                existingEnterprise.Active = false; 
                existingEnterprise.DeleteDate = DateTime.UtcNow;
                await _enterpriseData.UpdateAsync(existingEnterprise); 
                _logger.LogInformation("Borrado lógico realizado para la empresa con ID: {Id}", id);
            }
            catch (EntityNotFoundException)
            {
                throw; 
            }
             catch (Utilities.Exceptions.ValidationException ex) 
            {
                _logger.LogWarning(ex, "Validación fallida durante borrado lógico de la empresa con ID: {Id}", id);
                throw;
            }
             catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar borrado lógico de la empresa con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar borrado lógico de la empresa con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al realizar borrado lógico de la empresa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar borrado lógico de la empresa con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de Enterprise a EnterpriseDto
        private EnterpriseDto MapToDTO(Enterprise enterprise)
        {
            return new EnterpriseDto
            {
                Id = enterprise.Id,
                NameEnterprise = enterprise.NameEnterprise,
                NitEnterprise = enterprise.NitEnterprise,
                Locate = enterprise.Locate,
                Observation = enterprise.Observation,
                PhoneEnterprise = enterprise.PhoneEnterprise,
                EmailEnterprise = enterprise.EmailEnterprise,
                Active = enterprise.Active,
            };
        }

        // Método para mapear de EnterpriseDto a Enterprise
        private Enterprise MapToEntity(EnterpriseDto enterpriseDto)
        {
            return new Enterprise
            {
                Id = enterpriseDto.Id,
                NameEnterprise = enterpriseDto.NameEnterprise,
                NitEnterprise = enterpriseDto.NitEnterprise,
                Locate = enterpriseDto.Locate,
                Observation = enterpriseDto.Observation,
                PhoneEnterprise = enterpriseDto.PhoneEnterprise,
                EmailEnterprise = enterpriseDto.EmailEnterprise,
                Active = enterpriseDto.Active
            };
        }

        // Método para mapear una lista de Enterprise a una lista de EnterpriseDto
        private IEnumerable<EnterpriseDto> MapToDTOList(IEnumerable<Enterprise> enterprises)
        {
            var enterprisesDTO = new List<EnterpriseDto>();
            foreach (var enterprise in enterprises)
            {
                enterprisesDTO.Add(MapToDTO(enterprise));
            }
            return enterprisesDTO;
        }

        // Método para mapear de DTO a una entidad existente (para actualización)
        private Enterprise MapToEntity(EnterpriseDto dto, Enterprise existingEnterprise)
        {
            existingEnterprise.NameEnterprise = dto.NameEnterprise;
            existingEnterprise.NitEnterprise = dto.NitEnterprise;
            existingEnterprise.Locate = dto.Locate;
            existingEnterprise.Observation = dto.Observation;
            existingEnterprise.PhoneEnterprise = dto.PhoneEnterprise;
            existingEnterprise.EmailEnterprise = dto.EmailEnterprise;
            existingEnterprise.Active = dto.Active;
            // No actualizamos el Id
            return existingEnterprise;
        }
    }
}
