using Business;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
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
    /// Controlador para la gestión de AprendizProgram en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AprendizProgramController : ControllerBase
    {
        private readonly AprendizProgramBusiness _aprendizProgramBusiness;
        private readonly ILogger<AprendizProgramController> _logger;

        /// <summary>
        /// Constructor del controlador de AprendizProgram
        /// </summary>
        public AprendizProgramController(AprendizProgramBusiness aprendizProgramBusiness, ILogger<AprendizProgramController> logger)
        {
            _aprendizProgramBusiness = aprendizProgramBusiness ?? throw new ArgumentNullException(nameof(aprendizProgramBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los programas de aprendiz en el sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AprendizProgramDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AprendizProgramDto>>> GetAprendizPrograms()
        {
            try
            {
                var aprendizPrograms = await _aprendizProgramBusiness.GetAllAprendizProgramsAsync();
                _logger.LogInformation("Se obtuvieron {Count} programas de aprendizaje.", aprendizPrograms.Count());
                return Ok(aprendizPrograms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al obtener todos los programas de aprendizaje.");
                return StatusCode(500, new { message = $"Error al obtener los programas de aprendizaje: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener todos los programas de aprendizaje.");
                return StatusCode(500, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Obtiene un programa de aprendiz específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AprendizProgramDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AprendizProgramDto>> GetAprendizProgram(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener un programa de aprendizaje con ID inválido: {Id}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }

            try
            {
                var aprendizProgram = await _aprendizProgramBusiness.GetAprendizProgramByIdAsync(id);
                _logger.LogInformation("Programa de aprendizaje con ID {Id} obtenido exitosamente.", id);
                return Ok(aprendizProgram);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("No se encontró el programa de aprendizaje con ID {Id}.", id);
                return NotFound(new { message = $"No se encontró el programa de aprendizaje con ID {id}." });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al obtener el programa de aprendizaje con ID {Id}.", id);
                return StatusCode(500, new { message = $"Error al obtener el programa de aprendizaje: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener el programa de aprendizaje con ID {Id}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Crea un nuevo programa de aprendiz en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AprendizProgramDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AprendizProgramDto>> PostAprendizProgram(AprendizProgramDto aprendizProgramDto)
        {
            if (aprendizProgramDto == null)
            {
                _logger.LogWarning("Intento de crear un programa de aprendizaje con datos nulos.");
                return BadRequest(new { message = "El programa de aprendizaje no puede ser nulo." });
            }

            try
            {
                var createdAprendizProgram = await _aprendizProgramBusiness.CreateAprendizProgramAsync(aprendizProgramDto);
                _logger.LogInformation("Programa de aprendizaje creado exitosamente con ID {Id}.", createdAprendizProgram.Id);
                return CreatedAtAction(nameof(GetAprendizProgram), new { id = createdAprendizProgram.Id }, createdAprendizProgram);
            }
            catch (Utilities.Exceptions.ValidationException vex)
            {
                _logger.LogWarning(vex, "Error de validación al crear el programa de aprendizaje.");
                return BadRequest(new { message = vex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al crear el programa de aprendizaje.");
                return StatusCode(500, new { message = $"Error al crear el programa de aprendizaje: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear el programa de aprendizaje.");
                return StatusCode(500, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        // PUT: api/AprendizProgram/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAprendizProgram(int id, AprendizProgramDto aprendizProgramDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de actualizar un programa de aprendizaje con ID inválido: {Id}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            if (id != aprendizProgramDto.Id)
            {
                _logger.LogWarning("Intento de actualizar un programa de aprendizaje donde el ID de la ruta {RouteId} no coincide con el ID del cuerpo {BodyId}", id, aprendizProgramDto.Id);
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del programa de aprendizaje proporcionado." });
            }

            try
            {
                await _aprendizProgramBusiness.UpdateAprendizProgramAsync(id, aprendizProgramDto);
                _logger.LogInformation("Programa de aprendizaje con ID {Id} actualizado exitosamente.", id);
                return Ok(new { message = $"Programa de aprendizaje con ID {id} actualizado exitosamente." });
            }
            catch (Utilities.Exceptions.ValidationException vex)
            {
                _logger.LogWarning(vex, "Error de validación al actualizar el programa de aprendizaje con ID {Id}", id);
                return BadRequest(new { message = vex.Message });
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("No se encontró el programa de aprendizaje con ID {Id} para actualizar.", id);
                return NotFound(new { message = $"No se encontró el programa de aprendizaje con ID {id}." });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar el programa de aprendizaje con ID {Id}", id);
                return StatusCode(500, new { message = $"Error al actualizar el programa de aprendizaje: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar el programa de aprendizaje con ID {Id}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        // PATCH: api/AprendizProgram/5
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchAprendizProgram(int id, [FromBody] AprendizProgramDto aprendizProgramDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de aplicar patch a un programa de aprendizaje con ID inválido: {Id}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            if (aprendizProgramDto == null)
            {
                _logger.LogWarning("Intento de aplicar patch a un programa de aprendizaje con ID {Id} con un DTO nulo.", id);
                return BadRequest(new { message = "El cuerpo de la solicitud (programa de aprendizaje) no puede ser nulo." });
            }

            if (aprendizProgramDto.Id != 0 && id != aprendizProgramDto.Id)
            {
                _logger.LogWarning("ID de ruta {RouteId} no coincide con ID de cuerpo {BodyId} en PATCH", id, aprendizProgramDto.Id);
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo." });
            }

            try
            {
                await _aprendizProgramBusiness.PatchAprendizProgramAsync(id, aprendizProgramDto);
                _logger.LogInformation("Patch aplicado exitosamente al programa de aprendizaje con ID {Id}.", id);
                return Ok(new { message = $"Patch aplicado exitosamente al programa de aprendizaje con ID {id}." });
            }
            catch (Utilities.Exceptions.ValidationException vex)
            {
                _logger.LogWarning(vex, "Error de validación al aplicar patch al programa de aprendizaje con ID {Id}", id);
                return BadRequest(new { message = vex.Message });
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("No se encontró el programa de aprendizaje con ID {Id} para aplicar patch.", id);
                return NotFound(new { message = $"No se encontró el programa de aprendizaje con ID {id}." });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al aplicar patch al programa de aprendizaje con ID {Id}", id);
                return StatusCode(500, new { message = $"Error al aplicar patch al programa de aprendizaje: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al aplicar patch al programa de aprendizaje con ID {Id}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        // DELETE: api/AprendizProgram/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAprendizProgram(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de eliminar un programa de aprendizaje con ID inválido: {Id}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }

            try
            {
                await _aprendizProgramBusiness.DeleteAprendizProgramAsync(id);
                _logger.LogInformation("Programa de aprendizaje con ID {Id} eliminado exitosamente.", id);
                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("No se encontró el programa de aprendizaje con ID {Id} para eliminar.", id);
                return NotFound(new { message = $"No se encontró el programa de aprendizaje con ID {id}." });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al eliminar el programa de aprendizaje con ID {Id}", id);
                return StatusCode(500, new { message = $"Error al eliminar el programa de aprendizaje: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar el programa de aprendizaje con ID {Id}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        // SOFT DELETE: api/AprendizProgram/5/soft
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SoftDeleteAprendizProgram(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de realizar borrado lógico a un programa de aprendizaje con ID inválido: {Id}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }

            try
            {
                await _aprendizProgramBusiness.SoftDeleteAprendizProgramAsync(id);
                _logger.LogInformation("Borrado lógico realizado exitosamente para el programa de aprendizaje con ID {Id}.", id);
                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("No se encontró el programa de aprendizaje con ID {Id} para realizar borrado lógico.", id);
                return NotFound(new { message = $"No se encontró el programa de aprendizaje con ID {id}." });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al realizar borrado lógico del programa de aprendizaje con ID {Id}", id);
                return StatusCode(500, new { message = $"Error al realizar el borrado lógico del programa de aprendizaje: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al realizar borrado lógico del programa de aprendizaje con ID {Id}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }
    }
}
