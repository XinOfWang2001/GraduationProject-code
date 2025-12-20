using Leap.ApplicationServices.AppGeneralServices.ModelConfigCreation;
using Leap.ApplicationServices.AppGeneralServices.ModelInputValidation;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.Strategies;
using Leap.Domain.Domain.ModelConfig.Enums;

namespace LeapDataScienceAPI.ProgramSetup
{
    // This extension method class will provide the classes that will be used during runtime.
    public static class RunTimeClassSetup
    {
        public static void RegisterRuntimeClasses(this IServiceCollection services)
        {
            services.AddKeyedSingleton<IInputValidatorStrategy<ModelConfigDTO>, SVMInputValidator>(ModelAlgorithm.SVMREGRESSION);
            services.AddKeyedSingleton<IInputValidatorStrategy<ModelConfigDTO>, LinearRegressionInputValidator>(ModelAlgorithm.LINEAR_REGRESSION);
            services.AddKeyedSingleton<IInputValidatorStrategy<ModelConfigDTO>, ForecastingModelValidator>(ModelType.FORECASTING);

            services.AddKeyedSingleton<IAlgorithmCreationStrategy, LinearRegressionParamStrategy>(ModelAlgorithm.LINEAR_REGRESSION);
            services.AddKeyedSingleton<IAlgorithmCreationStrategy, SVMParamStrategy>(ModelAlgorithm.SVMREGRESSION);

        }
    }
}
