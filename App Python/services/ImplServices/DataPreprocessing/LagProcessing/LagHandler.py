import polars as pl
from polars import DataFrame

from const import LAG_FORMAT
from services.AbstractServices import ILagHandler

# NOTE: Might be possibly suited as calculation.
class LagHandler(ILagHandler):
    """
    This class will perform a lag operation on the data.
    It will shift per column COLS_DEPTH.
    Based on COLS_WIDTH, it will create additional columns.

    NOTE: COLS_WIDTH  should be immutable,
    because models require that sorted in a certain order in order for them to work.
    A change in one of these values at one point, breaks all of the other models.
    Create another class implementing ILagHandler interface to change implementation.
    """

    COLS_WIDTH: int = 2

    def execute_change(self, df: DataFrame, input_columns: list[str], cols_depth=2):
        if len(df) <= cols_depth:
            raise ValueError("Cannot apply lag to an dataset lower then 1")

        if len(input_columns) == 0:
            return df

        for cols in input_columns:
            for cols_layer in range(1, 3):
                df = df.with_columns(
                    [
                        (pl.col(cols).shift(cols_layer)).alias(
                            LAG_FORMAT.format(col=cols, index=cols_layer)
                        )
                    ]
                )
        return df
