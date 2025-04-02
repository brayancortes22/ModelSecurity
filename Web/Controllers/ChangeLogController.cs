using Business;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de registros de cambios en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ChangeLogController : ControllerBase
    {
        private readonly ChangeLogBusiness _changeLogBusiness;
        private readonly ILogger<ChangeLogController> _logger;

        /// <summary>
        /// Constructor del controlador de registros de cambios
        /// </summary>
        /// <param name="changeLogBusiness">Capa de negocio de registros de cambios</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public ChangeLogController(ChangeLogBusiness changeLogBusiness, ILogger<ChangeLogController> logger)
        {
            _changeLogBusiness = changeLogBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de cambios del sistema
        /// </summary>
        /// <returns>Lista de registros de cambios</returns>
        /// <response code="200">Retorna la lista de registros</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChangeLogDTOAuto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllChangeLogs()
        {
            try
            {
                var changeLogs = await _changeLogBusiness.GetAllChangeLogsAsync();
                return Ok(changeLogs);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registros de cambios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un registro de cambio específico por su ID
        /// </summary>
        /// <param name="id">ID del registro</param>
        /// <returns>Registro de cambio solicitado</returns>
        /// <response code="200">Retorna el registro solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Registro no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ChangeLogDTOAuto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetChangeLogById(int id)
        {
            try
            {
                var changeLog = await _changeLogBusiness.GetChangeLogByIdAsync(id);
                return Ok(changeLog);
            }
            catch (Utilities.Exceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el registro con ID: {ChangeLogId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro no encontrado con ID: {ChangeLogId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registro con ID: {ChangeLogId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo registro de cambio en el sistema
        /// </summary>
        /// <param name="changeLogDto">Datos del registro a crear</param>
        /// <returns>Registro de cambio creado</returns>
        /// <response code="201">Retorna el registro creado</response>
        /// <response code="400">Datos del registro no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ChangeLogDTOAuto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateChangeLog([FromBody] ChangeLogDTOAuto changeLogDto)
        {
            try
            {
                var createdChangeLog = await _changeLogBusiness.CreateChangeLogAsync(changeLogDto);
                return CreatedAtAction(nameof(GetChangeLogById), new { Id = createdChangeLog.Id }, createdChangeLog);
            }
            catch (Utilities.Exceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear registro de cambio");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear registro de cambio");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
} 