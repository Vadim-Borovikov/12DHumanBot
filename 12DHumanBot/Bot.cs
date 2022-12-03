using _12DHumanBot.Commands;
using _12DHumanBot.Model;
using AbstractBot;
using AbstractBot.Commands;
using GoogleSheetsManager.Providers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace _12DHumanBot;

public sealed class Bot : BotBaseCustom<Config>, IDisposable
{
    internal readonly SheetsProvider GoogleSheetsProvider;
    internal readonly FigureManager Manager;

    public Bot(Config config) : base(config)
    {
        GoogleSheetsProvider = new SheetsProvider(config, config.GoogleSheetId);
        Manager = new FigureManager(this);
    }

    public void Dispose() => GoogleSheetsProvider.Dispose();

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
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

    protected override async Task ProcessTextMessageAsync(Message textMessage, Chat senderChat,
        CommandBase? command = null, string? payload = null)
    {
        if (textMessage.Text is not null)
        {
            bool separated = await Manager.TrySeparate(textMessage.Chat, textMessage.Text);
            if (separated)
            {
                return;
            }
        }

        await base.ProcessTextMessageAsync(textMessage, senderChat, command, payload);
    }
}