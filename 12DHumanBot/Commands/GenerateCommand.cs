using _12DHumanBot.Model;
using AbstractBot.Operations;
using Telegram.Bot.Types;

namespace _12DHumanBot.Commands;

internal sealed class GenerateCommand : CommandOperation
{
    protected override byte MenuOrder => 2;

    protected override Access AccessLevel => Access.SuperAdmin;

    public GenerateCommand(Bot bot, FigureManager manager) : base(bot, "generate", "сгенерировать базу с нуля")
    {
        _manager = manager;
    }

    protected override Task ExecuteAsync(Message message, long _, string? __) => _manager.Generate(message.Chat);

    private readonly FigureManager _manager;
}