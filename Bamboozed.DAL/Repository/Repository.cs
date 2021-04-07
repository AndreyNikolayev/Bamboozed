using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bamboozed.Domain.Attributes;
using CSharpFunctionalExtensions;
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

                result.AddRange(tableQueryResult.Results.Select(MapDynamicEntity));

                continuationToken = tableQueryResult.ContinuationToken;
            } while (continuationToken != null);

            return result;
        }

        private T MapDynamicEntity(DynamicTableEntity entity)
        {
            var constructors = typeof(T).GetConstructors();
            var targetConstructor = constructors.Length == 1
                ? constructors.First()
                : constructors.SingleOrDefault(p => p.GetCustomAttribute<MappedConstructorAttribute>() != null);

            if (targetConstructor == null)
            {
                throw new Exception($"Could not choose mapping constructor for type {typeof(T).Name}");
            }

            var ctorParameters = GetParameters(targetConstructor.GetParameters(), entity);


            return (T)targetConstructor.Invoke(ctorParameters.ToArray());
        }

        private static IEnumerable<object> GetParameters(IEnumerable<ParameterInfo> parameterInfos, DynamicTableEntity entity)
        {
            foreach (var parameterInfo in parameterInfos)
            {
                var dynamicPropertyKey = entity.Properties.Keys.FirstOrDefault(p =>
                    p.Equals(parameterInfo.Name, StringComparison.InvariantCultureIgnoreCase));

                if (dynamicPropertyKey == null)
                {
                    throw new Exception($"Could not map {parameterInfo.Name} constructor parameter for type {typeof(T).Name}");
                }

                yield return ChangeType(entity.Properties[dynamicPropertyKey].PropertyAsObject, parameterInfo.ParameterType);
            }
        }

        private static object ChangeType(object propertyValue, Type propertyType)
        {
            var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (type.GetTypeInfo().IsEnum)
                return Enum.Parse(type, propertyValue.ToString());
            if (type == typeof(DateTimeOffset))
                return new DateTimeOffset((DateTime)propertyValue);
            if (type == typeof(TimeSpan))
                return TimeSpan.Parse(propertyValue.ToString(), CultureInfo.InvariantCulture);
            if (type == typeof(uint))
                return (uint)(int)propertyValue;
            if (type == typeof(ulong))
                return (ulong)(long)propertyValue;
            return type == typeof(byte) ? ((byte[])propertyValue)[0] : Convert.ChangeType(propertyValue, type, CultureInfo.InvariantCulture);
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
