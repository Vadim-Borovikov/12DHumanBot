using _12DHumanBot.Model;
using AbstractBot.Operations;
using Telegram.Bot.Types;

namespace _12DHumanBot.Commands;

internal sealed class LoadCommand : CommandOperation
{
    protected override byte MenuOrder => 3;

    protected override Access AccessLevel => Access.Admin;

    public LoadCommand(Bot bot, FigureManager manager) : base(bot, "load", "загрузить базу из таблицы")
    {
        _manager = manager;
    }

    protected override Task ExecuteAsync(Message message, long _, string? __) => _manager.Load(message.Chat);

    private readonly FigureManager _manager;
}