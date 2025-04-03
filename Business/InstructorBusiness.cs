using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class InstructorBusiness
    {
        private readonly InstructorData _instructorData;
        private readonly PersonData _personData;
        private readonly ILogger _logger;

        public InstructorBusiness(InstructorData instructorData, PersonData personData, ILogger logger)
        {
            _instructorData = instructorData;
            _personData = personData;
            _logger = logger;
        }

        public async Task<IEnumerable<InstructorDTOAuto>> GetAllInstructoresAsync()
        {
            try
            {
                var instructores = await _instructorData.GetAllAsync();
                var instructoresDTO = new List<InstructorDTOAuto>();

                foreach (var instructor in instructores)
                {
                    var person = await _personData.GetByIdAsync(instructor.PersonId);
                    
                    var instructorDto = new InstructorDTOAuto
                    {
                        Id = instructor.Id,
                        PersonId = instructor.PersonId,
                        Active = instructor.Active,
                        
                    };

                    instructoresDTO.Add(instructorDto);
                }

                return instructoresDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los instructores");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de instructores", ex);
            }
        }

        public async Task<InstructorDTOAuto> GetInstructorByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un instructor con ID inválido: {InstructorId}", id);
                throw new ValidationException("id", "El ID del instructor debe ser mayor que cero");
            }

            try
            {
                var instructor = await _instructorData.GetByIdAsync(id);
                if (instructor == null)
                {
                    _logger.LogInformation("No se encontró ningún instructor con ID: {InstructorId}", id);
                    throw new EntityNotFoundException("Instructor", id);
                }

                var person = await _personData.GetByIdAsync(instructor.PersonId);

                return new InstructorDTOAuto
                {
                    Id = instructor.Id,
                    PersonId = instructor.PersonId,
                    Active = instructor.Active,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el instructor con ID: {InstructorId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el instructor con ID {id}", ex);
            }
        }

        public async Task<InstructorDTOAuto> CreateInstructorAsync(InstructorDTOAuto instructorDto)
        {
            try
            {
                ValidateInstructor(instructorDto);

                var instructor = new Instructor
                {
                    PersonId = instructorDto.PersonId,
                    Active = true,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    DeleteDate = DateTime.Now
                };

                var instructorCreado = await _instructorData.CreateAsync(instructor);
                    var person = await _personData.GetByIdAsync(instructorCreado.PersonId);

                return new InstructorDTOAuto
                {
                    Id = instructorCreado.Id,
                    PersonId = instructorCreado.PersonId,
                    Active = instructorCreado.Active,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo instructor");
                throw new ExternalServiceException("Base de datos", "Error al crear el instructor", ex);
            }
        }

        private void ValidateInstructor(InstructorDTOAuto instructorDto)
        {
            if (instructorDto == null)
            {
                throw new ValidationException("El objeto instructor no puede ser nulo");
            }

            if (instructorDto.PersonId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un instructor con PersonId inválido");
                throw new ValidationException("PersonId", "El ID de la persona es obligatorio y debe ser mayor que cero");
            }
        }
    }
} 