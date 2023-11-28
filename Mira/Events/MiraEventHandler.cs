using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mira.Database;
using Mira.Database.Entities;

namespace Mira.Events;

public class MiraEventHandler
{
    private readonly IServiceProvider _services;
    private readonly DiscordShardedClient _client;
    public MiraEventHandler(IServiceProvider services, DiscordShardedClient client)
    {
        this._services = services;
        this._client = client;
        
        this._client.GuildCreated += OnGuildJoinAsync;
        this._client.MessageDeleted += OnMessageDeletedAsync;
        this._client.MessageUpdated += OnMessageUpdatedAsync;
        this._client.GuildMemberAdded += OnGuildMemberAddAsync;
        this._client.GuildMemberRemoved += OnGuildMemberRemoveAsync;
    }
    
    
    #pragma warning disable CA1822
    // ReSharper disable MemberCanBeMadeStatic.Global
    public async Task OnErrorAsync(SlashCommandsExtension slash, SlashCommandErrorEventArgs e)
        // ReSharper restore MemberCanBeMadeStatic.Global
#pragma warning restore CA1822
    {
        slash.Client.Logger.LogError("An error has occurred: {Exception}", e.Exception);
        if (e.Exception is SlashExecutionChecksFailedException)
        {
            await e.Context.CreateResponseAsync("Only the bot owner can use these commands!", ephemeral: true);
        }
        else
        {
            await e.Context.CreateResponseAsync(
                $"An error has occurred, please report the following error on our discord:\n{e.Exception}",
                ephemeral: true);
        }
    }
    
    private async Task OnGuildJoinAsync(DiscordClient client, GuildCreateEventArgs e)
        {
            client.Logger.LogInformation("Bot joined a new guild: {guild}", e.Guild.Name);

            using IServiceScope scope = this._services.CreateScope();
            MiraContext ctx = scope.ServiceProvider.GetRequiredService<MiraContext>();
            Guild? existingGuild = await ctx.Guilds.FindAsync(e.Guild.Id);

            if (existingGuild is null)
            {
                ctx.Guilds.Add(new Guild { GuildId = e.Guild.Id, PremiumStatus = false });
                await ctx.SaveChangesAsync();
            }
        }

    private async Task OnMessageDeletedAsync(DiscordClient client, MessageDeleteEventArgs e)
        {
            using IServiceScope scope = this._services.CreateScope();
            MiraContext ctx = scope.ServiceProvider.GetRequiredService<MiraContext>();

            if (e.Message.Author.IsBot)
            {
                return;
            }

            Logging? logging = await ctx.Loggings.FindAsync(e.Guild.Id);

            if (logging is null)
            {
                return;
            }

            DiscordChannel logChannel = e.Guild.GetChannel(logging.LoggingChannelId);

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithAuthor(e.Message.Author.Username, "", e.Message.Author.AvatarUrl)
                .WithColor(DiscordColor.Purple)
                .AddField($"Message deleted in <#{e.Channel.Id}>", $"{e.Message.Content}")
                .WithFooter($"Message ID: {e.Message.Id}")
                .WithTimestamp(DateTime.UtcNow);

            await logChannel.SendMessageAsync(embedBuilder);
        }

    private async Task OnMessageUpdatedAsync(DiscordClient client, MessageUpdateEventArgs e)
        {
            using IServiceScope scope = this._services.CreateScope();
            MiraContext ctx = scope.ServiceProvider.GetRequiredService<MiraContext>();

            if (e.Message.Author.IsBot)
            {
                return;
            }

            Logging? logging = await ctx.Loggings.FindAsync(e.Guild.Id);

            if (logging is null)
            {
                return;
            }

            DiscordChannel logChannel = e.Guild.GetChannel(logging.LoggingChannelId);

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithAuthor(e.Message.Author.Username, "", e.Message.Author.AvatarUrl)
                .WithColor(DiscordColor.Purple)
                .WithDescription($"Message edited in {e.Message.JumpLink}")
                .AddField("__Before__", $"{e.MessageBefore.Content}")
                .AddField("__After__", $"{e.Message.Content}")
                .WithFooter($"Message ID: {e.Message.Id}")
                .WithTimestamp(DateTime.UtcNow);

            await logChannel.SendMessageAsync(embedBuilder);
        }

    private async Task OnGuildMemberAddAsync(DiscordClient client, GuildMemberAddEventArgs e)
        {
            using IServiceScope scope = this._services.CreateScope();
            MiraContext ctx = scope.ServiceProvider.GetRequiredService<MiraContext>();

            Logging? logging = await ctx.Loggings.FindAsync(e.Guild.Id);

            if (logging is null)
            {
                return;
            }

            DiscordChannel logChannel = e.Guild.GetChannel(logging.LoggingChannelId);

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithAuthor($"{e.Member.Username}", "", e.Member.AvatarUrl)
                .WithColor(DiscordColor.Green)
                .WithDescription($"**New member joined the guild.**\n{e.Member.Mention}")
                .AddField("Created at", $"<t:{e.Member.CreationTimestamp.ToUnixTimeSeconds()}:f>")
                .WithFooter($"Member ID: {e.Member.Id}")
                .WithTimestamp(DateTime.UtcNow);

            await logChannel.SendMessageAsync(embedBuilder);
        }

    private async Task OnGuildMemberRemoveAsync(DiscordClient client, GuildMemberRemoveEventArgs e)
        {
            using IServiceScope scope = this._services.CreateScope();
            MiraContext ctx = scope.ServiceProvider.GetRequiredService<MiraContext>();

            Logging? logging = await ctx.Loggings.FindAsync(e.Guild.Id);

            if (logging is null)
            {
                return;
            }

            DiscordChannel logChannel = e.Guild.GetChannel(logging.LoggingChannelId);

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithAuthor($"{e.Member.Username}", "", e.Member.AvatarUrl)
                .WithColor(DiscordColor.Red)
                .WithDescription($"**Member is no longer in the guild.**\n{e.Member.Mention}")
                .WithFooter($"Member ID: {e.Member.Id}")
                .WithTimestamp(DateTime.UtcNow);

            await logChannel.SendMessageAsync(embedBuilder);
        }
        
}
