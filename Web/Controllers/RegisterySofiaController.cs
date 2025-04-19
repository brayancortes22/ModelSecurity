using Business;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de registros de Sofia en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RegisterySofiaController : ControllerBase
    {
        private readonly RegisterySofiaBusiness _registerySofiaBusiness;
        private readonly ILogger<RegisterySofiaController> _logger;

        /// <summary>
        /// Constructor del controlador de registros de Sofia
        /// </summary>
        public RegisterySofiaController(RegisterySofiaBusiness registerySofiaBusiness, ILogger<RegisterySofiaController> logger)
        {
            _registerySofiaBusiness = registerySofiaBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de Sofia del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RegisterySofiaDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRegisterySofias()
        {
            try
            {
                var registerySofias = await _registerySofiaBusiness.GetAllRegisterySofiasAsync();
                return Ok(registerySofias);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registros de Sofia");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un registro de Sofia específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RegisterySofiaDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRegisterySofiaById(int id)
        {
            try
            {
                var registerySofia = await _registerySofiaBusiness.GetRegisterySofiaByIdAsync(id);
                return Ok(registerySofia);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el registro de Sofia con ID: {RegisterySofiaId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado con ID: {RegisterySofiaId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registro de Sofia con ID: {RegisterySofiaId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo registro de Sofia en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RegisterySofiaDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRegisterySofia([FromBody] RegisterySofiaDto registerySofiaDto)
        {
            try
            {
                var createdRegisterySofia = await _registerySofiaBusiness.CreateRegisterySofiaAsync(registerySofiaDto);
                return CreatedAtAction(nameof(GetRegisterySofiaById), new { id = createdRegisterySofia.Id }, createdRegisterySofia);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear registro de Sofia");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear registro de Sofia");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un registro de Sofia existente (reemplazo completo)
        /// </summary>
        /// <param name="id">ID del registro a actualizar</param>
        /// <param name="registerySofiaDto">Datos completos del registro para actualizar</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RegisterySofiaDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRegisterySofia(int id, [FromBody] RegisterySofiaDto registerySofiaDto)
        {
            try
            {
                var updatedRegistery = await _registerySofiaBusiness.UpdateRegisterySofiaAsync(id, registerySofiaDto);
                return Ok(updatedRegistery);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar registro de Sofia con ID: {RegisterySofiaId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado para actualizar con ID: {RegisterySofiaId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar registro de Sofia con ID: {RegisterySofiaId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un registro de Sofia existente
        /// </summary>
        /// <param name="id">ID del registro a actualizar</param>
        /// <param name="registerySofiaDto">Datos parciales a aplicar (Name, Description, Document)</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RegisterySofiaDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchRegisterySofia(int id, [FromBody] RegisterySofiaDto registerySofiaDto)
        {
            try
            {
                var patchedRegistery = await _registerySofiaBusiness.PatchRegisterySofiaAsync(id, registerySofiaDto);
                return Ok(patchedRegistery);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a registro de Sofia con ID: {RegisterySofiaId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado para aplicar patch con ID: {RegisterySofiaId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a registro de Sofia con ID: {RegisterySofiaId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina permanentemente un registro de Sofia
        /// </summary>
        /// <remarks>Precaución: Esta operación es irreversible y fallará si existen entidades dependientes.</remarks>
        /// <param name="id">ID del registro a eliminar</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)] // O 409 Conflict
        public async Task<IActionResult> DeleteRegisterySofia(int id)
        {
            try
            {
                await _registerySofiaBusiness.DeleteRegisterySofiaAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar registro de Sofia con ID: {RegisterySofiaId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado para eliminar con ID: {RegisterySofiaId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Puede ser error de FK
            {
                _logger.LogError(ex, "Error al eliminar registro de Sofia con ID: {RegisterySofiaId}. Posible dependencia.", id);
                return StatusCode(500, new { message = "Error al eliminar el registro de Sofia. Verifique si hay dependencias." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) un registro de Sofia
        /// </summary>
        /// <param name="id">ID del registro a desactivar</param>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteRegisterySofia(int id)
        {
            try
            {
                await _registerySofiaBusiness.SoftDeleteRegisterySofiaAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar soft-delete de registro de Sofia con ID: {RegisterySofiaId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado para soft-delete con ID: {RegisterySofiaId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete de registro de Sofia con ID: {RegisterySofiaId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
