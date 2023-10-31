using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Mira.Database;
using Mira.Database.Entities;

namespace Mira.Commands.Autocomplete;

public class StardewAutoCompletion : IAutocompleteProvider
{
    //private MiraContext DbContext { get; set; }
    
    public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {
        //var villagers = DbContext.StardewCharacters.ToList();
        //ctx.Client.Logger.LogInformation("Found {Count} villager entries", villagers.Count);

        return Task.FromResult(new[]
        {
            //new DiscordAutoCompleteChoice("Abigail", villagers[0].Villager),
            new DiscordAutoCompleteChoice("a", "s")
        }.AsEnumerable());
    }

}
