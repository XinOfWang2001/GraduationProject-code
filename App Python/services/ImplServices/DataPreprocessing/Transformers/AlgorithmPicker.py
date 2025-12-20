from sklearn.base import OutlierMixin, RegressorMixin
from sklearn.linear_model import LinearRegression
from sklearn.multioutput import MultiOutputRegressor
from sklearn.svm import SVR

from Domain import ModelAlgorithm, ModelConfig


class AlgorithmPicker:
    """
    This class is responsible for returning the right algorithm based on the configuration provided
    """

    def get_machine_learning_algorithm(
        self, modelconfig: ModelConfig
    ) -> RegressorMixin | OutlierMixin:
        """
        Will first decide if model is a forecasting or outlier detection.
        Then it assign the right model based on algorithmType and assign its parameters accordingly.
        """
        # Will be expanded with a outlier detection model
        return self._get_forecasting_algorithm(modelconfig)

    def _get_forecasting_algorithm(self, modelconfig: ModelConfig) -> RegressorMixin:
        """ "
        Will return the right forecasting model based on the algorithmtype
        """
        params = modelconfig.ModelParameter
        if modelconfig.ModelAlgorithm == ModelAlgorithm.SVMREGRESSION:
            return MultiOutputRegressor(SVR(kernel=params.Kernel))
        return LinearRegression()
