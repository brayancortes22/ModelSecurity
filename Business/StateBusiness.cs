using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.EntityFrameworkCore; // Para DbUpdateException
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los estados en el sistema.
    /// </summary>
    public class StateBusiness
    {
        private readonly StateData _stateData;
        private readonly ILogger<StateBusiness> _logger;

        public StateBusiness(StateData stateData, ILogger<StateBusiness> logger)
        {
            _stateData = stateData;
            _logger = logger;
        }

        // Método para obtener todos los estados como DTOs
        public async Task<IEnumerable<StateDto>> GetAllStatesAsync()
        {
            try
            {
                var states = await _stateData.GetAllAsync();
                return MapToDTOList(states);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los estados");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de estados", ex);
            }
        }

        // Método para obtener un estado por ID como DTO
        public async Task<StateDto> GetStateByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un estado con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del estado debe ser mayor que cero");
            }

            try
            {
                var state = await _stateData.GetByIdAsync(id);
                if (state == null)
                {
                    _logger.LogInformation("No se encontró ningún estado con ID: {Id}", id);
                    throw new EntityNotFoundException("State", id);
                }

                return MapToDTO(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el estado con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el estado con ID {id}", ex);
            }
        }

        // Método para crear un estado desde un DTO
        public async Task<StateDto> CreateStateAsync(StateDto stateDto)
        {
            try
            {
                ValidateState(stateDto);
                var state = MapToEntity(stateDto);
                state.CreateDate = DateTime.UtcNow; // Establecer fecha creación
                state.Active = true; // Activo por defecto

                var stateCreado = await _stateData.CreateAsync(state);
                return MapToDTO(stateCreado);
            }
            catch (DbUpdateException dbEx) // Captura posible UNIQUE constraint (TypeState?)
            {
                _logger.LogError(dbEx, "Error de base de datos al crear estado. ¿TypeState duplicado?");
                throw new ExternalServiceException("Base de datos", "Error al crear el estado. Verifique que TypeState no esté duplicado.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al crear nuevo estado: {TypeState}", stateDto?.TypeState ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el estado", ex);
            }
        }

        // Método para actualizar un estado existente (PUT)
        public async Task<StateDto> UpdateStateAsync(int id, StateDto stateDto)
        {
            if (id <= 0 || id != stateDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar un estado con un ID inválido o no coincidente: {StateId}, DTO ID: {DtoId}", id, stateDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateState(stateDto); // Validar datos completos

            try
            {
                var existingState = await _stateData.GetByIdAsync(id);
                if (existingState == null)
                {
                    _logger.LogInformation("No se encontró el estado con ID {StateId} para actualizar", id);
                    throw new EntityNotFoundException("State", id);
                }

                // Mapea los cambios del DTO a la entidad existente
                existingState = MapToEntity(stateDto, existingState);
                existingState.UpdateDate = DateTime.UtcNow; // Actualizar fecha modificación

                await _stateData.UpdateAsync(existingState); // Asume que UpdateAsync existe
                return MapToDTO(existingState);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al actualizar estado {StateId}. ¿TypeState duplicado?", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el estado con ID {id}. Verifique TypeState.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar estado {StateId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el estado con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente un estado (PATCH)
        public async Task<StateDto> PatchStateAsync(int id, StateDto stateDto)
        {
            if (id <= 0 || (stateDto.Id != 0 && id != stateDto.Id))
            {
                _logger.LogWarning("Se intentó aplicar patch a un estado con un ID inválido o no coincidente: {StateId}, DTO ID: {DtoId}", id, stateDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado en la URL es inválido o no coincide con el ID del DTO (si se proporcionó) para PATCH.");
            }

            try
            {
                var existingState = await _stateData.GetByIdAsync(id);
                if (existingState == null)
                {
                    _logger.LogInformation("No se encontró el estado con ID {StateId} para aplicar patch", id);
                    throw new EntityNotFoundException("State", id);
                }

                bool changed = false;

                // Aplicar cambios parciales
                if (stateDto.TypeState != null && existingState.TypeState != stateDto.TypeState)
                {
                    if (string.IsNullOrWhiteSpace(stateDto.TypeState)) throw new Utilities.Exceptions.ValidationException("TypeState", "TypeState no puede ser vacío en PATCH.");
                    existingState.TypeState = stateDto.TypeState; changed = true;
                }
                 if (stateDto.Description != null && existingState.Description != stateDto.Description)
                 {
                     existingState.Description = stateDto.Description; changed = true;
                 }
                 if (existingState.Active != stateDto.Active)
                 {
                     existingState.Active = stateDto.Active;
                     existingState.DeleteDate = stateDto.Active ? (DateTime?)null : DateTime.UtcNow;
                     changed = true;
                 }

                if (changed)
                {
                    existingState.UpdateDate = DateTime.UtcNow;
                    await _stateData.UpdateAsync(existingState);
                    _logger.LogInformation("Aplicado patch al estado con ID {StateId}", id);
                }
                else
                {
                    _logger.LogInformation("No se detectaron cambios en la solicitud PATCH para el estado con ID {StateId}", id);
                }

                return MapToDTO(existingState);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al aplicar patch a estado {StateId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al estado con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a estado {StateId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al estado con ID {id}", ex);
            }
        }

        // Método para eliminar un estado (DELETE persistente)
        // ADVERTENCIA: Puede fallar si hay AprendizProcessInstructors asociados.
        public async Task DeleteStateAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un estado con un ID inválido: {StateId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del estado debe ser mayor a 0");
            }
            try
            {
                var existingState = await _stateData.GetByIdAsync(id);
                if (existingState == null)
                {
                    _logger.LogInformation("No se encontró el estado con ID {StateId} para eliminar (persistente)", id);
                    throw new EntityNotFoundException("State", id);
                }

                // ADVERTENCIA: Fallará si existen dependencias.
                bool deleted = await _stateData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Estado con ID {StateId} eliminado exitosamente (persistente)", id);
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar (persistente) el estado con ID {StateId}.", id);
                    throw new EntityNotFoundException("State", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx) // Captura FK violation
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar el estado {StateId}. Posible FK con AprendizProcessInstructor.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el estado con ID {id}. Verifique dependencias.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al eliminar (persistente) el estado {StateId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el estado con ID {id}", ex);
            }
        }

        // Método para eliminar lógicamente un estado (soft delete)
        public async Task SoftDeleteStateAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar soft-delete a un estado con un ID inválido: {StateId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del estado debe ser mayor a 0");
            }
            try
            {
                var stateToDeactivate = await _stateData.GetByIdAsync(id);
                if (stateToDeactivate == null)
                {
                    _logger.LogInformation("No se encontró el estado con ID {StateId} para desactivar (soft-delete)", id);
                    throw new EntityNotFoundException("State", id);
                }

                if (!stateToDeactivate.Active)
                {
                    _logger.LogInformation("El estado con ID {StateId} ya está inactivo.", id);
                    return;
                }

                stateToDeactivate.Active = false;
                stateToDeactivate.DeleteDate = DateTime.UtcNow;
                stateToDeactivate.UpdateDate = DateTime.UtcNow;
                await _stateData.UpdateAsync(stateToDeactivate);

                _logger.LogInformation("Estado con ID {StateId} marcado como inactivo (soft-delete)", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete del estado {StateId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar el estado con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al realizar soft-delete del estado {StateId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar el estado con ID {id}", ex);
            }
        }

        // Método para validar el DTO (modificado)
        private void ValidateState(StateDto stateDto)
        {
            if (stateDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto State no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(stateDto.TypeState))
            {
                // Corregido el mensaje y el nombre del campo
                _logger.LogWarning("Se intentó crear/actualizar un estado con TypeState vacío");
                throw new Utilities.Exceptions.ValidationException("TypeState", "El TypeState del estado es obligatorio");
            }
            // Podría validarse la longitud de TypeState y Description
        }

        //Funciones de mapeos 
        // Método para mapear de State a StateDto
        private StateDto MapToDTO(State state)
        {
            return new StateDto
            {
                Id = state.Id,
                TypeState = state.TypeState,
                Description = state.Description,
                Active = state.Active // si existe la entidad
            };
        }

        // Método para mapear de StateDto a State (para creación)
        private State MapToEntity(StateDto stateDto)
        {
            return new State
            {
                // Id = stateDto.Id, // No en creación
                TypeState = stateDto.TypeState,
                Description = stateDto.Description,
                Active = stateDto.Active // Usualmente true al crear
            };
        }

        // Método para mapear de DTO a una entidad existente (para actualización PUT)
        private State MapToEntity(StateDto stateDto, State existingState)
        {
            existingState.TypeState = stateDto.TypeState;
            existingState.Description = stateDto.Description;
            existingState.Active = stateDto.Active;

            // Actualizar DeleteDate basado en Active
            if (existingState.Active && existingState.DeleteDate.HasValue)
            {
                existingState.DeleteDate = null;
            }
            else if (!existingState.Active && !existingState.DeleteDate.HasValue)
            {
                existingState.DeleteDate = DateTime.UtcNow;
            }
            return existingState;
        }

        // Método para mapear una lista de State a una lista de StateDto
        private IEnumerable<StateDto> MapToDTOList(IEnumerable<State> states)
        {
            return states.Select(MapToDTO).ToList();
        }
    }
}
