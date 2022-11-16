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

        Figure.Types = _bot.Config.LengthNames.ToDictionary(p => p.Key.ToByte().GetValue(), p => p.Value);
    }

    public async Task Load(Chat chat)
    {
        Message statusMessage = await _bot.SendTextMessageAsync(chat, "_Загружаю базу…_", ParseMode.MarkdownV2);
        SheetData<FigureInfo> data = await DataManager<FigureInfo>.LoadAsync(_bot.GoogleSheetsProvider,
            _bot.Config.GoogleRangeAll, additionalConverters: AdditionalConverters);

        _vertices = data.Instances
                        .Where(i => i.Length == 1)
                        .Select(i => i.Convert())
                        .OfType<Vertex>()
                        .ToDictionary(v => v.Number, v => v);

        _figures = data.Instances
                       .Where(i => i.Length > 1)
                       .Select(i => i.Convert(_vertices))
                       .RemoveNulls()
                       .ToDictionary(f => f.GetCode(), f => f);

        foreach (Vertex v in _vertices.Values)
        {
            _figures[v.GetCode()] = v;
        }

        await _bot.FinalizeStatusMessageAsync(statusMessage,
            $"{Environment.NewLine}Загружено фигур: {_figures.Count}\\.");
    }

    public async Task Generate(Chat chat)
    {
        Message statusMessage = await _bot.SendTextMessageAsync(chat, "_Генерирую фигуры…_", ParseMode.MarkdownV2);

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

        await _bot.FinalizeStatusMessageAsync(statusMessage,
            $"{Environment.NewLine}Создано фигур: {_figures.Count}\\.");

        List<string> titles =
            await GoogleSheetsManager.Utils.LoadTitlesAsync(_bot.GoogleSheetsProvider, _bot.Config.GoogleRangeAll);
        await Save(chat, _figures.Values, titles);
    }

    public async Task Update(Chat chat)
    {
        if (_vertices is null)
        {
            await _bot.SendTextMessageAsync(chat, "База пуста!");
            await Load(chat);
        }

        if (_vertices is null)
        {
            throw new NullReferenceException(nameof(_vertices));
        }

        if (_figures is null)
        {
            throw new NullReferenceException(nameof(_figures));
        }

        Message statusMessage =
            await _bot.SendTextMessageAsync(chat, "_Обновляю базу данными из рабочего листа…_", ParseMode.MarkdownV2);

        const int sheetIndex = 1;
        SheetData<FigureInfo> data = await DataManager<FigureInfo>.LoadAsync(_bot.GoogleSheetsProvider,
            _bot.Config.GoogleRange, sheetIndex, additionalConverters: AdditionalConverters);
        foreach (Figure f in data.Instances
                                 .Select(i => i.Convert(_vertices))
                                 .RemoveNulls())
        {
            Figure figure = _figures[f.GetCode()];
            figure.Name = f.Name;
            figure.Comment = f.Comment;
        }

        await _bot.FinalizeStatusMessageAsync(statusMessage);

        await Save(chat, _figures.Values, data.Titles);
    }

    public async Task<bool> TrySeparate(Chat chat, string code)
    {
        if (_figures is null || !_figures.ContainsKey(code))
        {
            return false;
        }

        string title = string.Format(_bot.Config.GoogleRangeWorkingTemplate, code);
        Message statusMessage =
            await _bot.SendTextMessageAsync(chat, $"_Настраиваю рабочий лист для {code}…_", ParseMode.MarkdownV2);
        const int sheetIndex = 1;
        await GoogleSheetsManager.Utils.RenameSheetAsync(_bot.GoogleSheetsProvider, sheetIndex, title);

        Figure figure = _figures[code];
        SortedSet<Figure> subfigures =
            new(_figures.Values.Where(f => f.Vertices.All(v => figure.Vertices.Contains(v))));

        List<Figure> sorted = new();
        while (subfigures.Count > 1)
        {
            Figure first = subfigures.First();
            sorted.Add(first);
            subfigures.Remove(first);
            Figure pair = GetComplimentary(first, figure);
            sorted.Add(pair);
            subfigures.Remove(pair);
        }
        sorted.Add(subfigures.First());

        Figure maxFigure = _figures.Values.OrderDescending().First();
        Figure complimentary = GetComplimentary(figure, maxFigure);
        sorted.Add(complimentary);

        string range = $"{title}!{_bot.Config.GoogleRange}";

        await _bot.GoogleSheetsProvider.ClearValuesAsync(range);

        List<FigureInfo> infos = sorted.Select(f => f.Convert()).ToList();
        List<string> titles =
            await GoogleSheetsManager.Utils.LoadTitlesAsync(_bot.GoogleSheetsProvider, _bot.Config.GoogleRange);
        SheetData<FigureInfo> data = new(infos, titles);
        await DataManager<FigureInfo>.SaveAsync(_bot.GoogleSheetsProvider, range, data);

        await _bot.FinalizeStatusMessageAsync(statusMessage);
        return true;
    }

    private Figure GetComplimentary(Figure current, Figure full)
    {
        if (_figures is null)
        {
            throw new NullReferenceException(nameof(_figures));
        }

        SortedSet<Vertex> vertices = new(full.Vertices.Except(current.Vertices));
        string code = Vertex.GetCode(vertices);
        return _figures[code];
    }

    private async Task Save(Chat chat, IEnumerable<Figure> figures, IList<string> titles)
    {
        Message statusMessage =
            await _bot.SendTextMessageAsync(chat, "_Сохраняю базу в таблицу…_", ParseMode.MarkdownV2);
        SheetData<FigureInfo> data = new(figures.Order().Select(f => f.Convert()).ToList(), titles);
        await DataManager<FigureInfo>.SaveAsync(_bot.GoogleSheetsProvider, _bot.Config.GoogleRangeAll, data);
        await _bot.FinalizeStatusMessageAsync(statusMessage);
    }

    private static readonly Dictionary<Type, Func<object?, object?>> AdditionalConverters = new()
    {
        { typeof(byte), o => o.ToByte() }
    };
}