using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bamboozed.Application.Entities;
using Bamboozed.Application.Enums;
using Bamboozed.Application.Extensions;
using Bamboozed.Application.Interfaces;
using MimeKit;

namespace Bamboozed.Application.Services
{
    public class RequestParser: IRequestParser
    {
        private readonly Regex _requestorNameRegex = new Regex("^Subject: (.*) requested time off", RegexOptions.Multiline);
        private readonly Regex _approverNameRegex = new Regex("^To: (.*) <", RegexOptions.Multiline);
        private readonly Regex _timeOffTypeRegex = new Regex("^\\d+ days? of (\\S*).$", RegexOptions.Multiline);
        private readonly Regex _datesRegex = new Regex("^(\\w{3}, \\w{3} \\d{1,2})(?: - )?(\\w{3}, \\w{3} \\d{1,2})?.$", RegexOptions.Multiline);
        private readonly Regex _requestDateRegex = new Regex("^Date: (\\w{3}, \\w{3} \\d{1,2})", RegexOptions.Multiline);
        private readonly Regex _approveLinkRegex = new Regex("Approve.{1,2}<([^>]*)>", RegexOptions.Singleline);
        private readonly Regex _denyLinkRegex = new Regex("Deny.{1,2}<([^>]*)>", RegexOptions.Singleline);
        private readonly Regex _reviewLinkRegex = new Regex("Review this request in BambooHR.{1,2}<([^>]*)>", RegexOptions.Singleline);
        private const string DateFormat = "ddd, MMM d";

        public TimeOffRequest ParseRequest(MimeMessage message)
        {
            var body = message.TextBody;
            var datesMatch = _datesRegex.Match(body);

            return new TimeOffRequest
            {
                ApproverEmail = message.From.Mailboxes.First().Address,
                RequestorName = _requestorNameRegex.Match(body).Groups[1].Value,
                ApproverName = _approverNameRegex.Match(body).Groups[1].Value,
                ApproveLink = _approveLinkRegex.Match(body).Groups[1].Value,
                DenyLink = _denyLinkRegex.Match(body).Groups[1].Value,
                ReviewLink = _reviewLinkRegex.Match(body).Groups[1].Value,
                TimeOffType = _timeOffTypeRegex.Match(body).Groups[1].Value.ParseEnumByDescription<TimeOffType>(),
                RequestDate = DateTime.ParseExact(_requestDateRegex.Match(body).Groups[1].Value, DateFormat, CultureInfo.InvariantCulture),
                StartDate = DateTime.ParseExact(datesMatch.Groups[1].Value, DateFormat, CultureInfo.InvariantCulture),
                EndDate = DateTime.ParseExact(String.IsNullOrEmpty(datesMatch.Groups[2].Value) ? datesMatch.Groups[1].Value: datesMatch.Groups[2].Value, DateFormat, CultureInfo.InvariantCulture)
            };
        }
    }
}
