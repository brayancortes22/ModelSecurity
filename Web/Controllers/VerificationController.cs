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
    /// Controlador para la gestión de verificaciones en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class VerificationController : ControllerBase
    {
        private readonly VerificationBusiness _verificationBusiness;
        private readonly ILogger<VerificationController> _logger;

        /// <summary>
        /// Constructor del controlador de verificaciones
        /// </summary>
        public VerificationController(VerificationBusiness verificationBusiness, ILogger<VerificationController> logger)
        {
            _verificationBusiness = verificationBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las verificaciones del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VerificationDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllVerifications()
        {
            try
            {
                var verifications = await _verificationBusiness.GetAllVerificationsAsync();
                return Ok(verifications);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener verificaciones");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una verificación específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VerificationDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetVerificationById(int id)
        {
            try
            {
                var verification = await _verificationBusiness.GetVerificationByIdAsync(id);
                return Ok(verification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la verificación con ID: {VerificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Verificación no encontrada con ID: {VerificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener verificación con ID: {VerificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva verificación en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(VerificationDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateVerification([FromBody] VerificationDto verificationDto)
        {
            try
            {
                var createdVerification = await _verificationBusiness.CreateVerificationAsync(verificationDto);
                return CreatedAtAction(nameof(GetVerificationById), new { id = createdVerification.Id }, createdVerification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear verificación");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear verificación");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una verificación existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(VerificationDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateVerification(int id, [FromBody] VerificationDto verificationDto)
        {
            try
            {
                var updatedVerification = await _verificationBusiness.UpdateVerificationAsync(id, verificationDto);
                return Ok(updatedVerification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar verificación con ID: {VerificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Verificación no encontrada para actualizar con ID: {VerificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar verificación con ID: {VerificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente una verificación existente
        /// </summary>
        /// <param name="id">ID de la verificación a actualizar</param>
        /// <param name="verificationDto">Datos parciales a aplicar (Name, Observation)</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(VerificationDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchVerification(int id, [FromBody] VerificationDto verificationDto)
        {
            try
            {
                var patchedVerification = await _verificationBusiness.PatchVerificationAsync(id, verificationDto);
                return Ok(patchedVerification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a verificación con ID: {VerificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Verificación no encontrada para aplicar patch con ID: {VerificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a verificación con ID: {VerificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina permanentemente una verificación por su ID
        /// </summary>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteVerification(int id)
        {
            try
            {
                await _verificationBusiness.LogicalDeleteVerificationAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar soft-delete de verificación con ID: {VerificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Verificación no encontrada para soft-delete con ID: {VerificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete de verificación con ID: {VerificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
