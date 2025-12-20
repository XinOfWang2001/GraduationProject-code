using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.Domain.Domain.ModelConfig.ModelParams;

namespace Leap.ApplicationServices.Interfaces.Strategies
{
    public interface IAlgorithmCreationStrategy
    {
        ModelParameters BuildModelStrategy(AlgorithmDTO algorithmDTO);
        AlgorithmDTO BuildAlgorithmDTO(ModelParameters modelParameters);
        AlgorithmDTO BuildAlgorithmDTO();
    }
}
