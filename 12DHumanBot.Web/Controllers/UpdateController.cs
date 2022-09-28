using _12DHumanBot.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace _12DHumanBot.Web.Controllers;

public sealed class UpdateController : Controller
{
    public OkResult Post([FromServices] BotSingleton singleton, [FromBody] Update update)
    {
        singleton.Bot.Update(update);
        return Ok();
    }
}