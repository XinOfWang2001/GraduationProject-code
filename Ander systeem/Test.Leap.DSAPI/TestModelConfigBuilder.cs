using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.Domain.Domain.ModelConfig;
using Leap.Domain.Domain.ModelConfig.Enums;
using Leap.Domain.Domain.ModelConfig.ModelParams;
using Leap.Domain.Domain.Workspaces;
using LeapDataScienceAPI.ProgramSetup;
using LeapDataScienceAPI.Services.BuilderAndMappers.ModelBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Leap.DSAPI
{
    public class TestBuilderClass
    {
        private readonly IModelConfigBuilder modelConfigBuilder;
        private readonly IServiceProvider serviceProvider;
        private readonly IServiceCollection serviceCollection;

        public TestBuilderClass()
        {

            serviceCollection = new ServiceCollection();
            serviceCollection.RegisterRuntimeClasses();
            serviceProvider = serviceCollection.BuildServiceProvider();
            modelConfigBuilder = new ModelConfigBuilder(serviceProvider);
        }
        private static ModelConfiguration CreateDomain()
        {
            return new ModelConfiguration()
            {
                ModelConfigId = 1001,
                ParentWorkspace = new Workspace(),
                ModelConfigGuid = Guid.NewGuid(),
                DataSplitRatio = 0.7f,
                Name = "Test",
                DateForecasting = new DateTime(2025, 1, 1),
                DateTimeLevel = DateTimeLevel.STANDARD,
                ModelType = ModelType.FORECASTING,
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                ModelParameters = CreateParameters(),
                FeatureColumns = [new() { DataColumnsId = 1, Name = "Feature1", DataType = "Float", ParentConfigurationId = 1001 }, new() { DataColumnsId = 2, Name = "Feature2", DataType = "Float", ParentConfigurationId = 1001 }],
                TargetColumns = [new() { DataColumnsId = 1, Name = "Target1", DataType = "Float", ParentConfigurationId = 1001 }, new() { DataColumnsId = 2, Name = "Target2", DataType = "Float", ParentConfigurationId = 1001 }]
            };
        }

        private static ModelParameters CreateParameters()
        {
            return new LinearRegressionParameters()
            {
                Id = 1,
                NJobs = 1,
                TypeOfAlgorithm = "Lineare algorithme",
                ParentConfigurationId = 1001
            };
        }

        private static ModelConfigDTO CreateDummyDTO()
        {
            var algorithmDTO = new LinearRegressionDTO()
            {
                Id = 1,
                NJobs = 1,
                TypeOfAlgorithm = typeof(LinearRegressionDTO).Name,
            };

            return new ModelConfigDTO()
            {
                ParentWorkspaceGuid = new Guid("17321786-7d89-49d0-b0e1-bac9dc05383d"),
                ModelName = "TestDTO",
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                ForecastingDate = new DateTime(2025, 1, 1),
                DateTimeLevel = DateTimeLevel.STANDARD,
                ModelType = ModelType.FORECASTING,
                Features = [new() { ColumnName = "Feature1", DataType = "Float" }],
                Targets = [new() { ColumnName = "Target1", DataType = "Float" }],
                AlgorithmParameterDTO = algorithmDTO,
            };
        }

        private static Workspace CreateWorkspace()
        {
            return new Workspace()
            {
                WorkspaceGuid = new Guid("17321786-7d89-49d0-b0e1-bac9dc05383d")
            };
        }

        // Code: A-21a
        [Fact]
        public void TestIfNonExistentAlgorithmReturnsError()
        {
            ModelConfiguration config = CreateDomain();
            config.ModelAlgorithm = ModelAlgorithm.NONE;
            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = modelConfigBuilder.BuildDTO(config);
            });
        }

        // Code: A-21b
        [Fact]
        public void TestIfNonExistentAlgorithmDTOReturnsError()
        {
            // Arrange 
            ModelConfigDTO dTO = CreateDummyDTO();
            Workspace workspace = CreateWorkspace();
            dTO.ModelAlgorithm = ModelAlgorithm.NONE;

            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = modelConfigBuilder.BuildCompleteDomain(dTO, workspace);
            });
        }

        // Code: A-21c
        [Fact]
        public void TestCorrectMappingToDTO()
        {
            // Arrange
            ModelConfiguration config = CreateDomain();

            // Act
            ModelConfigDTO result = modelConfigBuilder.BuildDTO(config);

            // Assert
            Assert.IsType<LinearRegressionDTO>(result.AlgorithmParameterDTO);
            Assert.Equal(result.DataSplitRatio, config.DataSplitRatio);
            Assert.Equal(result.ModelName, config.Name);
            Assert.Equal(result.ModelAlgorithm, config.ModelAlgorithm);
            Assert.Equal(result.DateTimeLevel, config.DateTimeLevel);
            Assert.Equal(result.ModelConfigGuid, config.ModelConfigGuid);
            Assert.Equal(result.Targets.Count(), config.TargetColumns.Count);
            Assert.Equal(result.Features.Count(), config.FeatureColumns.Count);
        }

        // Code: A-21d
        [Fact]
        public void TestCorrectMappingToDomain()
        {
            // Arrange
            ModelConfigDTO config = CreateDummyDTO();
            Workspace workspace = CreateWorkspace();
            // Act
            ModelConfiguration result = modelConfigBuilder.BuildCompleteDomain(config, workspace);

            // Assert
            Assert.IsType<LinearRegressionParameters>(result.ModelParameters);
            Assert.Equal(result.DataSplitRatio, config.DataSplitRatio);
            Assert.Equal(result.Name, config.ModelName);
            Assert.Equal(result.ModelAlgorithm, config.ModelAlgorithm);
            Assert.Equal(result.DateTimeLevel, config.DateTimeLevel);
            Assert.Equal(result.ModelConfigGuid, config.ModelConfigGuid);
            Assert.Equal(result.TargetColumns.Count, config.Targets.Count());
            Assert.Equal(result.FeatureColumns.Count, config.Features.Count());
        }
    }
}
