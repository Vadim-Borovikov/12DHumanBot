using AbstractBot;
using AbstractBot.Commands;
using Telegram.Bot.Types;

namespace _12DHumanBot.Commands;

internal sealed class GenerateCommand : CommandBase<Bot, Config>
{
    public override BotBase<Bot, Config>.AccessType Access => BotBase<Bot, Config>.AccessType.SuperAdmin;

    public GenerateCommand(Bot bot) : base(bot, "generate", "сгенерировать базу с нуля") { }

    public override Task ExecuteAsync(Message message, bool fromChat, string? payload)
    {
        return Bot.Manager.Generate(message.Chat);
    }
}