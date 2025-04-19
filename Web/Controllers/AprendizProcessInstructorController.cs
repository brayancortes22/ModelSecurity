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
    /// Controlador para la gestión de AprendizProcessInstructor en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AprendizProcessInstructorController : ControllerBase
    {
        private readonly AprendizProcessInstructorBusiness _aprendizProcessInstructorBusiness;
        private readonly ILogger<AprendizProcessInstructorController> _logger;

        /// <summary>
        /// Constructor del controlador de AprendizProcessInstructor
        /// </summary>
        public AprendizProcessInstructorController(AprendizProcessInstructorBusiness aprendizProcessInstructorBusiness, ILogger<AprendizProcessInstructorController> logger)
        {
            _aprendizProcessInstructorBusiness = aprendizProcessInstructorBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los procesos de aprendiz con instructor en el sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AprendizProcessInstructorDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAprendizProcessInstructors()
        {
            try
            {
                var aprendizProcessInstructors = await _aprendizProcessInstructorBusiness.GetAllAprendizProcessInstructorsAsync();
                return Ok(aprendizProcessInstructors);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener procesos de aprendiz con instructor");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un proceso de aprendiz con instructor específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AprendizProcessInstructorDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAprendizProcessInstructorById(int id)
        {
            try
            {
                var aprendizProcessInstructor = await _aprendizProcessInstructorBusiness.GetAprendizProcessInstructorByIdAsync(id);
                return Ok(aprendizProcessInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el proceso de aprendiz con instructor con ID: {AprendizProcessInstructorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso de aprendiz con instructor no encontrado con ID: {AprendizProcessInstructorId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener proceso de aprendiz con instructor con ID: {AprendizProcessInstructorId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo proceso de aprendiz con instructor en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AprendizProcessInstructorDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAprendizProcessInstructor([FromBody] AprendizProcessInstructorDto aprendizProcessInstructorDto)
        {
            try
            {
                var createdAprendizProcessInstructor = await _aprendizProcessInstructorBusiness.CreateAprendizProcessInstructorAsync(aprendizProcessInstructorDto);
                return CreatedAtAction(nameof(GetAprendizProcessInstructorById), new { id = createdAprendizProcessInstructor.Id }, createdAprendizProcessInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear proceso de aprendiz con instructor");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear proceso de aprendiz con instructor");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una relación Aprendiz-Proceso-Instructor existente.
        /// </summary>
        /// <param name="id">ID de la relación a actualizar.</param>
        /// <param name="dto">Datos actualizados de la relación.</param>
        /// <response code="200">Retorna la relación actualizada.</response>
        /// <response code="400">Si el ID es inválido, no coincide con el DTO, o los datos del DTO son inválidos.</response>
        /// <response code="404">Si no se encuentra la relación con el ID especificado.</response>
        /// <response code="500">Si ocurre un error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AprendizProcessInstructorDto), 200)]
        [ProducesResponseType(typeof(object), 400)] // Cambiado para objeto anónimo
        [ProducesResponseType(typeof(object), 404)] // Cambiado para objeto anónimo
        [ProducesResponseType(typeof(object), 500)] // Cambiado para objeto anónimo
        public async Task<IActionResult> UpdateAprendizProcessInstructor(int id, [FromBody] AprendizProcessInstructorDto dto)
        {
            // Opcional: verificar si el modelo es válido si usas DataAnnotations en el DTO
            // if (!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
            // }

            try
            {
                var updatedDto = await _aprendizProcessInstructorBusiness.UpdateAprendizProcessInstructorAsync(id, dto);
                return Ok(updatedDto);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar relación {RelationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación no encontrada para actualizar con ID: {RelationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Captura errores de BD u otros servicios
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar relación {RelationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex) // Captura genérica para otros errores inesperados
            {
                 _logger.LogError(ex, "Error inesperado al actualizar relación {RelationId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." }); // Mensaje genérico para el cliente
            }
        }

        /// <summary>
        /// Elimina una relación Aprendiz-Proceso-Instructor por su ID.
        /// </summary>
        /// <param name="id">ID de la relación a eliminar.</param>
        /// <response code="204">Si la eliminación fue exitosa.</response>
        /// <response code="400">Si el ID proporcionado es inválido.</response>
        /// <response code="404">Si no se encuentra la relación con el ID especificado.</response>
        /// <response code="500">Si ocurre un error interno del servidor (p.ej., violación de FK).</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // NoContent
        [ProducesResponseType(typeof(object), 400)] // Cambiado para objeto anónimo
        [ProducesResponseType(typeof(object), 404)] // Cambiado para objeto anónimo
        [ProducesResponseType(typeof(object), 500)] // Cambiado para objeto anónimo
        public async Task<IActionResult> DeleteAprendizProcessInstructor(int id)
        {
            try
            {
                await _aprendizProcessInstructorBusiness.DeleteAprendizProcessInstructorAsync(id);
                return NoContent(); // 204 No Content es la respuesta estándar para DELETE exitoso
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar eliminar relación {RelationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación no encontrada para eliminar con ID: {RelationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Captura errores de BD (como FK violation) u otros
            {
                _logger.LogError(ex, "Error de servicio externo al eliminar relación {RelationId}", id);
                // Podrías dar un mensaje más específico si sabes que es una FK
                // if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547) // Ejemplo para SQL Server FK violation
                // {
                //    return Conflict(new { message = "No se puede eliminar la relación porque está siendo utilizada."; });
                // }
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex) // Captura genérica
            {
                 _logger.LogError(ex, "Error inesperado al eliminar relación {RelationId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }
    }
}