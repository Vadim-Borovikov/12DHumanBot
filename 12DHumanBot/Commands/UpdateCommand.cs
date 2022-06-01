using AbstractBot;
using Telegram.Bot.Types;

namespace _12DHumanBot.Commands;

internal sealed class UpdateCommand : CommandBase<Bot, Config>
{
    protected override string Name => "update";
    protected override string Description => "Обновить базу из второго листа и стереть его";

    public override BotBase<Bot, Config>.AccessType Access => BotBase<Bot, Config>.AccessType.Admins;

    public UpdateCommand(Bot bot) : base(bot) { }

    public override Task ExecuteAsync(Message message, bool fromChat, string? payload)
    {
        return Bot.Manager.Update(message.Chat.Id);
    }
}