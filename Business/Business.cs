using Data;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los roles del sistema.
    /// </summary>
    public class RolBusiness
    {
        private readonly RolData _rolData;
        private readonly ILogger _logger;

        public RolBusiness(RolData rolData, ILogger logger)
        {
            _rolData = rolData;
            _logger = logger;
        }

        // Método para obtener todos los roles como DTOs
        public async Task<IEnumerable<RolDTOAuto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _rolData.GetAllAsync();
                var rolesDTO = new List<RolDTOAuto>();

                foreach (var rol in roles)
                {
                    rolesDTO.Add(new RolDTOAuto
                    {
                        Id = rol.Id,
                        Name = rol.Name,
                        Active = rol.Active // Si existe en la entidad
                    });
                }

                return rolesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        // Método para obtener un rol por ID como DTO
        public async Task<RolDtoAuto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var rol = await _rolData.GetByIdAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                return new RolDTOAuto
                {
                    Id = rol.Id,
                    Name = rol.Name,
                    Active = rol.Active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Método para crear un rol desde un DTO
        public async Task<RolDTOAuto> CreateRolAsync(RolDTOAuto RolDto)
        {
            try
            {
                ValidateRol(RolDto);

                var rol = new Rol
                {
                    Name = RolDto.Name,
                    Active = RolDto.Active // Si existe en la entidad
                };

                var rolCreado = await _rolData.CreateAsync(rol);

                return new RolDTOAuto
                {
                    Id = rolCreado.Id,
                    Name = rolCreado.Name,
                    Active = rolCreado.Active // Si existe en la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", RolDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRol(RolDTOAuto RolDto)
        {
            if (RolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(RolDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del rol es obligatorio");
            }
        }
    }
}