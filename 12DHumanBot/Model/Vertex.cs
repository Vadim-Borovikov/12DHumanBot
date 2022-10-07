namespace _12DHumanBot.Model;

internal sealed class Vertex : Figure
{
    public readonly byte Number;

    public string GetName()
    {
        if (!string.IsNullOrWhiteSpace(Name))
        {
            return Name;
        }

        return Types is null ? "" : $"{{{Types[1]} {Number}}}";
    }

    public Vertex(byte number, string? name = null, string? comment = null)
        : base(new List<byte> { number }, name, comment)
    {
        Number = number;
        Vertices.Add(this);
    }

    public static string GetCode(IEnumerable<Vertex> vertices)
    {
        return string.Join(CodeSeparator, vertices.Select(v => v.Number));
    }

    public const string CodeSeparator = ";";
}