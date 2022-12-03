using AbstractBot;
using AbstractBot.Commands;
using Telegram.Bot.Types;

namespace _12DHumanBot.Commands;

internal sealed class UpdateCommand : CommandBaseCustom<Bot, Config>
{
    public override BotBase.AccessType Access => BotBase.AccessType.Admins;

    public UpdateCommand(Bot bot) : base(bot, "update", "обновить базу из рабочего листа") { }

    public override Task ExecuteAsync(Message message, Chat chat, string? payload) => Bot.Manager.Update(chat);
}