using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.DTO.External_Services;
using Leap.Domain.Domain.DataConfig;
using Leap.Domain.Domain.DataSource;
using Leap.Domain.Domain.Workspaces;

namespace LeapDataScienceAPI.Services.BuilderAndMappers.Mappers
{
    public static class DataExtracterMapper
    {
        public static DataExtracter MapToDomain(this DataExtractConfigDTO dto,
            Project project,
            Workspace workspace)
        {
            return new DataExtracter()
            {
                ParentWorkspace = workspace,
                ProcessId = dto.ProcessId,
                DataSourceConfig = new DataSourceConfig()
                {
                    StartDate = dto.StartDate.GetValueOrDefault(),
                    EndDate = dto.EndDate.GetValueOrDefault(),
                    AssignedProject = project,
                    Sensors = [.. dto.SensorsSelected.Select(obs => obs.MapToDomain(project.Id))],
                    ValueTypes = [.. dto.ValueTypesSelected.Select(vt => vt.MapToDomain(project.Id))],
                    DataPoints = dto.AmountOfData,
                    TimeLevel = dto.TimeLevelDTO?.TimelevelId,
                    TimelevelName = dto.TimeLevelDTO?.TimelevelName,
                    TimelevelRange = dto.TimeLevelDTO?.TimelevelRange
                },
            };
        }

        public static DataExtractConfigDTO MapVTToDTO(this DataExtractConfigDTO dto, List<ValueTypes> VTs)
        {
            dto.ValueTypesSelected = VTs.Select(vt => vt.MapToDTO());
            return dto;
        }

        public static DataExtractConfigDTO MapObsToDTO(this DataExtractConfigDTO dto, List<SensorObject> obs)
        {
            dto.SensorsSelected = obs.Select(vt => vt.MapToDTO());
            return dto;
        }

        public static DataExtractConfigDTO MapToDTO(this DataExtracter dataExtracter)
        {
            DataSourceConfig sourceConfig = dataExtracter.DataSourceConfig;
            Project project = sourceConfig.AssignedProject;
            SwecoDataSource swecoDataSource = project.SwecoDataSource;
            DataSourceDTO dataSourceDto = swecoDataSource.MapToDTO();
            ProjectSourceDTO projectSourceDTO = project.MapToDTO();

            return new DataExtractConfigDTO()
            {
                // Algemene variabelen.
                WorkspaceId = dataExtracter.ParentWorkspace.WorkspaceGuid,
                ProcessId = dataExtracter.ProcessId,
                DataSource = dataSourceDto,
                StartDate = dataExtracter.DataSourceConfig.StartDate,
                EndDate = dataExtracter.DataSourceConfig.EndDate,
                TimeLevelDTO = new TimeLevelDTO()
                {
                    TimelevelId = dataExtracter.DataSourceConfig.TimeLevel ?? 0,
                    TimelevelName = dataExtracter.DataSourceConfig.TimelevelName,
                    TimelevelRange = dataExtracter.DataSourceConfig.TimelevelRange,
                },
                // Dit moet nog gefixed worden.
                ProjectDTO = projectSourceDTO,
                AmountOfData = sourceConfig.DataPoints,
                Message = "Succesvol gewijzigd.",
                StatusCode = 201
            }.MapObsToDTO(sourceConfig.Sensors).MapVTToDTO(sourceConfig.ValueTypes);
        }

        public static DataRequestDTO MapToDataRequestDTO(this DataExtracter dataExtracter, bool ProvideData)
        {
            DataSourceConfig dataSourceConfig = dataExtracter.DataSourceConfig;
            return new DataRequestDTO()
            {
                WorkspaceId = dataExtracter.ParentWorkspace.WorkspaceGuid.ToString(),
                Token = dataSourceConfig.GetProjectToken(),
                DataSource = dataSourceConfig.GetDataSourceName(),
                StartDateUnix = dataSourceConfig.GetUnixStartDate(),
                EndDateUnix = dataSourceConfig.GetUnixEndDate(),
                Project = dataSourceConfig.GetProjectName(),
                Points = dataSourceConfig.DataPoints,
                Timelevel = dataSourceConfig.TimeLevel,
                TimelevelRange = dataSourceConfig.GetTimeRange(),
                ObservationIds = dataSourceConfig.GetObservationIds(),
                ValueTypeIds = dataSourceConfig.GetValueTypeIds(),
                ProvideData = ProvideData
            };
        }
    }
}
