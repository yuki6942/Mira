using System.Reflection;
using DSharpPlus;
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
        private static IServiceProvider _services;
        
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
                shardSlash.SlashCommandErrored += HandleErrorAsync;
            }

            client.GuildCreated += GuildJoinAsync;
            await client.StartAsync();
            await Task.Delay(-1);
        }

        private static async Task HandleErrorAsync(SlashCommandsExtension slash,
            SlashCommandErrorEventArgs e)
        {
            slash.Client.Logger.LogError("An error has occured: {Exception}",
                e.Exception);
            await e.Context.CreateResponseAsync(
                $"An error has occured, please report following error on our discord:\n{e.Exception}",
                ephemeral: true);
        }

        private static async Task GuildJoinAsync(DiscordClient client,GuildCreateEventArgs e)
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
    }
}
