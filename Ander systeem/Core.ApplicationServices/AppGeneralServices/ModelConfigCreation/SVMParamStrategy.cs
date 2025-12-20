using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.Strategies;
using Leap.Domain.Domain.ModelConfig.ModelParams;

namespace Leap.ApplicationServices.AppGeneralServices.ModelConfigCreation
{
    public class SVMParamStrategy : IAlgorithmCreationStrategy
    {
        public AlgorithmDTO BuildAlgorithmDTO(ModelParameters modelParameters)
        {
            var casted = (SVMParameters)modelParameters;
            return new SVMDTO() { Id = modelParameters.Id, Kernel = casted.Kernel, TypeOfAlgorithm = casted.TypeOfAlgorithm };
        }

        public AlgorithmDTO BuildAlgorithmDTO()
        {
            return new SVMDTO();
        }

        public ModelParameters BuildModelStrategy(AlgorithmDTO algorithmDTO)
        {
            var casted = (SVMDTO)algorithmDTO;
            return new SVMParameters() { Id = casted.Id, Kernel = casted.Kernel };
        }
    }
}
