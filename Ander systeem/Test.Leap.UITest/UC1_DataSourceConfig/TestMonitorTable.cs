using Bunit;
using Leap.ApplicationServices.DTO.External_Services;
using LeapDataScienceTool.Components.DataSourceProcess.MonitorInfoTable;
using MudBlazor;
using MudBlazor.Services;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC1_DataSourceConfig
{
    public class TestMonitorTable : TestContext
    {
        private void Load()
        {
            Services.RegisterUIComponents();
            JSInterop.SetupVoid("mudPopover.initialize", "mud-popover-provider", 0, 24);
            JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            Services.AddMudServices();
            JSInterop.Setup<int>("mudpopoverHelper.countProviders");
            Services.AddMudPopoverService();
        }
        // BZ-6
        [Fact]
        public void TestMonitorTableCreation()
        {
            Load();
            RenderComponent<MudPopoverProvider>();
            MonitorInfoDTO DataSet = new()
            {
                Observations = [
                     new () { Id = 1, Name = "SW-4_DZ", ValueTypeIds = [101, 104] },
                     new() { Id = 2, Name = "SW-5_DY", ValueTypeIds = [104, 106], },
                     new () { Id = 3, Name = "SW-4_DY", ValueTypeIds = [101, 106] },
                     new() { Id = 4, Name = "SW-5_DX", ValueTypeIds = [104], },
                 ],
                Valuetypes = [
                     new() {
                         Id = 101,
                         Name = "DZ",
                         Quantity = "mm",
                         UnitAbbr = "mm"
                     },
                     new() {
                         Id = 104,
                         Name = "Dy",
                         Quantity = "mm",
                     },
                     new() {
                         Id = 106,
                         Name = "Temperatuur",
                         Quantity = "C", }
                         ],
                TimeLevels = [],
            };

            var monitorInfoTable = RenderComponent<MonitorInfoTable>(param =>
            {
                param.Add(monitor => monitor.monitorInfoDTO, DataSet);
            });

            MonitorInfoTable tableComponent = monitorInfoTable.Instance;

            Assert.Equal(1, tableComponent.ObservationValuetypes.ElementAt(0).Id);
            Assert.Equal(101, tableComponent.ObservationValuetypes.ElementAt(0).ValueTypes.ElementAt(0).Id);
            Assert.Equal("DZ", tableComponent.ObservationValuetypes.ElementAt(0).ValueTypes.ElementAt(0).Name);
            Assert.Equal(1, tableComponent.ObservationValuetypes.ElementAt(0).Id);
            Assert.Equal(104, tableComponent.ObservationValuetypes.ElementAt(0).ValueTypes.ElementAt(1).Id);
            Assert.Equal("Dy", tableComponent.ObservationValuetypes.ElementAt(0).ValueTypes.ElementAt(1).Name);
        }
    }
}
