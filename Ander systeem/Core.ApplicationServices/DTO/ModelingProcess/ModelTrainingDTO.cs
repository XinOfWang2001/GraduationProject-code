using Leap.ApplicationServices.DTO.Calculations;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.ModelDTO;

namespace Leap.ApplicationServices.DTO.ModelingProcess
{
    // This is the DTO that contains data necessary for modelling. Used to send data to the FastAPI service
    public class ModelRequestDTO
    {
        public DateTime DateOfAction { get; init; } = DateTime.Now;
        public required DataRequestDTO DataRequest { get; set; }
        public required ModelConfigDTO ModelConfig { get; set; }
        public IEnumerable<CalculationStepDTO> OperationList { get; set; } = [];
        // At a later stage, this DTO will also include a collection of preprocessing dto's.
    }
}
