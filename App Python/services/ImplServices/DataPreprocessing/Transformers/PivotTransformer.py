import polars as pl
from polars import DataFrame


class DataTransformer:
    def transform_into_pivot_table(self, data: DataFrame):
        return data.with_columns(
            pl.format("{}_{}_{}", "sensor", "valuetype", "unit").alias("on")
        ).pivot(on="on", index=["timestamp"], values="value")
