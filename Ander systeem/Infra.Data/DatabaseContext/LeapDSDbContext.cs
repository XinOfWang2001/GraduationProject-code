using Infra.Data.DataSeeder;
using Leap.Domain.Domain.Calculations;
using Leap.Domain.Domain.DataConfig;
using Leap.Domain.Domain.DataSource;
using Leap.Domain.Domain.ModelConfig;
using Leap.Domain.Domain.ModelConfig.ModelParams;
using Leap.Domain.Domain.ModelStorage;
using Leap.Domain.Domain.Workspaces;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.DatabaseContext
{
    // dotnet ef migrations add <Comment> --project Infra.Data --startup-project LeapDataScienceAPI
    public class LeapDSDBContext : DbContext
    {
        public DbSet<SensorObject> Sensor { get; set; }
        public DbSet<ValueTypes> ValueType { get; set; }
        public DbSet<DataExtracter> DataExtracter { get; set; }
        public DbSet<DataSourceConfig> DataSourceConfig { get; set; }
        public DbSet<IWADataSource> IWADataSources { get; set; }
        public DbSet<IoTDataSource> IoTDataSource { get; set; }
        public DbSet<SwecoDataSource> SwecoDataSources { get; set; }

        public DbSet<Project> Project { get; set; }
        public DbSet<Workspace> Workspace { get; set; }
        public DbSet<ModelConfiguration> ModelConfigurations { get; set; }
        public DbSet<ModelParameters> ModelParameters { get; set; }
        public DbSet<LinearRegressionParameters> LinearRegressionParameters { get; set; }
        public DbSet<SVMParameters> SVMParameters { get; set; }
        public DbSet<DataColumns> DataColumns { get; set; }
        public DbSet<FeatureColumns> FeatureColumns { get; set; }
        public DbSet<TargetColumns> TargetColumns { get; set; }
        public DbSet<ModelStorageAdress> ModelLocation { get; set; }

        public DbSet<CalculationStep> CalculationSteps { get; set; }
        public DbSet<Calculation> Calculations { get; set; }
        public DbSet<DynamicKPI> DynamicKPIs { get; set; }

        public LeapDSDBContext(DbContextOptions<LeapDSDBContext> options) : base(options)
        {
        }

        protected LeapDSDBContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sweco data sources
            modelBuilder.Entity<SwecoDataSource>(datasource =>
            {
                datasource
                .HasDiscriminator<string>("DataSourceType")
                .HasValue<SwecoDataSource>("SwecoWebSource")
                .HasValue<IWADataSource>("IwaDataSource")
                .HasValue<IoTDataSource>("IoTDataSource");
                datasource.HasKey("DataSourceId");

                datasource
                .HasMany(datasource => datasource.Projects)
                .WithOne(project => project.SwecoDataSource)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SensorObject>((sensor) =>
            {
                sensor.HasKey(["SensorId", "ProjectId"]);
                sensor
                .HasOne(obs => obs.Project)
                .WithMany(projects => projects.Observations)
                .HasForeignKey(obs => obs.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

                sensor.HasAlternateKey("SensorGuid");
                sensor.Property(s => s.SensorId)
                .ValueGeneratedNever();
            });

            modelBuilder.Entity<ValueTypes>((value) =>
            {
                value.HasKey(["ValueTypeId", "ProjectId"]);
                value.HasOne(vt => vt.Project)
                .WithMany(projects => projects.ValueTypes)
                .HasForeignKey(vt => vt.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
                value.Property(vt => vt.ValueTypeId)
                .ValueGeneratedNever();
            });

            modelBuilder.Entity<Workspace>((workspace) =>
            {
                workspace.HasKey("WorkspaceId");
                workspace.HasAlternateKey(ws => ws.WorkspaceGuid);
                //Workspace
                workspace
                .HasOne(workspace => workspace.DataExtraction)
                .WithOne(de => de.ParentWorkspace)
                .HasForeignKey<DataExtracter>(d => d.ParentWorkspaceId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

                workspace.HasOne(modelConfig => modelConfig.ModelConfig)
                .WithOne(mc => mc.ParentWorkspace)
                .HasForeignKey<ModelConfiguration>(mc => mc.ParentWorkspaceId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

                workspace.HasOne(modelStorage => modelStorage.ModelStorage)
                .WithOne(ms => ms.ParentWorkspace)
                .HasForeignKey<ModelStorageAdress>(ms => ms.ParentWorkspaceId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DataExtracter>((dataExtracter) =>
            {
                dataExtracter.HasKey(de => de.DataProcessId);
                dataExtracter.HasOne(d => d.ParentWorkspace)
                    .WithOne(w => w.DataExtraction)
                    .HasForeignKey<DataExtracter>(d => d.ParentWorkspaceId)
                    .IsRequired(false);

                dataExtracter
                .HasOne(d => d.DataSourceConfig)
                .WithOne(dsc => dsc.ParentExtracter)
                .HasForeignKey<DataSourceConfig>(d => d.ParentExtractId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Project>((project) =>
            {
                project.HasKey("Id");
                project
                .HasOne(project => project.SwecoDataSource)
                .WithMany(data => data.Projects)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DataSourceConfig>((DataSourceConfig) =>
            {
                DataSourceConfig.HasKey("ConfigId");
                DataSourceConfig
                .HasMany(configs => configs.Sensors)
                .WithMany(sensor => sensor.Configs);

                DataSourceConfig
                .HasMany(config => config.ValueTypes)
                .WithMany(sensor => sensor.Configs);

                DataSourceConfig
                .HasOne(config => config.AssignedProject)
                .WithMany(project => project.DataSourceConfigs)
                .OnDelete(DeleteBehavior.Cascade);

            });
            modelBuilder.Entity<ModelParameters>((parameters) =>
            {
                parameters
                .HasDiscriminator<string>("TypeOfAlgorithm")
                .HasValue<LinearRegressionParameters>("LinearRegression")
                .HasValue<SVMParameters>("SVM");

                parameters.HasOne(param => param.ParentConfiguration)
                .WithOne(mc => mc.ModelParameters)
                .HasForeignKey<ModelParameters>(param => param.ParentConfigurationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DataColumns>((columns) =>
            {
                columns.HasDiscriminator<string>("ColumnType")
                .HasValue<FeatureColumns>("FeatureColumns")
                .HasValue<TargetColumns>("TargetColumns");
            });

            modelBuilder.Entity<ModelConfiguration>((modelConfig) =>
            {
                modelConfig.HasKey(mc => mc.ModelConfigId);
                modelConfig.HasAlternateKey(mc => mc.ModelConfigGuid);
                // Feature columns
                modelConfig
                .HasMany(mc => mc.FeatureColumns)
                .WithOne(mp => mp.ParentConfiguration)
                .HasForeignKey(mp => mp.ParentConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);

                // Targets columns
                modelConfig
                .HasMany(mc => mc.TargetColumns)
                .WithOne(mp => mp.ParentConfiguration)
                .HasForeignKey(mp => mp.ParentConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);

                // One-on-one relation with Modelparameters.
                modelConfig
                .HasOne(mc => mc.ModelParameters)
                .WithOne(param => param.ParentConfiguration)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
                // One-on-one relation with Workspace
                modelConfig
                .HasOne(mc => mc.ParentWorkspace)
                .WithOne(workspace => workspace.ModelConfig)
                .HasForeignKey<ModelConfiguration>(mc => mc.ParentWorkspaceId)
                .IsRequired(false);
            });

            modelBuilder.Entity<ModelStorageAdress>((modelstorage) =>
            {
                modelstorage.HasKey(ms => ms.ModelStorageId);
                modelstorage
                .HasOne(ms => ms.ParentWorkspace)
                .WithOne(ws => ws.ModelStorage)
                .HasForeignKey<ModelStorageAdress>(ms => ms.ParentWorkspaceId)
                .IsRequired(false);
            });

            modelBuilder.Entity<CalculationStep>((steps) =>
            {
                steps
                .HasOne(step => step.Workspace)
                .WithMany(workspace => workspace.CalculationSteps)
                .HasForeignKey(step => step.WorkspaceGuid)
                .HasPrincipalKey(ws => ws.WorkspaceGuid);
            });

            modelBuilder.Entity<Calculation>((calculation) =>
            {
                calculation.HasDiscriminator<string>("CalculationType")
                .HasValue<DynamicKPI>("KPI");
                calculation.HasOne(calculation => calculation.CalculationStep)
                .WithMany(step => step.Calculations)
                .HasForeignKey(fk => fk.CalculationStepId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            FillData(modelBuilder);
            ConfigureEagerLoading(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureEagerLoading(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().Navigation(proj => proj.SwecoDataSource).AutoInclude();
            modelBuilder.Entity<ModelStorageAdress>().Navigation(ms => ms.ParentWorkspace).AutoInclude();
            modelBuilder.Entity<ModelConfiguration>().Navigation(mc => mc.ParentWorkspace).AutoInclude();
            modelBuilder.Entity<CalculationStep>().Navigation(step => step.Calculations).AutoInclude();
        }
        private void FillData(ModelBuilder modelBuilder)
        {
            // Projects and workspaces will be loaded in standard.
            modelBuilder.SeedStandardData();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
    }
}
