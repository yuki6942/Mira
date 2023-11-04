using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Mira.Commands.Autocomplete;
using Mira.Database;
using Mira.Database.Entities;


namespace Mira.Commands;

[SlashCommandGroup("stardew", "Commands about Stardew valley.")]
public class Stardew : ApplicationCommandModule
{
    private readonly MiraContext _context;
    
    public Stardew(MiraContext context)
    {
        _context = context;
    }
    
    [SlashCommand("villager", "List some information about that villager")]
    public async Task VillagerCommandAsync(InteractionContext ctx, [Option("villager", "The villager for whom you want to list gifts.", true)]
        [Autocomplete(typeof(StardewAutoCompletion))]
        string villagerId)
    {

        StardewCharacter? villager = await this._context.StardewCharacters.FirstOrDefaultAsync(c => c.Id.ToString() == villagerId);
        
        if (villager is null)
        {
            await ctx.CreateResponseAsync($"Villager `{villagerId}` not found.", ephemeral: true);
            return;
        }
        
        DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            .WithColor(DiscordColor.Purple)
            .WithTitle($"Information about {villager.Villager}")
            .AddField("__Birthday__", $"`{villager.Birthday}`")
            .AddField("__Loves__", $"`{villager.Loves}`")
            .AddField("__Likes__", $"`{villager.Likes}`")
            .AddField("__Neutral__", $"`{villager.Neutral}`")
            .AddField("__Dislikes__", $"`{villager.Dislikes}`")
            .AddField("__Hates__", $"`{villager.Hates}`");
            
        if (villager.Villager == "Universals")
        {
            embedBuilder
                .AddField("__Universal Dislike exceptions__","See the exceptions [here](https://stardewvalleywiki.com/Friendship#Universal_Hates)" )
                .AddField("__Universal Hates exceptions__", "See the exceptions [here](https://stardewvalleywiki.com/Friendship#Universal_Dislikes)");
        }
                        
        DiscordInteractionResponseBuilder response =
            new DiscordInteractionResponseBuilder()
                .AddEmbed(embedBuilder);
            
        if (villager.Villager != "Universals")
        {
            response.AddComponents(new DiscordLinkButtonComponent(
                $"https://stardewvalleywiki.com/{villager.Villager}",
                "Open Wiki"));
        }
        
        await ctx.CreateResponseAsync(response);
        
    }
}
