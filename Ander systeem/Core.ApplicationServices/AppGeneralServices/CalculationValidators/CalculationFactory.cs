using Leap.ApplicationServices.Interfaces.CalculationValidation;

namespace Leap.ApplicationServices.AppGeneralServices.CalculationValidators
{
    // Responsible for creating a calculation composite
    public class CalculationFactory
    {

        public CalculationFactory() { }

        public ICalculationComponent CreateCalculationValidators()
        {
            CalculationComposite composite = new();
            CalculationInputValidator calculationInputValidator = new();
            KPIParser kPIParser = new();
            composite.Validators = [calculationInputValidator, kPIParser];
            return composite;
        }
    }
}
