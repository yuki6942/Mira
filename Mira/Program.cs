using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mira.Database;
using Mira.Database.Entities;

namespace Mira
{

    public class Program
    {
#pragma warning disable CS8618
        private static IServiceProvider _services;
#pragma warning restore CS8618

#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming
        // ReSharper is weird and is causing a style error here
        public static async Task Main()
            // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006
        {
            ConfigurationBuilder builder = new();
            builder.AddUserSecrets<Program>();
            builder.AddEnvironmentVariables();
            IConfiguration config = builder.Build();

            DiscordShardedClient client = new(new DiscordConfiguration()
            {
                Token = config["DISCORD_TOKEN"]!,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                AutoReconnect = true
            });

            ServiceCollection collection = new();
            collection.AddDbContextPool<MiraContext>(db =>
                db.UseNpgsql(config["DB_STRING"]));
            IServiceProvider services = collection.BuildServiceProvider();

            IReadOnlyDictionary<int, SlashCommandsExtension> slash =
                await client.UseSlashCommandsAsync(
                    new SlashCommandsConfiguration()
                    {
                        Services = services
                    }
                );
            _services = services;

            
            
            foreach ((int _, SlashCommandsExtension shardSlash) in slash)
            {
                shardSlash.RegisterCommands(Assembly.GetExecutingAssembly(),
                    899005534198435840);
                shardSlash.SlashCommandErrored += OnErrorAsync;
            }

            client.GuildCreated += OnGuildJoinAsync;
            client.MessageDeleted += OnMessageDeletedAsync;
            client.MessageUpdated += OnMessageUpdatedAsync;
            client.GuildMemberAdded += OnGuildMemberAddAsync;
            client.GuildMemberRemoved += OnGuildMemberRemoveAsync;

            await client.StartAsync();
            await Task.Delay(-1);
        }
        
        // TODO: Make this into a EventHandler
        private static async Task OnErrorAsync(SlashCommandsExtension slash,
            SlashCommandErrorEventArgs e)
        {
            slash.Client.Logger.LogError("An error has occured: {Exception}",
                e.Exception);
            await e.Context.CreateResponseAsync(
                $"An error has occured, please report following error on our discord:\n{e.Exception}",
                ephemeral: true);
        }

        private static async Task OnGuildJoinAsync(DiscordClient client,GuildCreateEventArgs e)
        {
            client.Logger.LogInformation("Bot joined a new guild: {guild}", e.Guild.Name);

            using IServiceScope scope = _services.CreateScope();
            MiraContext ctx = scope.ServiceProvider.GetRequiredService<MiraContext>();
            
            Guild? existingGuild = await ctx.Guilds.FindAsync(e.Guild.Id);

            if (existingGuild is null)
            {
                ctx.Guilds.Add(new Guild { GuildId = e.Guild.Id, PremiumStatus = false });
                await ctx.SaveChangesAsync();
                
            }
            
        }

        private static async Task OnMessageDeletedAsync(DiscordClient client, MessageDeleteEventArgs e)
        {
            using IServiceScope scope = _services.CreateScope();
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

        private static async Task OnMessageUpdatedAsync(DiscordClient client, MessageUpdateEventArgs e)
        {
            using IServiceScope scope = _services.CreateScope();
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

        private static async Task OnGuildMemberAddAsync(DiscordClient client, GuildMemberAddEventArgs e)
        {
            using IServiceScope scope = _services.CreateScope();
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

        private static async Task OnGuildMemberRemoveAsync(DiscordClient client, GuildMemberRemoveEventArgs e)
        {
            using IServiceScope scope = _services.CreateScope();
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
    
}
