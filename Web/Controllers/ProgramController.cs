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
    /// Controlador para la gestión de programas en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProgramController : ControllerBase
    {
        private readonly ProgramBusiness _programBusiness;
        private readonly ILogger<ProgramController> _logger;

        /// <summary>
        /// Constructor del controlador de programas
        /// </summary>
        public ProgramController(ProgramBusiness programBusiness, ILogger<ProgramController> logger)
        {
            _programBusiness = programBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProgramDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPrograms()
        {
            try
            {
                var programs = await _programBusiness.GetAllProgramsAsync();
                return Ok(programs);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un programa específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProgramDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProgramById(int id)
        {
            try
            {
                var program = await _programBusiness.GetProgramByIdAsync(id);
                return Ok(program);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el programa con ID: {ProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Programa no encontrado con ID: {ProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programa con ID: {ProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo programa en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProgramDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProgram([FromBody] ProgramDto programDto)
        {
            try
            {
                var createdProgram = await _programBusiness.CreateProgramAsync(programDto);
                return CreatedAtAction(nameof(GetProgramById), new { id = createdProgram.Id }, createdProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear programa");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear programa");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un programa existente (reemplazo completo).
        /// </summary>
        /// <param name="id">ID del programa a actualizar.</param>
        /// <param name="programDto">Datos actualizados del programa.</param>
        /// <response code="200">Retorna el programa actualizado.</response>
        /// <response code="400">Si el ID es inválido, no coincide, o los datos son inválidos.</response>
        /// <response code="404">Si no se encuentra el programa.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProgramDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> UpdateProgram(int id, [FromBody] ProgramDto programDto)
        {
            // Opcional: if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updatedProgram = await _programBusiness.UpdateProgramAsync(id, programDto);
                return Ok(updatedProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar programa {ProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Programa no encontrado para actualizar con ID: {ProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar programa {ProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al actualizar programa {ProgramId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un programa existente.
        /// </summary>
        /// <param name="id">ID del programa a actualizar.</param>
        /// <param name="programDto">Datos parciales a actualizar.</param>
        /// <remarks>NOTA: Se recomienda usar JsonPatch.</remarks>
        /// <response code="200">Retorna el programa con los cambios aplicados.</response>
        /// <response code="400">Si el ID o los datos son inválidos.</response>
        /// <response code="404">Si no se encuentra el programa.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ProgramDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> PatchProgram(int id, [FromBody] ProgramDto programDto) // Usar JsonPatchDocument<ProgramDto> sería ideal
        {
            try
            {
                var patchedProgram = await _programBusiness.PatchProgramAsync(id, programDto);
                return Ok(patchedProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a programa {ProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Programa no encontrado para aplicar patch con ID: {ProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al aplicar patch a programa {ProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al aplicar patch a programa {ProgramId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Elimina permanentemente un programa por su ID.
        /// </summary>
        /// <param name="id">ID del programa a eliminar.</param>
        /// <remarks>ADVERTENCIA: Operación destructiva. Fallará si tiene dependencias (AprendizPrograms, InstructorPrograms).</remarks>
        /// <response code="204">Si la eliminación fue exitosa.</response>
        /// <response code="400">Si el ID es inválido.</response>
        /// <response code="404">Si no se encuentra el programa.</response>
        /// <response code="500">Si ocurre un error interno (p.ej., violación de FK).</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            try
            {
                await _programBusiness.DeleteProgramAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar eliminar programa {ProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Programa no encontrado para eliminar con ID: {ProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Captura errores de BD (FK violation)
            {
                _logger.LogError(ex, "Error de servicio externo al eliminar programa {ProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al eliminar programa {ProgramId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) un programa por su ID.
        /// </summary>
        /// <param name="id">ID del programa a desactivar.</param>
        /// <response code="204">Si la desactivación fue exitosa o ya estaba inactivo.</response>
        /// <response code="400">Si el ID es inválido.</response>
        /// <response code="404">Si no se encuentra el programa.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> SoftDeleteProgram(int id)
        {
            try
            {
                await _programBusiness.SoftDeleteProgramAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar programa {ProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Programa no encontrado para desactivar con ID: {ProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al desactivar programa {ProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al desactivar programa {ProgramId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }
    }
}
