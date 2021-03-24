using System;
using Bamboozed.Application.Enums;
using Bamboozed.Application.Extensions;

namespace Bamboozed.Application.Entities
{
    public class TimeOffRequest
    {
        public string ApproverEmail { get; set; }
        public string RequestorName { get; set; }
        public string ApproverName { get; set; }
        public TimeOffType TimeOffType { get; set; }
        public DateTime StartDate { get;set; }
        public DateTime EndDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string ApproveLink { get; set; }
        public string DenyLink { get; set; }
        public string ReviewLink { get; set; }

        public string ApprovedMessage
        {
            get
            {
                var periodDescription = StartDate.ToString("ddd, MMM d") + (EndDate.Equals(StartDate)
                    ? ""
                    : $"-{EndDate:ddd, MMM d}");

                return $"{RequestorName} {TimeOffType.GetDescription()} on {periodDescription} is approved by {ApproverName}";
            }
        }
    }
}
