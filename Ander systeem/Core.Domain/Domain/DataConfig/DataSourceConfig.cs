using Leap.Domain.Domain.DataSource;

namespace Leap.Domain.Domain.DataConfig
{
    public class DataSourceConfig
    {
        public int ConfigId { get; set; }

        // The date range between two dates.
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required Project AssignedProject { get; set; }
        public int DataPoints { get; set; } = -1;

        public int? TimeLevel { get; set; } = -1; // TimeLevelNames represents the Id of a timelevel, of the webapipi API. Which is a number
        public string? TimelevelName { get; set; } = string.Empty;
        public float? TimelevelRange { get; set; } = 432000000000.0f;

        // Many-to-many relation to sensors.
        // Sensors and Config in-between --> DataSourceConfigSensorObject, possible be created for easier querying
        public List<SensorObject> Sensors { get; set; } = [];

        // Sensors and Config in-between --> DataSourceConfigValueTypes, possible be created for easier querying
        public List<ValueTypes> ValueTypes { get; set; } = [];
        public int ParentExtractId { get; set; }
        public DataExtracter ParentExtracter { get; set; }

        public void UpdateConfig(DataSourceConfig inputConfig)
        {
            StartDate = inputConfig.StartDate;
            EndDate = inputConfig.EndDate;
            DataPoints = inputConfig.DataPoints;
            TimeLevel = inputConfig.TimeLevel;
            TimelevelName = inputConfig.TimelevelName;
            TimelevelRange = inputConfig.TimelevelRange;
            AssignedProject = inputConfig.AssignedProject;
            Sensors = inputConfig.Sensors;
            ValueTypes = inputConfig.ValueTypes;
        }

        public string GetDataSourceName()
        {
            return AssignedProject.SwecoDataSource.SourceName;
        }

        public string GetProjectName()
        {
            return AssignedProject.Name;
        }

        public string GetProjectToken()
        {
            return AssignedProject.ProjectToken;
        }
        public List<int> GetObservationIds()
        {
            return [.. Sensors.Select(Sensors => Sensors.SensorId)];
        }

        public List<int> GetValueTypeIds()
        {
            return [.. ValueTypes.Select(ValueTypes => ValueTypes.ValueTypeId)];
        }

        public long GetUnixStartDate()
        {
            // Convert the StartDate to Unix timestamp in milliseconds
            long startUnix = GetUnix(StartDate);
            // Return the start date as Unix timestamp
            return startUnix;
        }

        public long GetUnixEndDate()
        {
            // Convert the StartDate to Unix timestamp in milliseconds
            long startUnix = GetUnix(EndDate);
            // Return the start date as Unix timestamp
            return startUnix;
        }

        public float? GetTimeRange()
        {
            if (DataPoints == -1)
            {
                return TimelevelRange;
            }
            else
            {
                // Return a default timelevelRange in Nanoseconds
                return 432000000000.0f;
            }
        }


        private static long GetUnix(DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeMilliseconds();
        }
    }
}
