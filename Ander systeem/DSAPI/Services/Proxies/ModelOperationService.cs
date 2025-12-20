using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelingProcess;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.Calculations;
using Leap.Domain.Domain.ModelStorage;
using Leap.Domain.Domain.Workspaces;
using LeapDataScienceAPI.Services.BuilderAndMappers.Mappers;

namespace LeapDataScienceAPI.Services.Proxies
{
    public class ModelOperationService(
        IPythonFacadeService pythonFacadeService,
        IWorkspaceRepository workspaceRepository,
        ICalculationRepository calculationRepository,
        IModelConfigBuilder modelConfigBuilder,
        IModelStorageRepository modelStorageRepository) : IModelOperationService
    {
        private readonly IPythonFacadeService pythonFacadeService = pythonFacadeService;
        private readonly IWorkspaceRepository workspaceRepository = workspaceRepository;
        private readonly IModelConfigBuilder modelConfigBuilder = modelConfigBuilder;
        private readonly IModelStorageRepository modelStorageRepository = modelStorageRepository;

        public async Task<ModelResultDataDTO?> TriggerModelTraining(ModelTrainingRequestDTO dto)
        {
            ModelRequestDTO modelRequestDTO = CombineEntitiesForRequest(dto);

            return await pythonFacadeService.TriggerModelTraining(modelRequestDTO);
        }

        public async Task<ModelStorageDTO?> TriggerModelStorage(ModelStorageCreationRequestDTO dto)
        {
            ModelRequestDTO modelRequest = CombineEntitiesForRequest(dto);
            // Trigger model storage
            var result = await pythonFacadeService.StoreModel(modelRequest);
            // Convert DTO to Domain and store model address.
            Workspace workspace = workspaceRepository.Get(dto.WorkspaceGuid)!;
            var entity = result!.MapToDomain(workspace);
            await PersistModelLocation(workspace, entity, dto.Overwrite);
            // Return result
            return result;
        }

        public async Task<ModelStorageDTO?> GetModelStorage(Guid WorkspaceGuid)
        {
            var result = await modelStorageRepository.GetByWorkspace(WorkspaceGuid);
            return result?.MapToDTO();
        }

        private async Task PersistModelLocation(Workspace ws, ModelStorageAdress location, bool Overwrite)
        {
            if (Overwrite)
            {
                await modelStorageRepository.Update(ws.WorkspaceGuid, location);
            }
            else
            {
                await modelStorageRepository.Create(location);
            }
        }

        private ModelRequestDTO CombineEntitiesForRequest(ModelTrainingRequestDTO dto)
        {
            Workspace WorkSpace = Validate(dto.WorkspaceGuid);
            if (!WorkSpace.ValidateCompleteness())
            {
                throw new InvalidDataException("Incomplete Workspace entiteit. DataExtractConfig & Modelconfig required for preview model training");
            }
            IEnumerable<CalculationStep> steps = calculationRepository.Get(dto.WorkspaceGuid);
            return new()
            {
                DataRequest = WorkSpace.DataExtraction!.MapToDataRequestDTO(true),
                ModelConfig = modelConfigBuilder.BuildDTO(WorkSpace.ModelConfig!),
                OperationList = steps.MapToDTO()
            };
        }

        private ModelRequestDTO CombineEntitiesForRequest(ModelStorageCreationRequestDTO dto)
        {
            Workspace WorkSpace = Validate(dto.WorkspaceGuid);
            if (!WorkSpace.ValidateCompleteness())
            {
                throw new InvalidDataException("Incomplete Workspace entiteit. DataExtractConfig & Modelconfig required for model storage");
            }
            IEnumerable<CalculationStep> steps = calculationRepository.Get(dto.WorkspaceGuid);
            return new()
            {
                DataRequest = WorkSpace.DataExtraction!.MapToDataRequestDTO(true),
                ModelConfig = modelConfigBuilder.BuildDTO(WorkSpace.ModelConfig!),
                OperationList = steps.MapToDTO()
            };
        }

        private Workspace Validate(Guid workspaceGuid)
        {
            Workspace? WorkSpace = workspaceRepository.Get(workspaceGuid);
            return WorkSpace ?? throw new InvalidDataException("Workspace not found");

        }

        public async Task<bool> DeleteModelFile(Guid WorkspaceGuid)
        {
            // Retrieve model address 
            var Address = await modelStorageRepository.GetByWorkspace(WorkspaceGuid);
            // If no address was found, return true.
            if (Address != null)
            {
                // Send request to FastAPI
                await pythonFacadeService.DeleteModel(Address.ModelStorageAddress);
            }
            return true;
        }
    }
}
