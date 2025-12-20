using Leap.Domain.Domain.Calculations;

namespace Leap.ApplicationServices.Interfaces.CalculationValidation
{
    public interface ICalculationComponent
    {
        public bool Parse(IEnumerable<CalculationStep> Steps);
        protected bool Validate(CalculationStep step);

        public string ReturnError();
    }
}
