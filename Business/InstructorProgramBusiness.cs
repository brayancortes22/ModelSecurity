using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class InstructorProgramBusiness
    {
        private readonly InstructorProgramData _instructorProgramData;
        private readonly InstructorData _instructorData;
        private readonly ProgramData _programData;
        private readonly ILogger _logger;

        public InstructorProgramBusiness(
            InstructorProgramData instructorProgramData,
            InstructorData instructorData,
            ProgramData programData,
            ILogger logger)
        {
            _instructorProgramData = instructorProgramData;
            _instructorData = instructorData;
            _programData = programData;
            _logger = logger;
        }

        public async Task<IEnumerable<InstructorProgramDTOAuto>> GetAllInstructorProgramsAsync()
        {
            try
            {
                var instructorPrograms = await _instructorProgramData.GetAllAsync();
                var instructorProgramsDTO = new List<InstructorProgramDTOAuto>();

                foreach (var instructorProgram in instructorPrograms)
                {
                    var instructor = await _instructorData.GetByidAsync(instructorProgram.InstructorId);
                    var program = await _programData.GetByidAsync(instructorProgram.ProgramId);

                    var instructorProgramDto = new InstructorProgramDTOAuto
                    {
                        Id = instructorProgram.Id,
                        InstructorId = instructorProgram.InstructorId,
                        ProgramId = instructorProgram.ProgramId,
                        Active = instructorProgram.Active,
                        Instructor = instructor != null ? new InstructorDTOAuto
                        {
                            Id = instructor.Id,
                            PersonId = instructor.PersonId,
                            Active = instructor.Active
                        } : null,
                        Program = program != null ? new ProgramDTOAuto
                        {
                            Id = program.Id,
                            Name = program.Name,
                            Code = program.Code,
                            Version = program.Version,
                            Duration = program.Duration,
                            Active = program.Active
                        } : null
                    };

                    instructorProgramsDTO.Add(instructorProgramDto);
                }

                return instructorProgramsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones instructor-programa");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de relaciones instructor-programa", ex);
            }
        }

        public async Task<InstructorProgramDTOAuto> GetInstructorProgramByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una relación instructor-programa con ID inválido: {Id}", id);
                throw new ValidationException("id", "El ID de la relación instructor-programa debe ser mayor que cero");
            }

            try
            {
                var instructorProgram = await _instructorProgramData.GetByidAsync(id);
                if (instructorProgram == null)
                {
                    _logger.LogInformation("No se encontró ninguna relación instructor-programa con ID: {Id}", id);
                    throw new EntityNotFoundException("InstructorProgram", id);
                }

                var instructor = await _instructorData.GetByidAsync(instructorProgram.InstructorId);
                var program = await _programData.GetByidAsync(instructorProgram.ProgramId);

                return new InstructorProgramDTOAuto
                {
                    Id = instructorProgram.Id,
                    InstructorId = instructorProgram.InstructorId,
                    ProgramId = instructorProgram.ProgramId,
                    Active = instructorProgram.Active,
                    Instructor = instructor != null ? new InstructorDTOAuto
                    {
                        Id = instructor.Id,
                        PersonId = instructor.PersonId,
                        Active = instructor.Active
                    } : null,
                    Program = program != null ? new ProgramDTOAuto
                    {
                        Id = program.Id,
                        Name = program.Name,
                        Code = program.Code,
                        Version = program.Version,
                        Duration = program.Duration,
                        Active = program.Active
                    } : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación instructor-programa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la relación instructor-programa con ID {id}", ex);
            }
        }

        public async Task<InstructorProgramDTOAuto> CreateInstructorProgramAsync(InstructorProgramDTOAuto instructorProgramDto)
        {
            try
            {
                await ValidateInstructorProgram(instructorProgramDto);

                var instructorProgram = new InstructorProgram
                {
                    InstructorId = instructorProgramDto.InstructorId,
                    ProgramId = instructorProgramDto.ProgramId,
                    Active = true,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    DeleteDate = DateTime.Now
                };

                var instructorProgramCreado = await _instructorProgramData.CreateAsync(instructorProgram);
                var instructor = await _instructorData.GetByidAsync(instructorProgramCreado.InstructorId);
                var program = await _programData.GetByidAsync(instructorProgramCreado.ProgramId);

                return new InstructorProgramDTOAuto
                {
                    Id = instructorProgramCreado.Id,
                    InstructorId = instructorProgramCreado.InstructorId,
                    ProgramId = instructorProgramCreado.ProgramId,
                    Active = instructorProgramCreado.Active,
                    Instructor = instructor != null ? new InstructorDTOAuto
                    {
                        Id = instructor.Id,
                        PersonId = instructor.PersonId,
                        Active = instructor.Active
                    } : null,
                    Program = program != null ? new ProgramDTOAuto
                    {
                        Id = program.Id,
                        Name = program.Name,
                        Code = program.Code,
                        Version = program.Version,
                        Duration = program.Duration,
                        Active = program.Active
                    } : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva relación instructor-programa");
                throw new ExternalServiceException("Base de datos", "Error al crear la relación instructor-programa", ex);
            }
        }

        private async Task ValidateInstructorProgram(InstructorProgramDTOAuto instructorProgramDto)
        {
            if (instructorProgramDto == null)
            {
                throw new ValidationException("El objeto instructor-programa no puede ser nulo");
            }

            if (instructorProgramDto.InstructorId <= 0)
            {
                _logger.LogWarning("Se intentó crear una relación instructor-programa con InstructorId inválido");
                throw new ValidationException("InstructorId", "El ID del instructor es obligatorio y debe ser mayor que cero");
            }

            if (instructorProgramDto.ProgramId <= 0)
            {
                _logger.LogWarning("Se intentó crear una relación instructor-programa con ProgramId inválido");
                throw new ValidationException("ProgramId", "El ID del programa es obligatorio y debe ser mayor que cero");
            }

            // Verificar que el instructor existe
            var instructor = await _instructorData.GetByidAsync(instructorProgramDto.InstructorId);
            if (instructor == null)
            {
                throw new ValidationException("InstructorId", $"No existe un instructor con ID {instructorProgramDto.InstructorId}");
            }

            // Verificar que el programa existe
            var program = await _programData.GetByidAsync(instructorProgramDto.ProgramId);
            if (program == null)
            {
                throw new ValidationException("ProgramId", $"No existe un programa con ID {instructorProgramDto.ProgramId}");
            }
        }
    }
} 