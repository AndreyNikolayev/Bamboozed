using System;
using System.ComponentModel;
using Bamboozed.Domain.Extensions;

namespace Bamboozed.Domain.TimeOffRequest
{
    public class TimeOffRequest
    {
        public string ApproverEmail { get; }
        public string RequestorName { get; }
        public string ApproverName { get; }
        public TimeOffType TimeOffType { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public DateTime RequestDate { get; }
        public string ApproveLink { get; }
        public string DenyLink { get; }
        public string ReviewLink { get; }
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

        public TimeOffRequest(string approverEmail, string requestorName, string approverName, TimeOffType timeOffType,
            DateTime startDate, DateTime endDate, DateTime requestDate, string approveLink, string denyLink,
            string reviewLink)
        {
            if (string.IsNullOrWhiteSpace(approverEmail))
            {
                throw new ArgumentException($"{nameof(approverEmail)} cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(requestorName))
            {
                throw new ArgumentException($"{nameof(requestorName)} cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(approverName))
            {
                throw new ArgumentException($"{nameof(approverName)} cannot be empty");
            }
            if (timeOffType == TimeOffType.None)
            {
                throw new ArgumentException($"{nameof(timeOffType)} cannot be none");
            }
            if (startDate == DateTime.MinValue)
            {
                throw new ArgumentException($"{nameof(startDate)} cannot be emoty");
            }
            if (endDate == DateTime.MinValue)
            {
                throw new ArgumentException($"{nameof(endDate)} cannot be empty");
            }
            if (requestDate == DateTime.MinValue)
            {
                throw new ArgumentException($"{nameof(requestDate)} cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(approveLink))
            {
                throw new ArgumentException($"{nameof(approveLink)} cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(denyLink))
            {
                throw new ArgumentException($"{nameof(denyLink)} cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(reviewLink))
            {
                throw new ArgumentException($"{nameof(reviewLink)} cannot be empty");
            }

            if (startDate > endDate)
            {
                throw new ArgumentException($"{nameof(startDate)} cannot be after {nameof(endDate)}");
            }

            ApproverEmail = approverEmail;
            RequestorName = requestorName;
            ApproverName = approverName;
            TimeOffType = timeOffType;
            StartDate = startDate;
            EndDate = endDate;
            RequestDate = requestDate;
            ApproveLink = approveLink;
            DenyLink = denyLink;
            ReviewLink = reviewLink;
        }
    }

    public enum TimeOffType
    {
        None,
        [Description("Sick-leave")]
        SickLeave,
        [Description("Day-Off")]
        DayOff,
        Vacation
    }
}
