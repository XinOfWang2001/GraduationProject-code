using Bunit;
using Leap.ApplicationServices.DTO.DataResult;
using LeapDataScienceTool.Common.Components.Visualisations;

namespace Test.Leap.UITest.UC4_ModelTraining
{
    public class TestForecastGraphs : TestContext
    {
        /// <summary>
        /// BZ-24
        /// Functional requirement: Forecast data generating
        /// Testcase: Show graph with data
        /// Expected result: An original dataset with all of its elements, an prediction line, with it sline, 
        /// but with the last element of the original data, to simulate an connection between graphs.
        /// </summary>
        [Fact]
        public void TestForecastingGraphLoad()
        {
            DataSeries Original = new()
            {
                ColumnNames = ["X-1"],
                Timestamps = [
                    new DateTime(2020, 1, 1,13,00, 00),
                    new DateTime(2020, 1,2, 13, 0, 0)],
                Values = new() {
                    { "X-1", [40.0f, 50.0f] }
                }
            };
            DataSeries Predicted = new()
            {
                ColumnNames = ["X-1"],
                Timestamps = [
                    new DateTime(2020, 1, 3,13, 00, 00),
                    new DateTime(2020, 1, 4, 13, 0, 0)],
                Values = new() {
                    { "X-1", [55.0f, 60.0f] }
                }
            };

            var Graph = new ForecastingLineGraph()
            {
                ForecastData = Predicted,
                OriginalData = Original

            };
            Graph.FormatForecastData(Graph.OriginalData, Graph.ForecastData);

            var MyPlot = Graph.MyPlot;

            var scatterPlots = MyPlot.Plot.GetPlottables()
                .OfType<ScottPlot.Plottables.Scatter>()
                .ToList();

            var FirstPlot = scatterPlots.ElementAt(0);
            var SecondPlot = scatterPlots.ElementAt(1);

            var OriginalData = FirstPlot.Data.GetScatterPoints();
            var PredictedData = SecondPlot.Data.GetScatterPoints();

            Assert.Equal(2, OriginalData.Count);
            Assert.Equal(2, PredictedData.Count);
        }
    }
}
