using Bamboozed.Domain.Extensions;
using Bamboozed.Domain.TimeOffRequest;

namespace Bamboozed.Application.Extensions
{
    public static class DomainExtensions
    {
        public static string GetApprovedMessage(this TimeOffRequest request)
        {
            var periodDescription = request.StartDate.ToString("ddd, MMM d") + (request.EndDate.Equals(request.StartDate)
                ? ""
                : $"-{request.EndDate:ddd, MMM d}");

            return $"{request.RequestorName} {request.TimeOffType.GetDescription()} on {periodDescription} is approved";
        }

        public static string GetReviewMessage(this TimeOffRequest request)
        {
            var periodDescription = request.StartDate.ToString("ddd, MMM d") + (request.EndDate.Equals(request.StartDate)
                ? ""
                : $"-{request.EndDate:ddd, MMM d}");

            var link = $"<a href='{request.ReviewLink}'>Review link</a>";

            return $"{request.RequestorName} requested {request.TimeOffType.GetDescription()} on {periodDescription}. {link}";
        }
    }
}
