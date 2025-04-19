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
    /// Controlador para la gestión de modalidades en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TypeModalityController : ControllerBase
    {
        private readonly TypeModalityBusiness _typeModalityBusiness;
        private readonly ILogger<TypeModalityController> _logger;

        /// <summary>
        /// Constructor del controlador de modalidades
        /// </summary>
        public TypeModalityController(TypeModalityBusiness typeModalityBusiness, ILogger<TypeModalityController> logger)
        {
            _typeModalityBusiness = typeModalityBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las modalidades del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TypeModalityDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllTypeModalities()
        {
            try
            {
                var typeModalities = await _typeModalityBusiness.GetAllTypeModalitiesAsync();
                return Ok(typeModalities);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener modalidades");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una modalidad específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TypeModalityDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTypeModalityById(int id)
        {
            try
            {
                var typeModality = await _typeModalityBusiness.GetTypeModalityByIdAsync(id);
                return Ok(typeModality);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la modalidad con ID: {TypeModalityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Modalidad no encontrada con ID: {TypeModalityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener modalidad con ID: {TypeModalityId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva modalidad en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TypeModalityDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateTypeModality([FromBody] TypeModalityDto typeModalityDto)
        {
            try
            {
                var createdTypeModality = await _typeModalityBusiness.CreateTypeModalityAsync(typeModalityDto);
                return CreatedAtAction(nameof(GetTypeModalityById), new { id = createdTypeModality.Id }, createdTypeModality);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear modalidad");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear modalidad");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una modalidad existente (reemplazo completo)
        /// </summary>
        /// <param name="id">ID de la modalidad a actualizar</param>
        /// <param name="typeModalityDto">Datos de la modalidad para actualizar</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TypeModalityDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateTypeModality(int id, [FromBody] TypeModalityDto typeModalityDto)
        {
            try
            {
                var updatedModality = await _typeModalityBusiness.UpdateTypeModalityAsync(id, typeModalityDto);
                return Ok(updatedModality);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar modalidad con ID: {TypeModalityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Modalidad no encontrada para actualizar con ID: {TypeModalityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar modalidad con ID: {TypeModalityId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente una modalidad existente
        /// </summary>
        /// <remarks>
        /// La implementación actual en `TypeModalityBusiness` actualiza solo los campos `Name` y `Description` proporcionados.
        /// </remarks>
        /// <param name="id">ID de la modalidad a actualizar parcialmente</param>
        /// <param name="typeModalityDto">Datos parciales de la modalidad para aplicar</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(TypeModalityDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchTypeModality(int id, [FromBody] TypeModalityDto typeModalityDto)
        {
            // Nota: El DTO para PATCH podría ser diferente (ej. usando JsonPatchDocument)
            // pero por simplicidad usamos el mismo DTO.
            try
            {
                var patchedModality = await _typeModalityBusiness.PatchTypeModalityAsync(id, typeModalityDto);
                return Ok(patchedModality);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a modalidad con ID: {TypeModalityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Modalidad no encontrada para aplicar patch con ID: {TypeModalityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a modalidad con ID: {TypeModalityId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina permanentemente una modalidad
        /// </summary>
        /// <remarks>Precaución: Esta operación es irreversible y fallará si existen entidades dependientes.</remarks>
        /// <param name="id">ID de la modalidad a eliminar</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)] // Podría ser 409 Conflict si hay dependencias
        public async Task<IActionResult> DeleteTypeModality(int id)
        {
            try
            {
                await _typeModalityBusiness.DeleteTypeModalityAsync(id);
                return NoContent(); // Éxito, sin contenido
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar modalidad con ID: {TypeModalityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Modalidad no encontrada para eliminar con ID: {TypeModalityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Puede ser error de FK
            {
                _logger.LogError(ex, "Error al eliminar modalidad con ID: {TypeModalityId}. Posible dependencia.", id);
                // Devolver 500 o 409 (Conflict) podría ser apropiado dependiendo de la causa exacta
                return StatusCode(500, new { message = "Error al eliminar la modalidad. Verifique si hay dependencias." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) una modalidad
        /// </summary>
        /// <param name="id">ID de la modalidad a desactivar</param>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteTypeModality(int id)
        {
            try
            {
                await _typeModalityBusiness.SoftDeleteTypeModalityAsync(id);
                return NoContent(); // Éxito, sin contenido
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar soft-delete de modalidad con ID: {TypeModalityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Modalidad no encontrada para soft-delete con ID: {TypeModalityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete de modalidad con ID: {TypeModalityId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
