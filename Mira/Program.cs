using System.Reflection;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mira.Database;
using Mira.Events;

namespace Mira
{
    public class Program
    {
#pragma warning disable CS8618
        // ReSharper disable once NotAccessedField.Local
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

            // ReSharper disable once UnusedVariable
            MiraEventHandler miraEventHandler = new MiraEventHandler(services, client);
            
            foreach ((int _, SlashCommandsExtension shardSlash) in slash)
            {
                shardSlash.RegisterCommands(Assembly.GetExecutingAssembly(),
                    899005534198435840);
                shardSlash.SlashCommandErrored += miraEventHandler.OnErrorAsync;
            }

            await client.StartAsync();
            await Task.Delay(-1);
        }
    }

}
