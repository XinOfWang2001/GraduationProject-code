using Core.Domain.Domain.ModelConfig.Enums;
using Core.Domain.Domain.Workspaces;

namespace Leap.Domain.Domain.ModelStorage
{
    public class ModelStorageAdress
    {
        public int ModelStorageId { get; set; }

        // Will be formatted {workspaceid}_{modeltype}_latest.pkl
        public required string ModelStorageAddress { get; set; }
        public required string ModelStorageName { get; set; }
        public required string ModelStorageVersion { get; set; }
        public ModelType ModelType { get; set; }
        public ModelAlgorithm ModelAlgorithm { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        // One-on-One relation --> One Workspace -> One ModelStorage
        public required Workspace ParentWorkspace { get; set; }
        public int ParentWorkspaceId { get; set; }

        public void UpdateEntity(ModelStorageAdress modelStorageAdress)
        {
            ModelStorageAddress = modelStorageAdress.ModelStorageAddress;
            ModelStorageName = modelStorageAdress.ModelStorageName;
            ModelStorageVersion = modelStorageAdress.ModelStorageVersion;
            ModelType = modelStorageAdress.ModelType;
            ModelAlgorithm = modelStorageAdress.ModelAlgorithm;
            CreationDate = modelStorageAdress.CreationDate;
        }
    }
}
