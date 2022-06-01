using AbstractBot;
using GoogleSheetsManager;
using GryphonUtilities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace _12DHumanBot.Model;

internal sealed class FigureManager
{
    private readonly Bot _bot;
    private Dictionary<string, Figure>? _figures;
    private Dictionary<byte, Vertex>? _vertices;

    public FigureManager(Bot bot)
    {
        _bot = bot;
        Figure.Types = _bot.Config.LengthNames;
    }

    public async Task Load(ChatId chatId)
    {
        Message statusMessage = await _bot.SendTextMessageAsync(chatId, "_Загружаю базу…_", ParseMode.MarkdownV2);
        string range = _bot.Config.GoogleRangeAll.GetValue(nameof(_bot.Config.GoogleRangeAll));
        IList<Figure> figures = await DataManager.GetValuesAsync(_bot.GoogleSheetsProvider, Figure.Load, range);

        _vertices = new Dictionary<byte, Vertex>();
        _figures = new Dictionary<string, Figure>();

        foreach (Vertex v in figures.OfType<Vertex>())
        {
            _vertices[v.Number] = v;
            _figures[v.GetCode()] = v;
        }

        foreach (Figure f in figures.Where(f => f is not Vertex))
        {
            if (f.Numbers is null)
            {
                throw new NullReferenceException(nameof(f.Numbers));
            }

            foreach (byte n in f.Numbers)
            {
                f.Vertices.Add(_vertices[n]);
            }
            _figures[f.GetCode()] = f;
        }

        await _bot.Client.FinalizeStatusMessageAsync(statusMessage,
            $"{Environment.NewLine}Загружено фигур: {_figures.Count}\\.");
    }

    public async Task Generate(ChatId chatId)
    {
        Message statusMessage = await _bot.SendTextMessageAsync(chatId, "_Генерирую фигуры…_", ParseMode.MarkdownV2);

        _vertices = new Dictionary<byte, Vertex>();
        _figures = new Dictionary<string, Figure>();

        for (byte b = 1; b <= _bot.Config.MaxLength; ++b)
        {
            Vertex v = new(b);
            _vertices[b] = v;

            List<Figure> newFigures = new();
            foreach (Figure f in _figures.Values)
            {
                List<Vertex> vertices = new(f.Vertices)
                {
                    v
                };
                Figure newFigure = new(vertices);
                newFigures.Add(newFigure);
            }

            _figures[v.GetCode()] = v;
            foreach (Figure f in newFigures)
            {
                _figures[f.GetCode()] = f;
            }
        }

        await _bot.Client.FinalizeStatusMessageAsync(statusMessage,
            $"{Environment.NewLine}Создано фигур: {_figures.Count}\\.");

        await Save(chatId, _figures.Values);
    }

    public async Task Update(ChatId chatId)
    {
        if (_vertices is null)
        {
            await _bot.SendTextMessageAsync(chatId, "База пуста!");
            await Load(chatId);
        }

        if (_vertices is null)
        {
            throw new NullReferenceException(nameof(_vertices));
        }

        if (_figures is null)
        {
            throw new NullReferenceException(nameof(_figures));
        }

        Message statusMessage = await _bot.SendTextMessageAsync(chatId, "_Обновляю базу данными из рабочего листа…_", ParseMode.MarkdownV2);

        string range = _bot.Config.GoogleRange.GetValue(nameof(_bot.Config.GoogleRange));
        const int sheetIndex = 1;
        IList<Figure> figures =
            await DataManager.GetValuesAsync(_bot.GoogleSheetsProvider, Figure.Load, sheetIndex, range);
        foreach (Figure f in figures)
        {
            Figure figure = _figures[f.GetCode()];
            figure.Name = f.Name;
            figure.Comment = f.Comment;
        }

        await _bot.Client.FinalizeStatusMessageAsync(statusMessage);

        await Save(chatId, _figures.Values);
    }

    public async Task<bool> TrySeparate(ChatId chatId, string code)
    {
        if (_figures is null || !_figures.ContainsKey(code))
        {
            return false;
        }

        string template =
            _bot.Config.GoogleRangeWorkingTemplate.GetValue(nameof(_bot.Config.GoogleRangeWorkingTemplate));
        string title = string.Format(template, code);
        Message statusMessage =
            await _bot.SendTextMessageAsync(chatId, $"_Настраиваю рабочий лист для {code}…_", ParseMode.MarkdownV2);
        const int sheetIndex = 1;
        await DataManager.RenameSheetAsync(_bot.GoogleSheetsProvider, sheetIndex, title);

        Figure figure = _figures[code];
        SortedSet<Figure> subfigures =
            new(_figures.Values.Where(f => f.Vertices.All(v => figure.Vertices.Contains(v))));

        List<Figure> sorted = new();
        while (subfigures.Count > 1)
        {
            Figure first = subfigures.First();
            sorted.Add(first);
            subfigures.Remove(first);
            SortedSet<Vertex> pairVertices = new(figure.Vertices.Except(first.Vertices));
            string pairCode = Vertex.GetCode(pairVertices);
            Figure pair = _figures[pairCode];
            sorted.Add(pair);
            subfigures.Remove(pair);
        }
        sorted.Add(subfigures.First());

        string rangePostfix = _bot.Config.GoogleRange.GetValue(nameof(_bot.Config.GoogleRange));
        string range = $"{title}!{rangePostfix}";

        await _bot.GoogleSheetsProvider.ClearValuesAsync(range);
        await DataManager.UpdateValuesAsync(_bot.GoogleSheetsProvider, range, sorted);

        await _bot.Client.FinalizeStatusMessageAsync(statusMessage);
        return true;
    }

    private async Task Save(ChatId chatId, IEnumerable<Figure> figures)
    {
        Message statusMessage =
            await _bot.SendTextMessageAsync(chatId, "_Сохраняю базу в таблицу…_", ParseMode.MarkdownV2);
        string range = _bot.Config.GoogleRangeAll.GetValue(nameof(_bot.Config.GoogleRangeAll));
        await DataManager.UpdateValuesAsync(_bot.GoogleSheetsProvider, range,
            figures.OrderBy(f => f.GetLength()).ToList());
        await _bot.Client.FinalizeStatusMessageAsync(statusMessage);
    }
}
