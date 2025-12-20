using Leap.ApplicationServices.DTO;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.ApplicationServices.Interfaces.Strategies;
using Leap.Domain.Domain.ModelConfig;
using Leap.Domain.Domain.ModelConfig.ModelParams;
using Leap.Domain.Domain.Workspaces;

namespace LeapDataScienceAPI.Services.BuilderAndMappers.ModelBuilders
{
    public class ModelConfigBuilder(IServiceProvider serviceProvider) : IModelConfigBuilder
    {
        private readonly IServiceProvider serviceProvider = serviceProvider;

        public ModelConfiguration BuildBase(ModelConfigDTO dto, Workspace workspace)
        {
            return new ModelConfiguration()
            {
                ParentWorkspace = workspace,
                ModelConfigGuid = dto.ModelConfigGuid,
                DataSplitRatio = dto.DataSplitRatio,
                Name = dto.ModelName,
                DateForecasting = dto.ForecastingDate.GetValueOrDefault(),
                DateTimeLevel = dto.DateTimeLevel,
                ModelType = dto.ModelType,
                ModelAlgorithm = dto.ModelAlgorithm,
                ModelParameters = BuildAlgorithm(dto)
            };
        }


        public ModelConfiguration BuildCompleteDomain(ModelConfigDTO dto, Workspace workspace)
        {
            var configEntity = BuildBase(dto, workspace);
            configEntity.FeatureColumns = BuildFeatureColumns(dto);
            configEntity.TargetColumns = BuildTargetColumns(dto);
            return configEntity;
        }

        public ModelConfigDTO BuildDTO(ModelConfiguration modelConfig)
        {
            return new ModelConfigDTO()
            {
                ParentWorkspaceGuid = modelConfig.ParentWorkspace.WorkspaceGuid,
                ModelConfigGuid = modelConfig.ModelConfigGuid,
                ModelName = modelConfig.Name,
                ModelAlgorithm = modelConfig.ModelAlgorithm,
                ModelType = modelConfig.ModelType,
                ForecastingDate = modelConfig.DateForecasting,
                DataSplitRatio = modelConfig.DataSplitRatio,
                DateTimeLevel = modelConfig.DateTimeLevel,
                AlgorithmParameterDTO = BuildAlgorithmDTO(modelConfig),
                Targets = modelConfig.TargetColumns.Select(t => new DataColumnDTO() { Id = t.DataColumnsId, ColumnName = t.Name, DataType = t.DataType }),
                Features = modelConfig.FeatureColumns.Select(t => new DataColumnDTO() { Id = t.DataColumnsId, ColumnName = t.Name, DataType = t.DataType })
            };
        }

        private static ICollection<FeatureColumns> BuildFeatureColumns(ModelConfigDTO dto)
        {
            return [.. dto.Features.Select(f => new FeatureColumns() { Name = f.ColumnName, DataType = f.DataType, })];
        }

        private static ICollection<TargetColumns> BuildTargetColumns(ModelConfigDTO dto)
        {
            return [.. dto.Targets.Select(f => new TargetColumns() { Name = f.ColumnName, DataType = f.DataType })];
        }

        private ModelParameters BuildAlgorithm(ModelConfigDTO configDTO)
        {
            var service = serviceProvider.GetRequiredKeyedService<IAlgorithmCreationStrategy>(configDTO.ModelAlgorithm);
            return service.BuildModelStrategy(configDTO.AlgorithmParameterDTO);
        }

        private AlgorithmDTO BuildAlgorithmDTO(ModelConfiguration modelConfig)
        {
            var service = serviceProvider.GetRequiredKeyedService<IAlgorithmCreationStrategy>(modelConfig.ModelAlgorithm);
            return service.BuildAlgorithmDTO(modelConfig.ModelParameters);
        }

    }
}
