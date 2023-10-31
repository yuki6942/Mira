using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Mira.Commands.Autocomplete;
using Mira.Database;

namespace Mira.Commands;

[SlashCommandGroup("stardew", "Commands about Stardew valley.")]
public class Stardew : ApplicationCommandModule
{
    private MiraContext DbContext { get; set; }
    [SlashCommand("gifts", "List the liked & hated gifts about an character")]
    public async Task GiftCommandAsync(InteractionContext ctx, [Option("character", "The character for whom you want to list gifts.", true)]
        [Autocomplete(typeof(StardewAutoCompletion))]
        string character)
    {
        await ctx.CreateResponseAsync($"{DbContext.StardewCharacters.ToList()}");
    }
}
