using _12DHumanBot.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace _12DHumanBot.Web.Controllers;

public sealed class UpdateController : Controller
{
    public async Task<OkResult> Post([FromServices] BotSingleton singleton, [FromBody] Update update)
    {
        await singleton.Bot.UpdateAsync(update);
        return Ok();
    }
}