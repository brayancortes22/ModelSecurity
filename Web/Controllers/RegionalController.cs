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
    /// Controlador para la gestión de regiones en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RegionalController : ControllerBase
    {
        private readonly RegionalBusiness _regionalBusiness;
        private readonly ILogger<RegionalController> _logger;

        /// <summary>
        /// Constructor del controlador de regiones
        /// </summary>
        public RegionalController(RegionalBusiness regionalBusiness, ILogger<RegionalController> logger)
        {
            _regionalBusiness = regionalBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las regiones del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RegionalDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRegionals()
        {
            try
            {
                var regionals = await _regionalBusiness.GetAllRegionalsAsync();
                return Ok(regionals);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener regiones");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una región específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RegionalDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRegionalById(int id)
        {
            try
            {
                var regional = await _regionalBusiness.GetRegionalByIdAsync(id);
                return Ok(regional);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la región con ID: {RegionalId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Región no encontrada con ID: {RegionalId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener región con ID: {RegionalId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva región en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RegionalDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRegional([FromBody] RegionalDto regionalDto)
        {
            try
            {
                var createdRegional = await _regionalBusiness.CreateRegionalAsync(regionalDto);
                return CreatedAtAction(nameof(GetRegionalById), new { id = createdRegional.Id }, createdRegional);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear región");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear región");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una región existente (reemplazo completo).
        /// </summary>
        /// <param name="id">ID de la región a actualizar.</param>
        /// <param name="regionalDto">Datos actualizados de la región.</param>
        /// <response code="200">Retorna la región actualizada.</response>
        /// <response code="400">Si el ID es inválido, no coincide con el DTO, o los datos del DTO son inválidos.</response>
        /// <response code="404">Si no se encuentra la región con el ID especificado.</response>
        /// <response code="500">Si ocurre un error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RegionalDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> UpdateRegional(int id, [FromBody] RegionalDto regionalDto)
        {
             // Opcional: if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updatedRegional = await _regionalBusiness.UpdateRegionalAsync(id, regionalDto);
                return Ok(updatedRegional); // 200 OK con el objeto actualizado
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar región {RegionalId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Región no encontrada para actualizar con ID: {RegionalId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex) // Errores de BD u otros servicios
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar región {RegionalId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
            catch (Exception ex) // Captura genérica
            {
                 _logger.LogError(ex, "Error inesperado al actualizar región {RegionalId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza parcialmente una región existente.
        /// </summary>
        /// <param name="id">ID de la región a actualizar.</param>
        /// <param name="regionalDto">Datos parciales a actualizar. Solo incluir los campos a modificar.</param>
        /// <remarks>NOTA: Se recomienda usar JsonPatch para PATCH, pero aquí se usa el DTO completo por simplicidad. Los campos no proporcionados (null) no se modificarán.</remarks>
        /// <response code="200">Retorna la región con los cambios aplicados.</response>
        /// <response code="400">Si el ID es inválido o los datos del DTO son inválidos.</response>
        /// <response code="404">Si no se encuentra la región con el ID especificado.</response>
        /// <response code="500">Si ocurre un error interno del servidor.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RegionalDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> PatchRegional(int id, [FromBody] RegionalDto regionalDto) // Idealmente usaría JsonPatchDocument<RegionalDto>
        {
             // Opcional: Validación específica para PATCH si es necesario
            try
            {
                // Usando el DTO completo para PATCH (simplificado)
                var patchedRegional = await _regionalBusiness.PatchRegionalAsync(id, regionalDto);
                return Ok(patchedRegional); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a región {RegionalId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Región no encontrada para aplicar patch con ID: {RegionalId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al aplicar patch a región {RegionalId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al aplicar patch a región {RegionalId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." }); // 500 Internal Server Error
            }
        }


        /// <summary>
        /// Elimina permanentemente una región por su ID.
        /// </summary>
        /// <param name="id">ID de la región a eliminar.</param>
        /// <remarks>ADVERTENCIA: Esta operación es destructiva y fallará si la región tiene Centros asociados.</remarks>
        /// <response code="204">Si la eliminación fue exitosa.</response>
        /// <response code="400">Si el ID proporcionado es inválido.</response>
        /// <response code="404">Si no se encuentra la región con el ID especificado.</response>
        /// <response code="500">Si ocurre un error interno (p.ej., violación de FK por Centros asociados).</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // NoContent
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> DeleteRegional(int id)
        {
            try
            {
                await _regionalBusiness.DeleteRegionalAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar eliminar región {RegionalId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Región no encontrada para eliminar con ID: {RegionalId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex) // Captura errores de BD (como FK violation)
            {
                _logger.LogError(ex, "Error de servicio externo al eliminar región {RegionalId}", id);
                // Podría devolverse un 409 Conflict si es una violación de FK conocida
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
            catch (Exception ex) // Captura genérica
            {
                 _logger.LogError(ex, "Error inesperado al eliminar región {RegionalId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) una región por su ID.
        /// </summary>
        /// <param name="id">ID de la región a desactivar.</param>
        /// <response code="204">Si la desactivación fue exitosa o la región ya estaba inactiva.</response>
        /// <response code="400">Si el ID proporcionado es inválido.</response>
        /// <response code="404">Si no se encuentra la región con el ID especificado.</response>
        /// <response code="500">Si ocurre un error interno del servidor.</response>
        [HttpDelete("{id}/soft")] // Ruta distinta para soft delete
        [ProducesResponseType(204)] // NoContent
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> SoftDeleteRegional(int id)
        {
            try
            {
                await _regionalBusiness.SoftDeleteRegionalAsync(id);
                return NoContent(); // 204 No Content incluso si ya estaba inactivo
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar región {RegionalId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Región no encontrada para desactivar con ID: {RegionalId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex) // Errores de BD
            {
                _logger.LogError(ex, "Error de servicio externo al desactivar región {RegionalId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
            catch (Exception ex) // Captura genérica
            {
                 _logger.LogError(ex, "Error inesperado al desactivar región {RegionalId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }
    }
}
