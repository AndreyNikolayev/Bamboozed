using System;
using CSharpFunctionalExtensions;

namespace Bamboozed.Domain.TimeOffRequest
{
    public class TimeOffRequestLog: Entity<string>
    {
        public string ApproverEmail { get; private set; }
        public string RequestorName { get; private set; }
        public TimeOffType TimeOffType { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime RequestDate { get; private set; }
        public TimeOffAction Status { get; private set; }

        public TimeOffRequestLog() { }

        public TimeOffRequestLog(TimeOffRequest request, TimeOffAction status): base(Guid.NewGuid().ToString())
        {
            ApproverEmail = request.ApproverEmail;
            RequestorName = request.RequestorName;
            TimeOffType = request.TimeOffType;
            StartDate = request.StartDate;
            EndDate = request.EndDate;
            RequestDate = request.RequestDate;
            Status = status;
        }
    }

    public enum TimeOffAction
    {
        Approve,
        Review,
        Deny
    }
}
