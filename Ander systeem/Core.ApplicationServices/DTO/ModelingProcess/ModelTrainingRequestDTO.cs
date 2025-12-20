namespace Leap.ApplicationServices.DTO.ModelingProcess
{
    public class ModelTrainingRequestDTO
    {
        public required Guid WorkspaceGuid { get; set; }
        public required Guid DataExtractConfigGuid { get; set; }
        public required Guid ModelConfigGuid { get; set; }
    }
}
