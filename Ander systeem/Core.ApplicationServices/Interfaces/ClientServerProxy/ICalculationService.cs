using Leap.ApplicationServices.DTO.Calculations;

namespace Leap.ApplicationServices.Interfaces.ClientServerProxy
{
    public interface ICalculationService
    {
        public Task<CalculationRequestDTO> GetCalculations(Guid WorkspaceGuid);
        public Task<CalculationWriteDTO> OverwriteCalculations(CalculationWriteDTO calculationRequest);
    }
}
