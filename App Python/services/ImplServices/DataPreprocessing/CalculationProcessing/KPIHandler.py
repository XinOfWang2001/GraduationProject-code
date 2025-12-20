import re
import logging
from polars import DataFrame, sql_expr

from Domain import CalculationStep, CalculationUnion
from services.AbstractServices import ICalculationHandler


class KPIHandler(ICalculationHandler):
    """
    This class is responsible for the creation of new columns
    based on provided string evaluations.
    """

    dynamic_kpi_step: CalculationStep

    def __init__(self, kpi_step: CalculationStep):
        self.dynamic_kpi_step = kpi_step

    def execute(self, data):
        for calc in self.dynamic_kpi_step.Operations:

            data = self._execute(data, calc)
            logging.info(data.head())
            logging.info(data.columns)
        return data

    def _correct_column(self, calculation_string: str):
        """
        This method is meant to correct the column within a string evaluation.
        Example: "temp-1 + 2" --> " `temp-1` + 2"

        The reason this method is needed, because sql_eval cannot parse a column containing a operator.
        It will only parse temp and not the "-1" part of the calculation, resulting in a failing calculation.
        """
        # Perhaps an less complex solution needs to be implemented.
        tokens = re.findall(r"[A-Za-z_][\w\-|+|*|/|]*", calculation_string)

        for token in tokens:
            if not re.fullmatch(r"\d+(\.\d+)?", token):
                calculation_string = re.sub(
                    rf"\b{re.escape(token)}\b", f"`{token}`", calculation_string
                )

        return calculation_string

    def _execute(self, data: DataFrame, calculation: CalculationUnion):
        input_string = self._correct_column(calculation.CalculationString)
        return data.with_columns(sql_expr(input_string).alias(calculation.OutputColumn))
