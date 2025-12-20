from abc import ABC, abstractmethod


class AbstractExternalService(ABC):
    """This is the abstract class/interface for retrieval
    of data from Sweco internal data-sources"""

    @abstractmethod
    async def return_value(self, request):
        """Will return values from external datasource"""
