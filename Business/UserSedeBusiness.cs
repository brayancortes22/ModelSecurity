using Data;
using Entity.DTOautogestion;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las sedes de usuario en el sistema.
    /// </summary>
    public class UserSedeBusiness
    {
        private readonly UserSedeData _userSedeData;
        private readonly ILogger<UserSedeBusiness> _logger;

        public UserSedeBusiness(UserSedeData userSedeData, ILogger<UserSedeBusiness> logger)
        {
            _userSedeData = userSedeData;
            _logger = logger;
        }

        // Método para obtener todas las sedes de usuario como DTOs
        public async Task<IEnumerable<UserSedeDto>> GetAllUserSedesAsync()
        {
            try
            {
                var userSedes = await _userSedeData.GetAllAsync();
                return MapToDTOList(userSedes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las sedes de usuario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de sedes de usuario", ex);
            }
        }

        // Método para obtener una sede de usuario por ID como DTO
        public async Task<UserSedeDto> GetUserSedeByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una sede de usuario con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede de usuario debe ser mayor que cero");
            }

            try
            {
                var userSede = await _userSedeData.GetByIdAsync(id);
                if (userSede == null)
                {
                    _logger.LogInformation("No se encontró ninguna sede de usuario con ID: {Id}", id);
                    throw new EntityNotFoundException("userSede", id);
                }

                return MapToDTO(userSede);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sede de usuario con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la sede de usuario con ID {id}", ex);
            }
        }

        // Método para crear una sede de usuario desde un DTO
        public async Task<UserSedeDto> CreateUserSedeAsync(UserSedeDto userSedeDto)
        {
            try
            {
                ValidateUserSede(userSedeDto);
                var userSede = MapToEntity(userSedeDto);
                var userSedeCreado = await _userSedeData.CreateAsync(userSede);
                return MapToDTO(userSedeCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva sede de usuario: {UserId}", userSedeDto?.UserId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al crear la sede de usuario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateUserSede(UserSedeDto userSedeDto)
        {
            if (userSedeDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto UserSede no puede ser nulo");
            }

            if (userSedeDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una sede de usuario con UserId inválido");
                throw new Utilities.Exceptions.ValidationException("UserId", "El UserId de la sede de usuario es obligatorio y debe ser mayor a cero");
            }

            if (userSedeDto.SedeId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una sede de usuario con SedeId inválido");
                throw new Utilities.Exceptions.ValidationException("SedeId", "El SedeId de la sede de usuario es obligatorio y debe ser mayor a cero");
            }
        }

        // Método para actualizar una sede de usuario existente (reemplazo completo)
        public async Task<UserSedeDto> UpdateUserSedeAsync(int id, UserSedeDto userSedeDto)
        {
            if (id <= 0 || id != userSedeDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una sede de usuario con ID inválido o no coincidente: {UserSedeId}, DTO ID: {DtoId}", id, userSedeDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateUserSede(userSedeDto); // Reutilizamos la validación

            try
            {
                var existingUserSede = await _userSedeData.GetByIdAsync(id);
                if (existingUserSede == null)
                {
                    _logger.LogInformation("No se encontró la sede de usuario con ID {UserSedeId} para actualizar", id);
                    throw new EntityNotFoundException("UserSede", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingUserSede.UserId = userSedeDto.UserId;
                existingUserSede.SedeId = userSedeDto.SedeId;
                existingUserSede.StatusProcedure = userSedeDto.StatusProcedure;

                await _userSedeData.UpdateAsync(existingUserSede);
                return MapToDTO(existingUserSede);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar para que el controlador la maneje
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Podría ser violación de FK si UserId o SedeId no existen
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar la sede de usuario con ID {UserSedeId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar la sede de usuario con ID {id}. Verifique la existencia de User y Sede.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la sede de usuario con ID {UserSedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la sede de usuario con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente una sede de usuario existente (PATCH)
        // Principalmente para actualizar StatusProcedure
        public async Task<UserSedeDto> PatchUserSedeAsync(int id, UserSedeDto userSedeDto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a una sede de usuario con ID inválido: {UserSedeId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede de usuario debe ser mayor que cero.");
            }

            try
            {
                var existingUserSede = await _userSedeData.GetByIdAsync(id);
                if (existingUserSede == null)
                {
                    _logger.LogInformation("No se encontró la sede de usuario con ID {UserSedeId} para aplicar patch", id);
                    throw new EntityNotFoundException("UserSede", id);
                }

                bool updated = false;
                // Actualizar StatusProcedure si se proporciona en el DTO (podría ser null)
                if (userSedeDto.StatusProcedure != existingUserSede.StatusProcedure) // Comparar con el valor existente
                {
                     existingUserSede.StatusProcedure = userSedeDto.StatusProcedure;
                     updated = true;
                }

                // Normalmente no se actualizan UserId o SedeId con PATCH en una tabla de relación,
                // ya que eso cambiaría la relación fundamental. Si fuera necesario,
                // se añadiría lógica aquí similar a StatusProcedure.

                if (updated)
                {
                    await _userSedeData.UpdateAsync(existingUserSede);
                }
                else
                {
                    _logger.LogInformation("No se realizaron cambios en la sede de usuario con ID {UserSedeId} durante el patch.", id);
                }

                return MapToDTO(existingUserSede);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch a la sede de usuario con ID {UserSedeId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la sede de usuario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a la sede de usuario con ID {UserSedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la sede de usuario con ID {id}", ex);
            }
        }

        // Método para eliminar una sede de usuario (DELETE persistente)
        public async Task DeleteUserSedeAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar una sede de usuario con ID inválido: {UserSedeId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede de usuario debe ser mayor a 0");
            }
            try
            {
                 var existingUserSede = await _userSedeData.GetByIdAsync(id); // Verificar existencia primero
                if (existingUserSede == null)
                {
                     _logger.LogInformation("No se encontró la sede de usuario con ID {UserSedeId} para eliminar", id);
                    throw new EntityNotFoundException("UserSede", id);
                }

                bool deleted = await _userSedeData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Sede de usuario con ID {UserSedeId} eliminada exitosamente", id);
                }
                else
                {
                    // Esto no debería ocurrir si GetByIdAsync funcionó, pero por si acaso
                    _logger.LogWarning("No se pudo eliminar la sede de usuario con ID {UserSedeId}.", id);
                    throw new EntityNotFoundException("UserSede", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            // No se espera DbUpdateException por FK aquí, ya que es una tabla de enlace.
            // Si hubiera otras dependencias *de* UserSede, se manejarían aquí.
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar la sede de usuario con ID {UserSedeId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar la sede de usuario con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de UserSede a UserSedeDto
        private UserSedeDto MapToDTO(UserSede userSede)
        {
            return new UserSedeDto
            {
                Id = userSede.Id,
                UserId = userSede.UserId,
                SedeId = userSede.SedeId,
                StatusProcedure = userSede.StatusProcedure
            };
        }

        // Método para mapear de UserSedeDto a UserSede
        private UserSede MapToEntity(UserSedeDto userSedeDto)
        {
            return new UserSede
            {
                Id = userSedeDto.Id,
                UserId = userSedeDto.UserId,
                SedeId = userSedeDto.SedeId,
                StatusProcedure = userSedeDto.StatusProcedure
            };
        }

        // Método para mapear una lista de UserSede a una lista de UserSedeDto
        private IEnumerable<UserSedeDto> MapToDTOList(IEnumerable<UserSede> userSedes)
        {
            var userSedesDTO = new List<UserSedeDto>();
            foreach (var userSede in userSedes)
            {
                userSedesDTO.Add(MapToDTO(userSede));
            }
            return userSedesDTO;
        }
    }
}
