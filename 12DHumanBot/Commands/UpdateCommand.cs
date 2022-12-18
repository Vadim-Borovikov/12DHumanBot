using _12DHumanBot.Model;
using AbstractBot.Operations;
using Telegram.Bot.Types;

namespace _12DHumanBot.Commands;

internal sealed class UpdateCommand : CommandOperation
{
    protected override byte MenuOrder => 4;

    protected override Access AccessLevel => Access.Admin;

    public UpdateCommand(Bot bot, FigureManager manager) : base(bot, "update", "обновить базу из рабочего листа")
    {
        _manager = manager;
    }

    protected override Task ExecuteAsync(Message message, long _, string? __) => _manager.Update(message.Chat);

    private readonly FigureManager _manager;
}