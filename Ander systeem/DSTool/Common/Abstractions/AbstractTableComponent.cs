using Microsoft.AspNetCore.Components;

namespace LeapDataScienceTool.Common.Abstractions
{
    public abstract class AbstractTableComponent : ComponentBase
    {
        protected abstract void BuildTable();
    }
}
