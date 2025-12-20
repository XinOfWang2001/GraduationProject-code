from polars import DataFrame

from services.AbstractServices import ICalculationHandler


class CalculationRunner(ICalculationHandler):
    operations: list[ICalculationHandler] = []
    created_columns: list[str]
    existing_columns: list[str] # Columns retrieved from WEBAPI

    def __init__(self):
        self.operations = []
        super().__init__()

    def add_operation(self, operation: ICalculationHandler):
        self.operations.append(operation)

    def get_operation_list(self):
        return self.operations

    def execute(self, data: DataFrame):
        for ops in self.operations:
            """
            Will perform all of the operations provided to the class
            Dependent of how the Calculation Runner is built.
            """
            data = ops.execute(data)
        return data