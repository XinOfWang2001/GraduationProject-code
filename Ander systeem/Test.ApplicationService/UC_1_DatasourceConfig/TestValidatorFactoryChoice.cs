using Leap.ApplicationServices.AppGeneralServices.DataExtractDTOInput;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.ApplicationServices.Interfaces.Strategies;

namespace Test.ApplicationService.UC_1_DatasourceConfig
{
    public class TestValidatorFactoryChoice
    {
        private readonly IDataExtractValidatorFactory _validatorFactory;

        public TestValidatorFactoryChoice()
        {
            _validatorFactory = new ExtractDTOInputFactory();

        }

        // N-A-11
        [Fact]
        public void TestIfInvalidClassReturnsException()
        {
            DataSourceDTO irrelevantDTO = new DataSourceDTO();

            Assert.Throws<InvalidDataException>(() => _validatorFactory.GetInputValidator(irrelevantDTO));
        }

        // N-A-12
        [Fact]
        public void TestIfCertainConcreteClassesReturnsRightValidator()
        {
            DataExtractConfigDTO relevantDTO = new DataExtractConfigDTO();

            IInputValidatorStrategy<DataExtractConfigDTO> result = _validatorFactory.GetInputValidator(relevantDTO);

            Assert.Equal(nameof(DataExtractDTOValidator), result.GetType().Name);
        }
    }
}
