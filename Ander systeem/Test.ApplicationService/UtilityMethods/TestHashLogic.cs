using Leap.ApplicationServices.DTO;
using Leap.ApplicationServices.DTO.DataConfig;

namespace Test.ApplicationService.UtilityMethods
{
    public class TestHashLogic
    {
        /// <summary>
        /// Not related to any requirement.
        /// </summary>
        [Fact]
        public void TestEqualsValue()
        {
            ValueTypeDTO TestValue = new ValueTypeDTO() { Id = 1, Name = "Hello_Value" };
            ValueTypeDTO OtherValue = new ValueTypeDTO() { Id = 1, Name = "Hello_Value" };

            SensorDTO TestObservation = new SensorDTO() { Id = 1, Name = "Hello_Sensor" };
            SensorDTO OtherObservation = new SensorDTO() { Id = 1, Name = "Hello_Sensor" };

            DataColumnDTO TestColumn = new DataColumnDTO() { Id = 1, ColumnName = "Hello_Column" };
            DataColumnDTO OtherColumn = new DataColumnDTO() { Id = 1, ColumnName = "Hello_Column" };

            Assert.Equal(TestColumn, OtherColumn);
            Assert.Equal(TestValue, OtherValue);
            Assert.Equal(TestObservation, OtherObservation);
        }

        [Fact]
        public void TestEqualsValue2()
        {
            ValueTypeDTO TestValue = new ValueTypeDTO() { Id = 1, Name = "Hello_Value" };
            ValueTypeDTO OtherValue = new ValueTypeDTO() { Id = 1, Name = "Hello_Value" };

            SensorDTO TestObservation = new SensorDTO() { Id = 1, Name = "Hello_Sensor" };
            SensorDTO OtherObservation = new SensorDTO() { Id = 1, Name = "Hello_Sensor" };

            DataColumnDTO TestColumn = new DataColumnDTO() { Id = 1, ColumnName = "Hello_Column" };
            DataColumnDTO OtherColumn = new DataColumnDTO() { Id = 1, ColumnName = "Hello_Column" };

            Assert.False(TestColumn.Equals(OtherValue));
            Assert.False(TestValue.Equals(OtherObservation));
            Assert.False(TestObservation.Equals(OtherValue));

            Assert.False(TestColumn.Equals((DataColumnDTO?)null));
            Assert.False(TestValue.Equals((ValueTypeDTO?)null));
            Assert.False(TestObservation.Equals((SensorDTO?)null));

            Assert.True(TestValue.Equals(TestValue));
            Assert.True(TestObservation.Equals(TestObservation));
            Assert.True(TestColumn.Equals(TestColumn));
        }

        [Fact]
        public void TestToStringMethods()
        {
            ValueTypeDTO TestValue = new ValueTypeDTO() { Id = 1, Name = "Hello_Value" };
            ValueTypeDTO OtherValue = new ValueTypeDTO() { Id = 1, Name = "Hello_Value" };

            SensorDTO TestObservation = new SensorDTO() { Id = 1, Name = "Hello_Sensor" };
            SensorDTO OtherObservation = new SensorDTO() { Id = 1, Name = "Hello_Sensor" };

            DataColumnDTO TestColumn = new DataColumnDTO() { Id = 1, ColumnName = "Hello_Column" };
            DataColumnDTO OtherColumn = new DataColumnDTO() { Id = 1, ColumnName = "Hello_Column" };
            Assert.Equal(TestColumn.ToString(), OtherColumn.ToString());
            Assert.Equal(TestValue.ToString(), OtherValue.ToString());
            Assert.Equal(TestObservation.ToString(), OtherObservation.ToString());
        }

        [Fact]
        public void TestHashCodes()
        {
            ValueTypeDTO TestValue = new ValueTypeDTO() { Id = 201, Name = "Hello_Value" };
            ValueTypeDTO OtherValue = new ValueTypeDTO() { Id = 201, Name = "Hello_Value" };
            SensorDTO TestObservation = new SensorDTO() { Id = 101, Name = "Hello_Sensor" };
            SensorDTO OtherObservation = new SensorDTO() { Id = 101, Name = "Hello_Sensor" };
            DataColumnDTO TestColumn = new DataColumnDTO() { Id = 51, ColumnName = "Hello_Column" };
            DataColumnDTO OtherColumn = new DataColumnDTO() { Id = 51, ColumnName = "Hello_Column" };
            Assert.Equal(OtherValue.GetHashCode(), TestValue.GetHashCode());
            Assert.Equal(OtherObservation.GetHashCode(), TestObservation.GetHashCode());
            Assert.Equal(OtherColumn.GetHashCode(), TestColumn.GetHashCode());
        }
    }
}
