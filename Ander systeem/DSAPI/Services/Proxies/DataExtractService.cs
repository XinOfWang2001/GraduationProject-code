using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataConfig;
using Leap.Domain.Domain.DataSource;
using Leap.Domain.Domain.Workspaces;
using LeapDataScienceAPI.Services.BuilderAndMappers.Mappers;

namespace LeapDataScienceAPI.Services.Proxies
{
    public class DataExtractService(
        IWorkspaceRepository workshopRepository,
        IDataExtractRepository dataExtractRepository,
        IProjectRepository projectRepository) : IDataExtractService
    {
        private readonly IWorkspaceRepository workshopRepository = workshopRepository;
        private readonly IDataExtractRepository dataExtractRepository = dataExtractRepository;
        private readonly IProjectRepository projectRepository = projectRepository;

        public async Task<DataExtractConfigDTO?> RegisterDataExtractProcess(DataExtractConfigDTO config)
        {
            Workspace? workspace = workshopRepository.Get(config.WorkspaceId);
            if (workspace != null && workspace.DataExtraction != null)
            {
                return ReturnError("DataExtracter entiteit bestaat al");
            }
            DataExtracter inputDataConfig = ValidateRequiredEntities(config);
            await dataExtractRepository.Create(inputDataConfig);
            var dto = inputDataConfig.MapToDTO();
            dto.Message = "Succesvol opgeslagen";
            return dto;
        }

        public async Task<DataExtractConfigDTO?> UpdateDataExtractProcess(Guid procesId, DataExtractConfigDTO config)
        {
            DataExtracter inputDataConfig = ValidateRequiredEntities(config);
            await dataExtractRepository.Update(procesId, inputDataConfig);
            var endresult = inputDataConfig.MapToDTO();

            return endresult;
        }

        private DataExtracter ValidateRequiredEntities(DataExtractConfigDTO dto)
        {
            Workspace? workspace = workshopRepository.Get(dto.WorkspaceId);
            Project? project = projectRepository.Get(dto.ProjectDTO.Guid);
            if (workspace == null || project == null)
            {
                throw new InvalidOperationException("Entities not found");
            }
            return dto.MapToDomain(project, workspace);
        }

        private static DataExtractConfigDTO? ReturnError(string message)
        {
            return new DataExtractConfigDTO() { StatusCode = 401, Message = message };
        }
    }
}
