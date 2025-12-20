using Leap.ApplicationServices.AppGeneralServices.ModelConfigCreation;
using Leap.ApplicationServices.Interfaces.Strategies;
using Leap.Domain.Domain.ModelConfig.Enums;

namespace LeapDataScienceTool.ProgramSetup
{
    public static class RuntimeClasses
    {
        public static void RegisterRuntimeClasses(this IServiceCollection services)
        {
            services.AddKeyedSingleton<IAlgorithmCreationStrategy, LinearRegressionParamStrategy>(ModelAlgorithm.LINEAR_REGRESSION);
            services.AddKeyedSingleton<IAlgorithmCreationStrategy, SVMParamStrategy>(ModelAlgorithm.SVMREGRESSION);
        }
    }
}
