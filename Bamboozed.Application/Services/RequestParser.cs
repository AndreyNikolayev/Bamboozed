using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Bamboozed.Application.Entities;
using Bamboozed.Application.Enums;
using Bamboozed.Application.Extensions;
using MimeKit;

namespace Bamboozed.Application.Services
{
    public class RequestParser
    {
        private readonly Regex _requestorNameRegex = new Regex("<title>(.*) requested time off[^<]*</title>");
        private readonly Regex _timeOffTypeRegex = new Regex("\\d+ days? of (\\S*)");
        private readonly Regex _datesRegex = new Regex("(\\w{3}, \\w{3} \\d{1,2})(?: - )?(\\w{3}, \\w{3} \\d{1,2})?", RegexOptions.Multiline);
        private readonly Regex _approveLinkRegex = new Regex("<a[^>]*href=\"([^\"]*)\"[^>]*>Approve", RegexOptions.Singleline);
        private readonly Regex _denyLinkRegex = new Regex("<a[^>]*href=\"([^\"]*)\"[^>]*>Deny", RegexOptions.Singleline);
        private readonly Regex _reviewLinkRegex = new Regex("<a[^>]*href=\"([^\"]*)\"[^>]*>\\s*Review this request in BambooHR", RegexOptions.Singleline);
        private const string DateFormat = "ddd, MMM d";

        public TimeOffRequest ParseRequest(MimeMessage message)
        {
            var body = message.HtmlBody;
            var datesMatch = _datesRegex.Match(body);

            return new TimeOffRequest
            {
                ApproverEmail = message.To.Mailboxes.First().Address,
                ApproverName = message.To.Mailboxes.First().Name,
                RequestorName = _requestorNameRegex.Match(body).Groups[1].Value,
                TimeOffType = _timeOffTypeRegex.Match(body).Groups[1].Value.ParseEnumByDescription<TimeOffType>(),
                ApproveLink = _approveLinkRegex.Match(body).Groups[1].Value,
                DenyLink = _denyLinkRegex.Match(body).Groups[1].Value,
                ReviewLink = _reviewLinkRegex.Match(body).Groups[1].Value,
                RequestDate = message.Date.Date,
                StartDate = DateTime.ParseExact(datesMatch.Groups[1].Value, DateFormat, CultureInfo.InvariantCulture),
                EndDate = DateTime.ParseExact(String.IsNullOrEmpty(datesMatch.Groups[2].Value) ? datesMatch.Groups[1].Value: datesMatch.Groups[2].Value, DateFormat, CultureInfo.InvariantCulture)
            };
        }
    }
}
