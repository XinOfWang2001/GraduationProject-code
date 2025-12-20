namespace Leap.ApplicationServices.DTO.DataProcessDTO
{
    // Only being used to display choices to the user. Will be used along side the datasourceConfig.
    // The UI will create the config files
    public class DataSourceDTO : IDTO
    {
        public int DataSourceId { get; set; }
        public Guid DataSourceGuidId { get; set; }
        public string? DataSourceUrl { get; set; }
        public string? Name { get; set; }

        public List<ProjectSourceDTO> projectSourceDTOs { get; set; } = [];

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProjectSourceDTO
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string? Name { set; get; }
        public string? HumanReadableName { set; get; }

        public override string ToString()
        {
            return HumanReadableName;
        }
    }
}
