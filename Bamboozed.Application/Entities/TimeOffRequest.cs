using System;
using Bamboozed.Application.Enums;

namespace Bamboozed.Application.Entities
{
    public class TimeOffRequest
    {
        public string RequestorEmail { get; set; }
        public string RequestorName { get; set; }
        public string ApproverEmail { get; set; }
        public string ApproverName { get; set; }
        public TimeOffType TimeOffType { get; set; }
        public DateTime StartDate { get;set; }
        public DateTime EndDate { get; set; }
        public DateTime RequestDate { get; set; }

        public string ApprovedMessage =>
            $"{RequestorName} {TimeOffType} on {StartDate:ddd d MMM}-{EndDate:ddd d MMM} is approved by {ApproverName}";
    }
}
