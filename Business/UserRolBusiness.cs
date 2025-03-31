using Data;
using Entity.DTOautogestion.pivote;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con la relación entre usuarios y roles.
    /// Implementa la lógica de negocio para la gestión de las relaciones entre usuarios y roles.
    /// </summary>
    public class UserRolBusiness
    {
        // Dependencias inyectadas
        private readonly UserRolData _userRolData;        // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="userRolData">Servicio de acceso a datos para la relación user-rol</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public UserRolBusiness(UserRolData userRolData, ILogger logger)
        {
            _userRolData = userRolData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las relaciones user-rol del sistema y las convierte a DTOs
        /// </summary>
        /// <returns>Lista de relaciones user-rol en formato DTO</returns>
        public async Task<IEnumerable<UserRolDTOAuto>> GetAllUserRolsAsync()
        {
            try
            {
                // Obtener relaciones de la capa de datos
                var userRols = await _userRolData.GetAllAsync();
                var userRolsDTO = new List<UserRolDTOAuto>();

                // Convertir cada relación a DTO
                foreach (var userRol in userRols)
                {
                    userRolsDTO.Add(new UserRolDTOAuto
                    {
                        Id = userRol.Id,
                        UserId = userRol.UserId,
                        RolId = userRol.RolId
                    });
                }

                return userRolsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones user-rol");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de relaciones user-rol", ex);
            }
        }

        /// <summary>
        /// Obtiene una relación user-rol específica por su ID
        /// </summary>
        /// <param name="id">Identificador único de la relación</param>
        /// <returns>Relación user-rol en formato DTO</returns>
        public async Task<UserRolDTOAuto> GetUserRolByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una relación user-rol con ID inválido: {UserRolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación debe ser mayor que cero");
            }

            try
            {
                // Buscar la relación en la base de datos
                var userRol = await _userRolData.GetByIdAsync(id);
                if (userRol == null)
                {
                    _logger.LogInformation("No se encontró ninguna relación user-rol con ID: {UserRolId}", id);
                    throw new EntityNotFoundException("UserRol", id);
                }

                // Convertir la relación a DTO
                return new UserRolDTOAuto
                {
                    Id = userRol.Id,
                    UserId = userRol.UserId,
                    RolId = userRol.RolId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación user-rol con ID: {UserRolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la relación user-rol con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva relación user-rol en el sistema
        /// </summary>
        /// <param name="userRolDto">Datos de la relación a crear</param>
        /// <returns>Relación creada en formato DTO</returns>
        public async Task<UserRolDTOAuto> CreateUserRolAsync(UserRolDTOAuto userRolDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateUserRol(userRolDto);

                // Crear la entidad UserRol desde el DTO
                var userRol = new UserRol
                {
                    UserId = userRolDto.UserId,
                    RolId = userRolDto.RolId
                };

                // Guardar la relación en la base de datos
                var userRolCreado = await _userRolData.CreateAsync(userRol);

                // Convertir la relación creada a DTO para la respuesta
                return new UserRolDTOAuto
                {
                    Id = userRolCreado.Id,
                    UserId = userRolCreado.UserId,
                    RolId = userRolCreado.RolId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva relación user-rol");
                throw new ExternalServiceException("Base de datos", "Error al crear la relación user-rol", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de relación user-rol
        /// </summary>
        /// <param name="userRolDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateUserRol(UserRolDTOAuto userRolDto)
        {
            // Validar que el DTO no sea nulo
            if (userRolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto relación user-rol no puede ser nulo");
            }

            // Validar que el UserId sea válido
            if (userRolDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación con UserId inválido: {UserId}", userRolDto.UserId);
                throw new Utilities.Exceptions.ValidationException("UserId", "El ID del usuario debe ser mayor que cero");
            }

            // Validar que el RolId sea válido
            if (userRolDto.RolId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación con RolId inválido: {RolId}", userRolDto.RolId);
                throw new Utilities.Exceptions.ValidationException("RolId", "El ID del rol debe ser mayor que cero");
            }
        }
    }
} 