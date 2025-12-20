namespace Leap.Domain.Domain.DataSource
{
    // The assumption of this class is that the Datasource is a WebAPI. 
    public abstract class SwecoDataSource
    {
        public int DataSourceId { get; set; }
        public Guid DataSourceGUIDId { get; set; }
        public string? SourceName { get; set; }
        public string? BaseUrl { get; set; }
        // Only is used to determine which data source will be used to request.
        public string TypeOfSource { get; set; }

        public List<Project> Projects { get; set; } = [];
    }
}
