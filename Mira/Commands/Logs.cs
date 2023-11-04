using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Mira.Database;
using Mira.Database.Entities;

namespace Mira.Commands;

[SlashCommandGroup("settings", "Server settings commands.")]
public class Logs : ApplicationCommandModule
{
    private readonly MiraContext _context;

    public Logs(MiraContext context)
    {
        this._context = context;
    }
    
    [SlashCommand("logging", "Set or change the logging channel for this guild")]
    [SlashRequireGuild]
    [SlashRequireUserPermissions(Permissions.Administrator)]
    public async Task LoggingAsync(InteractionContext ctx,
        [Option("channel", "The logging channel", true)] DiscordChannel loggingChannel)
    {
#pragma warning disable CS4014
        // ReSharper disable once MethodHasAsyncOverload
        Logging? existingLogging = this._context.Loggings.Find(ctx.Guild.Id);
        // ReSharper restore MethodHasAsyncOverload
#pragma warning restore CS4014
        
        if (existingLogging is null)
        {
            this._context.Loggings.Add(new Logging
            {
                GuildId = ctx.Guild.Id,
                LoggingChannelId = loggingChannel.Id
            });
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Logging channel set to <#{loggingChannel.Id}>.").AsEphemeral());
        }
        else
        {
            existingLogging.LoggingChannelId = loggingChannel.Id;
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Updated logging channel to <#{loggingChannel.Id}>.").AsEphemeral());
        }
#pragma warning disable CS4014
        // ReSharper disable once MethodHasAsyncOverload
        this._context.SaveChanges();
        // ReSharper restore MethodHasAsyncOverload
#pragma warning restore CS4014
    }
}
