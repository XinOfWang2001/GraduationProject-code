using Leap.ApplicationServices.AppGeneralServices.DataExtractDTOInput;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.Domain.Domain.DataConfig;
using Leap.Domain.Domain.DataSource;

namespace Test.ApplicationService.UC_1_DatasourceConfig
{
    public class TestValidationLogic
    {
        private readonly DataExtractDTOValidator validator;

        public TestValidationLogic()
        {
            validator = new DataExtractDTOValidator();
        }

        private static DataExtractConfigDTO CompleteDTO()
        {
            DataExtractConfigDTO dto = new()
            {
                WorkspaceId = new Guid("a86ff674-ae5a-472a-9479-aaacb5f5ce9e"),
                StartDate = new DateTime(2024, 11, 11),
                EndDate = new DateTime(2024, 12, 1),
                DataSource = new DataSourceDTO() { DataSourceId = 2 },
                SensorsSelected = [new SensorDTO { Id = 1, Name = "C-1" }],
                ValueTypesSelected = [new ValueTypeDTO { Id = 1, Name = "Temp" }],
                ProjectDTO = new ProjectSourceDTO() { Id = 1, HumanReadableName = "KTYE_Project", Guid = new Guid("77d3c0ea-91b5-4e6f-9e1e-f2937edfd167"), Name = "KTYE_Project_name" },
            };
            return dto;
        }

        // Code: A-7
        [Fact]
        public void TestValidwebapipipiDataExtractDTO()
        {
            var dto = CompleteDTO();

            var result = validator.Validate(dto);

            Assert.True(result);
        }

        // Code: A-1
        [Fact]
        public void TestInValidwebapipipiDataExtractDTOWithoutDataSource()
        {
            var dto = new DataExtractConfigDTO();

            var result = validator.Validate(dto);
            var message = validator.GetErrorMessage();
            Assert.False(result);
            Assert.Equal("Requires at least one data source must be selected", message);
        }

        // Code: A-2
        [Fact]
        public void TestInvalidwebapipipiDataExtractDTOWithoutProject()
        {
            var dto = new DataExtractConfigDTO()
            {
                DataSource = new DataSourceDTO(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
            };

            var result = validator.Validate(dto);
            var message = validator.GetErrorMessage();
            Assert.False(result);
            Assert.Equal("Requires at least one project", message);
        }
        // Code: A-3
        [Fact]
        public void TestInvalidwebapipipiDataExtractDTOWithoutSensor()
        {
            var dto = new DataExtractConfigDTO()
            {
                DataSource = new DataSourceDTO(),
                ProjectDTO = new ProjectSourceDTO(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
            };

            var result = validator.Validate(dto);
            var message = validator.GetErrorMessage();
            Assert.False(result);
            Assert.Equal("Requires at least one sensor", message);
        }

        // Code: A-4
        [Fact]
        public void TestInvalidwebapipipiDataExtractDTOWithoutValueTypes()
        {
            var dto = new DataExtractConfigDTO()
            {
                DataSource = new DataSourceDTO(),
                ProjectDTO = new ProjectSourceDTO(),
                SensorsSelected = [new()],
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
            };

            var result = validator.Validate(dto);
            var message = validator.GetErrorMessage();
            Assert.False(result);
            Assert.Equal("Requires at least one valuetypes", message);
        }

        // Code: A-5
        [Fact]
        public void TestInvalidwebapipipiDataExtractDTOWithInvalidStartDate()
        {
            var dto = CompleteDTO();
            dto.StartDate = DateTime.Now.AddDays(6);
            dto.EndDate = DateTime.Now.AddDays(3);

            var result = validator.Validate(dto);
            var message = validator.GetErrorMessage();

            Assert.False(result);
            Assert.Equal("Startdate must not be later then the enddate", message);
        }

        // A-XX
        [Fact]
        public void TestIfDataPointsMinusOneReturnsDefaultTimeRange()
        {
            var domain = new DataSourceConfig()
            {
                AssignedProject = new Project()
                {
                    Name = "Hello",
                    HumanReadableName = "Hello"
                },
                DataPoints = -1
            };

            Assert.Equal(432000000000.0f, domain.GetTimeRange());
        }

        // A-XX
        [Fact]
        public void TestIfDataPointsPlusOneReturnsActualTimeRange()
        {
            var domain = new DataSourceConfig()
            {
                AssignedProject = new Project()
                {
                    Name = "Hello",
                    HumanReadableName = "Hello",
                },
                DataPoints = -1,
                TimelevelRange = 1.0f
            };

            Assert.Equal(1.0f, domain.GetTimeRange());
        }
    }
}
