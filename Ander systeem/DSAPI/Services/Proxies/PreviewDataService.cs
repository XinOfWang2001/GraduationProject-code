using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using Leap.ApplicationServices.Interfaces.Repositories;
using LeapDataScienceAPI.Services.BuilderAndMappers.Mappers;

namespace LeapDataScienceAPI.Services.Proxies
{
    public class PreviewDataService(
        IDataExtractRepository extractRepo,
        IPythonFacadeService pythonAPIService) : IPreviewDataService
    {
        private readonly IDataExtractRepository _extractRepo = extractRepo;
        private readonly IPythonFacadeService _pythonAPIService = pythonAPIService;

        /// <summary>
        public async Task<PreviewDataDTO> GetPreviewData(Guid workspaceGuid, bool ProvideData)
        {
            // Get data extract process by its Guid.
            var result = _extractRepo.GetByWorkspace(workspaceGuid);
            if (result == null)
            {
                throw new InvalidOperationException("DataExtract configuration not found");
            }
            // Map domain to a DataRequestDTO
            var requestDto = result.MapToDataRequestDTO(ProvideData);
            // Send request to Python API.
            var dataRequest = await _pythonAPIService.RequestPreviewData(requestDto);
            if (dataRequest == null)
            {
                throw new InvalidOperationException("Failed to retrieve data from Python API");
            }
            // Return data
            return dataRequest;
        }
    }
}
