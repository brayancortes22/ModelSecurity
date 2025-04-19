using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los aprendices del sistema.
    /// </summary>
    public class AprendizBusiness
    {
        private readonly AprendizData _aprendizData;
        private readonly ILogger<AprendizBusiness> _logger;

        public AprendizBusiness(AprendizData aprendizData, ILogger<AprendizBusiness> logger)
        {
            _aprendizData = aprendizData;
            _logger = logger;
        }

        // Método para obtener todos los aprendices como DTOs
        public async Task<IEnumerable<AprendizDto>> GetAllAprendizAsync()
        {
            try
            {
                var aprendices = await _aprendizData.GetAllAsync();
                return MapToDTOList(aprendices);
                //var aprendicesDTO = new List<AprendizDto>();

                //foreach (var aprendiz in aprendices)
                //{
                //    aprendicesDTO.Add(new AprendizDto
                //    {
                //        Id = aprendiz.Id,
                //        PreviuosProgram = aprendiz.PreviuosProgram,
                //        UserId = aprendiz.UserId,
                //        Active = aprendiz.Active //si existe la entidad
                //    });
                //}

                //return aprendicesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los aprendices");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de aprendices", ex);
            }
        }

        // Método para obtener un aprendiz por ID como DTO
        public async Task<AprendizDto> GetAprendizByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un aprendiz con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del aprendiz debe ser mayor que cero");
            }

            try
            {
                var aprendiz = await _aprendizData.GetByIdAsync(id);
                if (aprendiz == null)
                {
                    _logger.LogInformation("No se encontró ningún aprendiz con ID: {Id}", id);
                    throw new EntityNotFoundException("Aprendiz", id);
                }
                return MapToDTO(aprendiz);
                //return new AprendizDto
                //{
                //    Id = aprendiz.Id,
                //    PreviuosProgram = aprendiz.PreviuosProgram,
                //    UserId = aprendiz.UserId,
                //    Active = aprendiz.Active //si existe la entidad
                //};
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el aprendiz con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el aprendiz con ID {id}", ex);
            }
        }

        // Método para crear un usuario desde un DTO
        public async Task<AprendizDto> CreateAprendizAsync(AprendizDto aprendizDto)
        {
            try
            {
                ValidateAprendiz(aprendizDto);
                var aprendiz = MapToEntity(aprendizDto);
                
                var aprendizCreado = await _aprendizData.CreateAsync(aprendiz);
                return MapToDTO(aprendizCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo aprendiz: {Name}", aprendizDto?.PreviousProgram ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el aprendiz", ex);
            }
        }

        // Método para actualizar un aprendiz existente
        public async Task<AprendizDto> UpdateAprendizAsync(int id, AprendizDto aprendizDto)
        {
            if (id <= 0 || id != aprendizDto.Id)
            {
                 _logger.LogWarning("Se intentó actualizar un aprendiz con un ID invalido o no coincidente: {AprendizId}, DTO ID: {DtoId}", id, aprendizDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
             ValidateAprendiz(aprendizDto); // Reutiliza la validación existente

            try
            {
                var existingAprendiz = await _aprendizData.GetByIdAsync(id); // Asume que _aprendizData existe y tiene GetByidAsync
                if (existingAprendiz == null)
                {
                    _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para actualizar", id);
                    throw new EntityNotFoundException("Aprendiz", id);
                }

                // Mapea los cambios del DTO a la entidad existente
                 existingAprendiz = MapToEntity(aprendizDto, existingAprendiz); // Usar mapeo para actualizar

                await _aprendizData.UpdateAsync(existingAprendiz); // Asume que UpdateAsync devuelve Task/Task<bool>
                // UpdateAsync devuelve bool/void, mapeamos la entidad que ya modificamos
                return MapToDTO(existingAprendiz);
            }
            catch (EntityNotFoundException) // Reapropagar si no se encontró
            {
                throw;
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error al actualizar el aprendiz con ID {AprendizId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el aprendiz con ID {id}", ex);
            }
        }

         // Método para actualizar parcialmente un aprendiz
        public async Task<AprendizDto> PatchAprendizAsync(int id, AprendizDto aprendizDto)
        {
             if (id <= 0 || id != aprendizDto.Id)
             {
                 _logger.LogWarning("Se intentó aplicar patch a un aprendiz con un ID invalido o no coincidente: {AprendizId}, DTO ID: {DtoId}", id, aprendizDto.Id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO para PATCH.");
             }
             // Podría necesitar una validación específica para Patch
             // ValidateAprendizPatch(aprendizDto);
             // ValidateAprendiz(aprendizDto); // O validar solo campos presentes

            try
            {
                 var existingAprendiz = await _aprendizData.GetByIdAsync(id);
                 if (existingAprendiz == null)
                 {
                     _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para aplicar patch", id);
                     throw new EntityNotFoundException("Aprendiz", id);
                 }

                existingAprendiz.PreviousProgram = aprendizDto.PreviousProgram;
                existingAprendiz.Active = aprendizDto.Active;


                await _aprendizData.UpdateAsync(existingAprendiz); // Asume que no devuelve la entidad
                 return MapToDTO(existingAprendiz); // Mapea la entidad modificada
            }
             catch (EntityNotFoundException)
             {
                 throw;
             }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error al aplicar patch al aprendiz con ID {AprendizId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al aprendiz con ID {id}", ex);
            }
        }

        // Método para eliminar un aprendiz (persistente)
        public async Task DeleteAprendizAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar un aprendiz con un ID invalido: {AprendizId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del aprendiz debe ser mayor a 0");
            }
            try
            {
                var existingAprendiz = await _aprendizData.GetByIdAsync(id);
                 if (existingAprendiz == null)
                 {
                     _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para eliminar", id);
                     throw new EntityNotFoundException("Aprendiz", id);
                 }

                await _aprendizData.DeleteAsync(id); // Asume DeleteAsync toma id
                _logger.LogInformation("Aprendiz con ID {AprendizId} eliminado exitosamente (persistente)", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error al eliminar el aprendiz con ID {AprendizId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar el aprendiz con ID {id}", ex);
            }
        }

        // Método para eliminar lógicamente un aprendiz (soft delete)
        public async Task SoftDeleteAprendizAsync(int id)
        {
             if (id <= 0)
             {
                 _logger.LogWarning("Se intentó realizar soft-delete a un aprendiz con un ID invalido: {AprendizId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del aprendiz debe ser mayor a 0");
             }
            try
            {
                 var aprendizToDeactivate = await _aprendizData.GetByIdAsync(id);
                 if (aprendizToDeactivate == null)
                 {
                     _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para desactivar (soft-delete)", id);
                     throw new EntityNotFoundException("Aprendiz", id);
                 }

                 if (!aprendizToDeactivate.Active) // Asume propiedad Active
                 {
                    _logger.LogInformation("El aprendiz con ID {AprendizId} ya está inactivo.", id);
                    return;
                 }

                aprendizToDeactivate.Active = false; // Marcar como inactivo
                await _aprendizData.UpdateAsync(aprendizToDeactivate); // Persistir el cambio
                _logger.LogInformation("Aprendiz con ID {AprendizId} marcado como inactivo (soft-delete)", id);

            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete del aprendiz con ID {AprendizId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar el aprendiz con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateAprendiz(AprendizDto aprendizDto)
        {
            if (aprendizDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto aprendiz no puede ser nulo");
            }
            
            
            // Añadir más validaciones específicas si son necesarias
        }

        //Funciones de mapeos 
        // Método para mapear de Aprendiz a AprendizDto
        private AprendizDto MapToDTO(Aprendiz aprendiz)
        {
            return new AprendizDto
            {
                Id = aprendiz.Id,
                PreviousProgram = aprendiz.PreviousProgram,
                UserId = aprendiz.UserId,
                Active = aprendiz.Active // Si existe en la entidad
            };
        }

        // Método para mapear de AprendizDto a Aprendiz
        private Aprendiz MapToEntity(AprendizDto aprendizDto)
        {
            return new Aprendiz
            {
                Id = aprendizDto.Id,
                PreviousProgram = aprendizDto.PreviousProgram,
                UserId = aprendizDto.UserId,
                Active = aprendizDto.Active // Si existe en la entidad
            };
        }

        // Método para mapear de AprendizDTO a una entidad Aprendiz existente (para actualización)
        private Aprendiz MapToEntity(AprendizDto aprendizDto, Aprendiz existingAprendiz)
        {
            // Actualiza la entidad existente con los valores del DTO
            
             // ... otras propiedades ...
             existingAprendiz.PreviousProgram = aprendizDto.PreviousProgram; // Asegurarse que las otras propiedades sí se mapean
             existingAprendiz.UserId = aprendizDto.UserId;
             existingAprendiz.Active = aprendizDto.Active;
            return existingAprendiz;
        }

        // Método para mapear una lista de Aprendiz a una lista de AprendizDto
        private IEnumerable<AprendizDto> MapToDTOList(IEnumerable<Aprendiz> aprendices)
        {
            var aprendicesDTO = new List<AprendizDto>();
            foreach (var aprendiz in aprendices)
            {
                aprendicesDTO.Add(MapToDTO(aprendiz));
            }
            return aprendicesDTO;
        }

    }
}
