using AbstractBot;
using AbstractBot.Commands;
using Telegram.Bot.Types;

namespace _12DHumanBot.Commands;

internal sealed class UpdateCommand : CommandBase<Bot, Config>
{
    public override BotBase<Bot, Config>.AccessType Access => BotBase<Bot, Config>.AccessType.Admins;

    public UpdateCommand(Bot bot) : base(bot, "update", "обновить базу из рабочего листа") { }

    public override Task ExecuteAsync(Message message, bool fromChat, string? payload)
    {
        return Bot.Manager.Update(message.Chat);
    }
}