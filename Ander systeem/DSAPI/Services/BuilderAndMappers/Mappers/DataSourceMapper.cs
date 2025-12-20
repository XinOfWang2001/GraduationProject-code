using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.Domain.Domain.DataSource;

namespace LeapDataScienceAPI.Services.BuilderAndMappers.Mappers
{
    public static class DataSourceMapper
    {
        public static DataSourceDTO MapToDTO(this SwecoDataSource dataSource)
        {
            return new DataSourceDTO()
            {
                DataSourceGuidId = dataSource.DataSourceGUIDId,
                DataSourceId = dataSource.DataSourceId,
                DataSourceUrl = dataSource.BaseUrl,
                Name = dataSource.SourceName,
                projectSourceDTOs = [.. dataSource.Projects.Select(proj => proj.MapToDTO())]
            };
        }

        public static ProjectSourceDTO MapToDTO(this Project project)
        {

            return new ProjectSourceDTO()
            {
                Id = project.Id,
                Guid = project.ProjectGuid,
                Name = project.Name,
                HumanReadableName = project.HumanReadableName
            };
        }
    }
}
