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
}