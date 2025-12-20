using Leap.ApplicationServices.Interfaces.CalculationValidation;
using Leap.Domain.Domain.Calculations;

namespace Leap.ApplicationServices.AppGeneralServices.CalculationValidators
{
    /// <summary>
    /// Validate if at least one calculation is present
    /// </summary>
    public class CalculationInputValidator : ICalculationComponent
    {
        private const string InputError = "Amount of calculations should be at least 1";
        private string ErrorMessage { get; set; } = string.Empty;
        public bool Parse(IEnumerable<CalculationStep> Steps)
        {
            if (!Steps.All(Validate))
            {
                ErrorMessage = InputError;
                return false;
            }
            return true;
        }

        public string ReturnError()
        {
            return ErrorMessage;
        }

        public bool Validate(CalculationStep step)
        {
            return step.Calculations.Count != 0;
        }
    }
}
