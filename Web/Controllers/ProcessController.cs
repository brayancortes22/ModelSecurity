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
    /// Controlador para la gestión de procesos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProcessController : ControllerBase
    {
        private readonly ProcessBusiness _processBusiness;
        private readonly ILogger<ProcessController> _logger;

        /// <summary>
        /// Constructor del controlador de procesos
        /// </summary>
        public ProcessController(ProcessBusiness processBusiness, ILogger<ProcessController> logger)
        {
            _processBusiness = processBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los procesos del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProcessDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllProcesses()
        {
            try
            {
                var processes = await _processBusiness.GetAllProcessesAsync();
                return Ok(processes);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener procesos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un proceso específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProcessDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProcessById(int id)
        {
            try
            {
                var process = await _processBusiness.GetProcessByIdAsync(id);
                return Ok(process);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el proceso con ID: {ProcessId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso no encontrado con ID: {ProcessId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener proceso con ID: {ProcessId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo proceso en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProcessDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProcess([FromBody] ProcessDto processDto)
        {
            try
            {
                var createdProcess = await _processBusiness.CreateProcessAsync(processDto);
                return CreatedAtAction(nameof(GetProcessById), new { id = createdProcess.Id }, createdProcess);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear proceso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear proceso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un proceso existente (reemplazo completo)
        /// </summary>
        /// <param name="id">ID del proceso a actualizar</param>
        /// <param name="processDto">Datos completos del proceso para actualizar</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProcessDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProcess(int id, [FromBody] ProcessDto processDto)
        {
            try
            {
                var updatedProcess = await _processBusiness.UpdateProcessAsync(id, processDto);
                return Ok(updatedProcess);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar proceso con ID: {ProcessId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso no encontrado para actualizar con ID: {ProcessId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar proceso con ID: {ProcessId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un proceso existente
        /// </summary>
        /// <param name="id">ID del proceso a actualizar</param>
        /// <param name="processDto">Datos parciales a aplicar (TypeProcess, Observation)</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ProcessDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchProcess(int id, [FromBody] ProcessDto processDto)
        {
            try
            {
                var patchedProcess = await _processBusiness.PatchProcessAsync(id, processDto);
                return Ok(patchedProcess);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a proceso con ID: {ProcessId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso no encontrado para aplicar patch con ID: {ProcessId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a proceso con ID: {ProcessId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina permanentemente un proceso
        /// </summary>
        /// <remarks>Precaución: Esta operación es irreversible y fallará si existen entidades dependientes.</remarks>
        /// <param name="id">ID del proceso a eliminar</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)] // O 409 Conflict
        public async Task<IActionResult> DeleteProcess(int id)
        {
            try
            {
                await _processBusiness.DeleteProcessAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar proceso con ID: {ProcessId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso no encontrado para eliminar con ID: {ProcessId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Puede ser error de FK
            {
                _logger.LogError(ex, "Error al eliminar proceso con ID: {ProcessId}. Posible dependencia.", id);
                return StatusCode(500, new { message = "Error al eliminar el proceso. Verifique si hay dependencias." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) un proceso
        /// </summary>
        /// <param name="id">ID del proceso a desactivar</param>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteProcess(int id)
        {
            try
            {
                await _processBusiness.SoftDeleteProcessAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar soft-delete de proceso con ID: {ProcessId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso no encontrado para soft-delete con ID: {ProcessId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete de proceso con ID: {ProcessId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
