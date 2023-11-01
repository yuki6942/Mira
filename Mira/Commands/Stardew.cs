using DSharpPlus;
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
    public async Task GiftCommandAsync(InteractionContext ctx, [Option("character", "The character for whom you want to list gifts.", true)]
        [Autocomplete(typeof(StardewAutoCompletion))]
        string characterId)
    {

        StardewCharacter? character = await this._context.StardewCharacters.FirstOrDefaultAsync(c => c.Id.ToString() == characterId);

        if (character != null)
        {

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Purple)
                .WithTitle($"Information about {character.Villager}")
                .AddField("__Birthday__", $"`{character.Birthday}`")
                .AddField("__Loves__", $"`{character.Loves}`")
                .AddField("__Likes__", $"`{character.Likes}`")
                .AddField("__Neutral__", $"`{character.Neutral}`")
                .AddField("__Dislikes__", $"`{character.Dislikes}`")
                .AddField("__Hates__", $"`{character.Hates}`");
            
            if (character.Villager == "Universals")
            {
                embedBuilder
                    .AddField("__Universal Dislike exceptions__","See the exceptions [here](https://stardewvalleywiki.com/Friendship#Universal_Hates)" )
                    .AddField("__Universal Hates exceptions__", "See the exceptions [here](https://stardewvalleywiki.com/Friendship#Universal_Dislikes)");
            }
                        
            DiscordInteractionResponseBuilder response =
                new DiscordInteractionResponseBuilder()
                    .AddEmbed(embedBuilder);
            
            if (character.Villager != "Universals")
            {
                response.AddComponents(new DiscordLinkButtonComponent(
                    $"https://stardewvalleywiki.com/{character.Villager}",
                    "Open Wiki"));

            }
            
            await ctx.CreateResponseAsync(response);
            return;
        }

        await ctx.CreateResponseAsync($"Character with Id {characterId} not found.");
    }
}
