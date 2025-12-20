using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.Interfaces.Strategies;

namespace Leap.ApplicationServices.AppGeneralServices.DataExtractDTOInput
{
    public class DataExtractDTOValidator : IInputValidatorStrategy<DataExtractConfigDTO>
    {
        private string ErrorMessage { get; set; }
        public string GetErrorMessage()
        {
            return ErrorMessage;
        }

        public bool Validate(DataExtractConfigDTO input)
        {
            // Reset error message
            ErrorMessage = string.Empty;
            // Mostly as the 
            var result = AtLeastOneVTs(input) &
                AtLeastOneSensor(input) &
                ValidStartDate(input) &
                PresentProject(input) &
                PresentDataSource(input);
            return result;
        }

        private bool PresentDataSource(DataExtractConfigDTO input)
        {
            var valid = IsNotNull(input.DataSource);
            if (!valid)
            {
                ErrorMessage = "Requires at least one data source must be selected";
            }
            return IsNotNull(valid);
        }

        private bool PresentProject(DataExtractConfigDTO input)
        {
            var valid = IsNotNull(input.ProjectDTO);
            if (!valid)
            {
                ErrorMessage = "Requires at least one project";
            }
            return valid;

        }

        private bool AtLeastOneSensor(DataExtractConfigDTO input)
        {
            var valid = input.SensorsSelected.Any();
            if (!valid)
            {
                ErrorMessage = "Requires at least one sensor";
            }
            return valid;
        }

        private bool AtLeastOneVTs(DataExtractConfigDTO input)
        {
            var valid = input.ValueTypesSelected.Any();
            if (!valid)
            {
                ErrorMessage = "Requires at least one valuetypes";
            }
            return valid;
        }

        private bool ValidStartDate(DataExtractConfigDTO input)
        {
            var valid = input.StartDate < input.EndDate;
            if (!valid)
            {
                ErrorMessage = "Startdate must not be later then the enddate";
            }
            return valid;
        }

        private static bool IsNotNull(object? obj)
        {
            return obj != null;
        }
    }
}
