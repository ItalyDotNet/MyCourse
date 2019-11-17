using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IDatabaseAccessor
    {
        IAsyncEnumerable<IDataRecord> QueryAsync(FormattableString query);
    }
}