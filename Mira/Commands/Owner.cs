using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Microsoft.EntityFrameworkCore;
using Mira.Database;
using Mira.Database.Entities;

namespace Mira.Commands;

[SlashCommandGroup("owner", "Commands for the bot owner"), SlashRequireOwner]
public class Owner : ApplicationCommandModule
{
    private readonly MiraContext _context;

    public Owner(MiraContext context)
    {
        _context = context;
    }

    [SlashCommand("setpremium", "Set the premium status of a user")]
    public async Task SetPremiumCommandAsync(InteractionContext ctx,
        [Option("user", "The user to set premium status for.", true)] DiscordUser user,
        [Option("status", "The premium status (true/false).", true)] bool premiumStatus)
    {
        ulong userId = user.Id;

        User? dbUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (dbUser is null)
        {
            dbUser = new User { UserId = userId, PremiumStatus = premiumStatus };
            _context.Users.Add(dbUser);
        }
        else
        {
            dbUser.PremiumStatus = premiumStatus;
        }
        
        await _context.SaveChangesAsync();

        await ctx.CreateResponseAsync($"Premium status for user `{user.Username}` `({userId})`  set to `{premiumStatus}`.");
    }



    [SlashCommand("checkpremium", "Check the premium status of a user")]
    public async Task CheckPremiumCommandAsync(InteractionContext ctx,
        [Option("user", "The user to check premium status for.", true)] DiscordUser user)
    {
        ulong userId = user.Id;

        User? dbUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (dbUser is null)
        {
            dbUser = new User { UserId = userId, PremiumStatus = false };
            _context.Users.Add(dbUser);
            await _context.SaveChangesAsync();
        }

        await ctx.CreateResponseAsync($"Premium status for user `{user.Username}` `({userId})` is `{dbUser.PremiumStatus}`.");
    }

}
