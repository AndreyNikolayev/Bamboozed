using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Bamboozed.DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : Entity<string>
    {
        private readonly CloudTableClient _tableClient;
        private const string PartitionKey = "Bamboozed";

        public Repository(CloudTableClient tableClient)
        {
            _tableClient = tableClient;
        }

        public Task Add(T entity)
        {
            var adapter = new TableEntityAdapter<T>(entity, PartitionKey, entity.Id)
            {
                Timestamp = DateTimeOffset.UtcNow
            };
            return Execute(TableOperation.Insert(adapter));
        }

        public Task Delete(T entity)
        {
            var adapter = new TableEntityAdapter<T>(entity, PartitionKey, entity.Id)
            {
                ETag = "*"
            };
            return Execute(TableOperation.Delete(adapter));
        }

        public Task Edit(T entity)
        {
            var adapter = new TableEntityAdapter<T>(entity, PartitionKey, entity.Id)
            {
                Timestamp = DateTimeOffset.UtcNow,
                ETag = "*"
            };
            return Execute(TableOperation.Replace(adapter));
        }

        private async Task Execute(TableOperation operation)
        {
            var table = await GetTable();
            await table.ExecuteAsync(operation);
        }

        public async Task<Maybe<T>> GetById(string id)
        {
            var entities = await Get(id);

            return entities.FirstOrDefault();
        }

        public Task<IEnumerable<T>> Get()
        {
            return Get(null);
        }

        private Task<IEnumerable<T>> Get(string id)
        {
            var query = new TableQuery();

            var filterConditions = new List<string>
            {
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey)
            };

            if (id != null)
            {
                filterConditions.Add(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id));
            }

            var whereClause = string.Join(" and ", filterConditions.Select(p => $"({p})"));

            query = query.Where(whereClause);

            return ExecuteQuery(query);
        }

        private async Task<IEnumerable<T>> ExecuteQuery(TableQuery query)
        {
            var table = await GetTable();
            var result = new List<T>();

            TableContinuationToken continuationToken = null;

            do
            {
                var tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                result.AddRange(tableQueryResult.Results
                    .Select(p => 
                        TableEntity.ConvertBack<T>(p.Properties, new OperationContext())
                        )
                );

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
