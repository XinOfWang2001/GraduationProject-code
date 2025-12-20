using Leap.ApplicationServices.DTO.ModelDTO;
using Microsoft.AspNetCore.Components;

namespace LeapDataScienceTool.Components.ModelingProcess.ModelAlgorithms
{
    public abstract class IAlgorithmConfigComponent : ComponentBase
    {
        // A parent modelconfig object to apply two-way binding to.
        [Parameter]
        public required ModelConfigDTO ModelConfigDTO { get; set; }

        protected override void OnInitialized()
        {
            ModelConfigDTO.AlgorithmParameterDTO = AssignAlgorithm();
        }
        public abstract AlgorithmDTO AssignAlgorithm();
    }
}
