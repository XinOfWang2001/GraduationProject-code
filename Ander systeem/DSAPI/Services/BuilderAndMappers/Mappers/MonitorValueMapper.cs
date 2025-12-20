using Leap.ApplicationServices.DTO.DataConfig;
using Leap.Domain.Domain.DataConfig;

namespace LeapDataScienceAPI.Services.BuilderAndMappers.Mappers
{
    public static class MonitorValueMapper
    {
        public static SensorObject MapToDomain(this SensorDTO dTO, int ProjectId)
        {
            return new()
            {
                SensorId = dTO.Id,
                SensorName = dTO.Name,
                ProjectId = ProjectId
            };
        }

        public static ValueTypes MapToDomain(this ValueTypeDTO dTO, int ProjectId)
        {
            return new()
            {
                ValueTypeId = dTO.Id,
                ValueTypeName = dTO.Name,
                ProjectId = ProjectId
            };
        }

        public static ValueTypeDTO MapToDTO(this ValueTypes valueTypes)
        {
            return new()
            {
                Id = valueTypes.ValueTypeId,
                Name = valueTypes.ValueTypeName,
            };
        }

        public static SensorDTO MapToDTO(this SensorObject sensor)
        {
            return new()
            {
                Id = sensor.SensorId,
                Name = sensor.SensorName,
            };
        }
    }
}
