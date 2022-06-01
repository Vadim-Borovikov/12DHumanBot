using GoogleSheetsManager;
using GryphonUtilities;

namespace _12DHumanBot.Model;

internal class Figure : ISavable, IComparable<Figure>
{
    IList<string> ISavable.Titles => Titles;

    public readonly SortedSet<Vertex> Vertices = new();

    public string? Name;
    public string? Comment;

    public readonly List<byte>? Numbers;

    public byte GetLength() => (byte) Vertices.Count;

    public string GetCode() => Vertex.GetCode(Vertices);

    public Figure(IEnumerable<Vertex> vertices) => Vertices = new SortedSet<Vertex>(vertices);

    protected Figure() { }

    private Figure(List<byte> numbers, string? name, string? comment)
    {
        Numbers = numbers;
        Name = name;
        Comment = comment;
    }

    public static Figure Load(IDictionary<string, object?> valueSet)
    {
        List<byte> numbers = valueSet[VerticesTitle].ToBytes().GetValue(VerticesTitle);
        string? name = valueSet[NameTitle]?.ToString();
        string? comment = valueSet[CommentTitle]?.ToString();

        return numbers.Count > 1
            ? new Figure(numbers, name, comment)
            : new Vertex(numbers[0], name, comment);
    }

    public IDictionary<string, object?> Convert()
    {
        return new Dictionary<string, object?>
        {
            { VerticesTitle, GetCode() },
            { VerticesNamesTitle, string.Join(NamesSeparator, Vertices.Select(v => v.GetName())) },
            { LengthTitle, GetLength() },
            { TypeTitle, Types?[GetLength()] },
            { NameTitle, Name },
            { CommentTitle, Comment },
        };
    }

    public int CompareTo(Figure? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null)
        {
            return 1;
        }

        int lengthCompare = GetLength().CompareTo(other.GetLength());
        if (lengthCompare != 0)
        {
            return lengthCompare;
        }

        // ReSharper disable once LoopCanBeConvertedToQuery
        List<Vertex> myVertices = Vertices.ToList();
        List<Vertex> othersVertices = other.Vertices.ToList();
        for (int i = 0; i < Vertices.Count; ++i)
        {
            int c = myVertices[i].Number.CompareTo(othersVertices[i].Number);
            if (c != 0)
            {
                return c;
            }
        }

        return 0;
    }

    public static Dictionary<byte, string>? Types;

    private static readonly IList<string> Titles = new List<string>
    {
        VerticesTitle,
        VerticesNamesTitle,
        LengthTitle,
        TypeTitle,
        NameTitle,
        CommentTitle,
    };

    private const string VerticesTitle = "Номера вершин";
    private const string VerticesNamesTitle = "Названия вершин";
    private const string LengthTitle = "Длина";
    private const string TypeTitle = "Тип";
    private const string NameTitle = "Название";
    private const string CommentTitle = "Комментарий";

    public const string CodeSeparator = ";";
    private const string NamesSeparator = "; ";
}
