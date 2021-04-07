using System;
using Bamboozed.Domain.Attributes;
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

        [MappedConstructor]
        public TimeOffRequestLog(string id, string approverEmail, string requestorName, TimeOffType timeOffType,
            DateTime startDate, DateTime endDate, DateTime requestDate, TimeOffAction status) : base(id)
        {
            ApproverEmail = approverEmail;
            RequestorName = requestorName;
            TimeOffType = timeOffType;
            StartDate = startDate;
            EndDate = endDate;
            RequestDate = requestDate;
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
