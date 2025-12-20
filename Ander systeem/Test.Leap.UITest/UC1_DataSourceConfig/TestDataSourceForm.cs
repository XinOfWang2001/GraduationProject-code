using Bunit;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.DTO.External_Services;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;
using LeapDataScienceTool.Common.Énums;
using LeapDataScienceTool.Components.DataSourceProcess;
using LeapDataScienceTool.ProgramSetup;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC1_DataSourceConfig
{
    public class DataExtractEditFormTests : TestContext
    {
        private readonly Mock<IMudDialogInstance> _dialogInstance;
        private readonly Mock<IDataSourceService> _datasourceServiceHandler;
        private readonly Mock<IDataExtractService> _dataExtractProcessService;
        private readonly Mock<IMonitorDataService> _monitorService;
        private readonly Mock<IDialogService> _dialogService;

        public DataExtractEditFormTests()
        {
            _dialogInstance = new Mock<IMudDialogInstance>();
            _datasourceServiceHandler = new Mock<IDataSourceService>();
            _dataExtractProcessService = new Mock<IDataExtractService>();
            _monitorService = new Mock<IMonitorDataService>();
            _dialogService = new Mock<IDialogService>();
        }

        public DataExtractConfigDTO GetDummyDTO()
        {
            DataExtractConfigDTO dto = new DataExtractConfigDTO()
            {
                WorkspaceId = new Guid("a86ff674-ae5a-472a-9479-aaacb5f5ce9e"),
                StartDate = new DateTime(2024, 11, 11),
                EndDate = new DateTime(2024, 12, 1),
                DataSource = new DataSourceDTO() { DataSourceId = 2 },
                SensorsSelected = new List<SensorDTO> { new SensorDTO { Id = 1, Name = "C-1" } },
                ValueTypesSelected = new List<ValueTypeDTO> { new ValueTypeDTO { Id = 1, Name = "Temp" } },
                ProjectDTO = new ProjectSourceDTO() { Id = 1, HumanReadableName = "KTYE_Project", Guid = new Guid("77d3c0ea-91b5-4e6f-9e1e-f2937edfd167"), Name = "KTYE_Project_name" },
            };
            return dto;
        }

        public MonitorInfoDTO GetDummyInfoDTO()
        {
            return new()
            {
                Observations = [
                    new(){
                        Id = 1, Name = "C-1",ValueTypeIds = [1],

                    }],
                Valuetypes = [new() { Id = 1, Name = "Temp" }],
            };
        }

        [Fact]
        public Task TestIfKnownDataConfigSetsStateToCONCEPT()
        {
            DataProcesState ExpectedStatus = DataProcesState.CONCEPT;
            MonitorInfoDTO monitorInfo = GetDummyInfoDTO();
            // Arrange
            _datasourceServiceHandler.Setup(service => service.GetData()).ReturnsAsync(new List<DataSourceDTO>() {
                new DataSourceDTO() {
                    DataSourceGuidId = Guid.NewGuid(),
                    Name = "webapi_Server",
                    projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
                }
            });
            _datasourceServiceHandler.Setup(service => service.GetOne(It.IsAny<int>())).ReturnsAsync(new DataSourceDTO()
            {
                DataSourceGuidId = Guid.NewGuid(),
                Name = "webapi_Server",
                projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
            });
            _monitorService.Setup(service => service.GetMonitorInfoAsync(It.IsAny<MonitorInfoRequest>())).ReturnsAsync(monitorInfo);

            // Register mocks in the DI container
            Services.AddSingleton(_datasourceServiceHandler.Object);
            Services.AddSingleton(_dataExtractProcessService.Object);
            Services.AddSingleton(_monitorService.Object);
            Services.AddSingleton(_dialogInstance.Object);
            Services.AddSingleton(_dialogService.Object);

            var dataExtractProcess = new DataExtractProcess
            {
                DataProcess = new DataExtractConfigDTO()
            };

            // Act: Render the component
            var cut = RenderComponent<DataSourceDialog>(parameters => parameters
                .Add(cp => cp.MudDialog, _dialogInstance.Object)
                .Add(p => p.DataExtractProcess, dataExtractProcess)
            );

            // Assert
            Assert.Equal(ExpectedStatus, cut.Instance.Status);
            return Task.CompletedTask;
        }
        // Updaten
        [Fact]
        public void TestIfKnownDataConfigSetsStateToSET()
        {
            DataProcesState ExpectedStatus = DataProcesState.SET;
            MonitorInfoDTO monitorInfo = GetDummyInfoDTO();
            // Arrange
            _datasourceServiceHandler.Setup(service => service.GetData()).ReturnsAsync(new List<DataSourceDTO>() {
                new DataSourceDTO() {
                    DataSourceGuidId = Guid.NewGuid(),
                    Name = "webapi_Server",
                    projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
                }
            });
            _datasourceServiceHandler.Setup(service => service.GetOne(It.IsAny<int>())).ReturnsAsync(new DataSourceDTO()
            {
                DataSourceGuidId = Guid.NewGuid(),
                Name = "webapi_Server",
                projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
            });
            _monitorService.Setup(service => service.GetMonitorInfoAsync(It.IsAny<MonitorInfoRequest>())).ReturnsAsync(monitorInfo);

            // Register mocks in the DI container
            Services.AddSingleton(_datasourceServiceHandler.Object);
            Services.AddSingleton(_dataExtractProcessService.Object);
            Services.AddSingleton(_monitorService.Object);
            Services.AddSingleton(_dialogInstance.Object);
            Services.AddSingleton(_dialogService.Object);

            var dataExtractProcess = new DataExtractProcess
            {
                DataProcess = GetDummyDTO(),
                Status = DataProcesState.SET,
            };

            // Act: Render the component
            var cut = RenderComponent<DataSourceDialog>(parameters => parameters
                .Add(cp => cp.MudDialog, _dialogInstance.Object)
                .Add(p => p.DataExtractProcess, dataExtractProcess)
            );
            // Assert
            Assert.Equal(ExpectedStatus, cut.Instance.Status);
        }

        // N-BZ-33
        [Fact]
        public async Task TestIfNoSelectedDataSourceInvalidatesSubmission()
        {
            // Arrange
            bool ExpectedState = false;
            MonitorInfoDTO monitorInfo = GetDummyInfoDTO();
            _datasourceServiceHandler.Setup(service => service.GetData()).ReturnsAsync(new List<DataSourceDTO>());
            _datasourceServiceHandler.Setup(service => service.GetData()).ReturnsAsync(new List<DataSourceDTO>() {
                new DataSourceDTO() {
                    DataSourceGuidId = Guid.NewGuid(),
                    Name = "webapi_Server",
                    projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
                }
            });
            _datasourceServiceHandler.Setup(service => service.GetOne(It.IsAny<int>())).ReturnsAsync(new DataSourceDTO()
            {
                DataSourceGuidId = Guid.NewGuid(),
                Name = "webapi_Server",
                projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
            });
            _monitorService.Setup(service => service.GetMonitorInfoAsync(It.IsAny<MonitorInfoRequest>())).ReturnsAsync(
                new MonitorInfoDTO()
                {
                    Observations = [new() { Id = 1, ValueTypeIds = [1, 2] }],
                    Valuetypes = [new MonitorInfoValueType() { Id = 1, Name = "Temp", Quantity = "C", UnitAbbr = "Temp" }],
                });

            // Register mocks in the DI container
            Services.AddSingleton(_datasourceServiceHandler.Object);
            Services.AddSingleton(_dataExtractProcessService.Object);
            Services.AddSingleton(_monitorService.Object);
            Services.AddSingleton(_dialogInstance.Object);
            Services.AddSingleton(_dialogService.Object);
            var dto = GetDummyDTO();
            var dataExtractProcess = new DataExtractProcess
            {
                DataProcess = dto,
                Status = DataProcesState.SET,
            };

            // Act: Render the component
            var cut = RenderComponent<DataSourceDialog>(parameters => parameters
                .Add(cp => cp.MudDialog, _dialogInstance.Object)
                .Add(p => p.DataExtractProcess, dataExtractProcess)
            );
            // Validate if their not null
            Assert.NotNull(cut.Instance.DataExtractProcess.DataProcess.ProjectDTO);
            Assert.NotEmpty(cut.Instance.DataExtractProcess.DataProcess.SensorsSelected);
            Assert.NotEmpty(cut.Instance.DataExtractProcess.DataProcess.ValueTypesSelected);
            // Empty datasource field
            await cut.Instance.OnDataSourceSelection(null);

            // Assert if Succes is set to false
            Assert.Null(cut.Instance.DataExtractProcess.DataProcess.ProjectDTO);
            Assert.Empty(cut.Instance.DataExtractProcess.DataProcess.SensorsSelected);
            Assert.Empty(cut.Instance.DataExtractProcess.DataProcess.ValueTypesSelected);
        }

        // Code: BZ-7
        [Fact]
        public async Task TestIfSelectedDataSourcerShowsProjects()
        {
            // Arrange
            MonitorInfoDTO monitorInfo = GetDummyInfoDTO();
            _datasourceServiceHandler.Setup(service => service.GetData()).ReturnsAsync(new List<DataSourceDTO>() {
                new DataSourceDTO() {
                    DataSourceGuidId = Guid.NewGuid(),
                    Name = "webapi_Server",
                    projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
                }
            });
            _datasourceServiceHandler.Setup(service => service.GetOne(It.IsAny<int>()))
                .ReturnsAsync(new DataSourceDTO()
                {
                    DataSourceId = 1,
                    projectSourceDTOs = new List<ProjectSourceDTO>() {
                    new() { HumanReadableName = "Project 1"} ,
                    new() { HumanReadableName = "Project 2"}
                }
                });
            _monitorService.Setup(service => service.GetMonitorInfoAsync(It.IsAny<MonitorInfoRequest>())).ReturnsAsync(monitorInfo);

            // Register mocks in the DI container
            Services.AddSingleton(_datasourceServiceHandler.Object);
            Services.AddSingleton(_dataExtractProcessService.Object);
            Services.AddSingleton(_monitorService.Object);
            Services.AddSingleton(_dialogInstance.Object);
            Services.AddSingleton(_dialogService.Object);
            var dto = GetDummyDTO();

            var dataExtractProcess = new DataExtractProcess
            {
                DataProcess = dto,
                Status = DataProcesState.SET,
            };
            var cut = RenderComponent<DataSourceDialog>(parameters => parameters
                .Add(cp => cp.MudDialog, _dialogInstance.Object)
                .Add(p => p.DataExtractProcess, dataExtractProcess)
            );
            // First deselect.
            await cut.Instance.OnDataSourceSelection(null);

            // Validate if their not null
            Assert.Null(cut.Instance.DataExtractProcess.DataProcess.ProjectDTO);
            Assert.Empty(cut.Instance.DataExtractProcess.DataProcess.SensorsSelected);
            Assert.Empty(cut.Instance.DataExtractProcess.DataProcess.ValueTypesSelected);
            // Act: Render the component
            // Empty datasource field
            await cut.Instance.OnDataSourceSelection(new DataSourceDTO() { DataSourceId = 1, DataSourceUrl = "https:/localhost", Name = "Random Datasource" });

            // Assert if Succes is set to false
            Assert.Equal(2, cut.Instance.Projects.Count());
        }

        // N-BZ-34
        [Fact]
        public async Task TestIfUnSelectedProjectsResetsValueTypesAndSensors()
        {
            // Arrange
            MonitorInfoDTO monitorInfo = GetDummyInfoDTO();
            _datasourceServiceHandler.Setup(service => service.GetData()).ReturnsAsync(new List<DataSourceDTO>() {
                new DataSourceDTO() {
                    DataSourceGuidId = Guid.NewGuid(),
                    Name = "webapi_Server",
                    projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
                }
            });
            _datasourceServiceHandler.Setup(service => service.GetOne(It.IsAny<int>()))
                .ReturnsAsync(new DataSourceDTO()
                {
                    DataSourceId = 1,
                    projectSourceDTOs = new List<ProjectSourceDTO>() {
                    new() { HumanReadableName = "Project 1"} ,
                    new() { HumanReadableName = "Project 2"}
                }
                });
            _monitorService.Setup(service => service.GetMonitorInfoAsync(It.IsAny<MonitorInfoRequest>())).ReturnsAsync(monitorInfo);

            // Register mocks in the DI container
            Services.AddSingleton(_datasourceServiceHandler.Object);
            Services.AddSingleton(_dataExtractProcessService.Object);
            Services.AddSingleton(_monitorService.Object);
            Services.AddSingleton(_dialogInstance.Object);
            Services.AddSingleton(_dialogService.Object);
            var dto = GetDummyDTO();

            var dataExtractProcess = new DataExtractProcess
            {
                DataProcess = dto,
                Status = DataProcesState.SET,
            };
            var cut = RenderComponent<DataSourceDialog>(parameters => parameters
                .Add(cp => cp.MudDialog, _dialogInstance.Object)
                .Add(p => p.DataExtractProcess, dataExtractProcess)
            );
            // Act: Render the component
            // Empty datasource field
            await cut.Instance.OnProjectSelection(null);

            // Assert if Succes is set to false
            Assert.Empty(dto.SensorsSelected);
            Assert.Empty(dto.ValueTypesSelected);
        }

        // Code: N-BZ-35
        [Fact]
        public async Task TestIfPostShowsErrorMessage()
        {
            // Arrange
            MonitorInfoDTO monitorInfo = GetDummyInfoDTO();
            _datasourceServiceHandler.Setup(service => service.GetData()).ReturnsAsync(new List<DataSourceDTO>() {
                new DataSourceDTO() {
                    DataSourceGuidId = Guid.NewGuid(),
                    Name = "webapi_Server",
                    projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
                }
            });
            _monitorService.Setup(service => service.GetMonitorInfoAsync(It.IsAny<MonitorInfoRequest>())).ReturnsAsync(monitorInfo);
            _datasourceServiceHandler.Setup(service => service.GetOne(It.IsAny<int>()))
                .ReturnsAsync(new DataSourceDTO()
                {
                    DataSourceId = 1,
                    projectSourceDTOs = new List<ProjectSourceDTO>() {
                    new() { HumanReadableName = "Project 1"} ,
                    new() { HumanReadableName = "Project 2"}
                }
                });
            _dataExtractProcessService.Setup(service => service.RegisterDataExtractProcess(It.IsAny<DataExtractConfigDTO>()))
                .ReturnsAsync((DataExtractConfigDTO)null);

            // Register mocks in the DI container
            Services.AddSingleton(_datasourceServiceHandler.Object);
            Services.AddSingleton(_dataExtractProcessService.Object);
            Services.AddSingleton(_monitorService.Object);
            Services.AddSingleton(_dialogInstance.Object);
            Services.AddSingleton(_dialogService.Object);
            var dto = GetDummyDTO();

            var dataExtractProcess = new DataExtractProcess
            {
                DataProcess = dto,
                Status = DataProcesState.CONCEPT,
            };
            var cut = RenderComponent<DataSourceDialog>(parameters => parameters
                .Add(cp => cp.MudDialog, _dialogInstance.Object)
                .Add(p => p.DataExtractProcess, dataExtractProcess)
            );
            // Act: Render the component
            // Empty datasource field
            await cut.InvokeAsync(() => cut.Instance.Confirm());

            Assert.Equal(DataProcesState.CONCEPT, cut.Instance.DataExtractProcess.Status);
        }

        // BZ-11
        [Fact]
        public async Task TestSuccessfullPost()
        {
            // Arrange
            MonitorInfoDTO monitorInfo = GetDummyInfoDTO();
            Services.RegisterProxyServices();
            Services.RegisterCustomUIServices();
            Services.RegisterUIComponents();

            Mock<IServerAPI> serverAPI = new Mock<IServerAPI>();
            serverAPI.Setup(api => api.GetAll<DataSourceDTO>(It.IsAny<string>())).ReturnsAsync(
                new List<DataSourceDTO>() {
                new DataSourceDTO() {
                    DataSourceGuidId = Guid.NewGuid(),
                    Name = "webapi_Server",
                    projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
                }
            });
            serverAPI.Setup(api => api.Get<MonitorInfoDTO>(It.IsAny<string>())).ReturnsAsync(monitorInfo);
            serverAPI.Setup(api => api.Get<DataSourceDTO>(It.IsAny<string>())).ReturnsAsync(new DataSourceDTO());
            serverAPI.Setup(api => api.Post<DataExtractConfigDTO>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(GetDummyDTO());
            Services.AddSingleton(serverAPI.Object);

            // Register mocks in the DI container
            var dto = GetDummyDTO();

            var dataExtractProcess = new DataExtractProcess
            {
                DataProcess = dto,
                Status = DataProcesState.CONCEPT,
            };
            var cut = RenderComponent<DataSourceDialog>(parameters => parameters
                .Add(cp => cp.MudDialog, _dialogInstance.Object)
                .Add(p => p.DataExtractProcess, dataExtractProcess)
            );
            // Act: Render the component
            // Empty datasource field
            await cut.InvokeAsync(() => cut.Instance.Confirm());

            Assert.Equal(DataProcesState.SET, cut.Instance.DataExtractProcess.Status);
        }
        // N-BZ-36A
        [Fact]
        public void TestOnSensorEmptySelect()
        {
            MonitorInfoDTO monitorInfoDTO = new MonitorInfoDTO()
            {
                Observations = [
                    new() { Id = 1, Name= "test", ValueTypeIds = [11, 12]},
                    new() { Id = 2, Name= "test2",  ValueTypeIds = [12]},
                    new() { Id = 3, Name= "test3", ValueTypeIds = [24, 25]},
                    new() { Id = 4, Name= "test4", ValueTypeIds = [11, 25]},
                    ],
                Valuetypes = [
                    new() { Id = 1, Name = "Dummy 11" },
                    new() { Id = 12, Name = "Dummy 12" },
                    new() { Id = 24, Name = "Dummy 24" },
                    new() { Id = 25, Name = "Dummy 25" }
                    ],
            };

            List<SensorDTO> InputSensors = [
                new() { Id = 1, Name = "Jacob"},
                new() { Id = 4, Name = "Bobby"}
                ];
            // Arrange
            _datasourceServiceHandler.Setup(service => service.GetData()).ReturnsAsync(new List<DataSourceDTO>() {
                new DataSourceDTO() {
                    DataSourceGuidId = Guid.NewGuid(),
                    Name = "webapi_Server",
                    projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
                }
            });
            _datasourceServiceHandler.Setup(service => service.GetOne(It.IsAny<int>())).ReturnsAsync(new DataSourceDTO()
            {
                DataSourceGuidId = Guid.NewGuid(),
                Name = "webapi_Server",
                projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
            });
            _monitorService.Setup(service => service.GetMonitorInfoAsync(It.IsAny<MonitorInfoRequest>())).ReturnsAsync(monitorInfoDTO);

            // Register mocks in the DI container
            Services.AddSingleton(_datasourceServiceHandler.Object);
            Services.AddSingleton(_dataExtractProcessService.Object);
            Services.AddSingleton(_monitorService.Object);
            Services.AddSingleton(_dialogInstance.Object);
            Services.AddSingleton(_dialogService.Object);

            var dataExtractProcess = new DataExtractProcess
            {
                DataProcess = GetDummyDTO(),
                Status = DataProcesState.SET,
            };

            // Act: Render the component
            var cut = RenderComponent<DataSourceDialog>(parameters => parameters
                .Add(cp => cp.MudDialog, _dialogInstance.Object)
                .Add(p => p.DataExtractProcess, dataExtractProcess)
            );
            var InstanceOfComponent = cut.Instance;

            InstanceOfComponent.OnSensorSelection([]);

            var CurrentIds = InstanceOfComponent.ValueTypes.Select(vt => vt.Id).ToArray();
            // Assert\
            int[] ExpectedCollection = [11, 12, 25];
            Assert.Empty(CurrentIds);
        }
        // N-BZ-36B
        [Fact]
        public void TestOnSensorSelect()
        {
            MonitorInfoDTO monitorInfoDTO = new MonitorInfoDTO()
            {
                Observations = [
                    new() { Id = 1, ValueTypeIds = [11, 12]},
                    new() { Id = 2, ValueTypeIds = [12]},
                    new() { Id = 3, ValueTypeIds = [24, 25]},
                    new() { Id = 4, ValueTypeIds = [11, 25]},
                    ],
                Valuetypes = [
                    new() { Id = 1, Name = "Dummy 1" },
                    new() { Id = 11, Name = "Dummy 11" },
                    new() { Id = 12, Name = "Dummy 12" },
                    new() { Id = 24, Name = "Dummy 24" },
                    new() { Id = 25, Name = "Dummy 25" }
                    ],
            };

            List<SensorDTO> InputSensors = [
                new() { Id = 1, Name = "Jacob"},
                new() { Id = 4, Name = "Bobby"}
                ];
            // Arrange
            _datasourceServiceHandler.Setup(service => service.GetData()).ReturnsAsync(new List<DataSourceDTO>() {
                new DataSourceDTO() {
                    DataSourceGuidId = Guid.NewGuid(),
                    Name = "webapi_Server",
                    projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
                }
            });
            _datasourceServiceHandler.Setup(service => service.GetOne(It.IsAny<int>())).ReturnsAsync(new DataSourceDTO()
            {
                DataSourceGuidId = Guid.NewGuid(),
                Name = "webapi_Server",
                projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
            });
            // Used for initial load
            _monitorService.Setup(service => service.GetMonitorInfoAsync(It.IsAny<MonitorInfoRequest>())).ReturnsAsync(monitorInfoDTO);

            // Register mocks in the DI container
            Services.AddSingleton(_datasourceServiceHandler.Object);
            Services.AddSingleton(_dataExtractProcessService.Object);
            Services.AddSingleton(_monitorService.Object);
            Services.AddSingleton(_dialogInstance.Object);
            Services.AddSingleton(_dialogService.Object);

            var dataExtractProcess = new DataExtractProcess
            {
                DataProcess = GetDummyDTO(),
                Status = DataProcesState.SET,
            };

            // Act: Render the component
            var cut = RenderComponent<DataSourceDialog>(parameters => parameters
                .Add(cp => cp.MudDialog, _dialogInstance.Object)
                .Add(p => p.DataExtractProcess, dataExtractProcess)
            );
            // Overriding 
            _monitorService.Setup(service => service.GetMonitorInfoAsync(It.IsAny<MonitorInfoRequest>())).ReturnsAsync(monitorInfoDTO);
            var InstanceOfComponent = cut.Instance;

            InstanceOfComponent.OnSensorSelection(InputSensors);

            var CurrentIds = InstanceOfComponent.ValueTypes.Select(vt => vt.Id).ToArray();
            // Assert\
            int[] ExpectedCollection = [11, 12, 25];
            Assert.Equal(ExpectedCollection.Length, CurrentIds.Length);
        }

        // N-BZ-36C
        [Fact]
        public void TestOnSensorSelectIfNoMonitorValuesReturned()
        {
            MonitorInfoDTO monitorInfoDTO = GetDummyInfoDTO();

            List<SensorDTO> InputSensors = [
                new() { Id = 1, Name = "Jacob"},
                new() { Id = 4, Name = "Bobby"}
                ];
            // Arrange
            _datasourceServiceHandler.Setup(service => service.GetData()).ReturnsAsync(new List<DataSourceDTO>() {
                new DataSourceDTO() {
                    DataSourceGuidId = Guid.NewGuid(),
                    Name = "webapi_Server",
                    projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
                }
            });
            _datasourceServiceHandler.Setup(service => service.GetOne(It.IsAny<int>())).ReturnsAsync(new DataSourceDTO()
            {
                DataSourceGuidId = Guid.NewGuid(),
                Name = "webapi_Server",
                projectSourceDTOs = [
                        new ProjectSourceDTO(){
                            Id = 1,
                            HumanReadableName = "Test",
                            Name = "test"
                        }
                        ]
            });
            _monitorService.Setup(service => service.GetMonitorInfoAsync(It.IsAny<MonitorInfoRequest>())).ReturnsAsync(monitorInfoDTO);

            // Register mocks in the DI container
            Services.AddSingleton(_datasourceServiceHandler.Object);
            Services.AddSingleton(_dataExtractProcessService.Object);
            Services.AddSingleton(_monitorService.Object);
            Services.AddSingleton(_dialogInstance.Object);
            Services.AddSingleton(_dialogService.Object);

            var dataExtractProcess = new DataExtractProcess
            {
                DataProcess = GetDummyDTO(),
                Status = DataProcesState.SET,
            };

            // Act: Render the component
            var cut = RenderComponent<DataSourceDialog>(parameters => parameters
                .Add(cp => cp.MudDialog, _dialogInstance.Object)
                .Add(p => p.DataExtractProcess, dataExtractProcess)
            );
            var InstanceOfComponent = cut.Instance;
            // Next request should return zero.
            InstanceOfComponent.monitorInfo = new MonitorInfoDTO();
            InstanceOfComponent.OnSensorSelection(InputSensors);

            var CurrentIds = InstanceOfComponent.ValueTypes.Select(vt => vt.Id).ToArray();
            // Assert
            Assert.Empty(CurrentIds);
        }
    }
}
