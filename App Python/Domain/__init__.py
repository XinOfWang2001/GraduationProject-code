from .Domain.Enums.Enums import (CalculationType, DateTimeLevel,
                                 ModelAlgorithm, ModelType)
from .Domain.Input.Calculations import (Calculation, CalculationStep,
                                        CalculationUnion, DynamicKPI)
from .Domain.Input.DataColumn import DataColumn
from .Domain.Input.DataRequest import DataRequest
from .Domain.Input.ModelConfig import ModelConfig
from .Domain.Input.ModelLocation import ModelLocation
from .Domain.Input.ModelParameters import (LinearRegressionParam, ParamUnion,
                                           SVMParam)
from .Domain.Input.ModelRequest import ModelRequest
from .Domain.Input.PredictionParameters import ModelPredictParams
from .Domain.ModelResult.DataSeries import DataSeries
from .Domain.ModelResult.ModelResult import ModelResult
from .Domain.ModelResult.ModelTrainingsResult import (ModelTrainingsResult,
                                                      TrainingFeatureClass)
from .Domain.ModelResult.PredictionResult import PredictionResult
from .DTO.AlgorithmDTO import (SVMDTO, AlgorithmDTO, AlgorithmUnion,
                               LinearRegressionDTO)
from .DTO.CalculationDTO import KPIDTO, CalculationDTO, CalculationDTOUnion
from .DTO.CalculationStepDTO import CalculationStepDTO
from .DTO.DataRequestDTO import DataRequestDTO
from .DTO.DataResponseDTO import (DataColumnDTO, DataResponseDTO, MetricsDTO,
                                  ModelResultDTO, PredictionResultDTO)
from .DTO.DeleteDTO import DeleteDTO
from .DTO.ModelConfigDTO import ModelConfigDTO
from .DTO.ModelForecastDTO import ModelForecastRequestDTO, ModelTimeForecastDTO
from .DTO.ModelRequestDTO import ModelRequestDTO
from .DTO.ModelStorageDTO import ModelStorageCreationDTO, ModelStorageDTO

__init__ = [
    ModelResult,
    ModelTrainingsResult,
    ModelAlgorithm,
    ModelType,
    DateTimeLevel,
    CalculationType,
    DataResponseDTO,
    DataRequestDTO,
    DataColumnDTO,
    AlgorithmDTO,
    LinearRegressionDTO,
    SVMDTO,
    AlgorithmUnion,
    CalculationDTO,
    CalculationDTOUnion,
    KPIDTO,
    CalculationStepDTO,
    ModelConfigDTO,
    ModelRequestDTO,
    ModelStorageDTO,
    ModelStorageCreationDTO,
    DataSeries,
    TrainingFeatureClass,
    MetricsDTO,
    ModelResultDTO,
    PredictionResultDTO,
    ModelForecastRequestDTO,
    ModelTimeForecastDTO,
    DeleteDTO,
    ModelConfig,
    Calculation,
    DynamicKPI,
    CalculationUnion,
    CalculationStep,
    DataColumn,
    ParamUnion,
    LinearRegressionParam,
    SVMParam,
    DataRequest,
    ModelRequest,
    ModelLocation,
    ModelPredictParams,
    PredictionResult,
]
