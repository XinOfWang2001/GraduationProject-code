using Infra.Data.DatabaseContext;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.ModelConfig;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.ImplRepositories
{
    public class ModelConfigRepository : IModelConfigRepository
    {
        private readonly LeapDSDBContext leapDSDBContext;

        public ModelConfigRepository(LeapDSDBContext leapDSDBContext)
        {
            this.leapDSDBContext = leapDSDBContext;
        }

        public async Task<ModelConfiguration?> GetOne(Guid guid)
        {
            try
            {
                var result = leapDSDBContext.ModelConfigurations
                .Include(config => config.FeatureColumns)
                .Include(config => config.TargetColumns)
                .Include(config => config.ModelParameters)
                .Where(config => config.ModelConfigGuid.Equals(guid));

                return await result.FirstAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
                return null;
            }
        }
        public async Task<ModelConfiguration> Create(ModelConfiguration configuration)
        {
            leapDSDBContext.ModelConfigurations.Add(configuration);
            await leapDSDBContext.SaveChangesAsync();
            return configuration;
        }

        public async Task<ModelConfiguration?> Update(Guid configGuid, ModelConfiguration configuration)
        {
            try
            {
                var existingConfig = await GetOne(configuration.ModelConfigGuid);
                if (existingConfig != null)
                {
                    // Delete old relations.
                    configuration = await HandleDataColumns(existingConfig, configuration);
                    existingConfig.UpdateEntity(configuration);
                    // Perform update
                    leapDSDBContext.ModelConfigurations.Update(existingConfig);
                    await leapDSDBContext.SaveChangesAsync();
                }
                return existingConfig;
            }
            catch
            {
                throw;
            }

        }

        // Will remove the 
        private async Task<ModelConfiguration> HandleDataColumns(ModelConfiguration existingConfig, ModelConfiguration configuration)
        {
            // Remove old relation
            var features = existingConfig.FeatureColumns;
            var targets = existingConfig.TargetColumns;

            leapDSDBContext.TargetColumns.RemoveRange(targets);
            leapDSDBContext.FeatureColumns.RemoveRange(features);
            await leapDSDBContext.SaveChangesAsync();
            return configuration;
        }
    }
}
