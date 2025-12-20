from Domain import (KPIDTO, AlgorithmUnion, CalculationStep,
                    CalculationStepDTO, DataColumn, DataColumnDTO, DataRequest,
                    DataRequestDTO, DynamicKPI, LinearRegressionDTO,
                    LinearRegressionParam, ModelConfig, ModelConfigDTO,
                    ModelForecastRequestDTO, ModelPredictParams, ModelRequest,
                    ModelRequestDTO, ModelStorageCreationDTO,
                    ModelTimeForecastDTO, ParamUnion, SVMParam)

from ..Utils.UtilityFunctions import get_interval


class DTODomainMapper:

    def map_modeltraining_request_to_domain(self, dto: ModelRequestDTO):
        """
        ModelRequestDTO to ModelRequest
        """
        dataRequest = self.map_datarequest_dto_to_domain(dto.DataRequest)
        operations = self.map_operation_to_domain(dto.OperationList)
        model_config = self.map_modelconfig_dto_to_domain(dto.ModelConfig)
        return ModelRequest(
            DataRequest=dataRequest, ModelConfig=model_config, Operations=operations
        )

    def map_modelstorage_request_to_domain(self, dto: ModelStorageCreationDTO):
        """
        ModelStorageCreationDTO to ModelRequest
        """
        dataRequest = self.map_datarequest_dto_to_domain(dto.DataRequest)
        operations = self.map_operation_to_domain(dto.OperationList)
        model_config = self.map_modelconfig_dto_to_domain(dto.ModelConfig)
        return ModelRequest(
            DataRequest=dataRequest, ModelConfig=model_config, Operations=operations
        )

    def map_prediction_request_to_domain(self, dto: ModelForecastRequestDTO):
        """
        Converts ModelForecastRequestDTO to ModelRequest + ModelLocation & PredictionParameters
        """
        dataRequest = self.map_datarequest_dto_to_domain(dto.DataRequest)
        model_config = self.map_modelconfig_dto_to_domain(dto.ModelConfig)
        operations = self.map_operation_to_domain(dto.OperationList)
        model_predict_params = self.convert_modelforecast_forecast_params(
            dto.ModelPredictionParameters
        )
        return ModelRequest(
            DataRequest=dataRequest,
            ModelConfig=model_config,
            ModelLocation=dto.ModelStorageAddress,
            PredictionParameters=model_predict_params,
            Operations=operations,
        )

    def convert_modelforecast_forecast_params(self, dto: ModelTimeForecastDTO):
        """
        Converts ModelTimeForecastDTO to ModelPredictParams
        """

        end_date_microseconds = get_interval(dto.PeriodsInAdvance)
        end_date = dto.CurrentDate + end_date_microseconds
        return self.map_to_model_forecast_params(dto.CurrentDate, end_date)

    def map_operation_to_domain(
        self, operation: list[CalculationStepDTO]
    ) -> list[CalculationStep]:
        operation_domain = []

        for step in operation:
            step_domain = self._map_step_to_domain(step)
            operation_domain.append(step_domain)
        return operation_domain

    def _map_step_to_domain(self, step: CalculationStepDTO) -> CalculationStep:
        calculations = []
        for calculation in step.Calculations:
            result = self._map_kpi_to_domain(calculation)
            calculations.append(result)
        return CalculationStep(
            Order=step.Order,
            CalculationType=step.CalculationType,
            Operations=calculations,
        )

    def _map_kpi_to_domain(self, dynamic: KPIDTO):
        return DynamicKPI(
            CalculationString=dynamic.CalculationString,
            OutputColumn=dynamic.OutputColumn,
            OperationList=dynamic.OperationsList,
        )

    def map_to_model_forecast_params(self, start_date, end_date):
        return ModelPredictParams(current_date=start_date, end_date=end_date)

    def map_datarequest_dto_to_domain(self, dto: DataRequestDTO) -> DataRequest:
        """
        Maps DataRequestDTO to DataRequest Domain class
        """
        return DataRequest(
            WorkspaceId=dto.WorkspaceId,
            Token=dto.Token,
            DataSource=dto.DataSource,
            StartDateUnix=dto.StartDateUnix,
            EndDateUnix=dto.EndDateUnix,
            Project=dto.Project,
            Points=dto.Points,
            Timelevel=dto.Timelevel,
            TimelevelRange=dto.TimelevelRange,
            ObservationIds=dto.ObservationIds,
            ValueTypeIds=dto.ValueTypeIds,
            ProvideData=dto.ProvideData,
        )

    def map_modelconfig_dto_to_domain(self, dto: ModelConfigDTO) -> ModelConfig:
        """
        Maps ModelConfigDTO to ModelConfig Domain class
        """
        return ModelConfig(
            ModelConfigGuid=dto.ModelConfigGuid,
            ParentWorkspaceGuid=dto.ParentWorkspaceGuid,
            ModelName=dto.ModelName,
            DataSplitRatio=dto.DataSplitRatio,
            ForecastingDate=dto.ForecastingDate,
            DateTimeLevel=dto.DateTimeLevel,
            ModelType=dto.ModelType,
            ModelAlgorithm=dto.ModelAlgorithm,
            Features=self.map_column_dto_domain(dto.Features),
            Targets=self.map_column_dto_domain(dto.Targets),
            ModelParameter=self.map_algorithm_to_domain(dto.AlgorithmParameterDTO),
        )

    def map_algorithm_to_domain(self, dto: AlgorithmUnion) -> ParamUnion:
        """
        Maps algorithm_dto to domain objects
        """
        if isinstance(dto, LinearRegressionDTO):
            return LinearRegressionParam(
                Id=dto.Id, TypeOfAlgorithm=dto.TypeOfAlgorithm, NJobs=dto.NJobs
            )
        return SVMParam(
            Id=dto.Id, TypeOfAlgorithm=dto.TypeOfAlgorithm, Kernel=dto.Kernel
        )

    def map_column_dto_domain(self, cols: list[DataColumnDTO]):
        """
        Maps datacolumn dto to its domain model
        """
        columns = []
        for col in cols:
            column: DataColumn = DataColumn(
                ColumnName=col.ColumnName, DataType=col.DataType
            )
            columns.append(column)
        return columns
