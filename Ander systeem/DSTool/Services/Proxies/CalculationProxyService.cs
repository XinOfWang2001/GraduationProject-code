using Leap.ApplicationServices.DTO.Calculations;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;

namespace LeapDataScienceTool.Services.Proxies
{
    public class CalculationProxyService(IServerAPI serverAPI) : ICalculationService
    {
        private readonly IServerAPI serverAPI = serverAPI;

        public async Task<CalculationRequestDTO> GetCalculations(Guid WorkspaceGuid)
        {
            return await serverAPI.Get<CalculationRequestDTO>($"calculations/{WorkspaceGuid}");
        }

        public async Task<CalculationWriteDTO> OverwriteCalculations(CalculationWriteDTO calculationRequest)
        {
            return await serverAPI.Put<CalculationWriteDTO>("calculations", calculationRequest);
        }
    }
}
