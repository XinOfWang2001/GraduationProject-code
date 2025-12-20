
using Leap.Domain.Domain.Calculations;
using Leap.Domain.Domain.DataConfig;
using Leap.Domain.Domain.ModelConfig;
using Leap.Domain.Domain.ModelStorage;

namespace Leap.Domain.Domain.Workspaces
{
    public class Workspace
    {
        public int WorkspaceId { get; set; }
        public Guid WorkspaceGuid { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Workspace empty";

        // Foreign key relation
        public DataExtracter? DataExtraction { get; set; }
        public int? DataExtractionId { get; set; }

        // Foreign key relation. The Primary model configuration.
        // Has one-to-one/zero.
        public ModelConfiguration? ModelConfig { get; set; }

        public ModelStorageAdress? ModelStorage { get; set; }
        // Foreign key
        // public LinkedList<DataProcess> Processes { get; set; } = [];

        public IEnumerable<CalculationStep> CalculationSteps { get; set; } = new List<CalculationStep>();

        public bool ValidateCompleteness()
        {
            return DataExtraction != null && ModelConfig != null;
        }
    }
}
