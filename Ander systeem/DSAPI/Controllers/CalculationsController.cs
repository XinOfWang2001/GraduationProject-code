using Leap.ApplicationServices.DTO.Calculations;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Microsoft.AspNetCore.Mvc;

namespace LeapDataScienceAPI.Controllers
{
    /// <summary>
    /// Assumption:
    /// All of the write and update operations will update all of the calculations as a whole.
    /// If one step updates from 2 -> 3, then everyother entity will be updated.
    /// For this reason, WorkspaceGUID is used to retrieve all calculations related to the workspace.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CalculationsController(ICalculationService calculationService) : ControllerBase
    {
        private readonly ICalculationService calculationService = calculationService;

        [HttpGet("{workspaceGuid}")]
        public async Task<ActionResult<CalculationRequestDTO>> Get(Guid workspaceGuid)
        {
            var collection = await calculationService.GetCalculations(workspaceGuid);
            return Ok(collection);
        }

        [HttpPut]
        public async Task<ActionResult<CalculationWriteDTO>> Overwrite(CalculationWriteDTO calculationWrite)
        {
            try
            {
                await calculationService.OverwriteCalculations(calculationWrite);
                return Ok(calculationWrite);
            }
            catch (Exception ex)
            {
                calculationWrite.Message = ex.Message;
                calculationWrite.StatusCode = 400;
                return BadRequest(calculationWrite);
            }
        }
    }
}
