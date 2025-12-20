using Leap.Domain.Domain.ModelStorage;

namespace Leap.ApplicationServices.Interfaces.Repositories
{
    public interface IModelStorageRepository
    {
        //Task<ModelStorageAdress> GetByWorkspace(Guid WorkspaceGuid);
        Task<ModelStorageAdress> Create(ModelStorageAdress modelStorageAdress);

        Task<ModelStorageAdress> Update(Guid WorkspaceGuid, ModelStorageAdress modelStorageAdress);

        /// <summary>
        /// This will search modelstorage data based on its parentGuid.
        /// The assumption will be that this entity will be created, with an existing workspace
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <returns></returns>
        Task<ModelStorageAdress?> GetByWorkspace(Guid workspaceId);
    }
}
