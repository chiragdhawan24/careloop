using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CareLoop.Infrastructure.Persistence;

public sealed class CareLoopDbContextFactory
    : IDesignTimeDbContextFactory<CareLoopDbContext>
{
    public CareLoopDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<CareLoopDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Database=careloop_design;Username=postgres")
            .UseSnakeCaseNamingConvention();

        return new CareLoopDbContext(optionsBuilder.Options);
    }
}