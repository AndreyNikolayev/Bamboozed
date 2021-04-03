using System.ComponentModel;

namespace Bamboozed.Application.Enums
{
    public enum TimeOffType
    {
        [Description("Sick-leave")]
        SickLeave,
        [Description("Day-Off")]
        DayOff,
        Vacation
    }
}
