using Microsoft.EntityFrameworkCore;
using Mira.Database.Entities;

namespace Mira.Database;


public class MiraContext : DbContext
{
    public DbSet<StardewCharacter> StardewCharacters { get; set; }
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Logging> Loggings { get; set; }
#pragma warning disable CS8618 
    public MiraContext(DbContextOptions<MiraContext> options): base(options)
#pragma warning restore CS8618 
    {}
    
    
}
