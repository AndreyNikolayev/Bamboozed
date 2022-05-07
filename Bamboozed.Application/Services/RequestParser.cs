using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Bamboozed.Domain.Extensions;
using Bamboozed.Domain.TimeOffRequest;
using MimeKit;

namespace Bamboozed.Application.Services
{
    public class RequestParser
    {
        private readonly Regex _requestorNameRegex = new Regex("<title>(.*) requested time off[^<]*</title>");
        private readonly Regex _timeOffTypeRegex = new Regex("\\d+ days? of ([^\\n]*)");
        private readonly Regex _datesRegex = new Regex("(\\w{3}, \\w{3} \\d{1,2})(?: - )?(\\w{3}, \\w{3} \\d{1,2})?", RegexOptions.Multiline);
        private readonly Regex _approveLinkRegex = new Regex("<a[^>]*href=\"([^\"]*)\"[^>]*>Approve", RegexOptions.Singleline);
        private readonly Regex _denyLinkRegex = new Regex("<a[^>]*href=\"([^\"]*)\"[^>]*>Deny", RegexOptions.Singleline);
        private readonly Regex _reviewLinkRegex = new Regex("<a[^>]*href=\"([^\"]*)\"[^>]*>\\s*Review this request in BambooHR", RegexOptions.Singleline);
        private const string DateFormat = "ddd, MMM d";

        public TimeOffRequest ParseRequest(MimeMessage message)
        {
            var body = message.HtmlBody;
            var datesMatch = _datesRegex.Match(body);

            var lol = _timeOffTypeRegex.Match(body).Groups[1].Value;

            return new TimeOffRequest
            (
                message.To.Mailboxes.First().Address,
                _requestorNameRegex.Match(body).Groups[1].Value,
                message.To.Mailboxes.First().Name,
                _timeOffTypeRegex.Match(body).Groups[1].Value.ParseEnumByDescription<TimeOffType>(),
                DateTime.ParseExact(datesMatch.Groups[1].Value, DateFormat, CultureInfo.InvariantCulture),
                DateTime.ParseExact(string.IsNullOrEmpty(datesMatch.Groups[2].Value) ? datesMatch.Groups[1].Value : datesMatch.Groups[2].Value, DateFormat, CultureInfo.InvariantCulture),
                message.Date.Date,
                _approveLinkRegex.Match(body).Groups[1].Value,
                _denyLinkRegex.Match(body).Groups[1].Value,
                 _reviewLinkRegex.Match(body).Groups[1].Value
            );
        }
    }
}
