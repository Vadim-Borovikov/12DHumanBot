using AbstractBot;
using Telegram.Bot.Types;

namespace _12DHumanBot.Commands;

internal sealed class LoadCommand : CommandBase<Bot, Config>
{
    protected override string Name => "load";
    protected override string Description => "Загрузить базу из таблицы";

    public LoadCommand(Bot bot) : base(bot) { }

    public override Task ExecuteAsync(Message message, bool fromChat, string? payload)
    {
        return Bot.Manager.Load(message.Chat);
    }
}