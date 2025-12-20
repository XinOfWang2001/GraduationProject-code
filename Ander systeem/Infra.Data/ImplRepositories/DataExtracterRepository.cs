using Infra.Data.DatabaseContext;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataConfig;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.ImplRepositories
{
    public class DataExtracterRepository : IDataExtractRepository
    {
        private readonly LeapDSDBContext dbContext;

        public DataExtracterRepository(LeapDSDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<DataExtracter?> Create(DataExtracter dataExtracter)
        {
            try
            {
                // Here logic
                dataExtracter = EditValueTypesOfConfig(dataExtracter);
                dataExtracter = EditSensorsOfConfig(dataExtracter);
                dbContext.DataExtracter.Add(dataExtracter);
                await dbContext.SaveChangesAsync();
                return dataExtracter;
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<DataExtracter> Get()
        {
            return dbContext.DataExtracter.ToList();
        }

        public DataExtracter? Get(Guid procesId)
        {
            return dbContext.DataExtracter
                .Include(x => x.ParentWorkspace)
                .Include(x => x.DataSourceConfig)
                .ThenInclude(config => config.ValueTypes)
                .Include(x => x.DataSourceConfig)
                .ThenInclude(config => config.Sensors)
                .Include(x => x.DataSourceConfig)
                .ThenInclude(config => config.AssignedProject)
                .ThenInclude(config => config.SwecoDataSource)
                .Include(x => x.DataSourceConfig)
                .Where(dataExtracter => dataExtracter.ProcessId == procesId)
                .FirstOrDefault();
        }

        public DataExtracter? GetByWorkspace(Guid workspaceGuid)
        {
            var query = dbContext.DataExtracter
                    .Include(x => x.ParentWorkspace)
                    .Include(x => x.DataSourceConfig)
                    .ThenInclude(config => config.AssignedProject)
                    .ThenInclude(proj => proj.SwecoDataSource)
                    .Include(x => x.DataSourceConfig)
                    .ThenInclude(config => config.Sensors)
                    .Include(x => x.DataSourceConfig)
                    .ThenInclude(config => config.ValueTypes)
                    .Include(x => x.DataSourceConfig)
                    .Where(x => x.ParentWorkspace.WorkspaceGuid == workspaceGuid);
            return query.FirstOrDefault();
        }

        public async Task<DataExtracter?> Update(Guid procesId, DataExtracter inputExtracter)
        {
            try
            {
                // Loop over all the sensors and valuetypes.
                // Change data
                DataExtracter currentExtracter = Get(procesId);
                UpdateConfiguration(currentExtracter, inputExtracter);
                currentExtracter = EditValueTypesOfConfig(currentExtracter);
                currentExtracter = EditSensorsOfConfig(currentExtracter);
                dbContext.DataExtracter.Update(inputExtracter);
                await dbContext.SaveChangesAsync();
                return inputExtracter;
            }
            catch
            {
                throw new DbUpdateException("Wijziging mislukt.");
            }
        }

        private DataExtracter EditSensorsOfConfig(DataExtracter extracter)
        {
            var config = extracter.DataSourceConfig;
            // Sort inserted valuetypes to seperate list.
            List<SensorObject> dataSensors = new(config.Sensors.Count);
            // Loop through each
            foreach (var sensor in config.Sensors)
            {
                // Here extra validation
                SensorObject? sensorValue = dbContext.Sensor.Find([sensor.SensorId, sensor.ProjectId]);
                // If entity does not exist. Create valuetype to database
                if (sensorValue == null)
                {
                    dbContext.Sensor.Add(sensor);
                    dataSensors.Add(sensor);
                }
                else
                {
                    dataSensors.Add(sensorValue);
                }
            }
            // Remove old combination
            config.Sensors.Clear();
            // Add new combination valueTypes to entity.
            config.Sensors.AddRange(dataSensors);
            return extracter;
        }

        private DataExtracter EditValueTypesOfConfig(DataExtracter extracter)
        {
            var config = extracter.DataSourceConfig;
            // Sort inserted valuetypes to seperate list.
            List<ValueTypes> databaseValueTypes = new(config.ValueTypes.Count);
            // Loop through each
            foreach (var dataVt in config.ValueTypes)
            {
                // Here extra validation
                ValueTypes? valueTypes = dbContext.ValueType.Find([dataVt.ValueTypeId, dataVt.ProjectId]);
                // If entity does not exist. Create valuetype to database
                if (valueTypes == null)
                {
                    dbContext.ValueType.Add(dataVt);
                    databaseValueTypes.Add(dataVt);
                }
                else
                {
                    databaseValueTypes.Add(valueTypes);
                }
            }
            // Remove old combination
            config.ValueTypes.Clear();
            // Add new combination valueTypes to entity.
            config.ValueTypes.AddRange(databaseValueTypes);
            return extracter;
        }

        private void UpdateConfiguration(DataExtracter currentExtracter, DataExtracter inputConfig)
        {
            currentExtracter.UpdateExtracter(inputConfig);
        }
    }
}
