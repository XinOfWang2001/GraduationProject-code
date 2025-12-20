"""
This class is responsible for creating time related features.
- Either by creating features on date level or on datetime level

Date-level: Day, Month, Year.
Datetime-level: Day, Month, Year, Hour, Minutes, Weekday
"""

from polars import DataFrame
from sklearn.base import BaseEstimator, TransformerMixin

from const import TIMESTAMP


class DateTransformer(BaseEstimator, TransformerMixin):
    """
    This transformer will create time based features on date-level.
    - Day, Month, Year.
    """

    def transform(self, X: DataFrame):
        return X.with_columns(
            [
                X[TIMESTAMP].dt.year().alias("year").cast(int),
                X[TIMESTAMP].dt.month().alias("month").cast(int),
                X[TIMESTAMP].dt.day().alias("day").cast(int),
            ]
        )

    def fit(self, X: DataFrame, y: DataFrame = None, **fit_params):
        return self

    def fit_transform(self, X: DataFrame, y: DataFrame = None, **fit_params):
        return self.fit(X, y).transform(X)


class DateTimeTransformer(DateTransformer):
    """
    This transformer will in addition to date features, add features related to time.
    - (DateTransformer-Features) + Hour, Minutes
    """

    def transform(self, X: DataFrame):
        X = super().transform(X)
        return X.with_columns(
            [
                X[TIMESTAMP].dt.hour().alias("hour").cast(int),
                X[TIMESTAMP].dt.minute().alias("minute").cast(int),
                X[TIMESTAMP].dt.second().alias("second").cast(int),
                X[TIMESTAMP].dt.ordinal_day().alias("day_of_the_year").cast(int),
                X[TIMESTAMP].dt.weekday().alias("weekday").cast(int),
                X[TIMESTAMP].dt.is_business_day().alias("is_business_day").cast(int),
            ]
        )

    def fit(self, X, y=None, **fit_params):
        return self

    def fit_transform(self, X, y=None, **fit_params):
        return self.fit(X, y).transform(X)
