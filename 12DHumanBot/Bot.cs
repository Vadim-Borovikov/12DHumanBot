using AbstractBot;
using Telegram.Bot.Types;

namespace _12DHumanBot;

public sealed class Bot : BotBaseGoogleSheets<Bot, Config>
{
    public Bot(Config config) : base(config)
    {
    }

    protected override async Task ProcessTextMessageAsync(Message textMessage, bool fromChat,
        CommandBase<Bot, Config>? command = null, string? payload = null)
    {
        await SendTextMessageAsync(textMessage.Chat.Id, "Hello world!");
    }
}