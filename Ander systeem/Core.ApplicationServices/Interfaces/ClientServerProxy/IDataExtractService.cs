using Leap.ApplicationServices.DTO.DataConfig;

namespace Leap.ApplicationServices.Interfaces.ClientServerProxy
{
    public interface IDataExtractService
    {
        public Task<DataExtractConfigDTO?> RegisterDataExtractProcess(DataExtractConfigDTO config);
        public Task<DataExtractConfigDTO?> UpdateDataExtractProcess(Guid procesId, DataExtractConfigDTO config);
    }
}
