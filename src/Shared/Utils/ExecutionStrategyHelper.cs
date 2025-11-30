using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;

namespace Sphera.API.Shared.Utils;

public static class ExecutionStrategyHelper
{
    public static async Task<T> ExecuteAsync<T>(SpheraDbContext dbContext, Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () => await operation());
    }
}
