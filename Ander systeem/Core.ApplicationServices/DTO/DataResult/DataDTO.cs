namespace Leap.ApplicationServices.DTO.DataResult
{
    // Abstract data class for receiving data from Python service.
    public class DataDTO : IDTO
    {
        // DataSet is the string representation of the JSON object.
        public DataSeries? DataSet { get; set; }
        public IEnumerable<DataColumnDTO> DataColumns { get; set; } = [];
    }

    // Response class from Python service for receiving preview data.
    public class PreviewDataDTO : DataDTO
    {
        // Holds the Columns of the data
        public int DataCount { get; set; } = 0;
    }

    public class ModelResultDataDTO : DataDTO
    {
        // Testvalidation set
        public required DataSeries PredictionSet { get; set; }
        public required Dictionary<string, IEnumerable<MetricsDTO>> MetricsKeyValue { get; set; }
    }
}
