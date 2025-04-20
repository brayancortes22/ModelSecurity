using Business;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;
using System;
using Microsoft.AspNetCore.Http;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Concept en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ConceptController : ControllerBase
    {
        private readonly ConceptBusiness _conceptBusiness;
        private readonly ILogger<ConceptController> _logger;

        /// <summary>
        /// Constructor del controlador de Concept
        /// </summary>
        public ConceptController(ConceptBusiness conceptBusiness, ILogger<ConceptController> logger)
        {
            _conceptBusiness = conceptBusiness ?? throw new ArgumentNullException(nameof(conceptBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los conceptos del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConceptDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllConcepts()
        {
            try
            {
                var concepts = await _conceptBusiness.GetAllConceptsAsync();
                _logger.LogInformation("Se obtuvieron {Count} conceptos.", concepts.Count());
                return Ok(concepts);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al obtener todos los conceptos");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al obtener todos los conceptos.");
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Obtiene un concepto específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConceptDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConceptById(int id)
        {
             if (id <= 0)
            {
                 _logger.LogWarning("Intento de obtener un concepto con ID inválido: {ConceptId}", id);
                 return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            try
            {
                var concept = await _conceptBusiness.GetConceptByIdAsync(id);
                _logger.LogInformation("Concepto con ID {ConceptId} obtenido exitosamente.", id);
                return Ok(concept);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el concepto con ID: {ConceptId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Concepto no encontrado con ID: {ConceptId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al obtener concepto con ID: {ConceptId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al obtener concepto con ID: {ConceptId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Crea un nuevo concepto en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ConceptDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateConcept([FromBody] ConceptDto conceptDto)
        {
            if (conceptDto == null)
            {
                 _logger.LogWarning("Intento de crear un concepto con datos nulos.");
                 return BadRequest(new { message = "El concepto no puede ser nulo." });
            }

            try
            {
                var createdConcept = await _conceptBusiness.CreateConceptAsync(conceptDto);
                 _logger.LogInformation("Concepto creado exitosamente con ID {ConceptId}.", createdConcept.Id);
                return CreatedAtAction(nameof(GetConceptById), new { id = createdConcept.Id }, createdConcept);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear concepto");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al crear concepto");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al crear concepto");
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Actualiza un concepto existente en el sistema.
        /// </summary>
        /// <param name="id">ID del concepto a actualizar.</param>
        /// <param name="conceptDto">Datos actualizados del concepto.</param>
        /// <returns>Respuesta indicando éxito o fracaso.</returns>
        /// <response code="200">Concepto actualizado exitosamente.</response>
        /// <response code="400">ID inválido o datos del concepto no válidos.</response>
        /// <response code="404">Concepto no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateConcept(int id, [FromBody] ConceptDto conceptDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de actualizar un concepto con ID inválido: {ConceptId}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            if (conceptDto == null || id != conceptDto.Id)
            {
                _logger.LogWarning("Datos inválidos para actualizar el concepto con ID: {ConceptId}. DTO: {@ConceptDto}", id, conceptDto);
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del concepto proporcionado o el cuerpo es nulo." });
            }

            try
            {
                await _conceptBusiness.UpdateConceptAsync(id, conceptDto);
                _logger.LogInformation("Concepto con ID {ConceptId} actualizado exitosamente.", id);
                return Ok(new { message = $"Concepto con ID {id} actualizado exitosamente." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar concepto con ID: {ConceptId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Concepto no encontrado para actualizar con ID: {ConceptId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar concepto con ID: {ConceptId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar concepto con ID: {ConceptId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un concepto existente en el sistema.
        /// </summary>
        /// <param name="id">ID del concepto a actualizar parcialmente.</param>
        /// <param name="conceptDto">Datos a actualizar del concepto.</param>
        /// <returns>Respuesta indicando éxito o fracaso.</returns>
        /// <response code="200">Concepto actualizado parcialmente.</response>
        /// <response code="400">ID inválido o datos no válidos.</response>
        /// <response code="404">Concepto no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchConcept(int id, [FromBody] ConceptDto conceptDto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Intento de aplicar patch a un concepto con ID inválido: {ConceptId}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            if (conceptDto == null)
            {
                _logger.LogWarning("Intento de aplicar patch a un concepto con ID {ConceptId} con un DTO nulo.", id);
                return BadRequest(new { message = "El cuerpo de la solicitud (concepto) no puede ser nulo." });
            }
             if (conceptDto.Id != 0 && id != conceptDto.Id)
            {
                 _logger.LogWarning("ID de ruta {RouteId} no coincide con ID de cuerpo {BodyId} en PATCH", id, conceptDto.Id);
                 return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo." });
            }

            try
            {
                await _conceptBusiness.PatchConceptAsync(id, conceptDto);
                 _logger.LogInformation("Patch aplicado exitosamente al concepto con ID {ConceptId}.", id);
                return Ok(new { message = $"Patch aplicado exitosamente al concepto con ID {id}." });
            }
             catch (ValidationException ex)
             {
                 _logger.LogWarning(ex, "Validación fallida al aplicar patch al concepto con ID: {ConceptId}", id);
                 return BadRequest(new { message = ex.Message });
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Concepto no encontrado para aplicar patch con ID: {ConceptId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error externo al aplicar patch al concepto con ID: {ConceptId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
             }
             catch (Exception ex)
             {
                _logger.LogError(ex, "Error inesperado al aplicar patch al concepto con ID: {ConceptId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
             }
        }

        /// <summary>
        /// Elimina un concepto del sistema (eliminación persistente).
        /// </summary>
        /// <param name="id">ID del concepto a eliminar.</param>
        /// <returns>Respuesta indicando éxito.</returns>
        /// <response code="204">Concepto eliminado exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Concepto no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteConcept(int id)
        {
             if (id <= 0)
             {
                 _logger.LogWarning("Intento de eliminar un concepto con ID inválido: {ConceptId}", id);
                 return BadRequest(new { message = "El ID proporcionado es inválido." });
             }

             try
             {
                 await _conceptBusiness.DeleteConceptAsync(id);
                  _logger.LogInformation("Concepto con ID {ConceptId} eliminado exitosamente.", id);
                 return NoContent();
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Concepto no encontrado para eliminar con ID: {ConceptId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error externo al eliminar concepto con ID: {ConceptId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error al eliminar el concepto: {ex.Message}" });
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "Error inesperado al eliminar concepto con ID: {ConceptId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
             }
        }

         /// <summary>
        /// Desactiva un concepto en el sistema (eliminación lógica).
        /// </summary>
        /// <param name="id">ID del concepto a desactivar.</param>
        /// <returns>Respuesta indicando éxito.</returns>
        /// <response code="204">Concepto desactivado exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Concepto no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SoftDeleteConcept(int id)
        {
             if (id <= 0)
             {
                 _logger.LogWarning("Intento de realizar borrado lógico a un concepto con ID inválido: {ConceptId}", id);
                 return BadRequest(new { message = "El ID proporcionado es inválido." });
             }

             try
             {
                 await _conceptBusiness.SoftDeleteConceptAsync(id);
                 _logger.LogInformation("Borrado lógico realizado exitosamente para el concepto con ID {ConceptId}.", id);
                 return NoContent();
             }
             catch (ValidationException ex)
             {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar concepto con ID: {ConceptId}", id);
                 return BadRequest(new { message = ex.Message });
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Concepto no encontrado para desactivar con ID: {ConceptId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error externo al desactivar concepto con ID: {ConceptId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "Error inesperado al desactivar concepto con ID: {ConceptId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
             }
        }
    }
}
