using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mira.Database;

public class DesignTimeFactory : IDesignTimeDbContextFactory<MiraContext>
{
    public MiraContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<MiraContext> builder = new();
        builder.UseNpgsql("");
        
        builder.UseNpgsql().UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        return new MiraContext(builder.Options);
    }  
}
