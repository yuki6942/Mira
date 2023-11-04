using System.ComponentModel.DataAnnotations;

namespace Mira.Database.Entities;

public class Logging
{
    // Add Key here cause EntityFramework probably doesn't like both being named "Id", even after changing it it still doesn't like it
    [Key]
    public ulong GuildId { get; set; }
    
    public ulong LoggingChannelId { get; set; }
}
