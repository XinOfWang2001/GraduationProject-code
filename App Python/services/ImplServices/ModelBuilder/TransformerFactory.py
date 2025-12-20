from Domain import DateTimeLevel, ModelConfig

from ..DataPreprocessing.Transformers import DateTimeTransformer, DateTransformer


class TransformerFactory:
    """
    A simple factory, responsible for all model pipeline transformer class creation.
    """

    def create_date_transformer(self, modelconfig: ModelConfig) -> DateTransformer:
        if modelconfig.DateTimeLevel == DateTimeLevel.STANDARD:
            return DateTimeTransformer()
        return DateTransformer()
