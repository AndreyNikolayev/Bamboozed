using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Bamboozed.Domain.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<Result<T>> ToResultTask<T>(this Task<T> task)
        {
            var result = await task.ConfigureAwait(Result.DefaultConfigureAwait);

            return Result.Success(result);
        }
    }
}
