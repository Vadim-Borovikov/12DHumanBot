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
    {
        Number = number;
        Vertices.Add(this);

        Name = name;
        Comment = comment;
    }

    public static string GetCode(SortedSet<Vertex> vertices)
    {
        return string.Join(CodeSeparator, vertices.Select(v => v.Number));
    }
}