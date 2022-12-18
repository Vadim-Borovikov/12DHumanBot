using _12DHumanBot.Model;
using AbstractBot.Operations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace _12DHumanBot;

internal sealed class SliceOperation : Operation
{
    protected override byte MenuOrder => 5;

    protected override Access AccessLevel => Access.Admin;

    public SliceOperation(Bot bot, FigureManager manager) : base(bot)
    {
        MenuDescription = "*код фигуры* – настроить рабочий лист для работы с фигурой";
        _manager = manager;
    }

    protected override async Task<ExecutionResult> TryExecuteAsync(Message message, long senderId)
    {
        if ((message.Type != MessageType.Text) || string.IsNullOrWhiteSpace(message.Text)
                                               || !_manager.Contains(message.Text))
        {
            return ExecutionResult.UnsuitableOperation;
        }

        if (!IsAccessSuffice(senderId))
        {
            return ExecutionResult.InsufficentAccess;
        }

        await _manager.Separate(message.Chat, message.Text);
        return ExecutionResult.Success;
    }

    private readonly FigureManager _manager;
}