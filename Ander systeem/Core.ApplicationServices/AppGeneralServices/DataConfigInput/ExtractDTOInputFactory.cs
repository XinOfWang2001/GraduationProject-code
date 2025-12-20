using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.ApplicationServices.Interfaces.Strategies;

namespace Leap.ApplicationServices.AppGeneralServices.DataExtractDTOInput
{
    public class ExtractDTOInputFactory : IDataExtractValidatorFactory
    {
        public IInputValidatorStrategy<DataExtractConfigDTO> GetInputValidator(object data)
        {
            if (data is DataExtractConfigDTO)
            {
                return GetwebapiDataExtractValidator();
            }
            throw new InvalidDataException("Alleen DataExtractConfigDTO's");
        }

        public IInputValidatorStrategy<DataExtractConfigDTO> GetwebapiDataExtractValidator()
        {
            return new DataExtractDTOValidator();
        }
    }
}
