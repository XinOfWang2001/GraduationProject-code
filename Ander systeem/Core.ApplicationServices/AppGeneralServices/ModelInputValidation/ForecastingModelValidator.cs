using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.Strategies;

namespace Leap.ApplicationServices.AppGeneralServices.ModelInputValidation
{
    public class ForecastingModelValidator : IInputValidatorStrategy<ModelConfigDTO>
    {
        public string ErrorMessage { get; set; } = string.Empty;

        public string GetErrorMessage()
        {
            return ErrorMessage;
        }

        public bool Validate(ModelConfigDTO input)
        {
            return AtLeastOneTarget(input);
        }

        private bool AtLeastOneTarget(ModelConfigDTO input)
        {
            bool result = input.Targets.Any();
            if (result)
            {
                return result;
            }
            ErrorMessage = "When training a forecasting model, at least one target variable should be selected";
            return false;
        }
    }
}
