using Business;
using Entity.DTOautogestion;
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
    /// Controlador para la gestión de Enterprise en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EnterpriseController : ControllerBase
    {
        private readonly EnterpriseBusiness _enterpriseBusiness;
        private readonly ILogger<EnterpriseController> _logger;

        /// <summary>
        /// Constructor del controlador de Enterprise
        /// </summary>
        public EnterpriseController(EnterpriseBusiness enterpriseBusiness, ILogger<EnterpriseController> logger)
        {
            _enterpriseBusiness = enterpriseBusiness ?? throw new ArgumentNullException(nameof(enterpriseBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todas las empresas del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EnterpriseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllEnterprises()
        {
            try
            {
                var enterprises = await _enterpriseBusiness.GetAllEnterprisesAsync();
                 _logger.LogInformation("Se obtuvieron {Count} empresas.", enterprises.Count());
                return Ok(enterprises);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al obtener todas las empresas.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al obtener todas las empresas.");
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Obtiene una empresa específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnterpriseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEnterpriseById(int id)
        {
             if (id <= 0)
             {
                 _logger.LogWarning("Intento de obtener una empresa con ID inválido: {EnterpriseId}", id);
                 return BadRequest(new { message = "El ID proporcionado es inválido." });
             }
            try
            {
                var enterprise = await _enterpriseBusiness.GetEnterpriseByIdAsync(id);
                 _logger.LogInformation("Empresa con ID {EnterpriseId} obtenida exitosamente.", id);
                return Ok(enterprise);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la empresa con ID: {EnterpriseId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Empresa no encontrada con ID: {EnterpriseId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al obtener empresa con ID: {EnterpriseId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al obtener empresa con ID: {EnterpriseId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Crea una nueva empresa en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(EnterpriseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEnterprise([FromBody] EnterpriseDto enterpriseDto)
        {
            if (enterpriseDto == null)
            {
                 _logger.LogWarning("Intento de crear una empresa con datos nulos.");
                 return BadRequest(new { message = "La empresa no puede ser nula." });
            }
            try
            {
                var createdEnterprise = await _enterpriseBusiness.CreateEnterpriseAsync(enterpriseDto);
                 _logger.LogInformation("Empresa creada exitosamente con ID {EnterpriseId}.", createdEnterprise.Id);
                return CreatedAtAction(nameof(GetEnterpriseById), new { id = createdEnterprise.Id }, createdEnterprise);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear empresa");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al crear empresa");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al crear empresa");
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Actualiza una empresa existente en el sistema.
        /// </summary>
        /// <param name="id">ID de la empresa a actualizar.</param>
        /// <param name="enterpriseDto">Datos actualizados de la empresa.</param>
        /// <returns>Respuesta indicando éxito o fracaso.</returns>
        /// <response code="200">Empresa actualizada exitosamente.</response>
        /// <response code="400">ID inválido o datos de la empresa no válidos.</response>
        /// <response code="404">Empresa no encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEnterprise(int id, [FromBody] EnterpriseDto enterpriseDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de actualizar una empresa con ID inválido: {EnterpriseId}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            if (enterpriseDto == null || id != enterpriseDto.Id)
            {
                _logger.LogWarning("Datos inválidos para actualizar la empresa con ID: {EnterpriseId}. DTO: {@EnterpriseDto}", id, enterpriseDto);
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID de la empresa proporcionada o el cuerpo es nulo." });
            }

            try
            {
                await _enterpriseBusiness.UpdateEnterpriseAsync(id, enterpriseDto);
                _logger.LogInformation("Empresa con ID {EnterpriseId} actualizada exitosamente.", id);
                return Ok(new { message = $"Empresa con ID {id} actualizada exitosamente." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar empresa con ID: {EnterpriseId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Empresa no encontrada para actualizar con ID: {EnterpriseId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar empresa con ID: {EnterpriseId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar empresa con ID: {EnterpriseId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Actualiza parcialmente una empresa existente en el sistema.
        /// </summary>
        /// <param name="id">ID de la empresa a actualizar parcialmente.</param>
        /// <param name="enterpriseDto">Datos a actualizar de la empresa.</param>
        /// <returns>Respuesta indicando éxito o fracaso.</returns>
        /// <response code="200">Empresa actualizada parcialmente.</response>
        /// <response code="400">ID inválido o datos no válidos.</response>
        /// <response code="404">Empresa no encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchEnterprise(int id, [FromBody] EnterpriseDto enterpriseDto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Intento de aplicar patch a una empresa con ID inválido: {EnterpriseId}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            if (enterpriseDto == null)
            {
                _logger.LogWarning("Intento de aplicar patch a una empresa con ID {EnterpriseId} con un DTO nulo.", id);
                return BadRequest(new { message = "El cuerpo de la solicitud (empresa) no puede ser nulo." });
            }
             if (enterpriseDto.Id != 0 && id != enterpriseDto.Id)
            {
                 _logger.LogWarning("ID de ruta {RouteId} no coincide con ID de cuerpo {BodyId} en PATCH para Empresa", id, enterpriseDto.Id);
                 return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo." });
            }

            try
            {
                await _enterpriseBusiness.PatchEnterpriseAsync(id, enterpriseDto);
                 _logger.LogInformation("Patch aplicado exitosamente a la empresa con ID {EnterpriseId}.", id);
                return Ok(new { message = $"Patch aplicado exitosamente a la empresa con ID {id}." });
            }
             catch (ValidationException ex)
             {
                 _logger.LogWarning(ex, "Validación fallida al aplicar patch a la empresa con ID: {EnterpriseId}", id);
                 return BadRequest(new { message = ex.Message });
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Empresa no encontrada para aplicar patch con ID: {EnterpriseId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error externo al aplicar patch a la empresa con ID: {EnterpriseId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
             }
             catch (Exception ex)
             {
                _logger.LogError(ex, "Error inesperado al aplicar patch a la empresa con ID: {EnterpriseId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
             }
        }

        /// <summary>
        /// Elimina una empresa del sistema (eliminación persistente).
        /// </summary>
        /// <param name="id">ID de la empresa a eliminar.</param>
        /// <returns>Respuesta indicando éxito.</returns>
        /// <response code="204">Empresa eliminada exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Empresa no encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEnterprise(int id)
        {
             if (id <= 0)
             {
                 _logger.LogWarning("Intento de eliminar una empresa con ID inválido: {EnterpriseId}", id);
                 return BadRequest(new { message = "El ID proporcionado es inválido." });
             }

             try
             {
                 await _enterpriseBusiness.DeleteEnterpriseAsync(id);
                  _logger.LogInformation("Empresa con ID {EnterpriseId} eliminada exitosamente.", id);
                 return NoContent();
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Empresa no encontrada para eliminar con ID: {EnterpriseId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error externo al eliminar empresa con ID: {EnterpriseId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error al eliminar la empresa: {ex.Message}" });
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "Error inesperado al eliminar empresa con ID: {EnterpriseId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
             }
        }

         /// <summary>
        /// Desactiva una empresa en el sistema (eliminación lógica).
        /// </summary>
        /// <param name="id">ID de la empresa a desactivar.</param>
        /// <returns>Respuesta indicando éxito.</returns>
        /// <response code="204">Empresa desactivada exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Empresa no encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SoftDeleteEnterprise(int id)
        {
             if (id <= 0)
             {
                 _logger.LogWarning("Intento de realizar borrado lógico a una empresa con ID inválido: {EnterpriseId}", id);
                 return BadRequest(new { message = "El ID proporcionado es inválido." });
             }

             try
             {
                 await _enterpriseBusiness.SoftDeleteEnterpriseAsync(id);
                 _logger.LogInformation("Borrado lógico realizado exitosamente para la empresa con ID {EnterpriseId}.", id);
                 return NoContent();
             }
             catch (ValidationException ex)
             {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar empresa con ID: {EnterpriseId}", id);
                 return BadRequest(new { message = ex.Message });
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Empresa no encontrada para desactivar con ID: {EnterpriseId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error externo al desactivar empresa con ID: {EnterpriseId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "Error inesperado al desactivar empresa con ID: {EnterpriseId}", id);
                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
             }
        }
    }
}