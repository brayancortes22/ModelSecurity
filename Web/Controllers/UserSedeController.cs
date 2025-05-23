﻿using Business;
using Entity.DTOautogestion;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de asociaciones usuario-sede en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserSedeController : ControllerBase
    {
        private readonly UserSedeBusiness _userSedeBusiness;
        private readonly ILogger<UserSedeController> _logger;

        /// <summary>
        /// Constructor del controlador de usuario-sede
        /// </summary>
        public UserSedeController(UserSedeBusiness userSedeBusiness, ILogger<UserSedeController> logger)
        {
            _userSedeBusiness = userSedeBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las asociaciones usuario-sede del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserSedeDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUserSedes()
        {
            try
            {
                var userSedes = await _userSedeBusiness.GetAllUserSedesAsync();
                return Ok(userSedes);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener asociaciones usuario-sede");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una asociación usuario-sede específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserSedeDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserSedeById(int id)
        {
            try
            {
                var userSede = await _userSedeBusiness.GetUserSedeByIdAsync(id);
                return Ok(userSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la asociación usuario-sede con ID: {UserSedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Asociación usuario-sede no encontrada con ID: {UserSedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener asociación usuario-sede con ID: {UserSedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva asociación usuario-sede en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserSedeDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUserSede([FromBody] UserSedeDto userSedeDto)
        {
            try
            {
                var createdUserSede = await _userSedeBusiness.CreateUserSedeAsync(userSedeDto);
                return CreatedAtAction(nameof(GetUserSedeById), new { id = createdUserSede.Id }, createdUserSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear asociación usuario-sede");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear asociación usuario-sede");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una asociación usuario-sede existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserSedeDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUserSede(int id, [FromBody] UserSedeDto userSedeDto)
        {
            try
            {
                var updatedUserSede = await _userSedeBusiness.UpdateUserSedeAsync(id, userSedeDto);
                return Ok(updatedUserSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar asociación usuario-sede con ID: {UserSedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Asociación usuario-sede no encontrada para actualizar con ID: {UserSedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar asociación usuario-sede con ID: {UserSedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente una asociación usuario-sede existente (ej. StatusProcedure)
        /// </summary>
        /// <param name="id">ID de la asociación a actualizar</param>
        /// <param name="userSedeDto">Datos parciales a aplicar (principalmente StatusProcedure)</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(UserSedeDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchUserSede(int id, [FromBody] UserSedeDto userSedeDto)
        {
            try
            {
                var patchedUserSede = await _userSedeBusiness.PatchUserSedeAsync(id, userSedeDto);
                return Ok(patchedUserSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a asociación usuario-sede con ID: {UserSedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Asociación usuario-sede no encontrada para aplicar patch con ID: {UserSedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a asociación usuario-sede con ID: {UserSedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina permanentemente una asociación usuario-sede por su ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUserSede(int id)
        {
            try
            {
                await _userSedeBusiness.DeleteUserSedeAsync(id);
                return NoContent(); // 204 No Content es apropiado para eliminaciones exitosas
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al eliminar asociación usuario-sede con ID: {UserSedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Asociación usuario-sede no encontrada para eliminar con ID: {UserSedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar asociación usuario-sede con ID: {UserSedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
