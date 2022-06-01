using AbstractBot;
using GryphonUtilities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace _12DHumanBot.Commands;

internal sealed class StartCommand : CommandBase<Bot, Config>
{
    protected override string Name => "start";
    protected override string Description => "Список команд";

    public StartCommand(Bot bot) : base(bot) { }

    public override Task ExecuteAsync(Message message, bool fromChat, string? payload)
    {
        User user = message.From.GetValue(nameof(message.From));
        return Bot.SendTextMessageAsync(user.Id, Bot.GetDescriptionFor(user.Id), ParseMode.MarkdownV2);
    }
}