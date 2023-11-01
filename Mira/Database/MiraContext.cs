using Microsoft.EntityFrameworkCore;
using Mira.Database.Entities;

namespace Mira.Database;

public class MiraContext : DbContext
{
    public DbSet<StardewCharacter> StardewCharacters { get; set; }
    public DbSet<Guild> Guilds { get; set; }
    public MiraContext(DbContextOptions<MiraContext> options): base(options)
    {}
}
