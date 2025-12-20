using Leap.ApplicationServices.DTO.Workspace;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Microsoft.AspNetCore.Mvc;

namespace LeapDataScienceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        private readonly IWorkspaceService workspaceService;

        // Endpoint to get one Workspace.
        public WorkspaceController(IWorkspaceService workspaceService)
        {
            this.workspaceService = workspaceService;
        }

        // Endpoint to create Workspace.
        [HttpPost]
        public async Task<ActionResult<WorkspaceConfigDTO>> Post(WorkspaceConfigDTO workshopConfigDTO)
        {
            try
            {
                var result = await workspaceService.RegisterWorkspace(workshopConfigDTO);
                return Ok(result);
            }
            catch
            {
                HttpContext.Response.StatusCode = 400;
                return BadRequest(workshopConfigDTO);
            }
        }
        [HttpGet("{workspaceGuid}")]
        public async Task<WorkspaceConfigDTO?> GetOne(Guid workspaceGuid)
        {
            var result = await workspaceService.GetWorkspace(workspaceGuid);
            if (result == null)
            {
                HttpContext.Response.StatusCode = 404;
                return null;
            }
            return result;
        }

        [HttpGet]
        public async Task<IEnumerable<WorkspaceConfigDTO>> Get()
        {
            var result = await workspaceService.GetAllWorkspaces();
            return result;
        }

        [HttpDelete("{workspaceGuid}")]
        public async Task<ActionResult> Delete(Guid workspaceGuid)
        {
            try
            {
                await workspaceService.DeleteWorkspace(workspaceGuid);
                return Ok();
            }
            catch
            {
                Console.WriteLine("Deletion questionable");
                return BadRequest();
            }
        }
    }
}
