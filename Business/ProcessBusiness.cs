using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los procesos en el sistema.
    /// </summary>
    public class ProcessBusiness
    {
        private readonly ProcessData _processData;
        private readonly ILogger<ProcessBusiness> _logger;

        public ProcessBusiness(ProcessData processData, ILogger<ProcessBusiness> logger)
        {
            _processData = processData;
            _logger = logger;
        }

        // Método para obtener todos los procesos como DTOs
        public async Task<IEnumerable<ProcessDto>> GetAllProcessesAsync()
        {
            try
            {
                var processes = await _processData.GetAllAsync();
                return MapToDTOList(processes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los procesos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de procesos", ex);
            }
        }

        // Método para obtener un proceso por ID como DTO
        public async Task<ProcessDto> GetProcessByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un proceso con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del proceso debe ser mayor que cero");
            }

            try
            {
                var process = await _processData.GetByIdAsync(id);
                if (process == null)
                {
                    _logger.LogInformation("No se encontró ningún proceso con ID: {Id}", id);
                    throw new EntityNotFoundException("process", id);
                }

                return MapToDTO(process);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el proceso con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el proceso con ID {id}", ex);
            }
        }

        // Método para crear un proceso desde un DTO
        public async Task<ProcessDto> CreateProcessAsync(ProcessDto processDto)
        {
            try
            {
                ValidateProcess(processDto);
                var process = MapToEntity(processDto);
                process.CreateDate = DateTime.UtcNow;
                var processCreado = await _processData.CreateAsync(process);
                return MapToDTO(processCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo proceso: {Name}", processDto?.TypeProcess ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el proceso", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateProcess(ProcessDto processDto)
        {
            if (processDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Process no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(processDto.TypeProcess))
            {
                _logger.LogWarning("Se intentó crear/actualizar un proceso con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del proceso es obligatorio");
            }
        }

        // Método para actualizar un proceso existente (reemplazo completo)
        public async Task<ProcessDto> UpdateProcessAsync(int id, ProcessDto processDto)
        {
            if (id <= 0 || id != processDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar un proceso con ID inválido o no coincidente: {ProcessId}, DTO ID: {DtoId}", id, processDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateProcess(processDto); // Reutilizamos la validación

            try
            {
                var existingProcess = await _processData.GetByIdAsync(id);
                if (existingProcess == null)
                {
                    _logger.LogInformation("No se encontró el proceso con ID {ProcessId} para actualizar", id);
                    throw new EntityNotFoundException("Process", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingProcess.TypeProcess = processDto.TypeProcess;
                existingProcess.Observation = processDto.Observation;
                existingProcess.Active = processDto.Active;
                existingProcess.UpdateDate = DateTime.UtcNow;

                await _processData.UpdateAsync(existingProcess);
                return MapToDTO(existingProcess);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar el proceso con ID {ProcessId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar el proceso con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar el proceso con ID {ProcessId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el proceso con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente un proceso (PATCH)
        public async Task<ProcessDto> PatchProcessAsync(int id, ProcessDto processDto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a un proceso con ID inválido: {ProcessId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del proceso debe ser mayor que cero.");
            }

            try
            {
                var existingProcess = await _processData.GetByIdAsync(id);
                if (existingProcess == null)
                {
                    _logger.LogInformation("No se encontró el proceso con ID {ProcessId} para aplicar patch", id);
                    throw new EntityNotFoundException("Process", id);
                }

                bool updated = false;
                existingProcess.UpdateDate = DateTime.UtcNow;

                // Actualizar TypeProcess si se proporciona y es diferente
                if (!string.IsNullOrWhiteSpace(processDto.TypeProcess) && processDto.TypeProcess != existingProcess.TypeProcess)
                {
                    existingProcess.TypeProcess = processDto.TypeProcess;
                    updated = true;
                }
                // Actualizar Observation si se proporciona y es diferente (puede ser null)
                if (processDto.Observation != null && processDto.Observation != existingProcess.Observation)
                {
                     existingProcess.Observation = processDto.Observation;
                     updated = true;
                }
                // No actualizamos Active en PATCH

                if (updated)
                {
                    await _processData.UpdateAsync(existingProcess);
                    _logger.LogInformation("Patch aplicado al proceso con ID: {ProcessId}", id);
                }
                else
                {
                    _logger.LogInformation("No se realizaron cambios en el proceso con ID {ProcessId} durante el patch.", id);
                }

                return MapToDTO(existingProcess);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch al proceso con ID {ProcessId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el proceso con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch al proceso con ID {ProcessId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el proceso con ID {id}", ex);
            }
        }

        // Método para eliminar un proceso (DELETE persistente)
        public async Task DeleteProcessAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar un proceso con ID inválido: {ProcessId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del proceso debe ser mayor a 0");
            }
            try
            {
                 var existingProcess = await _processData.GetByIdAsync(id); // Verificar existencia
                if (existingProcess == null)
                {
                     _logger.LogInformation("No se encontró el proceso con ID {ProcessId} para eliminar", id);
                    throw new EntityNotFoundException("Process", id);
                }

                bool deleted = await _processData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Proceso con ID {ProcessId} eliminado exitosamente", id);
                }
                else
                {
                     _logger.LogWarning("No se pudo eliminar el proceso con ID {ProcessId}.", id);
                    throw new EntityNotFoundException("Process", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Capturar error si hay FKs
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar el proceso con ID {ProcessId}. Posible violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el proceso con ID {id}. Verifique dependencias.", dbEx);
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar el proceso con ID {ProcessId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar el proceso con ID {id}", ex);
            }
        }

        // Método para desactivar (eliminar lógicamente) un proceso
        public async Task SoftDeleteProcessAsync(int id)
        {
             if (id <= 0)
            {
                 _logger.LogWarning("Se intentó realizar soft-delete a un proceso con ID inválido: {ProcessId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del proceso debe ser mayor a 0");
            }

             try
            {
                var existingProcess = await _processData.GetByIdAsync(id);
                if (existingProcess == null)
                {
                    _logger.LogInformation("No se encontró el proceso con ID {ProcessId} para soft-delete", id);
                    throw new EntityNotFoundException("Process", id);
                }

                 if (!existingProcess.Active)
                {
                     _logger.LogInformation("El proceso con ID {ProcessId} ya se encuentra inactivo.", id);
                     return; 
                }

                existingProcess.Active = false;
                existingProcess.DeleteDate = DateTime.UtcNow;
                await _processData.UpdateAsync(existingProcess); 
                 _logger.LogInformation("Proceso con ID {ProcessId} desactivado (soft-delete) exitosamente", id);
            }
             catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete del proceso con ID {ProcessId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar el proceso con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error general al realizar soft-delete del proceso con ID {ProcessId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar el proceso con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de Process a ProcessDto
        private ProcessDto MapToDTO(Process process)
        {
            return new ProcessDto
            {
                Id = process.Id,
                TypeProcess = process.TypeProcess,
                Observation = process.Observation,
                Active = process.Active // si existe la entidad
            };
        }

        // Método para mapear de ProcessDto a Process
        private Process MapToEntity(ProcessDto processDto)
        {
            return new Process
            {
                Id = processDto.Id,
                TypeProcess = processDto.TypeProcess,
                Observation = processDto.Observation,
                Active = processDto.Active // si existe la entidad
            };
        }

        // Método para mapear una lista de Process a una lista de ProcessDto
        private IEnumerable<ProcessDto> MapToDTOList(IEnumerable<Process> processes)
        {
            var processesDTO = new List<ProcessDto>();
            foreach (var process in processes)
            {
                processesDTO.Add(MapToDTO(process));
            }
            return processesDTO;
        }
    }
}
