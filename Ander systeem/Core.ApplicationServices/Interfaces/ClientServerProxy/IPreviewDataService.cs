using Leap.ApplicationServices.DTO.DataResult;

namespace Leap.ApplicationServices.Interfaces.ClientServerProxy
{
    public interface IPreviewDataService
    {
        // Will try to request to get preview data from the Python service. ProvideData is an optional parameter.
        public Task<PreviewDataDTO?> GetPreviewData(Guid workspaceGuid, bool ProvideData = false);
        // Will try to generate forecasting data, based on the used model, workspace and specific configuration set by the user.
        // Will try to get outlier detection data, based on the used model, workspace and specific configuration set by the user.
    }
}
