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
        Task<IEnumerable<T>> Get();
        Task<Maybe<T>> GetById(string id);
    }
}
