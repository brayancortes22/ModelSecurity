using Data;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los programas de aprendizaje en el sistema.
    /// </summary>
    public class AprendizProgramBusiness
    {
        private readonly AprendizProgramData _aprendizProgramData;
        private readonly ILogger<AprendizProgramBusiness> _logger;

        public AprendizProgramBusiness(AprendizProgramData aprendizProgramData, ILogger<AprendizProgramBusiness> logger)
        {
            _aprendizProgramData = aprendizProgramData;
            _logger = logger;
        }

        // Método para obtener todos los programas de aprendizaje como DTOs
        public async Task<IEnumerable<AprendizProgramDto>> GetAllAprendizProgramsAsync()
        {
            try
            {
                var programs = await _aprendizProgramData.GetAllAsync();
                return MapToDTOList(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los programas de aprendizaje");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de programas de aprendizaje", ex);
            }
        }

        // Método para obtener un programa de aprendizaje por ID como DTO
        public async Task<AprendizProgramDto> GetAprendizProgramByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un programa de aprendizaje con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa de aprendizaje debe ser mayor que cero");
            }

            try
            {
                var program = await _aprendizProgramData.GetByIdAsync(id);
                if (program == null)
                {
                    _logger.LogInformation("No se encontró ningún programa de aprendizaje con ID: {Id}", id);
                    throw new EntityNotFoundException("aprendizProgram", id);
                }

                return MapToDTO(program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el programa de aprendizaje con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el programa de aprendizaje con ID {id}", ex);
            }
        }

        // Método para crear un programa de aprendizaje desde un DTO
        public async Task<AprendizProgramDto> CreateAprendizProgramAsync(AprendizProgramDto aprendizProgramDto)
        {
            try
            {
                ValidateAprendizProgram(aprendizProgramDto);
                var program = MapToEntity(aprendizProgramDto);
                var programCreado = await _aprendizProgramData.CreateAsync(program);
                return MapToDTO(programCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo programa de aprendizaje");
                throw new ExternalServiceException("Base de datos", "Error al crear el programa de aprendizaje", ex);
            }
        }

        // Método para actualizar un programa de aprendizaje existente (PUT)
        public async Task<AprendizProgramDto> UpdateAprendizProgramAsync(int id, AprendizProgramDto dto)
        {
            if (id <= 0 || id != dto.Id)
            {
                _logger.LogWarning("Se intentó actualizar un programa de aprendizaje con ID inválido o no coincidente: {RelationId}, DTO ID: {DtoId}", id, dto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateAprendizProgram(dto); // Reutilizamos la validación

            try
            {
                var existingProgram = await _aprendizProgramData.GetByIdAsync(id);
                if (existingProgram == null)
                {
                    _logger.LogInformation("No se encontró el programa de aprendizaje con ID {RelationId} para actualizar", id);
                    throw new EntityNotFoundException("AprendizProgram", id);
                }

                existingProgram = MapToEntity(dto, existingProgram); // Usamos el nuevo overload de MapToEntity

                await _aprendizProgramData.UpdateAsync(existingProgram);
                return MapToDTO(existingProgram);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar el programa de aprendizaje con ID {RelationId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar el programa de aprendizaje con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar el programa de aprendizaje con ID {RelationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el programa de aprendizaje con ID {id}", ex);
            }
        }

         // Método para actualizar parcialmente un programa de aprendizaje (PATCH)
        public async Task<AprendizProgramDto> PatchAprendizProgramAsync(int id, AprendizProgramDto dto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a un programa de aprendizaje con ID inválido: {RelationId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa de aprendizaje debe ser mayor que cero.");
            }

            try
            {
                var existingProgram = await _aprendizProgramData.GetByIdAsync(id);
                if (existingProgram == null)
                {
                    _logger.LogInformation("No se encontró el programa de aprendizaje con ID {RelationId} para aplicar patch", id);
                    throw new EntityNotFoundException("AprendizProgram", id);
                }

                bool updated = false;

                // Actualizar campos si se proporcionan en el DTO y son diferentes
                 if (dto.AprendizId > 0 && dto.AprendizId != existingProgram.AprendizId)
                {
                    existingProgram.AprendizId = dto.AprendizId;
                    updated = true;
                }
                if (dto.ProgramId > 0 && dto.ProgramId != existingProgram.ProgramId)
                {
                    existingProgram.ProgramId = dto.ProgramId;
                    updated = true;
                }

                if (updated)
                {
                    await _aprendizProgramData.UpdateAsync(existingProgram);
                    _logger.LogInformation("Patch aplicado al programa de aprendizaje con ID: {RelationId}", id);
                }
                else
                {
                    _logger.LogInformation("No se realizaron cambios en el programa de aprendizaje con ID {RelationId} durante el patch.", id);
                }

                return MapToDTO(existingProgram);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch al programa de aprendizaje con ID {RelationId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el programa de aprendizaje con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch al programa de aprendizaje con ID {RelationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el programa de aprendizaje con ID {id}", ex);
            }
        }

        // Método para eliminar un programa de aprendizaje (DELETE persistente)
        public async Task DeleteAprendizProgramAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar un programa de aprendizaje con ID inválido: {RelationId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del programa de aprendizaje debe ser mayor a 0");
            }
            try
            {
                var existingProgram = await _aprendizProgramData.GetByIdAsync(id);
                if (existingProgram == null)
                {
                     _logger.LogInformation("No se encontró el programa de aprendizaje con ID {RelationId} para eliminar", id);
                    throw new EntityNotFoundException("AprendizProgram", id);
                }

                bool deleted = await _aprendizProgramData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Programa de aprendizaje con ID {RelationId} eliminado exitosamente", id);
                }
                else
                {
                     _logger.LogWarning("No se pudo eliminar el programa de aprendizaje con ID {RelationId}. Posiblemente no encontrada por la capa de datos.", id);
                    throw new EntityNotFoundException("AprendizProgram", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx) // Capturar error si hay FKs
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar el programa de aprendizaje con ID {RelationId}. Posible violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el programa de aprendizaje con ID {id}. Verifique dependencias.", dbEx);
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar el programa de aprendizaje con ID {RelationId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar el programa de aprendizaje con ID {id}", ex);
            }
        }

        // Método para realizar un borrado lógico (SOFT DELETE)
        public async Task SoftDeleteAprendizProgramAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar un borrado lógico a un programa de aprendizaje con ID inválido: {RelationId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa de aprendizaje debe ser mayor a 0.");
            }
            try
            {
                var existingProgram = await _aprendizProgramData.GetByIdAsync(id);
                if (existingProgram == null)
                {
                    _logger.LogInformation("No se encontró el programa de aprendizaje con ID {RelationId} para realizar borrado lógico", id);
                    throw new EntityNotFoundException("AprendizProgram", id);
                }

                bool deleted = await _aprendizProgramData.SoftDeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Borrado lógico del programa de aprendizaje con ID {RelationId} realizado exitosamente.", id);
                }
                else
                {
                    _logger.LogWarning("No se pudo realizar el borrado lógico del programa de aprendizaje con ID {RelationId}. Posiblemente no encontrado por la capa de datos.", id);
                    throw new EntityNotFoundException("AprendizProgram", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar borrado lógico del programa de aprendizaje con ID {RelationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar el borrado lógico del programa de aprendizaje con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error general al realizar el borrado lógico del programa de aprendizaje con ID {RelationId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al realizar el borrado lógico del programa de aprendizaje con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateAprendizProgram(AprendizProgramDto aprendizProgramDto)
        {
            if (aprendizProgramDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto AprendizProgram no puede ser nulo");
            }
        }

        //Funciones de mapeos
        // Método para mapear de AprendizProgram a AprendizProgramDto
        private AprendizProgramDto MapToDTO(AprendizProgram program)
        {
            return new AprendizProgramDto
            {
                Id = program.Id,
                AprendizId = program.AprendizId,
                ProgramId = program.ProgramId,
                Active = program.Active
            };
        }

        // Método para mapear de AprendizProgramDto a AprendizProgram (para creación)
        private AprendizProgram MapToEntity(AprendizProgramDto programDto)
        {
            return new AprendizProgram
            {
                Id = programDto.Id,
                AprendizId = programDto.AprendizId,
                ProgramId = programDto.ProgramId,
                Active = programDto.Active
            };
        }

         // Método para mapear de DTO a una entidad existente (para actualización)
        private AprendizProgram MapToEntity(AprendizProgramDto dto, AprendizProgram existingProgram)
        {
            existingProgram.AprendizId = dto.AprendizId;
            existingProgram.ProgramId = dto.ProgramId;
            existingProgram.Active = dto.Active;
            // No actualizamos el Id
            return existingProgram;
        }

        // Método para mapear una lista de AprendizProgram a una lista de AprendizProgramDto
        private IEnumerable<AprendizProgramDto> MapToDTOList(IEnumerable<AprendizProgram> programs)
        {
            var programsDTO = new List<AprendizProgramDto>();
            foreach (var program in programs)
            {
                programsDTO.Add(MapToDTO(program));
            }
            return programsDTO;
        }
    }
}
