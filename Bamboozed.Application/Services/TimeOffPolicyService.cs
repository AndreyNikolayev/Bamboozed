using System;
using System.Linq;
using System.Threading.Tasks;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.TimeOffPolicy;
using Bamboozed.Domain.TimeOffRequest;

namespace Bamboozed.Application.Services
{
    public class TimeOffPolicyService
    {
        private readonly IRepository<MaxDaysOffPolicy> _maxDaysOffPolicyRepository;
        private readonly IRepository<TimeOffRequestLog> _timeOffRequestLog;

        public TimeOffPolicyService(
            IRepository<MaxDaysOffPolicy> maxDaysOffPolicyRepository,
            IRepository<TimeOffRequestLog> timeOffRequestLog)
        {
            _maxDaysOffPolicyRepository = maxDaysOffPolicyRepository;
            _timeOffRequestLog = timeOffRequestLog;
        }

        public async Task<TimeOffAction> GetAction(TimeOffRequest request)
        {
            var maxDaysTimeOffPolicies = (await _maxDaysOffPolicyRepository.Get())
                .Where(p => p.UserEmail == request.ApproverEmail && p.TimeOffType == request.TimeOffType)
                .OrderBy(p => p.MaxDays)
                .ToList();

            if (!maxDaysTimeOffPolicies.Any())
            {
                return TimeOffAction.Review;
            }

            var consecutiveLength = await GetConsecutiveLength(request);

            foreach (var maxDaysTimeOffPolicy in maxDaysTimeOffPolicies
                .Where(maxDaysTimeOffPolicy => consecutiveLength <= maxDaysTimeOffPolicy.MaxDays))
            {
                return maxDaysTimeOffPolicy.Action;
            }

            return TimeOffAction.Review;
        }

        private async Task<int> GetConsecutiveLength(TimeOffRequest request)
        {
            var requestorLogs = (await _timeOffRequestLog.Get())
                .Where(p => p.RequestorName == request.ApproverName
                            && p.ApproverEmail == request.ApproverName
                            && p.TimeOffType == request.TimeOffType
                            && p.Status != TimeOffAction.Deny
                )
                .OrderBy(p => p.StartDate)
                .ToList();

            var consecutiveStartDate = request.StartDate;
            var consecutiveEndDate = request.EndDate;

            foreach (var log in requestorLogs)
            {
                if (log.StartDate.Equals(consecutiveEndDate.AddDays(1)))
                {
                    consecutiveEndDate = log.EndDate;
                }
            }

            foreach (var log in requestorLogs.OrderByDescending(p => p.EndDate))
            {
                if (log.EndDate.Equals(consecutiveStartDate.AddDays(-1)))
                {
                    consecutiveStartDate = log.StartDate;
                }
            }

            return Convert.ToInt32((consecutiveEndDate - consecutiveStartDate).TotalDays) + 1;
        }
    }
}
