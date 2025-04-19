using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los centros en el sistema.
    /// </summary>
    public class CenterBusiness
    {
        private readonly CenterData _centerData;
        private readonly ILogger<CenterBusiness> _logger;

        public CenterBusiness(CenterData centerData, ILogger<CenterBusiness> logger)
        {
            _centerData = centerData;
            _logger = logger;
        }

        // Método para obtener todos los centros como DTOs
        public async Task<IEnumerable<CenterDto>> GetAllCentersAsync()
        {
            try
            {
                var centers = await _centerData.GetAllAsync();
                return MapToDTOList(centers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los centros");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de centros", ex);
            }
        }

        // Método para obtener un centro por ID como DTO
        public async Task<CenterDto> GetCenterByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un centro con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del centro debe ser mayor que cero");
            }

            try
            {
                var center = await _centerData.GetByIdAsync(id);
                if (center == null)
                {
                    _logger.LogInformation("No se encontró ningún centro con ID: {Id}", id);
                    throw new EntityNotFoundException("center", id);
                }

                return MapToDTO(center);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el centro con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el centro con ID {id}", ex);
            }
        }

        // Método para crear un centro desde un DTO
        public async Task<CenterDto> CreateCenterAsync(CenterDto centerDto)
        {
            try
            {
                ValidateCenter(centerDto);
                var center = MapToEntity(centerDto);
                var centerCreado = await _centerData.CreateAsync(center);
                return MapToDTO(centerCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo centro: {Name}", centerDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el centro", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateCenter(CenterDto centerDto)
        {
            if (centerDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Center no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(centerDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un centro con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del centro es obligatorio");
            }
        }

        // Método para actualizar un centro
        public async Task UpdateCenterAsync(int id, CenterDto centerDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un centro con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del centro debe ser mayor que cero");
            }

            if (centerDto == null || id != centerDto.Id)
            {
                _logger.LogWarning("Datos inválidos para actualizar el centro con ID: {Id}. DTO: {@CenterDto}", id, centerDto);
                throw new Utilities.Exceptions.ValidationException("Los datos proporcionados son inválidos para la actualización.");
            }

            ValidateCenter(centerDto);

            try
            {
                var existingCenter = await _centerData.GetByIdAsync(id);
                if (existingCenter == null)
                {
                    _logger.LogInformation("No se encontró ningún centro para actualizar con ID: {Id}", id);
                    throw new EntityNotFoundException("center", id);
                }

                var centerToUpdate = MapToEntity(centerDto);
                await _centerData.UpdateAsync(centerToUpdate);
                _logger.LogInformation("Centro actualizado con ID: {Id}", id);
            }
            catch (EntityNotFoundException)
            {
                // Re-lanzar para que el controlador lo maneje
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el centro con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el centro con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente un centro (PATCH)
        public async Task PatchCenterAsync(int id, CenterPatchDto patchDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a un centro con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del centro debe ser mayor que cero");
            }

            if (patchDto == null)
            {
                _logger.LogWarning("Patch DTO nulo para el centro con ID: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("El objeto de patch no puede ser nulo.");
            }

            try
            {
                var existingCenter = await _centerData.GetByIdAsync(id);
                if (existingCenter == null)
                {
                    _logger.LogInformation("No se encontró ningún centro para aplicar patch con ID: {Id}", id);
                    throw new EntityNotFoundException("center", id);
                }

                // Aplicar los cambios del Patch DTO a la entidad existente
                // Solo se actualizan las propiedades si se proporcionan en el DTO
                if (patchDto.Name != null) existingCenter.Name = patchDto.Name.Value;
                if (patchDto.CodeCenter != null) existingCenter.CodeCenter = patchDto.CodeCenter.Value;
                if (patchDto.Active != null) existingCenter.Active = patchDto.Active.Value;
                if (patchDto.RegionalId != null) existingCenter.RegionalId = patchDto.RegionalId.Value;
                if (patchDto.Address != null) existingCenter.Address = patchDto.Address.Value;

                // Validar la entidad resultante después del patch
                ValidateCenter(MapToDTO(existingCenter)); // Reusamos la validación del DTO completo

                await _centerData.UpdateAsync(existingCenter);
                _logger.LogInformation("Centro actualizado parcialmente (patch) con ID: {Id}", id);
            }
            catch (EntityNotFoundException)
            {
                throw; // Re-lanzar para que el controlador lo maneje
            }
            catch (Utilities.Exceptions.ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al aplicar patch al centro con ID: {Id}", id);
                 throw; // Re-lanzar para que el controlador lo maneje
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aplicar patch al centro con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al centro con ID {id}", ex);
            }
        }

        // Método para eliminar un centro por ID
        public async Task DeleteCenterAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un centro con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del centro debe ser mayor que cero");
            }

            try
            {
                var existingCenter = await _centerData.GetByIdAsync(id);
                if (existingCenter == null)
                {
                    _logger.LogInformation("No se encontró ningún centro para eliminar con ID: {Id}", id);
                    throw new EntityNotFoundException("center", id);
                }

                await _centerData.DeleteAsync(id);
                _logger.LogInformation("Centro eliminado con ID: {Id}", id);
            }
            catch (EntityNotFoundException)
            {
                throw; // Re-lanzar para que el controlador lo maneje
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el centro con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el centro con ID {id}", ex);
            }
        }

        // Método para realizar un borrado lógico de un centro por ID
        public async Task SoftDeleteCenterAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar borrado lógico de un centro con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del centro debe ser mayor que cero");
            }

            try
            {
                 var existingCenter = await _centerData.GetByIdAsync(id);
                if (existingCenter == null)
                {
                    _logger.LogInformation("No se encontró ningún centro para borrado lógico con ID: {Id}", id);
                    throw new EntityNotFoundException("center", id);
                }

                // Validar si ya está inactivo (opcional, dependiendo de la lógica de negocio)
                if (!existingCenter.Active)
                {
                    _logger.LogInformation("El centro con ID {Id} ya está inactivo.", id);
                    // Se podría lanzar una excepción o simplemente no hacer nada
                    // throw new ValidationException($"El centro con ID {id} ya está inactivo.");
                    return; // Opcional: si ya está inactivo, no hacer nada.
                }


                existingCenter.Active = false; // Cambiar estado a inactivo
                await _centerData.UpdateAsync(existingCenter); // Usar UpdateAsync para cambiar el estado
                _logger.LogInformation("Borrado lógico realizado para el centro con ID: {Id}", id);
            }
            catch (EntityNotFoundException)
            {
                throw; // Re-lanzar para que el controlador lo maneje
            }
             catch (Utilities.Exceptions.ValidationException ex) // Capturar posible excepción de validación
            {
                _logger.LogWarning(ex, "Validación fallida durante borrado lógico del centro con ID: {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar borrado lógico del centro con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar borrado lógico del centro con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de Center a CenterDto
        private CenterDto MapToDTO(Center center)
        {
            return new CenterDto
            {
                Id = center.Id,
                Name = center.Name,
                CodeCenter = center.CodeCenter,
                Active = center.Active,
                RegionalId = center.RegionalId,
                Address = center.Address
            };
        }

        // Método para mapear de CenterDto a Center
        private Center MapToEntity(CenterDto centerDto)
        {
            return new Center
            {
                Id = centerDto.Id,
                Name = centerDto.Name,
                CodeCenter = centerDto.CodeCenter,
                Active = centerDto.Active,
                RegionalId = centerDto.RegionalId,
                Address = centerDto.Address
            };
        }

        // Método para mapear una lista de Center a una lista de CenterDto
        private IEnumerable<CenterDto> MapToDTOList(IEnumerable<Center> centers)
        {
            var centersDTO = new List<CenterDto>();
            foreach (var center in centers)
            {
                centersDTO.Add(MapToDTO(center));
            }
            return centersDTO;
        }
    }
}
