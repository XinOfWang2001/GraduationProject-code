using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.Interfaces.Strategies;

namespace Leap.ApplicationServices.Interfaces.Creational
{
    public interface IDataExtractValidatorFactory
    {
        // This method will decide which inputvalidator is chosen.
        public IInputValidatorStrategy<DataExtractConfigDTO> GetInputValidator(object data);
        protected IInputValidatorStrategy<DataExtractConfigDTO> GetwebapiDataExtractValidator();
    }
}
