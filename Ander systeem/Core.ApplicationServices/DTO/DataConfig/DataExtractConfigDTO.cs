using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.DTO.External_Services;

namespace Leap.ApplicationServices.DTO.DataConfig
{

    // This class will be used to configure the data.
    // This is the main DTO class for Data source configuration.
    // This form will be used to create and edit
    public class DataExtractConfigDTO : DataProcessDTO
    {
        public DataSourceDTO? DataSource { get; set; }
        public IEnumerable<SensorDTO> SensorsSelected { get; set; } = [];
        public IEnumerable<ValueTypeDTO> ValueTypesSelected { get; set; } = [];
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } = DateTime.Now;
        public ProjectSourceDTO? ProjectDTO { get; set; }
        public TimeLevelDTO? TimeLevelDTO { get; set; }
        public string PeriodName { get; set; } = "PD#day";
        public int AmountOfData { get; set; } = -1;
    }

    // These are the selected valuetypes
    public class ValueTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public override int GetHashCode()
        {
            return $"{Id}".GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is ValueTypeDTO vt && Equals(vt);
        }

        public bool Equals(ValueTypeDTO? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    // Selected MonitorObservationData
    public class SensorDTO
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return $"{Id}".GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is SensorDTO vt && Equals(vt);
        }

        public bool Equals(SensorDTO? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }
    }
}
