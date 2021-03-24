using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Bamboozed.Application.Enums
{
    public enum TimeOffType
    {
        [Description("Sick-leave")]
        SickLeave,
        [Description("Day-off")]
        DayOff,
        Vacation
    }
}
