using Infra.Data.DatabaseContext;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.ModelStorage;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.ImplRepositories
{
    public class ModelStorageRepository : IModelStorageRepository
    {
        private readonly LeapDSDBContext leapDSDBContext;

        public ModelStorageRepository(LeapDSDBContext leapDSDBContext)
        {
            this.leapDSDBContext = leapDSDBContext;
        }
        public async Task<ModelStorageAdress> Create(ModelStorageAdress modelStorageAdress)
        {
            await leapDSDBContext.ModelLocation.AddAsync(modelStorageAdress);
            await leapDSDBContext.SaveChangesAsync();
            return modelStorageAdress;
        }

        public async Task<ModelStorageAdress?> GetByWorkspace(Guid WorkspaceGuid)
        {
            return await leapDSDBContext.ModelLocation
                .Where(ml => ml.ParentWorkspace.WorkspaceGuid.Equals(WorkspaceGuid))
                .FirstOrDefaultAsync();
        }

        public async Task<ModelStorageAdress> Update(Guid WorkspaceGuid, ModelStorageAdress modelStorageAdress)
        {
            ModelStorageAdress CurrentEntity = await GetByWorkspace(WorkspaceGuid);
            CurrentEntity.UpdateEntity(modelStorageAdress);
            leapDSDBContext.ModelLocation.Update(CurrentEntity);
            await leapDSDBContext.SaveChangesAsync();
            return modelStorageAdress;
        }
    }
}
