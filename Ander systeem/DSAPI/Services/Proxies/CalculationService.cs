using Leap.ApplicationServices.AppGeneralServices.CalculationValidators;
using Leap.ApplicationServices.DTO.Calculations;
using Leap.ApplicationServices.Interfaces.CalculationValidation;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.Calculations;
using Leap.Domain.Domain.Workspaces;
using LeapDataScienceAPI.Services.BuilderAndMappers.Mappers;

namespace LeapDataScienceAPI.Services.Proxies
{
    public class CalculationService(IWorkspaceRepository workspaceRepository, ICalculationRepository calculationRepository, CalculationFactory calculationFactory) : ICalculationService
    {
        private readonly IWorkspaceRepository workspaceRepository = workspaceRepository;
        private readonly ICalculationRepository calculationRepository = calculationRepository;
        private readonly ICalculationComponent validator = calculationFactory.CreateCalculationValidators();

        public async Task<CalculationRequestDTO> GetCalculations(Guid WorkspaceGuid)
        {
            // Check if workspace exists?
            Workspace? workspace = GetWorkspace(WorkspaceGuid);
            var collection = calculationRepository.Get(WorkspaceGuid);
            // Map them to DTO
            var stepsDtos = collection.MapToDTO();
            // Return result
            return new CalculationRequestDTO() { Steps = stepsDtos };
        }

        public async Task<CalculationWriteDTO> OverwriteCalculations(CalculationWriteDTO calculationRequest)
        {
            Workspace? workspace = GetWorkspace(calculationRequest.WorkspaceGuid);
            IEnumerable<CalculationStep> steps = calculationRequest.Steps.MapToDomain(workspace);
            // Here validation
            if (!validator.Parse(steps))
            {
                throw new InvalidOperationException(validator.ReturnError());
            }
            // Here post request.
            await calculationRepository.Overwrite(calculationRequest.WorkspaceGuid, steps);
            return calculationRequest;
        }

        private Workspace GetWorkspace(Guid WorkspaceGuid)
        {
            Workspace? workspace = workspaceRepository.Get(WorkspaceGuid);
            return workspace ?? throw new InvalidOperationException("Invalid workspace");
        }
    }
}
