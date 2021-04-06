using System;
using Bamboozed.Domain.TimeOffRequest;

namespace Bamboozed.Domain.TimeOffPolicy
{
    public class MaxDaysOffPolicy: UserTimeOffPolicy
    {
        public int MaxDays { get; private set; }

        public MaxDaysOffPolicy(): base() { }

        public MaxDaysOffPolicy(string userEmail, TimeOffAction action, TimeOffType timeOffType, int maxDays)
            : base(userEmail, action, timeOffType)
        {
            if (maxDays <=0)
            {
                throw new ArgumentException($"{maxDays} cannot be less than 1");
            }

            MaxDays = maxDays;
        }
    }
}
