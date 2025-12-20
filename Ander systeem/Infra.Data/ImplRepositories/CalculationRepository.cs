using Infra.Data.DatabaseContext;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.Calculations;

namespace Infra.Data.ImplRepositories
{
    public class CalculationRepository(LeapDSDBContext dbContext) : ICalculationRepository
    {
        private readonly LeapDSDBContext dbContext = dbContext;

        public async Task Create(IEnumerable<CalculationStep> calculationSteps)
        {
            // Is an create necessary?
            // Order by order number
            calculationSteps.OrderBy((key) => key.Order);
            UpdateSteps(calculationSteps);
            await dbContext.CalculationSteps.AddRangeAsync(calculationSteps);
            await dbContext.SaveChangesAsync();
            return;
        }

        public IEnumerable<CalculationStep> Get(Guid WorkspaceGuid)
        {
            var steps = dbContext.CalculationSteps;

            return steps.Where(steps => steps.WorkspaceGuid.Equals(WorkspaceGuid));
        }

        // Assumption, updates are applied to all elements.
        public async Task Overwrite(Guid WorkspaceGuid, IEnumerable<CalculationStep> calculationSteps)
        {
            try
            {
                // First clear all steps.
                var steps = dbContext.CalculationSteps.Where(ws => ws.WorkspaceGuid == WorkspaceGuid);
                dbContext.RemoveRange(steps);
                // If it exist, clear current calculations, insert new calculations and update steps.
                dbContext.SaveChanges();
                // Overwrite individual calculations.
                await Create(calculationSteps);
                return;
            }
            catch
            {
                throw new IOException("Calculation storage went wrong. Please contact with technical specialist to investigate the issue.");
            }
        }

        private IEnumerable<CalculationStep> UpdateSteps(IEnumerable<CalculationStep> calculationSteps)
        {
            for (int i = 0; i < calculationSteps.Count(); i++)
            {
                calculationSteps.ElementAt(i).Order = i;
            }
            return calculationSteps;
        }
    }
}
