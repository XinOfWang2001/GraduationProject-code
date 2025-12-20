using Leap.ApplicationServices.AppGeneralServices.ModelInputValidation;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.Strategies;
using Leap.Domain.Domain.ModelConfig.Enums;

namespace Test.ApplicationService.UC_2_ModelConfig
{
    public class TestModelInputValidation
    {
        private IInputValidatorStrategy<ModelConfigDTO> linearRegressionValidatorStrategy;
        private IInputValidatorStrategy<ModelConfigDTO> svmValidatorStrategy;
        private IInputValidatorStrategy<ModelConfigDTO> ForecastingModelStrategy;
        public TestModelInputValidation()
        {
            linearRegressionValidatorStrategy = new LinearRegressionInputValidator();
            svmValidatorStrategy = new SVMInputValidator();
            ForecastingModelStrategy = new ForecastingModelValidator();
        }

        [Fact]
        public void TestIfLinearRegressionTestReceivesNoValidInput()
        {

            ModelConfigDTO modelConfig = new ModelConfigDTO
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                ModelAlgorithm = ModelAlgorithm.SVMREGRESSION,
                AlgorithmParameterDTO = new SVMDTO()
            };
            var result = linearRegressionValidatorStrategy.Validate(modelConfig);
            var error = linearRegressionValidatorStrategy.GetErrorMessage();
            Assert.False(result);
            Assert.Equal("Linear regression requires a valid set linear regression algorithm parameter.", error);
        }

        // Code: A-15
        // Functional requirement: Model training
        // Testcase: Test if Linear regression modeltype Enum and a any other ModelParameter class fails
        // Expected result: Need to return false as result.
        [Fact]
        public void TestIfModelAlgoAndOtherModelParameterClassIsConsistent()
        {

            ModelConfigDTO modelConfig = new ModelConfigDTO
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                AlgorithmParameterDTO = new SVMDTO()
            };
            var result = linearRegressionValidatorStrategy.Validate(modelConfig);

            Assert.False(result);
        }

        // Code: A-16
        // Functional requirement: Model training
        // Testcase: Test if Linear regression modeltype Enum and a any other ModelParameter class fails
        // Expected result: Need to return false as result.
        [Fact]
        public void TestIfLinearModelTypeAndModelParameterPasses()
        {
            ModelConfigDTO modelConfig = new ModelConfigDTO
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                AlgorithmParameterDTO = new LinearRegressionDTO()
            };
            var result = linearRegressionValidatorStrategy.Validate(modelConfig);

            Assert.True(result);
        }

        [Fact]
        public void TestIfSVMAndOtherModelParameterClassFails()
        {

            ModelConfigDTO modelConfig = new ModelConfigDTO
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                AlgorithmParameterDTO = new LinearRegressionDTO()
            };
            var result = svmValidatorStrategy.Validate(modelConfig);
            var error = svmValidatorStrategy.GetErrorMessage();
            Assert.False(result);
            Assert.Equal("SVM requires a valid set of SVM algorithm parameters.", error);
        }

        // Code: A-17
        // Functional requirement: Model training
        // Testcase: Test if Linear regression modeltype Enum and a any other ModelParameter class fails
        // Expected result: Need to return false as result.
        [Fact]
        public void TestIfSVMModelTypeAndModelParameterPasses()
        {
            ModelConfigDTO modelConfig = new()
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                ModelAlgorithm = ModelAlgorithm.SVMREGRESSION,
                AlgorithmParameterDTO = new SVMDTO()
            };
            var result = svmValidatorStrategy.Validate(modelConfig);

            Assert.True(result);
        }

        [Fact]
        // Code: A-18
        // Functional requirement: Model training
        // Testcase: Test if inputvalidator returns error when model configuration does not contain a target variable.
        // Expected result: Returns false
        public void TestModelForecastingConfigWithNoTargetsReturningError()
        {
            ModelConfigDTO modelConfig = new()
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                ModelAlgorithm = ModelAlgorithm.SVMREGRESSION,
                AlgorithmParameterDTO = new SVMDTO(),
                ModelType = ModelType.FORECASTING,
                Targets = [],
                Features = [],
            };
            var result = ForecastingModelStrategy.Validate(modelConfig);

            Assert.False(result);
            Assert.Equal("When training a forecasting model, at least one target variable should be selected", ForecastingModelStrategy.GetErrorMessage());
        }
    }
}
