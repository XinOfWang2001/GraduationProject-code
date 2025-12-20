using Leap.ApplicationServices.Interfaces.CalculationValidation;
using Leap.Domain.Domain.Calculations;

namespace Leap.ApplicationServices.AppGeneralServices.CalculationValidators
{
    public class KPIParser : ICalculationComponent
    {
        private HashSet<string> _valid_operators = new HashSet<string>() { "+", "-", "*", "/", "" };
        private const string OperatorError = "An calculation should not start or end with a operator.";
        private const string DoubleColsOrOperatorError = "No two operators or columns after eachother.";
        private string Error { get; set; } = string.Empty;

        public bool Parse(IEnumerable<CalculationStep> Steps)
        {
            return Steps.All(Validate);
        }

        public bool Validate(CalculationStep step)
        {
            // Cast calculations to KPI
            return step.Calculations.Select(kpi => (DynamicKPI)kpi).All(ValidateKPI);
        }

        private bool ValidateKPI(DynamicKPI dynamicKPI)
        {
            // Does not start and end with an operator
            if (StartsOrEndsWithOperator(dynamicKPI.GetCalculationArray()))
            {
                Error = OperatorError;
                return false;
            }
            return NoTwoColsOrOperatorBesides(dynamicKPI.GetCalculationArray());
        }

        private bool StartsOrEndsWithOperator(IEnumerable<string> operators)
        {
            return _valid_operators.Contains(operators.First()) || _valid_operators.Contains(operators.Last());
        }

        private bool NoTwoColsOrOperatorBesides(IEnumerable<string> operatorList)
        {
            bool prevIsOperator = true;
            return operatorList.All((item) =>
            {
                bool currIsOperator = _valid_operators.Contains(item);
                if (IsNotSameOperator(prevIsOperator, currIsOperator))
                {
                    prevIsOperator = currIsOperator;
                    return true;
                }
                Error = DoubleColsOrOperatorError;
                return false;
            });
        }

        private static bool IsNotSameOperator(bool prevOperator, bool currentOperator)
        {
            return prevOperator != currentOperator;
        }

        public string ReturnError()
        {
            return Error;
        }
    }
}
