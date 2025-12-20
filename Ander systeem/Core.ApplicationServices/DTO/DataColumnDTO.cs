namespace Leap.ApplicationServices.DTO
{
    public class DataColumnDTO
    {
        public int Id { get; set; } = -1;
        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{ColumnName}";
        }
        public override int GetHashCode()
        {
            return ColumnName.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is DataColumnDTO vt && Equals(vt);
        }

        public bool Equals(DataColumnDTO? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ColumnName == other.ColumnName;
        }
    }
}

