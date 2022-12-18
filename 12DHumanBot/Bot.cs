using _12DHumanBot.Commands;
using _12DHumanBot.Model;
using AbstractBot.Bots;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace _12DHumanBot;

public sealed class Bot : BotWithSheets<Config>
{
    public Bot(Config config) : base(config)
    {
        GoogleSheetsManager.Documents.Document document = DocumentsManager.GetOrAdd(Config.GoogleSheetId);
        _manager = new FigureManager(this, document);

        Operations.Add(new GenerateCommand(this, _manager));
        Operations.Add(new LoadCommand(this, _manager));
        Operations.Add(new UpdateCommand(this, _manager));

        Operations.Add(new SliceOperation(this, _manager));
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        long? logsChatId = Config.SuperAdminId;
        if (logsChatId.HasValue)
        {
            Chat logsChat = new()
            {
                Id = logsChatId.Value,
                Type = ChatType.Private
            };
            await _manager.Load(logsChat);
        }
    }

    private readonly FigureManager _manager;
}