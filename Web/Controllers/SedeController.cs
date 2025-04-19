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
    /// Controlador para la gestión de sedes en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SedeController : ControllerBase
    {
        private readonly SedeBusiness _sedeBusiness;
        private readonly ILogger<SedeController> _logger;

        /// <summary>
        /// Constructor del controlador de sedes
        /// </summary>
        public SedeController(SedeBusiness sedeBusiness, ILogger<SedeController> logger)
        {
            _sedeBusiness = sedeBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las sedes del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SedeDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllSedes()
        {
            try
            {
                var sedes = await _sedeBusiness.GetAllSedesAsync();
                return Ok(sedes);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener sedes");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una sede específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SedeDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSedeById(int id)
        {
            try
            {
                var sede = await _sedeBusiness.GetSedeByIdAsync(id);
                return Ok(sede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la sede con ID: {SedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener sede con ID: {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva sede en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(SedeDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateSede([FromBody] SedeDto sedeDto)
        {
            try
            {
                var createdSede = await _sedeBusiness.CreateSedeAsync(sedeDto);
                return CreatedAtAction(nameof(GetSedeById), new { id = createdSede.Id }, createdSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear sede");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear sede");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una sede existente (reemplazo completo).
        /// </summary>
        /// <param name="id">ID de la sede a actualizar.</param>
        /// <param name="sedeDto">Datos actualizados de la sede.</param>
        /// <response code="200">Retorna la sede actualizada.</response>
        /// <response code="400">Si el ID o los datos son inválidos.</response>
        /// <response code="404">Si no se encuentra la sede.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SedeDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> UpdateSede(int id, [FromBody] SedeDto sedeDto)
        {
            // Opcional: if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updatedSede = await _sedeBusiness.UpdateSedeAsync(id, sedeDto);
                return Ok(updatedSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar sede {SedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada para actualizar con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar sede {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al actualizar sede {SedeId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Actualiza parcialmente una sede existente.
        /// </summary>
        /// <param name="id">ID de la sede a actualizar.</param>
        /// <param name="sedeDto">Datos parciales a actualizar.</param>
        /// <remarks>NOTA: Se recomienda usar JsonPatch.</remarks>
        /// <response code="200">Retorna la sede con los cambios aplicados.</response>
        /// <response code="400">Si el ID o los datos son inválidos.</response>
        /// <response code="404">Si no se encuentra la sede.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(SedeDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> PatchSede(int id, [FromBody] SedeDto sedeDto)
        {
            try
            {
                var patchedSede = await _sedeBusiness.PatchSedeAsync(id, sedeDto);
                return Ok(patchedSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a sede {SedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada para aplicar patch con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al aplicar patch a sede {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al aplicar patch a sede {SedeId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Elimina permanentemente una sede por su ID.
        /// </summary>
        /// <param name="id">ID de la sede a eliminar.</param>
        /// <remarks>ADVERTENCIA: Operación destructiva. Fallará si tiene usuarios asociados (UserSedes).</remarks>
        /// <response code="204">Si la eliminación fue exitosa.</response>
        /// <response code="400">Si el ID es inválido.</response>
        /// <response code="404">Si no se encuentra la sede.</response>
        /// <response code="500">Si ocurre un error interno (p.ej., violación de FK).</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> DeleteSede(int id)
        {
            try
            {
                await _sedeBusiness.DeleteSedeAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar eliminar sede {SedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada para eliminar con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Captura errores de BD (FK violation)
            {
                _logger.LogError(ex, "Error de servicio externo al eliminar sede {SedeId}", id);
                // Considerar 409 Conflict para FK
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al eliminar sede {SedeId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) una sede por su ID.
        /// </summary>
        /// <param name="id">ID de la sede a desactivar.</param>
        /// <response code="204">Si la desactivación fue exitosa o ya estaba inactiva.</response>
        /// <response code="400">Si el ID es inválido.</response>
        /// <response code="404">Si no se encuentra la sede.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> SoftDeleteSede(int id)
        {
            try
            {
                await _sedeBusiness.SoftDeleteSedeAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar sede {SedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada para desactivar con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al desactivar sede {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al desactivar sede {SedeId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }
    }
}
