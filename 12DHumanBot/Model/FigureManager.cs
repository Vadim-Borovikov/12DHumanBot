using AbstractBot;
using GoogleSheetsManager;
using GoogleSheetsManager.Documents;
using GryphonUtilities.Extensions;
using Telegram.Bot.Types;

namespace _12DHumanBot.Model;

internal sealed class FigureManager
{
    public FigureManager(Bot bot, GoogleSheetsManager.Documents.Document document)
    {
        _bot = bot;
        _document = document;
        _all = _document.GetOrAddSheet(_bot.Config.GoogleTitleAll, AdditionalConverters);

        _figures = new Dictionary<string, Figure>();
        Figure.Types = _bot.Config.LengthNames.ToDictionary(p => p.Key.ToByte().GetValue(), p => p.Value);
    }

    public async Task Load(Chat chat)
    {
        await using (await StatusMessage.CreateAsync(_bot, chat, "Загружаю базу", GetStatusPostfixLoad))
        {
            SheetData<FigureInfo> data = await _all.LoadAsync<FigureInfo>(_bot.Config.GoogleRange);

            _vertices = data.Instances
                            .Where(i => i.Length == 1)
                            .Select(i => i.Convert())
                            .OfType<Vertex>()
                            .ToDictionary(v => v.Number, v => v);

            _figures.Clear();
            foreach (Figure figure in data.Instances
                                            .Where(i => i.Length > 1)
                                            .Select(i => i.Convert(_vertices))
                                            .RemoveNulls())
            {
                _figures[figure.GetCode()] = figure;
            }

            foreach (Vertex v in _vertices.Values)
            {
                _figures[v.GetCode()] = v;
            }
        }
    }

    public async Task Generate(Chat chat)
    {
        await using (await StatusMessage.CreateAsync(_bot, chat, "Генерирую фигуры", GetStatusPostfixGenerate))
        {
            _vertices = new Dictionary<byte, Vertex>();
            _figures.Clear();

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
        }

        List<string> titles = await _all.LoadTitlesAsync(_bot.Config.GoogleRange);
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

        SheetData<FigureInfo> data;
        await using (await StatusMessage.CreateAsync(_bot, chat, "Обновляю базу данными из рабочего листа"))
        {
            Sheet working = await GetWorkingSheetAsync();
            data = await working.LoadAsync<FigureInfo>(_bot.Config.GoogleRange);
            foreach (Figure f in data.Instances
                                     .Select(i => i.Convert(_vertices))
                                     .RemoveNulls())
            {
                Figure figure = _figures[f.GetCode()];
                figure.Name = f.Name;
                figure.Comment = f.Comment;
            }
        }

        await Save(chat, _figures.Values, data.Titles);
    }

    public bool Contains(string code) => _figures.ContainsKey(code);

    public async Task Separate(Chat chat, string code)
    {
        string title = string.Format(_bot.Config.GoogleTitleWorkingTemplate, code);
        await using (await StatusMessage.CreateAsync(_bot, chat, $"Настраиваю рабочий лист для {code}"))
        {
            Sheet working = await GetWorkingSheetAsync();
            await working.RenameAsync(title);

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

            await working.ClearAsync(_bot.Config.GoogleRange);

            List<FigureInfo> infos = sorted.Select(f => f.Convert()).ToList();
            List<string> titles = await _all.LoadTitlesAsync(_bot.Config.GoogleRange);
            SheetData<FigureInfo> data = new(infos, titles);
            await working.SaveAsync(_bot.Config.GoogleRange, data);
        }
    }

    private string GetStatusPostfixLoad() => $"{Environment.NewLine}Загружено фигур: {_figures.Count}\\.";
    private string GetStatusPostfixGenerate() => $"{Environment.NewLine}Создано фигур: {_figures.Count}\\.";

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
        await using (await StatusMessage.CreateAsync(_bot, chat, "Сохраняю базу в таблицу"))
        {
            SheetData<FigureInfo> data = new(figures.Order().Select(f => f.Convert()).ToList(), titles);
            await _all.SaveAsync(_bot.Config.GoogleRange, data);
        }
    }

    private static readonly Dictionary<Type, Func<object?, object?>> AdditionalConverters = new()
    {
        { typeof(byte), o => o.ToByte() }
    };

    private async Task<Sheet> GetWorkingSheetAsync()
    {
        _working ??= await _document.GetOrAddSheetAsync(WorkingSheetIndex, AdditionalConverters);
        return _working.GetValue($"Can't load sheet with id {WorkingSheetIndex}");
    }

    private Sheet? _working;

    private const int WorkingSheetIndex = 1;

    private readonly Bot _bot;
    private readonly GoogleSheetsManager.Documents.Document _document;
    private readonly Sheet _all;
    private readonly Dictionary<string, Figure> _figures;
    private Dictionary<byte, Vertex>? _vertices;
}