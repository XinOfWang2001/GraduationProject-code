using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.Strategies;
using Leap.Domain.Domain.ModelConfig.Enums;

namespace Leap.ApplicationServices.AppGeneralServices.ModelInputValidation
{
    public class SVMInputValidator : IInputValidatorStrategy<ModelConfigDTO>
    {
        public string GetErrorMessage()
        {
            return "SVM requires a valid set of SVM algorithm parameters.";
        }

        public bool Validate(ModelConfigDTO input)
        {
            return AlgorithmAndDTOAreConsistent(input);
        }

        private bool AlgorithmAndDTOAreConsistent(ModelConfigDTO input)
        {
            return input.ModelAlgorithm.Equals(ModelAlgorithm.SVMREGRESSION) && input.AlgorithmParameterDTO is SVMDTO;
        }
    }
}
