using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.Strategies;
using Leap.Domain.Domain.ModelConfig.Enums;

namespace Leap.ApplicationServices.AppGeneralServices.ModelInputValidation
{
    public class LinearRegressionInputValidator : IInputValidatorStrategy<ModelConfigDTO>
    {
        public string GetErrorMessage()
        {
            return "Linear regression requires a valid set linear regression algorithm parameter.";
        }

        public bool Validate(ModelConfigDTO input)
        {
            return AlgorithmAndDTOAreConsistent(input);
        }

        private bool AlgorithmAndDTOAreConsistent(ModelConfigDTO input)
        {
            return input.ModelAlgorithm.Equals(ModelAlgorithm.LINEAR_REGRESSION) && input.AlgorithmParameterDTO is LinearRegressionDTO;

        }
    }
}
