from Domain import CalculationStep, CalculationType, ModelRequest
from services.AbstractServices import ICalculationBuilder, ICalculationHandler

from ..DataPreprocessing.CalculationProcessing.CalculationRunner import CalculationRunner
from ..DataPreprocessing.CalculationProcessing.KPIHandler import KPIHandler


class CalculationBuilder(ICalculationBuilder):

    calculation_runner: CalculationRunner

    def __init__(self):
        self.reset()

    def reset(self):
        self.calculation_runner = CalculationRunner()
        return self
    
    def add_calculations(self, model_request: ModelRequest):
        """
        Add either Dynamic KPI handler or Aggregation Handler
        """
        for step in model_request.Operations:
            if (step.CalculationType == CalculationType.KPI):
                self._add_dynamic_kpi(step)
            else:
                continue
        return self
    
    def _add_dynamic_kpi(self, calculation_step: CalculationStep):
        calculation = KPIHandler(kpi_step=calculation_step)
        self.calculation_runner.add_operation(calculation)
        return self
    
    # Extended with aggregegations and other kinds of calculations.
    
    def build(self) -> ICalculationHandler:
        """
        Returns an build CalculationRunner
        """
        return self.calculation_runner