using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.Domain.Domain.ModelConfig.Enums;
using LeapDataScienceTool.Components.ModelingProcess.ModelAlgorithms;
using Microsoft.AspNetCore.Components;

namespace LeapDataScienceTool.Services
{
    public interface IAlgorithmComponentBuilder
    {
        RenderFragment? BuildAlgorithmComponent(ModelConfigDTO modelConfig);
    }

    public class AlgorithmBuilder : IAlgorithmComponentBuilder
    {
        public RenderFragment? BuildAlgorithmComponent(ModelConfigDTO modelConfig)
        {
            ModelAlgorithm modelAlgorithm = modelConfig.ModelAlgorithm;
            RenderFragment? AdvancedParameters = modelAlgorithm switch
            {
                ModelAlgorithm.LINEAR_REGRESSION => CreateLinearConfig(modelConfig),
                ModelAlgorithm.SVMREGRESSION => CreateSVMConfig(modelConfig),
                _ => null
            };
            return AdvancedParameters;
        }

        private RenderFragment CreateLinearConfig(ModelConfigDTO configDTO)
        {
            return builder =>
            {
                builder.OpenComponent<LinearRegressionConfig>(0);
                builder.AddAttribute(1, "ModelConfigDTO", configDTO);
                builder.CloseComponent();
            };
        }

        private RenderFragment CreateSVMConfig(ModelConfigDTO configDTO)
        {
            return builder =>
            {
                builder.OpenComponent<SVMConfig>(0);
                builder.AddAttribute(1, "ModelConfigDTO", configDTO);
                builder.CloseComponent();
            };
        }
    }
}
