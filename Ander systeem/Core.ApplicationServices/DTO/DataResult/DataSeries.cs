namespace Leap.ApplicationServices.DTO.DataResult
{
    public class DataSeries
    {
        public required IEnumerable<DateTime> Timestamps { get; set; }
        public required Dictionary<string, IEnumerable<float>> Values { get; set; }
        public required IEnumerable<string> ColumnNames { get; set; }
    }
}
