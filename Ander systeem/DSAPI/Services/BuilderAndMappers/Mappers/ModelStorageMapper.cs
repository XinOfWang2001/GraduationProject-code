using Leap.ApplicationServices.DTO.ModelingProcess;
using Leap.Domain.Domain.ModelStorage;
using Leap.Domain.Domain.Workspaces;

namespace LeapDataScienceAPI.Services.BuilderAndMappers.Mappers
{
    public static class ModelStorageMapper
    {
        public static ModelStorageAdress MapToDomain(this ModelStorageDTO DTO, Workspace parentWorkspace)
        {
            return new ModelStorageAdress()
            {
                ModelStorageAddress = DTO.ModelAddress,
                ModelStorageName = DTO.ModelName,
                ModelStorageVersion = DTO.ModelVersion,
                ParentWorkspace = parentWorkspace,
                ModelAlgorithm = DTO.ModelAlgorithm,
                ModelType = DTO.ModelType,
            };
        }

        public static ModelStorageDTO MapToDTO(this ModelStorageAdress domain)
        {
            return new ModelStorageDTO()
            {
                DateOfCreation = domain.CreationDate,
                ModelAddress = domain.ModelStorageAddress,
                ModelAlgorithm = domain.ModelAlgorithm,
                ModelType = domain.ModelType,
                ModelName = domain.ModelStorageName,
                ModelVersion = domain.ModelStorageVersion,
                WorkspaceGuid = domain.ParentWorkspace.WorkspaceGuid,
            };
        }
    }
}
