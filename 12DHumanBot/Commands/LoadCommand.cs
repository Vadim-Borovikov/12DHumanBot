using AbstractBot.Commands;
using Telegram.Bot.Types;

namespace _12DHumanBot.Commands;

internal sealed class LoadCommand : CommandBaseCustom<Bot, Config>
{
    public LoadCommand(Bot bot) : base(bot, "load", "загрузить базу из таблицы") { }

    public override Task ExecuteAsync(Message message, bool fromChat, string? payload)
    {
        return Bot.Manager.Load(message.Chat);
    }
}