using Leap.ApplicationServices.DTO.Calculations;
using Leap.Domain.Domain.Calculations;
using Leap.Domain.Domain.Workspaces;

namespace LeapDataScienceAPI.Services.BuilderAndMappers.Mappers
{
    public static class CalculationMapper
    {
        public static IEnumerable<CalculationStepDTO> MapToDTO(this IEnumerable<CalculationStep> steps)
        {
            return steps.Select(step => step.MapToDTO());
        }
        public static CalculationStepDTO MapToDTO(this CalculationStep step)
        {
            return new CalculationStepDTO() { CalculationType = step.CalculationType, StepId = step.CalculationStepId, Order = step.Order, Calculations = step.Calculations.Select(calcs => calcs.MapToDTO()) };
        }

        public static CalculationDTO MapToDTO(this Calculation step)
        {
            if (step.GetType() == typeof(DynamicKPI))
            {
                var kpi = (DynamicKPI)step;
                return new KPIDTO() { CalculationId = kpi.CalculationId, CalculationString = kpi.CalculationString, OperationsList = kpi.GetCalculationArray(), InputColumns = kpi.GetInputColumns(), OutputColumn = kpi.OutputColumn };
            }
            throw new NotImplementedException();
        }

        public static IEnumerable<CalculationStep> MapToDomain(this IEnumerable<CalculationStepDTO> steps, Workspace workspace)
        {
            return steps.Select(dto => dto.MapToDomain(workspace));
        }

        public static CalculationStep MapToDomain(this CalculationStepDTO steps, Workspace workspace)
        {
            var step = new CalculationStep() { CalculationType = steps.CalculationType, Order = steps.Order, Workspace = workspace, CalculationStepId = steps.StepId };
            var calculations = steps.Calculations.Select(calculation => calculation.MapToCalculation(step)).ToList();
            step.Calculations = calculations;
            return step;
        }

        public static Calculation MapToCalculation(this CalculationDTO calculationDTO, CalculationStep step)
        {

            if (calculationDTO.GetType() == typeof(KPIDTO))
            {
                KPIDTO kPIDTO = (KPIDTO)calculationDTO;
                var operationsList = ConvertOperationsStringToList(kPIDTO.OperationsList);
                return new DynamicKPI()
                {
                    InputColumns = ConvertOperationsStringToList(kPIDTO.InputColumns),
                    OutputColumn = kPIDTO.OutputColumn,
                    CalculationStep = step,
                    CalculationString = kPIDTO.CalculationString,
                    ConcatCalculationString = operationsList
                };
            }
            throw new NotImplementedException();
        }

        private static string ConvertOperationsStringToList(IEnumerable<string> operations)
        {
            return string.Join(",", operations);
        }
    }
}
