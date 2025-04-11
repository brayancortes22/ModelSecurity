﻿
using Data;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using System.ComponentModel.Design;
using Entity.DTOautogestion;


namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los roles del sistema.
    /// </summary>
    public class RolBusiness
    {
        private readonly RolData _rolData;
        private readonly ILogger<RolBusiness> _logger;

        public RolBusiness(RolData rolData, ILogger<RolBusiness> logger)
        {
            _rolData = rolData;
            _logger = logger;
        }
        // Método para obtener todos los roles como DTOs
        public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _rolData.GetAllAsync();
                //var rolesDTO = new List<RolDto>();
                return MapToDTOList(roles);

                //foreach (var rol in roles)
                //{
                //    rolesDTO.Add(new RolDto
                //    {
                //        Id = rol.Id,
                //        TypeRol = rol.TypeRol,
                //        Description = rol.Description,
                //        Active = rol.Active //si existe la entidad
                //    });
                //}
                //return rolesDTO;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los rolez");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }
        // Método para obtener un rol por su ID como DTO
        public async Task<RolDto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con un ID invalido: {RolId}",id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor a 0");
            }
            try
            {
                var rol = await _rolData.GetByidAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("No se encontró el rol con ID {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }
                //return new RolDto
                //{
                //    Id = rol.Id,
                //    TypeRol = rol.TypeRol,
                //    Description = rol.Description,
                //    Active = rol.Active //si existe la entidad
                //};
                return MapToDTO(rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error al obtener el rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

       
        // Método para crear un rol desde un DTO
        public async Task<RolDto> CreateRolAsync(RolDto RolDto)
        {
            try
            {
                ValidateRol(RolDto);
                var rol = MapToEntity(RolDto);
                //var rol = new Rol
                //{
                //    Id = RolDto.Id,
                //    TypeRol = RolDto.TypeRol,
                //    Description = RolDto.Description,
                //    Active = RolDto.Active //si existe la entidad
                //};

                var rolCreado = await _rolData.CreateAsync(rol);
                return MapToDTO(rolCreado);
                //return new RolDto
                //{
                //    Id = rol.Id,
                //    TypeRol = rol.TypeRol,
                //    Description = rol.Description,
                //    Active = rol.Active //si existe la entidad

                //};
              }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo rol: {RolNombre}", RolDto?.TypeRol?? "null");
                throw new ExternalServiceException("Base de datos", $"Error al crear el rol", ex);
            }
        }

        // Método para validar el DTO 
        private void ValidateRol(RolDto RolDto)
        {
            if (RolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException( "El objeto rol no puede ser nulo");
            }
            if (string.IsNullOrWhiteSpace(RolDto.TypeRol))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con nombre vacio");
                throw new Utilities.Exceptions.ValidationException("Name", "El nombre del rol nes obligatorio");
            }
        }
        //Funciones de mapeos 
        // Método para mapear de Rol a RolDTO
        private RolDto MapToDTO(Rol rol)
        {
            return new RolDto
            {
                Id = rol.Id,
                TypeRol = rol.TypeRol,
                Description = rol.Description,
                Active = rol.Active //si existe la entidad
            };

        }
        // Método para mapear de RolDTO a Rol
        private Rol MapToEntity(RolDto rolDTO)
        {
            return new Rol
            {
                Id = rolDTO.Id,
                TypeRol = rolDTO.TypeRol,
                Description = rolDTO.Description,
                Active = rolDTO.Active //si existe la entidad
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<RolDto> MapToDTOList(IEnumerable<Rol> roles)
        {
            var rolesDTO = new List<RolDto>();
            foreach (var rol in roles)
            {
                rolesDTO.Add(MapToDTO(rol));
            }
            return rolesDTO;
        }

    }
}
