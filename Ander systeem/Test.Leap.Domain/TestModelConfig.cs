using Leap.Domain.Domain.ModelConfig;
using Leap.Domain.Domain.ModelConfig.Enums;
using Leap.Domain.Domain.ModelConfig.ModelParams;
using Leap.Domain.Domain.Workspaces;

namespace Test.Leap.Domain
{
    public class TestModelConfig
    {
        public readonly Workspace DummyWorkspace;
        public readonly ModelParameters parameters;
        public TestModelConfig()
        {
            parameters = new LinearRegressionParameters();
            DummyWorkspace = new Workspace()
            {
                Name = "Test Workspace",
            };
        }

        // A-25
        [Fact(Skip = "Unix is unstable")]
        public void TestForecastDateUnix()
        {
            // Functional requirement: Model training
            // Testcase: Ensure that the forecast date is correctly set to the Unix epoch time when no date is provided.
            // Expect: Datetime should be formatted in Unix format, till the miliseconds.
            var modelConfiguration = new ModelConfiguration()
            {
                ParentWorkspace = DummyWorkspace,
                ModelParameters = parameters,
                DateForecasting = new DateTime(2025, 7, 26, 15, 30, 00)
            };
            var result = modelConfiguration.GetDateTimeUnix();
            Assert.Equal(1753536600000, result); // Expected Unix timestamp in milliseconds for the given date
        }

        // A-26
        [Fact]
        public void TestIfFeatureColumnsAreRequiredWhenModelConfigIsOutlier()
        {
            // Functional requirement: Model training
            // Testcase: Ensure that feature_columns are provided when the model configuration is set for outlier detection.
            // Expect: Should fail if no feature columns are provided.

            var modelConfiguration = new ModelConfiguration()
            {
                ParentWorkspace = DummyWorkspace,
                ModelParameters = parameters,
                ModelType = ModelType.OUTLIER_DETECTION,
                FeatureColumns = []
            };

            var result = modelConfiguration.ValidFeatureColumnSettings();

            Assert.False(result);
        }

        // A-27
        [Fact]
        public void TestIfTargetColumnsAreRequiredWhenModelConfigIsForecasting()
        {
            // Functional requirement: Model training
            // Testcase: Validate that target columns are required when the model configuration is set for forecasting.
            // Expect: Should fail if no target columns are provided.
            var modelConfiguration = new ModelConfiguration()
            {
                ParentWorkspace = DummyWorkspace,
                ModelParameters = parameters,
                ModelType = ModelType.FORECASTING
            };
            var result = modelConfiguration.ValidTargetColumnSettings();
            Assert.False(result);
        }

        // A-29
        [Fact]
        public void TestIfFeatureColumnsSucceedCheckWhenModelConfigIsOutlier()
        {
            // Functional requirement: Model training
            // Testcase: Ensure that feature_columns are provided when the model configuration is set for outlier detection.
            // Expect: Should return true
            var features = new List<FeatureColumns>()
            {
                new () { Name = "Temperature", DataType = "float" },
                new () { Name = "Humidity", DataType = "float" }
            };
            var modelConfiguration = new ModelConfiguration()
            {
                ParentWorkspace = DummyWorkspace,
                ModelParameters = parameters,
                ModelType = ModelType.OUTLIER_DETECTION,
                FeatureColumns = features
            };
            var result = modelConfiguration.ValidFeatureColumnSettings();
            var resultTarget = modelConfiguration.ValidTargetColumnSettings();
            Assert.True(result);
            Assert.True(resultTarget);
        }

        // A-28
        [Fact]
        public void TestIfTargetColumnsSucceedCheckWhenModelConfigIsForecasting()
        {
            // Functional requirement: Model training
            // Testcase: Validate that target columns are required when the model configuration is set for forecasting.
            // Expect: Should fail if no target columns are provided. Raise error.

            var target = new List<TargetColumns>()
            {
                new () { Name = "Temperature", DataType = "float" },
                new () { Name = "Humidity", DataType = "float" }
            };
            var modelConfiguration = new ModelConfiguration()
            {
                ParentWorkspace = DummyWorkspace,
                ModelParameters = parameters,
                ModelType = ModelType.FORECASTING,
                TargetColumns = target
            };
            var result = modelConfiguration.ValidTargetColumnSettings();
            var resultFeatures = modelConfiguration.ValidFeatureColumnSettings();
            Assert.True(result);
            Assert.True(resultFeatures);
        }
    }
}
