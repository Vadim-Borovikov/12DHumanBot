using GoogleSheetsManager;
using JetBrains.Annotations;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace _12DHumanBot.Model;

internal sealed class FigureInfo
{
    [SheetField("Номера вершин")]
    public string VerticesNumbers = null!;

    [SheetField("Названия вершин")]
    [UsedImplicitly]
    public string VerticesNames = null!;

    [SheetField("Длина")]
    [UsedImplicitly]
    public byte Length;

    [SheetField("Тип")]
    [UsedImplicitly]
    public string? Type;

    [SheetField("Название")]
    public string? Name;

    [SheetField("Комментарий")]
    public string? Comment;

    public Figure? Convert(Dictionary<byte, Vertex>? vertices = null)
    {
        List<byte>? numbers = VerticesNumbers.ToBytes();
        return numbers?.Count switch
        {
            null => null,
            0    => null,
            1    => new Vertex(numbers[0], Name, Comment),
            _    => vertices is null ? null : new Figure(numbers.Select(n => vertices[n]), Name, Comment)
        };
    }
}