using Microsoft.AspNetCore.Components;

namespace LeapDataScienceTool.Common.Abstractions
{
    public abstract class SynchronizerComponent : ComponentBase
    {
        public abstract void Synchronize(EventArgs args);
    }
}
