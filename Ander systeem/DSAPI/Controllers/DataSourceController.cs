using Leap.ApplicationServices.AppGeneralServices.ExternalServices;
using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.External_Services;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataSource;
using Microsoft.AspNetCore.Mvc;

namespace LeapDataScienceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSourceController : ControllerBase
    {
        private readonly IDataSourceService dataSourceService;
        private readonly ISwecoWebServices<IWAWebService> iwaWebServices;
        private readonly IPreviewDataService dataProxyService;
        private readonly IProjectRepository projectRepository;

        public DataSourceController(IDataSourceService dataSourceService,
            ISwecoWebServices<IWAWebService> iwaWebServices,
            IPreviewDataService dataProxyService, IProjectRepository projectRepository)
        {
            this.dataSourceService = dataSourceService;
            this.iwaWebServices = iwaWebServices;
            this.dataProxyService = dataProxyService;
            this.projectRepository = projectRepository;
        }

        // GET: api/<DataSourceController>
        [HttpGet]
        public async Task<IEnumerable<DataSourceDTO>> Get()
        {
            // Verstuur projecten
            return await dataSourceService.GetData();
        }

        // Used for calling the monitor-value endpoint of either IWA or IoT-hub API's.
        // Later in the project, the monitor-value endpoint will be used to retrieve data.
        [HttpGet("project/{projectId}")]
        public async Task<MonitorInfoDTO?> GetObservationDataDTOsAsync(int projectId)
        {
            Project? project = projectRepository.Get(projectId);
            if (project == null)
            {
                return new MonitorInfoDTO()
                {
                    StatusCode = 404,
                    Message = "Project is niet gevonden"
                };
            }
            // Make API call to Online-endpoint.
            MonitorInfoDTO? monitorInfo = await iwaWebServices.GetInfo(project.Name, project.ProjectToken);
            // Check if result is correct
            if (monitorInfo == null)
            {
                return new MonitorInfoDTO()
                {
                    StatusCode = 400,
                    Message = "Geen monitorings-informatie beschikbaar"
                };
            }
            monitorInfo.StatusCode = 200;
            return monitorInfo;
        }

        // GET api/<DataSourceController>/5
        [HttpGet("{id}")]
        public async Task<DataSourceDTO> GetOne(int id)
        {
            return await dataSourceService.GetOne(id);
        }

        // Wordt gebruikt om data te verzamelen.
        [HttpGet("{workspaceId}/preview-data")]
        public async Task<ActionResult<PreviewDataDTO>> GetData(Guid workspaceId, [FromQuery] bool provideData = false)
        {
            try
            {
                var result = await dataProxyService.GetPreviewData(workspaceId, provideData);
                return Ok(result);
            }
            catch (Exception exception)
            {
                // Send 404 if not found or failed an API Call.
                return NotFound(exception.Message);
            }
        }
    }
}

