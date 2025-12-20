using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.ApplicationServices.Interfaces.Repositories;
using LeapDataScienceAPI.Services.BuilderAndMappers.Mappers;
using Microsoft.AspNetCore.Mvc;


namespace LeapDataScienceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataExtractController : ControllerBase
    {
        private readonly IDataExtractService dataExtractProcessService;
        private readonly IDataExtractRepository dataExtractRepository;
        private readonly IDataExtractValidatorFactory dataExtractValidatorFactory;

        public DataExtractController(
            IDataExtractService dataExtractProcessService,
            IDataExtractRepository dataExtractRepository,
            IDataExtractValidatorFactory validatorFactory)
        {
            this.dataExtractProcessService = dataExtractProcessService;
            this.dataExtractRepository = dataExtractRepository;
            dataExtractValidatorFactory = validatorFactory;
        }
        // GET api/<DataExtractController>/5
        [HttpGet("{id}")]
        public ActionResult<DataExtractConfigDTO> Get(Guid id)
        {
            var result = dataExtractRepository.Get(id);
            if (result == null)
            {
                return BadRequest(ReturnError("Niet gevonden"));
            }
            DataExtractConfigDTO? dto = result.MapToDTO();
            return Ok(dto);
        }

        // POST api/<DataExtractController>
        [HttpPost]
        public async Task<ActionResult<DataExtractConfigDTO>> Post([FromBody] DataExtractConfigDTO body)
        {
            // Check if all required fields are present.
            try
            {
                var validator = dataExtractValidatorFactory.GetInputValidator(body);

                if (!validator.Validate(body))
                {
                    var BadResponseBody = ReturnError(validator.GetErrorMessage());
                    return BadRequest(BadResponseBody);
                }
                var response = await dataExtractProcessService.RegisterDataExtractProcess(body);
                // Return post body
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ReturnError(ex.Message));
            }
        }

        // PUT api/<DataExtractController>/5
        [HttpPut("{procesId}")]
        public async Task<ActionResult<DataExtractConfigDTO>> Put(Guid procesId, [FromBody] DataExtractConfigDTO body)
        {
            try
            {
                var validator = dataExtractValidatorFactory.GetInputValidator(body);
                if (!validator.Validate(body))
                {
                    var BadResponseBody = ReturnError(validator.GetErrorMessage());
                    return BadRequest(BadResponseBody);
                }
                var result = await dataExtractProcessService.UpdateDataExtractProcess(procesId, body);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ReturnError(ex.Message));
            }
        }

        private static DataExtractConfigDTO ReturnError(string message)
        {
            return new DataExtractConfigDTO()
            {
                StatusCode = 400,
                Message = message
            };
        }
    }
}
