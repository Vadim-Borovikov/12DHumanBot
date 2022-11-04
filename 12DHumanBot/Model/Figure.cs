namespace _12DHumanBot.Model;

internal class Figure : IComparable<Figure>
{
    public readonly SortedSet<Vertex> Vertices = new();

    public string? Name;
    public string? Comment;

    public byte GetLength() => (byte) Vertices.Count;

    public string GetCode() => Vertex.GetCode(Vertices);

    public Figure(IEnumerable<Vertex> vertices, string? name = null, string? comment = null) : this(name, comment)
    {
        Vertices = new SortedSet<Vertex>(vertices);
    }

    protected Figure(string? name, string? comment)
    {
        Name = name;
        Comment = comment;
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

    public FigureInfo Convert()
    {
        return new FigureInfo
        {
            VerticesNumbers = GetCode(),
            VerticesNames = string.Join(NamesSeparator, Vertices.Select(v => v.GetName())),
            Length = GetLength(),
            Type = Types?[GetLength()],
            Name = Name,
            Comment = Comment,
        };
    }

    public static Dictionary<byte, string>? Types;

    private const string NamesSeparator = "; ";
}