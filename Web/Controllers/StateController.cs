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
    /// Controlador para la gestión de estados en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StateController : ControllerBase
    {
        private readonly StateBusiness _stateBusiness;
        private readonly ILogger<StateController> _logger;

        /// <summary>
        /// Constructor del controlador de estados
        /// </summary>
        public StateController(StateBusiness stateBusiness, ILogger<StateController> logger)
        {
            _stateBusiness = stateBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los estados del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StateDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllStates()
        {
            try
            {
                var states = await _stateBusiness.GetAllStatesAsync();
                return Ok(states);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener estados");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un estado específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StateDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetStateById(int id)
        {
            try
            {
                var state = await _stateBusiness.GetStateByIdAsync(id);
                return Ok(state);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el estado con ID: {StateId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Estado no encontrado con ID: {StateId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener estado con ID: {StateId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo estado en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(StateDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateState([FromBody] StateDto stateDto)
        {
            try
            {
                var createdState = await _stateBusiness.CreateStateAsync(stateDto);
                return CreatedAtAction(nameof(GetStateById), new { id = createdState.Id }, createdState);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear estado");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear estado");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un estado existente (reemplazo completo)
        /// </summary>
        /// <param name="id">ID del estado a actualizar</param>
        /// <param name="stateDto">Datos del estado para actualizar</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(StateDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateState(int id, [FromBody] StateDto stateDto)
        {
            try
            {
                var updatedState = await _stateBusiness.UpdateStateAsync(id, stateDto);
                return Ok(updatedState);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar estado con ID: {StateId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Estado no encontrado para actualizar con ID: {StateId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar estado con ID: {StateId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un estado existente
        /// </summary>
        /// <remarks>
        /// Idealmente, esto usaría JsonPatch para aplicar cambios parciales.
        /// La implementación actual en `StateBusiness` actualiza solo los campos proporcionados en el DTO.
        /// </remarks>
        /// <param name="id">ID del estado a actualizar parcialmente</param>
        /// <param name="stateDto">Datos parciales del estado para aplicar</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(StateDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchState(int id, [FromBody] StateDto stateDto)
        {
            try
            {
                var patchedState = await _stateBusiness.PatchStateAsync(id, stateDto);
                return Ok(patchedState);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a estado con ID: {StateId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Estado no encontrado para aplicar patch con ID: {StateId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a estado con ID: {StateId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina permanentemente un estado
        /// </summary>
        /// <remarks>Precaución: Esta operación es irreversible y fallará si existen entidades dependientes.</remarks>
        /// <param name="id">ID del estado a eliminar</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteState(int id)
        {
            try
            {
                await _stateBusiness.DeleteStateAsync(id);
                return NoContent(); // Éxito, sin contenido
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar estado con ID: {StateId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Estado no encontrado para eliminar con ID: {StateId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                // Captura errores de FK
                _logger.LogError(ex, "Error al eliminar estado con ID: {StateId}. Posible dependencia.", id);
                return StatusCode(500, new { message = "Error al eliminar el estado. Verifique si hay dependencias." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) un estado
        /// </summary>
        /// <param name="id">ID del estado a desactivar</param>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteState(int id)
        {
            try
            {
                await _stateBusiness.SoftDeleteStateAsync(id);
                return NoContent(); // Éxito, sin contenido
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar soft-delete de estado con ID: {StateId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Estado no encontrado para soft-delete con ID: {StateId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete de estado con ID: {StateId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
