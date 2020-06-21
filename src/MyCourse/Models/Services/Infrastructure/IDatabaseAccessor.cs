using System;
using System.Data;
using System.Threading.Tasks;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IDatabaseAccessor
    {
        Task<DataSet> QueryAsync(FormattableString formattableQuery);
        Task<T> QueryScalarAsync<T>(FormattableString formattableQuery);
        Task<int> CommandAsync(FormattableString formattableCommand);
    }
}