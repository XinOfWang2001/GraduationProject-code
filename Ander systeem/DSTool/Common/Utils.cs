using System.ComponentModel;
using System.Reflection;
namespace LeapDataScienceTool.Common
{
    public static class Utils
    {

        public static string GetDescription<T>(this T source)
        {
            try
            {
                FieldInfo info = source.GetType().GetField(source.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return attributes[0].Description;
            }
            catch
            {
                return source.ToString();
            }
        }
    }
}
