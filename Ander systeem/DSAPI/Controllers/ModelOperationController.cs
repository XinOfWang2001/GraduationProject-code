using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelingProcess;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Microsoft.AspNetCore.Mvc;

namespace LeapDataScienceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelOperationController : ControllerBase
    {
        private readonly IModelOperationService modelOperationService;

        public ModelOperationController(IModelOperationService modelOperationService)
        {
            this.modelOperationService = modelOperationService;
        }

        [HttpPost("training-preview")]
        public async Task<ActionResult<ModelResultDataDTO>> TrainModel(ModelTrainingRequestDTO dto)
        {
            try
            {
                var result = await modelOperationService.TriggerModelTraining(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("model-storage")]
        public async Task<ModelStorageDTO?> StoreModel(ModelStorageCreationRequestDTO dto)
        {
            try
            {
                var result = await modelOperationService.TriggerModelStorage(dto);
                return result;
            }
            catch
            {
                HttpContext.Response.StatusCode = 400;
                return null;
            }
        }

        ///  modeloperation/model-storage/{WorkspaceGuid}      - GET
        [HttpGet("model-storage/{WorkspaceGuid}")]
        public async Task<ModelStorageDTO?> Get(Guid WorkspaceGuid)
        {
            var modelStorage = await modelOperationService.GetModelStorage(WorkspaceGuid);
            if (modelStorage == null)
            {
                HttpContext.Response.StatusCode = 404;
            }
            return modelStorage;
        }

        /// FUTURE ENDPOINTS:
        /// - modeloperation/model-storage                      - GET
        /// - modeloperation/model-prediction/{model-storage-id}   - POST

    }
}
