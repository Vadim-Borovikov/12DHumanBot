using AbstractBot;
using Telegram.Bot.Types;

namespace _12DHumanBot.Commands;

internal sealed class GenerateCommand : CommandBase<Bot, Config>
{
    protected override string Name => "generate";
    protected override string Description => "Сгенерировать базу с нуля";

    public override BotBase<Bot, Config>.AccessType Access => BotBase<Bot, Config>.AccessType.SuperAdmin;

    public GenerateCommand(Bot bot) : base(bot) { }

    public override Task ExecuteAsync(Message message, bool fromChat, string? payload)
    {
        return Bot.Manager.Generate(message.Chat.Id);
    }
}