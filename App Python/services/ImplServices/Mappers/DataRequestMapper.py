import logging

from polars import DataFrame

from const import TIMESTAMP
from Domain import (DataColumnDTO, DataResponseDTO, DataSeries, MetricsDTO,
                    ModelLocation, ModelResultDTO, ModelStorageDTO,
                    ModelTrainingsResult, PredictionResult,
                    PredictionResultDTO)


class DTOResponseMapper:
    """Maps data from a Dataframe to proper DTO"""

    def map_to_data_response_dto(
        self, df: DataFrame, want_data=False
    ) -> DataResponseDTO:
        """
        Maps a dataframe to a proper DTO, usable for the client applications using this service.
        """
        logging.debug("Map data to Response dto")
        data = None
        data_columns: list[DataColumnDTO] = []
        # Map
        for col, dtype in zip(df.columns, df.dtypes):
            column_dtype = {"ColumnName": col, "DataType": dtype._string_repr()}
            data_columns.append(DataColumnDTO(**column_dtype))
        # Only provide data if want_data is set to true.
        if want_data:
            data = self._format_dataset(df)
        return DataResponseDTO(data_columns, len(df), data)

    def map_to_model_result(self, model_result: ModelTrainingsResult):
        # Original data should be formatted to be only select target variables.
        # Only for forecasting models.
        # For outlier detection models This action does not have to be performed.
        original_data = model_result.data
        forecast_data = model_result.get_predicted_data()
        data_columns: list[DataColumnDTO] = []
        metrics_dtos: list[MetricsDTO] = []
        metrics_dict: dict[str, list[MetricsDTO]] = {}
        for col, dtype in zip(original_data.columns, original_data.dtypes):
            data_columns.append(
                DataColumnDTO(**{"ColumnName": col, "DataType": dtype._string_repr()})
            )

        for metric in model_result.model_metrics:
            dto = MetricsDTO(
                **{
                    "Metric": metric.metric_name,
                    "Column": metric.column_name,
                    "Value": metric.value,
                }
            )
            a_list = metrics_dict.get(metric.metric_name, [])
            a_list.append(dto)
            metrics_dict[metric.metric_name] = a_list
            metrics_dtos.append(dto)
        forecast_series = self._format_dataset(forecast_data)
        original_series = self._format_dataset(original_data)
        return ModelResultDTO(
            data_columns,
            len(original_data) + len(forecast_data),
            original_series,
            forecast_series,
            metrics_dict,
        )

    def map_to_prediction_result(self, result: PredictionResult) -> PredictionResultDTO:
        """
        Maps PredictionResult --> PredictionResultDTO
        """
        original_data = result.data
        forecast_data = result.get_predicted_data()
        data_columns: list[DataColumnDTO] = []
        for col, dtype in zip(original_data.columns, original_data.dtypes):
            data_columns.append(
                DataColumnDTO(**{"ColumnName": col, "DataType": dtype._string_repr()})
            )
        predicted_series = self._format_dataset(forecast_data)
        original_series = self._format_dataset(original_data)
        return PredictionResultDTO(
            data_columns,
            len(original_data) + len(forecast_data),
            original_series,
            predicted_series,
        )

    def map_to_model_storage(
        self, model_location: ModelLocation, model_result: ModelTrainingsResult
    ) -> ModelStorageDTO:
        """
        Maps model location to an dto
        """
        return ModelStorageDTO(
            WorkspaceGuid=model_location.WorkspaceGuid,
            ModelName=model_location.ModelName,
            ModelAddress=model_location.ModelAddress,
            ModelVersion=model_location.ModelVersion,
            ModelAlgorithm=model_result.model_config.ModelAlgorithm,
            ModelType=model_result.model_config.ModelType,
        )

    def _format_dataset(self, original: DataFrame):
        # Within the dataset only select the predicted values.
        # Will select columns
        index = original.select([TIMESTAMP])
        column_names = [col for col in original.columns if col != TIMESTAMP]
        value_dicts = {}
        for col_name in column_names:
            value_dicts[col_name] = original[col_name].to_list()

        return DataSeries(
            Timestamps=index[TIMESTAMP].to_list(),
            ColumnNames=column_names,
            Values=value_dicts,
        )
