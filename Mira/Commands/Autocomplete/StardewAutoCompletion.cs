using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Mira.Database;

namespace Mira.Commands.Autocomplete;

public class StardewAutoCompletion : IAutocompleteProvider
{

    private readonly MiraContext _context;
    
    public StardewAutoCompletion(MiraContext context)
    {
        _context = context;
    }
    
    public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {

        string userInput = ctx.OptionValue?.ToString()?.ToLower() ?? string.Empty;

        IQueryable<DiscordAutoCompleteChoice> villagers = this._context.StardewCharacters
            .Where(c => c.Villager.ToLower().StartsWith(userInput))
            .Take(25)
            .Select(c => new DiscordAutoCompleteChoice(c.Villager, c.Id.ToString()));

        return Task.FromResult(villagers.AsEnumerable());
    }
}

