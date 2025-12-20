using Microsoft.EntityFrameworkCore;

namespace Infra.Data.DatabaseContext
{
    public class DatabaseInitializer
    {
        private readonly LeapDSDBContext _dbContext;

        public DatabaseInitializer(LeapDSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InitializeAsync()
        {
            await _dbContext.Database.EnsureCreatedAsync();
        }

        public async Task ApplyMigrations()
        {
            await _dbContext.Database.MigrateAsync();
        }
    }
}
