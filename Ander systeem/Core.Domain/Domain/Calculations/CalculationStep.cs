using Leap.Domain.Domain.Workspaces;

namespace Leap.Domain.Domain.Calculations
{
    public class CalculationStep
    {
        // Database related
        public int CalculationStepId { get; set; }
        // Database related
        public Guid WorkspaceGuid { get; set; }
        public int Order { get; set; } = -1;
        public required CalculationType CalculationType { get; set; }

        public required Workspace Workspace { get; set; }
        public ICollection<Calculation> Calculations { get; set; } = new List<Calculation>();
    }
}
