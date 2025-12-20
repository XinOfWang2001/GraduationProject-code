using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Strategies;
using Leap.Domain.Domain.ModelConfig.Enums;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty Project, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LeapDataScienceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly IModelService modelProxyService;

        public ModelController(IServiceProvider serviceProvider, IModelService modelProxyService)
        {
            this.ServiceProvider = serviceProvider;
            this.modelProxyService = modelProxyService;
        }

        // GET api/<ModelController>/5
        [HttpGet("{ModelConfigGuid}")]
        public async Task<ModelConfigDTO?> Get(Guid ModelConfigGuid)
        {
            var result = await modelProxyService.GetModelConfig(ModelConfigGuid);
            if (result == null)
            {
                HttpContext.Response.StatusCode = 404;
                return null;
            }
            return result;
        }

        // POST api/<ModelController>
        [HttpPost]
        public async Task<ModelConfigDTO?> Post([FromBody] ModelConfigDTO dto)
        {
            try
            {
                bool validInput = ValidateInput(dto);
                if (!validInput)
                {
                    HttpContext.Response.StatusCode = 404;
                    return null;
                }
                var result = await modelProxyService.RegisterModelConfig(dto);
                return result;
            }
            catch
            {
                HttpContext.Response.StatusCode = 400;
                return null;
            }
        }

        // PUT api/<ModelController>/5
        [HttpPut("{ModelConfigGuid}")]
        public async Task<IActionResult> Put(Guid ModelConfigGuid, [FromBody] ModelConfigDTO dto)
        {
            bool validInput = ValidateInput(dto);
            if (!validInput) return BadRequest();

            try
            {
                var response = await modelProxyService.UpdateModelConfig(ModelConfigGuid, dto);
                return Ok(response);
            }
            catch
            {
                return BadRequest(dto);
            }
        }

        private bool ValidateInput(ModelConfigDTO dto)
        {
            IInputValidatorStrategy<ModelConfigDTO> validator = GetValidator(dto.ModelAlgorithm);
            IInputValidatorStrategy<ModelConfigDTO> modelvalidator = GetModelValidator(dto.ModelType);

            return validator.Validate(dto) && modelvalidator.Validate(dto);
        }

        private IInputValidatorStrategy<ModelConfigDTO> GetValidator(ModelAlgorithm modelAlgorithm)
        {
            return ServiceProvider.GetRequiredKeyedService<IInputValidatorStrategy<ModelConfigDTO>>(modelAlgorithm);
        }

        private IInputValidatorStrategy<ModelConfigDTO> GetModelValidator(ModelType modelType)
        {
            return ServiceProvider.GetRequiredKeyedService<IInputValidatorStrategy<ModelConfigDTO>>(modelType);
        }
    }
}
