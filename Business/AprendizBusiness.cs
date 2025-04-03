using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class AprendizBusiness
    {
        private readonly AprendizData _aprendizData;
        private readonly PersonData _personData;
        private readonly ILogger _logger;

        public AprendizBusiness(AprendizData aprendizData, PersonData personData, ILogger logger)
        {
            _aprendizData = aprendizData;
            _personData = personData;
            _logger = logger;
        }

        public async Task<IEnumerable<AprendizDTOAuto>> GetAllAprendicesAsync()
        {
            try
            {
                var aprendices = await _aprendizData.GetAllAsync();
                var aprendicesDTO = new List<AprendizDTOAuto>();

                foreach (var aprendiz in aprendices)
                {
                    var person = await _personData.GetByIdAsync(aprendiz.PersonId);
                    
                    var aprendizDto = new AprendizDTOAuto
                    {
                        Id = aprendiz.Id,
                        PersonId = aprendiz.PersonId,
                        Active = aprendiz.Active,
                        
                    };

                    aprendicesDTO.Add(aprendizDto);
                }

                return aprendicesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los aprendices");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de aprendices", ex);
            }
        }

        public async Task<AprendizDTOAuto> GetAprendizByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un aprendiz con ID inválido: {AprendizId}", id);
                throw new ValidationException("id", "El ID del aprendiz debe ser mayor que cero");
            }

            try
            {
                var aprendiz = await _aprendizData.GetByIdAsync(id);
                if (aprendiz == null)
                {
                    _logger.LogInformation("No se encontró ningún aprendiz con ID: {AprendizId}", id);
                    throw new EntityNotFoundException("Aprendiz", id);
                }

                var person = await _personData.GetByIdAsync(aprendiz.PersonId);

                return new AprendizDTOAuto
                {
                    Id = aprendiz.Id,
                    PersonId = aprendiz.PersonId,
                    Active = aprendiz.Active,
                    
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el aprendiz con ID: {AprendizId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el aprendiz con ID {id}", ex);
            }
        }

        public async Task<AprendizDTOAuto> CreateAprendizAsync(AprendizDTOAuto aprendizDto)
        {
            try
            {
                ValidateAprendiz(aprendizDto);

                var aprendiz = new Aprendiz
                {
                    PersonId = aprendizDto.PersonId,
                    Active = true,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    DeleteDate = DateTime.Now
                };

                var aprendizCreado = await _aprendizData.CreateAsync(aprendiz);
                var person = await _personData.GetByIdAsync(aprendizCreado.PersonId);

                return new AprendizDTOAuto
                {
                    Id = aprendizCreado.Id,
                    PersonId = aprendizCreado.PersonId,
                    Active = aprendizCreado.Active,
                    
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo aprendiz");
                throw new ExternalServiceException("Base de datos", "Error al crear el aprendiz", ex);
            }
        }

        private void ValidateAprendiz(AprendizDTOAuto aprendizDto)
        {
            if (aprendizDto == null)
            {
                throw new ValidationException("El objeto aprendiz no puede ser nulo");
            }

            if (aprendizDto.PersonId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un aprendiz con PersonId inválido");
                throw new ValidationException("PersonId", "El ID de la persona es obligatorio y debe ser mayor que cero");
            }
        }
    }
} 