using System;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.TimeOffPolicy;
using Bamboozed.Domain.TimeOffRequest;
using Bamboozed.Domain.User;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

namespace Bamboozed.DAL
{
    public static class ServiceRegistration
    {
        public static IServiceCollection UseDal(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<User>), (_) => RegisterRepository<User>());
            services.AddScoped(typeof(IRepository<TimeOffRequestLog>), (_) => RegisterRepository<TimeOffRequestLog>());
            services.AddScoped(typeof(IRepository<MaxDaysOffPolicy>), (_) => RegisterRepository<MaxDaysOffPolicy>());

            return services;
        }

        private static Repository<T> RegisterRepository<T>() where T : Entity<string>
        {
            var storageAccount =
                CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var tableClient = storageAccount.CreateCloudTableClient();
            return new Repository<T>(tableClient);
        }
    }
}
