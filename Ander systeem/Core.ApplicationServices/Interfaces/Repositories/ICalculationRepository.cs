using Leap.Domain.Domain.Calculations;

namespace Leap.ApplicationServices.Interfaces.Repositories
{
    public interface ICalculationRepository
    {
        IEnumerable<CalculationStep> Get(Guid WorkspaceGuid);
        Task Create(IEnumerable<CalculationStep> calculationSteps);

        Task Overwrite(Guid WorkspaceGuid, IEnumerable<CalculationStep> calculationSteps);
    }
}
