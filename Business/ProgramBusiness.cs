using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class ProgramBusiness
    {
        private readonly ProgramData _programData;
        private readonly ILogger _logger;

        public ProgramBusiness(ProgramData programData, ILogger logger)
        {
            _programData = programData;
            _logger = logger;
        }

        public async Task<IEnumerable<ProgramDTOAuto>> GetAllProgramsAsync()
        {
            try
            {
                var programs = await _programData.GetAllAsync();
                return programs.Select(program => new ProgramDTOAuto
                {
                    Id = program.Id,
                    Name = program.Name,
                    Code = program.Code,
                    Version = program.Version,
                    Duration = program.Duration,
                    Active = program.Active
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los programas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de programas", ex);
            }
        }

        public async Task<ProgramDTOAuto> GetProgramByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un programa con ID inválido: {ProgramId}", id);
                throw new ValidationException("id", "El ID del programa debe ser mayor que cero");
            }

            try
            {
                var program = await _programData.GetByidAsync(id);
                if (program == null)
                {
                    _logger.LogInformation("No se encontró ningún programa con ID: {ProgramId}", id);
                    throw new EntityNotFoundException("Program", id);
                }

                return new ProgramDTOAuto
                {
                    Id = program.Id,
                    Name = program.Name,
                    Code = program.Code,
                    Version = program.Version,
                    Duration = program.Duration,
                    Active = program.Active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el programa con ID: {ProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el programa con ID {id}", ex);
            }
        }

        public async Task<ProgramDTOAuto> CreateProgramAsync(ProgramDTOAuto programDto)
        {
            try
            {
                ValidateProgram(programDto);

                var program = new Program
                {
                    Name = programDto.Name,
                    Code = programDto.Code,
                    Version = programDto.Version,
                    Duration = programDto.Duration,
                    Active = true,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    DeleteDate = DateTime.Now
                };

                var programCreado = await _programData.CreateAsync(program);

                return new ProgramDTOAuto
                {
                    Id = programCreado.Id,
                    Name = programCreado.Name,
                    Code = programCreado.Code,
                    Version = programCreado.Version,
                    Duration = programCreado.Duration,
                    Active = programCreado.Active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo programa");
                throw new ExternalServiceException("Base de datos", "Error al crear el programa", ex);
            }
        }

        private void ValidateProgram(ProgramDTOAuto programDto)
        {
            if (programDto == null)
            {
                throw new ValidationException("El objeto programa no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(programDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa sin nombre");
                throw new ValidationException("Name", "El nombre del programa es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(programDto.Code))
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa sin código");
                throw new ValidationException("Code", "El código del programa es obligatorio");
            }

            if (programDto.Duration <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa con duración inválida");
                throw new ValidationException("Duration", "La duración del programa debe ser mayor que cero");
            }
        }
    }
} 