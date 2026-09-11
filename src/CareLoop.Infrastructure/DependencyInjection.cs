using CareLoop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace CareLoop.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(
        this IHostApplicationBuilder builder)
    {
        // builder.AddNpgsqlDbContext<CareLoopDbContext>(
        //     "careloopdb");
        
        builder.AddNpgsqlDbContext<CareLoopDbContext>(
            "careloopdb",
            configureDbContextOptions: options =>
                options.UseSnakeCaseNamingConvention());

        return builder;
    }
}