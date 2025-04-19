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
    /// Controlador para la gestión de Instructor en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorBusiness _instructorBusiness;
        private readonly ILogger<InstructorController> _logger;

        /// <summary>
        /// Constructor del controlador de Instructor
        /// </summary>
        public InstructorController(InstructorBusiness instructorBusiness, ILogger<InstructorController> logger)
        {
            _instructorBusiness = instructorBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los instructores del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InstructorDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllInstructors()
        {
            try
            {
                var instructors = await _instructorBusiness.GetAllInstructorsAsync();
                return Ok(instructors);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener instructores");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un instructor específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InstructorDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetInstructorById(int id)
        {
            try
            {
                var instructor = await _instructorBusiness.GetInstructorByIdAsync(id);
                return Ok(instructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el instructor con ID: {InstructorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Instructor no encontrado con ID: {InstructorId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener instructor con ID: {InstructorId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo instructor en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(InstructorDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateInstructor([FromBody] InstructorDto instructorDto)
        {
            try
            {
                var createdInstructor = await _instructorBusiness.CreateInstructorAsync(instructorDto);
                return CreatedAtAction(nameof(GetInstructorById), new { id = createdInstructor.Id }, createdInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear instructor");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear instructor");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un instructor existente (reemplazo completo)
        /// </summary>
        /// <param name="id">ID del instructor a actualizar</param>
        /// <param name="instructorDto">Datos completos del instructor para actualizar</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(InstructorDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateInstructor(int id, [FromBody] InstructorDto instructorDto)
        {
            try
            {
                var updatedInstructor = await _instructorBusiness.UpdateInstructorAsync(id, instructorDto);
                return Ok(updatedInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar instructor con ID: {InstructorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Instructor no encontrado para actualizar con ID: {InstructorId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar instructor con ID: {InstructorId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un instructor existente
        /// </summary>
        /// <param name="id">ID del instructor a actualizar</param>
        /// <param name="instructorDto">Datos parciales a aplicar (UserId, Active)</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(InstructorDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchInstructor(int id, [FromBody] InstructorDto instructorDto)
        {
            try
            {
                var patchedInstructor = await _instructorBusiness.PatchInstructorAsync(id, instructorDto);
                return Ok(patchedInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a instructor con ID: {InstructorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Instructor no encontrado para aplicar patch con ID: {InstructorId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a instructor con ID: {InstructorId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina permanentemente un instructor
        /// </summary>
        /// <remarks>Precaución: Esta operación es irreversible y fallará si existen entidades dependientes.</remarks>
        /// <param name="id">ID del instructor a eliminar</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)] // O 409 Conflict
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            try
            {
                await _instructorBusiness.DeleteInstructorAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar instructor con ID: {InstructorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Instructor no encontrado para eliminar con ID: {InstructorId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Puede ser error de FK
            {
                _logger.LogError(ex, "Error al eliminar instructor con ID: {InstructorId}. Posible dependencia.", id);
                return StatusCode(500, new { message = "Error al eliminar el instructor. Verifique si hay dependencias." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) un instructor
        /// </summary>
        /// <param name="id">ID del instructor a desactivar</param>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteInstructor(int id)
        {
            try
            {
                await _instructorBusiness.SoftDeleteInstructorAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar soft-delete de instructor con ID: {InstructorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Instructor no encontrado para soft-delete con ID: {InstructorId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete de instructor con ID: {InstructorId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
