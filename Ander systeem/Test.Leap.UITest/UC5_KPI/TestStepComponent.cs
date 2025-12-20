using Bunit;
using Leap.ApplicationServices.DTO.Calculations;
using Leap.ApplicationServices.DTO.Workspace;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;
using LeapDataScienceTool.Components.Aggregation___KPI;
using LeapDataScienceTool.PageManagers;
using LeapDataScienceTool.Services.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC5_KPI
{
    public class TestStepComponent : TestContext
    {

        // Test if insertion adds two steps with numbers 1, 2, 3
        [Fact]
        public void TestInsertionOfNewSteps()
        {
            // Assert
            Mock<IServerAPI> MockServerAPI = new();
            Mock<IWorkspaceManager> MockManager = new();
            MockManager.Setup(m => m.GetWorkspaceConfigDTO()).Returns(new WorkspaceConfigDTO() { WorkspaceGuid = new Guid("f7ac518c-e92a-487e-9367-37bb38dbb7ac") });
            Services.RegisterUIComponents();
            JSInterop.RegisterPopOverSetup(Services);
            Services.AddSingleton(MockServerAPI.Object);
            Services.AddSingleton(MockManager.Object);
            Services.AddSingleton<ICalculationService, CalculationProxyService>();
            var Component = RenderComponent<DynamicCalculationComponent>();
            KPIProcess firstProcess = new KPIProcess("SelectCalculation") { DataProcessType = "First", CalculationStep = new CalculationStepDTO() { Order = -1, Calculations = [] } };
            KPIProcess secondProcess = new KPIProcess("SelectCalculation") { DataProcessType = "Second", CalculationStep = new CalculationStepDTO() { Order = -1, Calculations = [] } };
            KPIProcess thirdProcess = new KPIProcess("SelectCalculation") { DataProcessType = "Third", CalculationStep = new CalculationStepDTO() { Order = -1, Calculations = [] } };
            // Two elements
            MudItemDropInfo<IDataProcess> FirstInsert = new(firstProcess, "DataManipulator", 0);
            MudItemDropInfo<IDataProcess> SecondInsert = new(secondProcess, "DataManipulator", 1);
            MudItemDropInfo<IDataProcess> ThirdInsert = new(thirdProcess, "DataManipulator", 2);
            // Act
            Component.Instance.InsertIntoDropzone(FirstInsert);
            Component.Instance.InsertIntoDropzone(SecondInsert);
            Component.Instance.InsertIntoDropzone(ThirdInsert);

            var processes = Component.Instance.DataProcesses.Where((proc) => proc.Order > -1);
            Assert.Equal(3, processes.Count());
            Assert.Equal("First", processes.ElementAt(0).DataProcessType);
            Assert.Equal("Second", processes.ElementAt(1).DataProcessType);
            Assert.Equal("Third", processes.ElementAt(2).DataProcessType);
            Assert.Equal(1, processes.ElementAt(0).Order);
            Assert.Equal(2, processes.ElementAt(1).Order);
            Assert.Equal(3, processes.ElementAt(2).Order);
        }

        // Test if switch 1 to 0, changes order of collection.
        [Fact]
        public void TestUpdateSteps()
        {
            // Assert
            Mock<IServerAPI> MockServerAPI = new();
            Mock<IWorkspaceManager> MockManager = new();
            MockManager.Setup(m => m.GetWorkspaceConfigDTO()).Returns(new WorkspaceConfigDTO() { WorkspaceGuid = new Guid("f7ac518c-e92a-487e-9367-37bb38dbb7ac") });
            Services.RegisterUIComponents();
            JSInterop.RegisterPopOverSetup(Services);
            Services.AddSingleton(MockServerAPI.Object);
            Services.AddSingleton(MockManager.Object);
            Services.AddSingleton<ICalculationService, CalculationProxyService>();
            var Component = RenderComponent<DynamicCalculationComponent>();
            KPIProcess firstProcess = new("SelectCalculation") { DataProcessType = "First", CalculationStep = new CalculationStepDTO() { Order = -1, Calculations = [] } };
            KPIProcess secondProcess = new("SelectCalculation") { DataProcessType = "Second", CalculationStep = new CalculationStepDTO() { Order = -1, Calculations = [] } };
            KPIProcess thirdProcess = new("SelectCalculation") { DataProcessType = "Third", CalculationStep = new CalculationStepDTO() { Order = -1, Calculations = [] } };
            // Two elements
            MudItemDropInfo<IDataProcess> FirstInsert = new(firstProcess, "DataManipulator", 0);
            MudItemDropInfo<IDataProcess> SecondInsert = new(secondProcess, "DataManipulator", 1);
            MudItemDropInfo<IDataProcess> ThirdInsert = new(thirdProcess, "DataManipulator", 2);
            // Act
            Component.Instance.InsertIntoDropzone(FirstInsert);
            Component.Instance.InsertIntoDropzone(SecondInsert);
            Component.Instance.InsertIntoDropzone(ThirdInsert);

            var processes = Component.Instance.DataProcesses.Where((proc) => proc.Order > -1);
            Assert.Equal(3, processes.Count());

            // Change zone to DataManipulator since the element is already present.
            MudItemDropInfo<IDataProcess> Update = new(processes.ElementAt(1), "DataManipulator", 0);
            Component.Instance.InsertIntoDropzone(Update);

            // Assert
            var NewProcesses = Component.Instance.DataProcesses.Where((proc) => proc.Order > -1);
            Assert.Equal(3, NewProcesses.Count());
            Assert.Equal("Second", processes.ElementAt(0).DataProcessType);
            Assert.Equal("First", processes.ElementAt(1).DataProcessType);
            Assert.Equal("Third", processes.ElementAt(2).DataProcessType);
            Assert.Equal(1, processes.ElementAt(0).Order);
            Assert.Equal(2, processes.ElementAt(1).Order);
            Assert.Equal(3, processes.ElementAt(2).Order);
        }

        // Test if removing step 1 component reorders to 1, 2
        [Fact]
        public void TestRemovalOfSteps()
        {
            // Assert
            Mock<IServerAPI> MockServerAPI = new();
            Mock<IWorkspaceManager> MockManager = new();
            MockManager.Setup(m => m.GetWorkspaceConfigDTO()).Returns(new WorkspaceConfigDTO() { WorkspaceGuid = new Guid("f7ac518c-e92a-487e-9367-37bb38dbb7ac") });
            Services.RegisterUIComponents();
            JSInterop.RegisterPopOverSetup(Services);
            Services.AddSingleton(MockServerAPI.Object);
            Services.AddSingleton(MockManager.Object);
            Services.AddSingleton<ICalculationService, CalculationProxyService>();
            var Component = RenderComponent<DynamicCalculationComponent>();
            KPIProcess firstProcess = new KPIProcess("SelectCalculation") { DataProcessType = "First", CalculationStep = new CalculationStepDTO() { Order = -1, Calculations = [] } };
            KPIProcess secondProcess = new KPIProcess("SelectCalculation") { DataProcessType = "Second", CalculationStep = new CalculationStepDTO() { Order = -1, Calculations = [] } };
            KPIProcess thirdProcess = new KPIProcess("SelectCalculation") { DataProcessType = "Third", CalculationStep = new CalculationStepDTO() { Order = -1, Calculations = [] } };
            // Two elements
            MudItemDropInfo<IDataProcess> FirstInsert = new(firstProcess, "DataManipulator", 0);
            MudItemDropInfo<IDataProcess> SecondInsert = new(secondProcess, "DataManipulator", 1);
            MudItemDropInfo<IDataProcess> ThirdInsert = new(thirdProcess, "DataManipulator", 2);
            // Act
            Component.Instance.InsertIntoDropzone(FirstInsert);
            Component.Instance.InsertIntoDropzone(SecondInsert);
            Component.Instance.InsertIntoDropzone(ThirdInsert);
            var processes = Component.Instance.DataProcesses.Where((proc) => proc.Order > -1);
            MudItemDropInfo<IDataProcess> Update = new(processes.ElementAt(1), "SelectCalculation", 0);
            Component.Instance.InsertIntoDropzone(Update);
            // Assert
            var NewProcesses = Component.Instance.DataProcesses.Where((proc) => proc.Order > -1);
            Assert.Equal(2, processes.Count());
            Assert.Equal("First", processes.ElementAt(0).DataProcessType);
            Assert.Equal("Third", processes.ElementAt(1).DataProcessType);
        }

        [Fact(Skip = "Not yet implemented")]
        public void TestSubmission()
        {
            Assert.Equal(1, 2);
        }

        [Fact(Skip = "Not yet implemented")]
        public void TestFailedSubmission()
        {
            Assert.Equal(1, 2);
        }
    }
}
