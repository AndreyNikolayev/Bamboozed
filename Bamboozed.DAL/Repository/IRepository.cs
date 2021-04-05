using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Bamboozed.DAL.Repository
{
    public interface IRepository<T> where T : Entity<string>
    {
        Task Add(T entity);
        Task Edit(T entity);
        Task Delete(T entity);
        Task<IEnumerable<T>> Get(FilterRequest filterRequest = null);
        Task<IEnumerable<T>> Get(string partitionKey);
        Task<T> Get(string partitionKey, string rowKey);
        Task<T> First(FilterRequest filterRequest = null);
        Task<T> First(string partitionKey);
        Task<T> FirstOrDefault(FilterRequest filterRequest = null);
        Task<T> FirstOrDefault(string partitionKey);
    }
}
