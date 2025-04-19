using Business;
using Data;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de aprendices en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AprendizController : ControllerBase
    {
        private readonly AprendizBusiness _AprendizBusiness;
        private readonly ILogger<AprendizController> _logger;

        /// <summary>
        /// Constructor del controlador de aprendices
        /// </summary>
        public AprendizController(AprendizBusiness AprendizBusiness, ILogger<AprendizController> logger)
        {
            _AprendizBusiness = AprendizBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los aprendices del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AprendizDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAprendices()
        {
            try
            {
                var aprendices = await _AprendizBusiness.GetAllAprendizAsync();
                return Ok(aprendices);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener aprendices");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un aprendiz específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AprendizDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAprendizById(int id)
        {
            try
            {
                var aprendiz = await _AprendizBusiness.GetAprendizByIdAsync(id);
                return Ok(aprendiz);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el aprendiz con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "aprenidz no encontrado con ID: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener aprendiz con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo aprendiz en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AprendizDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAprendiz([FromBody] AprendizDto AprendizDto)
        {
            try
            {
                var createdAprendiz = await _AprendizBusiness.CreateAprendizAsync(AprendizDto);
                return CreatedAtAction(nameof(GetAprendizById), new { id = createdAprendiz.Id }, createdAprendiz);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear aprendiz");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear aprendiz");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un aprendiz existente en el sistema.
        /// </summary>
        /// <param name="id">ID del aprendiz a actualizar.</param>
        /// <param name="aprendizDto">Datos actualizados del aprendiz.</param>
        /// <returns>El aprendiz actualizado.</returns>
        /// <response code="200">Retorna el aprendiz actualizado.</response>
        /// <response code="400">ID inválido o datos del aprendiz no válidos.</response>
        /// <response code="404">Aprendiz no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AprendizDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateAprendiz(int id, [FromBody] AprendizDto aprendizDto)
        {
            // Validar consistencia del ID
            if (id != aprendizDto.Id)
            {
                _logger.LogWarning("El ID de la ruta ({RouteId}) no coincide con el ID del cuerpo ({BodyId}) para la actualización del aprendiz.", id, aprendizDto.Id);
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo." });
            }

            try
            {
                var updatedAprendiz = await _AprendizBusiness.UpdateAprendizAsync(id, aprendizDto);
                return Ok(updatedAprendiz);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar aprendiz con ID: {AprendizId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Aprendiz no encontrado para actualizar con ID: {AprendizId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar aprendiz con ID: {AprendizId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un aprendiz existente.
        /// </summary>
        /// <param name="id">ID del aprendiz a actualizar parcialmente.</param>
        /// <param name="aprendizDto">Datos a actualizar del aprendiz.</param>
        /// <returns>El aprendiz actualizado.</returns>
        /// <response code="200">Retorna el aprendiz parcialmente actualizado.</response>
        /// <response code="400">ID inválido o datos del aprendiz no válidos.</response>
        /// <response code="404">Aprendiz no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(AprendizDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchAprendiz(int id, [FromBody] AprendizDto aprendizDto)
        {
            // Asegurar consistencia del ID para la implementación actual de negocio
             if (id != aprendizDto.Id)
             {
                  _logger.LogWarning("El ID de la ruta ({RouteId}) no coincide con el ID del cuerpo ({BodyId}) para el patch del aprendiz.", id, aprendizDto.Id);
                  return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo para PATCH." });
             }

            try
            {
                var patchedAprendiz = await _AprendizBusiness.PatchAprendizAsync(id, aprendizDto);
                return Ok(patchedAprendiz);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch al aprendiz con ID: {AprendizId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Aprendiz no encontrado para aplicar patch con ID: {AprendizId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch al aprendiz con ID: {AprendizId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un aprendiz del sistema (persistente).
        /// </summary>
        /// <param name="id">ID del aprendiz a eliminar.</param>
        /// <returns>No Content si la eliminación fue exitosa.</returns>
        /// <response code="204">Aprendiz eliminado exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Aprendiz no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAprendiz(int id)
        {
            try
            {
                await _AprendizBusiness.DeleteAprendizAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar eliminar aprendiz con ID: {AprendizId}", id);
                 return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Aprendiz no encontrado para eliminar con ID: {AprendizId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar aprendiz con ID: {AprendizId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Desactiva un aprendiz en el sistema (eliminación lógica).
        /// </summary>
        /// <param name="id">ID del aprendiz a desactivar.</param>
        /// <returns>No Content si la desactivación fue exitosa.</returns>
        /// <response code="204">Aprendiz desactivado exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Aprendiz no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPatch("soft-delete/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteAprendiz(int id)
        {
            try
            {
                await _AprendizBusiness.SoftDeleteAprendizAsync(id);
                return NoContent();
            }
             catch (ValidationException ex)
             {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar aprendiz con ID: {AprendizId}", id);
                 return BadRequest(new { message = ex.Message });
             }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Aprendiz no encontrado para desactivar con ID: {AprendizId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al desactivar aprendiz con ID: {AprendizId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
