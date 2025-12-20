using Leap.Domain.Domain.ModelConfig.Enums;

namespace Leap.ApplicationServices.DTO.ModelingProcess
{
    /// <summary>
    /// Functions as the request body for persisting
    /// </summary>
    public class ModelStorageCreationRequestDTO
    {
        public required Guid WorkspaceGuid { get; set; }
        public required Guid DataExtractConfigGuid { get; set; }
        public required Guid ModelConfigGuid { get; set; }
        public bool Overwrite { get; set; } = false;
    }


    /// <summary>
    /// Functions as the response dto after modelstorage was succesfull.
    /// 
    /// Contents:
    /// - WorkspaceId
    /// - Unique address of model location
    /// - Name of the model
    /// - Date of creation
    /// - Version
    /// </summary>
    public class ModelStorageDTO : IDTO
    {
        public required Guid WorkspaceGuid { get; set; }
        public required string ModelAddress { get; set; }
        public required string ModelName { get; set; }
        public required string ModelVersion { get; set; } = "latest";
        public required ModelType ModelType { get; set; }
        public required ModelAlgorithm ModelAlgorithm { get; set; }
        public required DateTime DateOfCreation { get; set; }
    }
}
