using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Bamboozed.DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : ITableEntity, new()
    {
        private readonly CloudTableClient _tableClient;

        public Repository(CloudTableClient tableClient)
        {
            _tableClient = tableClient;
        }

        public Task Add(T entity)
        {
            entity.Timestamp = DateTimeOffset.UtcNow;
            return Execute(TableOperation.Insert(entity));
        }

        public Task Delete(T entity)
        {
            return Execute(TableOperation.Delete(entity));
        }

        public Task Edit(T entity)
        {
            entity.Timestamp = DateTimeOffset.UtcNow;
            return Execute(TableOperation.Replace(entity));
        }

        private async Task Execute(TableOperation operation)
        {
            var table = await GetTable();
            await table.ExecuteAsync(operation);
        }

        public async Task<T> Get(string partitionKey, string rowKey)
        {
            var query = new TableQuery<T>().Where(
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey)
                ));

            var results = await ExecuteQuery(query);

            return results.FirstOrDefault();
        }

        public async Task<T> First(FilterRequest filterRequest = null)
        {
            var result = await Get(filterRequest);
            return result.First();
        }

        public async Task<T> First(string partitionKey)
        {
            var result = await Get(partitionKey);
            return result.First();
        }

        public async Task<T> FirstOrDefault(FilterRequest filterRequest = null)
        {
            var result = await Get(filterRequest);
            return result.FirstOrDefault();
        }

        public async Task<T> FirstOrDefault(string partitionKey)
        {
            var result = await Get(partitionKey);
            return result.FirstOrDefault();
        }

        public Task<IEnumerable<T>> Get(string partitionKey)
        {
            return Get(new FilterRequest(partitionKey));
        }

        public Task<IEnumerable<T>> Get(FilterRequest filterRequest = null)
        {
            var query = new TableQuery<T>();

            if (filterRequest == null)
            {
                return ExecuteQuery(query);
            }

            var filterConditions = new List<string>();

            if (filterRequest.PartitionKey != null)
            {
                filterConditions.Add(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, filterRequest.PartitionKey));
            }

            if (filterRequest.RowKey != null)
            {
                filterConditions.Add(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, filterRequest.RowKey));
            }

            var whereClause = string.Join(" and ", filterConditions.Select(p => $"({p})"));

            query = query.Where(whereClause);

            return ExecuteQuery(query);
        }

        private async Task<IEnumerable<T>> ExecuteQuery(TableQuery<T> query)
        {
            var table = await GetTable();
            var result = new List<T>();

            TableContinuationToken continuationToken = null;
            do
            {
                var tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                result.AddRange(tableQueryResult.Results);

                continuationToken = tableQueryResult.ContinuationToken;
            } while (continuationToken != null);

            return result;
        }

        private async Task<CloudTable> GetTable()
        {
            var table = _tableClient.GetTableReference(typeof(T).Name);
            if (!await table.ExistsAsync())
            {
                await table.CreateAsync();
            }
            return table;
        }
    }
}
