using Leap.ApplicationServices.Interfaces.CalculationValidation;
using Leap.Domain.Domain.Calculations;

namespace Leap.ApplicationServices.AppGeneralServices.CalculationValidators
{
    public class CalculationComposite : ICalculationComponent
    {
        public IEnumerable<ICalculationComponent> Validators { get; set; } = new List<ICalculationComponent>();
        private HashSet<string> _columns = new HashSet<string>();
        private string Error { get; set; }

        public bool Parse(IEnumerable<CalculationStep> Steps)
        {
            return Validators.All((val) => val.Parse(Steps));
        }

        public bool Validate(CalculationStep step)
        {
            return true;
        }

        public string ReturnError()
        {
            return string.Join("\n", Validators.Select(v => v.ReturnError()));
        }
    }
}
