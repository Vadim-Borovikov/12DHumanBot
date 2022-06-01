using _12DHumanBot.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace _12DHumanBot.Web.Controllers;

[Route("")]
public sealed class HomeController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromServices] BotSingleton singleton)
    {
        User model = await singleton.Bot.GetUserAsync();
        return View(model);
    }
}
