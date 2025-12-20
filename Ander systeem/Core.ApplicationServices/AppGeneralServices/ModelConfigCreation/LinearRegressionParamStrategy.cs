using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.Strategies;
using Leap.Domain.Domain.ModelConfig.ModelParams;

namespace Leap.ApplicationServices.AppGeneralServices.ModelConfigCreation
{
    public class LinearRegressionParamStrategy : IAlgorithmCreationStrategy
    {
        public AlgorithmDTO BuildAlgorithmDTO(ModelParameters modelParameters)
        {
            var algorithmDTO = (LinearRegressionParameters)modelParameters;
            return new LinearRegressionDTO() { Id = algorithmDTO.Id, NJobs = algorithmDTO.NJobs, TypeOfAlgorithm = algorithmDTO.TypeOfAlgorithm };
        }

        public AlgorithmDTO BuildAlgorithmDTO()
        {
            return new LinearRegressionDTO();
        }

        public ModelParameters BuildModelStrategy(AlgorithmDTO algorithmDTO)
        {
            var algorithm = (LinearRegressionDTO)algorithmDTO;
            return new LinearRegressionParameters() { Id = algorithmDTO.Id, NJobs = algorithm.NJobs };
        }
    }
}
