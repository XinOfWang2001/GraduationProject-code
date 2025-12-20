using Leap.ApplicationServices.DTO.Calculations;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.ModelDTO;

namespace Leap.ApplicationServices.DTO.Workspace
{
    public class WorkspaceConfigDTO
    {
        public int WorkshopId { get; set; }
        public Guid WorkspaceGuid { get; set; } = Guid.NewGuid();
        public string WorkspaceName { get; set; } = "Pipeline";
        // A List
        public DataExtractConfigDTO? DataSourceConfig { get; set; }
        public ModelConfigDTO? ModelConfigDTO { get; set; }
        public IEnumerable<CalculationStepDTO> CalculationStepsDTO { get; set; } = [];
    }
}
