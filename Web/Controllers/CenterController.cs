using Business;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;
using System; // Para ArgumentNullException y otros tipos base
using Microsoft.AspNetCore.Http; // Para StatusCodes

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Center en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CenterController : ControllerBase
    {
        private readonly CenterBusiness _centerBusiness;
        private readonly ILogger<CenterController> _logger;

        /// <summary>
        /// Constructor del controlador de Center
        /// </summary>
        public CenterController(CenterBusiness centerBusiness, ILogger<CenterController> logger)
        {
            _centerBusiness = centerBusiness ?? throw new ArgumentNullException(nameof(centerBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los centros del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CenterDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCenters()
        {
            try
            {
                var centers = await _centerBusiness.GetAllCentersAsync();
                 _logger.LogInformation("Se obtuvieron {Count} centros.", centers.Count());
                return Ok(centers);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al obtener todos los centros.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al obtener todos los centros.");
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Obtiene un centro específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CenterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCenterById(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Intento de obtener un centro con ID inválido: {CenterId}", id);
                 return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            try
            {
                var center = await _centerBusiness.GetCenterByIdAsync(id);
                 _logger.LogInformation("Centro con ID {CenterId} obtenido exitosamente.", id);
                return Ok(center);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el centro con ID: {CenterId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Centro no encontrado con ID: {CenterId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al obtener centro con ID: {CenterId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al obtener centro con ID: {CenterId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Crea un nuevo centro en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CenterDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCenter([FromBody] CenterDto centerDto)
        {
            if (centerDto == null)
            {
                 _logger.LogWarning("Intento de crear un centro con datos nulos.");
                 return BadRequest(new { message = "El centro no puede ser nulo." });
            }
            try
            {
                var createdCenter = await _centerBusiness.CreateCenterAsync(centerDto);
                 _logger.LogInformation("Centro creado exitosamente con ID {CenterId}.", createdCenter.Id);
                return CreatedAtAction(nameof(GetCenterById), new { id = createdCenter.Id }, createdCenter);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear centro");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al crear centro");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
             catch (Exception ex) // Captura general para errores inesperados
            {
                 _logger.LogError(ex, "Error inesperado al crear centro");
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Actualiza un centro existente en el sistema.
        /// </summary>
        /// <param name="id">ID del centro a actualizar.</param>
        /// <param name="centerDto">Datos actualizados del centro.</param>
        /// <returns>Respuesta indicando éxito o fracaso.</returns>
        /// <response code="200">Centro actualizado exitosamente.</response>
        /// <response code="400">ID inválido o datos del centro no válidos.</response>
        /// <response code="404">Centro no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCenter(int id, [FromBody] CenterDto centerDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de actualizar un centro con ID inválido: {CenterId}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            if (centerDto == null || id != centerDto.Id)
            {
                _logger.LogWarning("Datos inválidos para actualizar el centro con ID: {CenterId}. DTO: {@CenterDto}", id, centerDto);
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del centro proporcionado o el cuerpo es nulo." });
            }

            try
            {
                // CenterBusiness.UpdateCenterAsync no devuelve la entidad, así que no la capturamos.
                await _centerBusiness.UpdateCenterAsync(id, centerDto);
                _logger.LogInformation("Centro con ID {CenterId} actualizado exitosamente.", id);
                // Devolvemos Ok con un mensaje, similar a AprendizProgramController
                return Ok(new { message = $"Centro con ID {id} actualizado exitosamente." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar centro con ID: {CenterId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Centro no encontrado para actualizar con ID: {CenterId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar centro con ID: {CenterId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar centro con ID: {CenterId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un centro existente en el sistema.
        /// </summary>
        /// <param name="id">ID del centro a actualizar parcialmente.</param>
        /// <param name="centerDto">Datos a actualizar del centro.</param>
        /// <returns>Respuesta indicando éxito o fracaso.</returns>
        /// <response code="200">Centro actualizado parcialmente.</response>
        /// <response code="400">ID inválido o datos no válidos.</response>
        /// <response code="404">Centro no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchCenter(int id, [FromBody] CenterDto centerDto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Intento de aplicar patch a un centro con ID inválido: {CenterId}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            if (centerDto == null)
            {
                _logger.LogWarning("Intento de aplicar patch a un centro con ID {CenterId} con un DTO nulo.", id);
                return BadRequest(new { message = "El cuerpo de la solicitud (centro) no puede ser nulo." });
            }

            if (centerDto.Id != 0 && id != centerDto.Id)
            {
                 _logger.LogWarning("ID de ruta {RouteId} no coincide con ID de cuerpo {BodyId} en PATCH", id, centerDto.Id);
                 return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo." });
            }

            try
            {
                await _centerBusiness.PatchCenterAsync(id, centerDto);
                 _logger.LogInformation("Patch aplicado exitosamente al centro con ID {CenterId}.", id);
                return Ok(new { message = $"Patch aplicado exitosamente al centro con ID {id}." });
            }
             catch (ValidationException ex)
             {
                 _logger.LogWarning(ex, "Validación fallida al aplicar patch al centro con ID: {CenterId}", id);
                 return BadRequest(new { message = ex.Message });
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Centro no encontrado para aplicar patch con ID: {CenterId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error externo al aplicar patch al centro con ID: {CenterId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
             }
             catch (Exception ex)
             {
                _logger.LogError(ex, "Error inesperado al aplicar patch al centro con ID: {CenterId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
             }
        }

        /// <summary>
        /// Elimina un centro del sistema (eliminación persistente).
        /// </summary>
        /// <param name="id">ID del centro a eliminar.</param>
        /// <returns>Respuesta indicando éxito.</returns>
        /// <response code="204">Centro eliminado exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Centro no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCenter(int id)
        {
             if (id <= 0)
             {
                 _logger.LogWarning("Intento de eliminar un centro con ID inválido: {CenterId}", id);
                 return BadRequest(new { message = "El ID proporcionado es inválido." });
             }

             try
             {
                 await _centerBusiness.DeleteCenterAsync(id);
                  _logger.LogInformation("Centro con ID {CenterId} eliminado exitosamente.", id);
                 return NoContent(); // 204 No Content es apropiado para DELETE exitoso
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Centro no encontrado para eliminar con ID: {CenterId}", id);
                 return NotFound(new { message = ex.Message }); // 404 si no existe
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error externo al eliminar centro con ID: {CenterId}", id);
                 // Podría ser un problema de FK, informar error genérico
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error al eliminar el centro: {ex.Message}" });
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "Error inesperado al eliminar centro con ID: {CenterId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
             }
        }

         /// <summary>
        /// Desactiva un centro en el sistema (eliminación lógica).
        /// </summary>
        /// <param name="id">ID del centro a desactivar.</param>
        /// <returns>Respuesta indicando éxito.</returns>
        /// <response code="204">Centro desactivado exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Centro no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}/soft")] // Usando DELETE /{id}/soft consistentemente
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SoftDeleteCenter(int id)
        {
             if (id <= 0)
             {
                 _logger.LogWarning("Intento de realizar borrado lógico a un centro con ID inválido: {CenterId}", id);
                 return BadRequest(new { message = "El ID proporcionado es inválido." });
             }

             try
             {
                 await _centerBusiness.SoftDeleteCenterAsync(id);
                 _logger.LogInformation("Borrado lógico realizado exitosamente para el centro con ID {CenterId}.", id);
                 return NoContent(); // 204 No Content indica éxito
             }
             catch (ValidationException ex) // Capturar si SoftDelete valida y falla (ej: ya inactivo)
             {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar centro con ID: {CenterId}", id);
                 return BadRequest(new { message = ex.Message });
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Centro no encontrado para desactivar con ID: {CenterId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error externo al desactivar centro con ID: {CenterId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "Error inesperado al desactivar centro con ID: {CenterId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
             }
        }

    }
}
