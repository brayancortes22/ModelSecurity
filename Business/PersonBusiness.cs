using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class PersonBusiness
    {
        private readonly PersonData _personData;
        private readonly ILogger _logger;

        public PersonBusiness(PersonData personData, ILogger logger)
        {
            _personData = personData;
            _logger = logger;
        }

        public async Task<IEnumerable<PersonDTOAuto>> GetAllPersonsAsync()
        {
            try
            {
                var persons = await _personData.GetAllAsync();
                var personsDTO = new List<PersonDTOAuto>();

                foreach (var person in persons)
                {
                    personsDTO.Add(new PersonDTOAuto
                    {
                        Id = person.Id,
                        Name = person.Name,
                        FirstName = person.FirstName,
                        SecondName = person.SecondName,
                        FirstLastName = person.FirstLastName,
                        SecondLastName = person.SecondLastName,
                        PhoneNumber = person.PhoneNumber,
                        Email = person.Email,
                        TypeIdentification = person.TypeIdentification,
                        NumberIdentification = person.NumberIdentification,
                        Signing = person.Signing,
                        Active = person.Active
                    });
                }

                return personsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de personas", ex);
            }
        }

        public async Task<PersonDTOAuto> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una persona con ID inválido: {PersonId}", id);
                throw new ValidationException("id", "El ID de la persona debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ninguna persona con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return new PersonDTOAuto
                {
                    Id = person.Id,
                    Name = person.Name,
                    FirstName = person.FirstName,
                    SecondName = person.SecondName,
                    FirstLastName = person.FirstLastName,
                    SecondLastName = person.SecondLastName,
                    PhoneNumber = person.PhoneNumber,
                    Email = person.Email,
                    TypeIdentification = person.TypeIdentification,
                    NumberIdentification = person.NumberIdentification,
                    Signing = person.Signing,
                    Active = person.Active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la persona con ID {id}", ex);
            }
        }

        public async Task<PersonDTOAuto> CreatePersonAsync(PersonDTOAuto personDto)
        {
            try
            {
                ValidatePerson(personDto);

                var person = new Person
                {
                    Name = personDto.Name,
                    FirstName = personDto.FirstName,
                    SecondName = personDto.SecondName,
                    FirstLastName = personDto.FirstLastName,
                    SecondLastName = personDto.SecondLastName,
                    PhoneNumber = personDto.PhoneNumber,
                    Email = personDto.Email,
                    TypeIdentification = personDto.TypeIdentification,
                    NumberIdentification = personDto.NumberIdentification,
                    Signing = personDto.Signing,
                    Active = personDto.Active,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    DeleteDate = DateTime.Now
                };

                var personCreada = await _personData.CreateAsync(person);

                return new PersonDTOAuto
                {
                    Id = personCreada.Id,
                    Name = personCreada.Name,
                    FirstName = personCreada.FirstName,
                    SecondName = personCreada.SecondName,
                    FirstLastName = personCreada.FirstLastName,
                    SecondLastName = personCreada.SecondLastName,
                    PhoneNumber = personCreada.PhoneNumber,
                    Email = personCreada.Email,
                    TypeIdentification = personCreada.TypeIdentification,
                    NumberIdentification = personCreada.NumberIdentification,
                    Signing = personCreada.Signing,
                    Active = personCreada.Active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva persona");
                throw new ExternalServiceException("Base de datos", "Error al crear la persona", ex);
            }
        }

        private void ValidatePerson(PersonDTOAuto personDto)
        {
            if (personDto == null)
            {
                throw new ValidationException("El objeto persona no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(personDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona sin nombre");
                throw new ValidationException("Name", "El nombre de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(personDto.TypeIdentification))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona sin tipo de identificación");
                throw new ValidationException("TypeIdentification", "El tipo de identificación es obligatorio");
            }

            if (personDto.NumberIdentification <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con número de identificación inválido");
                throw new ValidationException("NumberIdentification", "El número de identificación debe ser mayor que cero");
            }
        }
    }
} 