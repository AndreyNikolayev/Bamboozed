using System;
using Bamboozed.DAL.Entities;
using Bamboozed.DAL.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Bamboozed.DAL
{
    public static class ServiceRegistration
    {
        public static IServiceCollection UseDal(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<User>), (_) => RegisterRepository<User>());

            return services;
        }

        private static Repository<T> RegisterRepository<T>() where T : ITableEntity, new()
        {
            var storageAccount =
                CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var tableClient = storageAccount.CreateCloudTableClient();
            return new Repository<T>(tableClient);
        }
    }
}
