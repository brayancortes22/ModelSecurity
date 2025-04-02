using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con el registro de cambios del sistema.
    /// Implementa la lógica de negocio para el seguimiento de cambios, incluyendo operaciones CRUD.
    /// </summary>
    public class ChangeLogBusiness
    {
        // Dependencias inyectadas
        private readonly ChangeLogData _changeLogData;    // Acceso a la capa de datos
        private readonly ILogger _logger;               // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="changeLogData">Servicio de acceso a datos para registros de cambios</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public ChangeLogBusiness(ChangeLogData changeLogData, ILogger logger)
        {
            _changeLogData = changeLogData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de cambios del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de registros de cambios en formato DTO</returns>
        public async Task<IEnumerable<ChangeLogDTOAuto>> GetAllChangeLogsAsync()
        {
            try
            {
                var changeLogs = await _changeLogData.GetAllAsync();
                var changeLogsDTO = new List<ChangeLogDTOAuto>();

                foreach (var changeLog in changeLogs)
                {
                    changeLogsDTO.Add(new ChangeLogDTOAuto
                    {
                        Id = changeLog.Id,
                        Name = changeLog.Name,
                        Action = changeLog.Action,
                        Description = $"Cambio realizado en la tabla {changeLog.IdTable}" +
                        $" por el usuario {changeLog.IdUser}"
                    });
                }

                return changeLogsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de cambios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de registros de cambios", ex);
            }
        }

        /// <summary>
        /// Obtiene un registro de cambio específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del registro de cambio</param>
        /// <returns>Registro de cambio en formato DTO</returns>
        public async Task<ChangeLogDTOAuto> GetChangeLogByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro de cambio con ID inválido: {ChangeLogId}", id);
                throw new ValidationException("id", "El ID del registro de cambio debe ser mayor que cero");
            }

            try
            {
                var changeLog = await _changeLogData.GetByIdAsync(id);
                if (changeLog == null)
                {
                    _logger.LogInformation("No se encontró ningún registro de cambio con ID: {ChangeLogId}", id);
                    throw new EntityNotFoundException("ChangeLog", id);
                }

                return new ChangeLogDTOAuto
                {
                    Id = changeLog.Id,
                    Name = changeLog.Name,
                    Action = changeLog.Action,
                    Description = $"Cambio realizado en la tabla {changeLog.IdTable} por el usuario {changeLog.IdUser}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de cambio con ID: {ChangeLogId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el registro de cambio con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo registro de cambio en el sistema
        /// </summary>
        /// <param name="changeLogDto">Datos del registro de cambio a crear</param>
        /// <returns>Registro de cambio creado en formato DTO</returns>
        public async Task<ChangeLogDTOAuto> CreateChangeLogAsync(ChangeLogDTOAuto changeLogDto)
        {
            try
            {
                ValidateChangeLog(changeLogDto);

                var changeLog = new ChangeLog
                {
                    CreateAT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    DeleteAT = null,
                    IdTable = 0, // Este valor debería ser proporcionado según la tabla específica
                    IdUser = 0,  // Este valor debería ser proporcionado por el usuario autenticado
                    IdPermission = 0, // Este valor debería ser proporcionado según los permisos
                    Action = changeLogDto.Action
                };

                var changeLogCreado = await _changeLogData.CreateAsync(changeLog);

                return new ChangeLogDTOAuto
                {
                    Id = changeLogCreado.Id,
                    Name = changeLogCreado.Name,
                    Action = changeLogCreado.Action,
                    Description = $"Cambio realizado en la tabla {changeLogCreado.IdTable}" +
                    $" por el usuario {changeLogCreado.IdUser}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo registro de cambio: {ChangeLogName}",
                    changeLogDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el registro de cambio", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de registro de cambio
        /// </summary>
        /// <param name="changeLogDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateChangeLog(ChangeLogDTOAuto changeLogDto)
        {
            if (changeLogDto == null)
            {
                throw new ValidationException("El objeto registro de cambio no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(changeLogDto.Name))
            {
                _logger.LogWarning("Se intentó crear un registro de cambio con nombre vacío");
                throw new ValidationException("name", "El nombre de la acción es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(changeLogDto.Description))
            {
                _logger.LogWarning("Se intentó crear un registro de cambio con descripción vacía");
                throw new ValidationException("Description", "La descripción del cambio es obligatoria");
            }
        }

        public async Task<bool> DeleteChangeLogAsync(int id)
        {
            try
            {
                return await _changeLogData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el registro de cambio con ID {ChangeLogId}", id);
                throw;
            }
        }
    }
}
