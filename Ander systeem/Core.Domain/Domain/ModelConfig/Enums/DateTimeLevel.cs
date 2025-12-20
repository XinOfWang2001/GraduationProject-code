using System.ComponentModel;

namespace Leap.Domain.Domain.ModelConfig.Enums
{
    public enum DateTimeLevel
    {
        [Description("Standard: Jaar, Maand, Dag, Uur, Werkdag, Dag van het jaar")]
        STANDARD = 0,

        [Description("Only_Dates: Jaar, Maand, Dag")]
        ONLY_DATES = 1,
    }
}
