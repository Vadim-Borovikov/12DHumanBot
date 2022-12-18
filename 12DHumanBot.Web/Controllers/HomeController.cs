using _12DHumanBot.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace _12DHumanBot.Web.Controllers;

[Route("")]
public sealed class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index([FromServices] BotSingleton singleton) => View(singleton.Bot.User);
}
