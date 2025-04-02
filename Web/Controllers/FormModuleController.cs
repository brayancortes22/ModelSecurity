using Business;
using Entity.DTOautogestion.pivote;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de relaciones entre formularios y módulos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FormModuleController : ControllerBase
    {
        private readonly FormModuleBusiness _formModuleBusiness;
        private readonly ILogger<FormModuleController> _logger;

        /// <summary>
        /// Constructor del controlador de relaciones formulario-módulo
        /// </summary>
        /// <param name="formModuleBusiness">Capa de negocio de relaciones formulario-módulo</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public FormModuleController(FormModuleBusiness formModuleBusiness, ILogger<FormModuleController> logger)
        {
            _formModuleBusiness = formModuleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las relaciones formulario-módulo del sistema
        /// </summary>
        /// <returns>Lista de relaciones formulario-módulo</returns>
        /// <response code="200">Retorna la lista de relaciones</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormModuleDTOAuto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllFormModules()
        {
            try
            {
                var formModules = await _formModuleBusiness.GetAllFormModulesAsync();
                return Ok(formModules);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener relaciones formulario-módulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una relación formulario-módulo específica por su ID
        /// </summary>
        /// <param name="id">ID de la relación</param>
        /// <returns>Relación formulario-módulo solicitada</returns>
        /// <response code="200">Retorna la relación solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Relación no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormModuleDTOAuto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormModuleById(int id)
        {
            try
            {
                var formModule = await _formModuleBusiness.GetFormModuleByIdAsync(id);
                return Ok(formModule);
            }
            catch (Utilities.Exceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la relación con ID: {FormModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación no encontrada con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener relación con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva relación formulario-módulo en el sistema
        /// </summary>
        /// <param name="formModuleDto">Datos de la relación a crear</param>
        /// <returns>Relación formulario-módulo creada</returns>
        /// <response code="201">Retorna la relación creada</response>
        /// <response code="400">Datos de la relación no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(FormModuleDTOAuto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateFormModule([FromBody] FormModuleDTOAuto formModuleDto)
        {
            try
            {
                var createdFormModule = await _formModuleBusiness.CreateFormModuleAsync(formModuleDto);
                return CreatedAtAction(nameof(GetFormModuleById), new { Id = createdFormModule.Id }, createdFormModule);
            }
            catch (Utilities.Exceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear relación formulario-módulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear relación formulario-módulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
} 