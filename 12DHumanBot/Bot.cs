using _12DHumanBot.Commands;
using _12DHumanBot.Model;
using AbstractBot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace _12DHumanBot;

public sealed class Bot : BotBaseGoogleSheets<Bot, Config>
{
    internal readonly FigureManager Manager;

    public Bot(Config config) : base(config) => Manager = new FigureManager(this);

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        Commands.Add(new StartCommand(this));
        Commands.Add(new GenerateCommand(this));
        Commands.Add(new LoadCommand(this));
        Commands.Add(new UpdateCommand(this));

        await base.StartAsync(cancellationToken);

        long? logsChatId = Config.SuperAdminId;
        if (logsChatId.HasValue)
        {
            Chat logsChat = new()
            {
                Id = logsChatId.Value,
                Type = ChatType.Private
            };
            await Manager.Load(logsChat);
        }
    }

    protected override async Task ProcessTextMessageAsync(Message textMessage, bool fromChat,
        CommandBase<Bot, Config>? command = null, string? payload = null)
    {
        if (textMessage.Text is not null)
        {
            bool separated = await Manager.TrySeparate(textMessage.Chat, textMessage.Text);
            if (separated)
            {
                return;
            }
        }

        await base.ProcessTextMessageAsync(textMessage, fromChat, command, payload);
    }
}