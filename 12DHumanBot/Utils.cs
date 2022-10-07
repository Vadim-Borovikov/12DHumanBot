using _12DHumanBot.Model;
using GryphonUtilities;

namespace _12DHumanBot;

internal static class Utils
{
    public static List<byte>? ToBytes(this object? o)
    {
        if (o is IEnumerable<byte> l)
        {
            return l.ToList();
        }
        return o?.ToString()?.Split(Vertex.CodeSeparator).Select(s => s.ToByte()).RemoveNulls().ToList();
    }

    public static byte? ToByte(this object? o)
    {
        if (o is byte b)
        {
            return b;
        }
        return byte.TryParse(o?.ToString(), out b) ? b : null;
    }
}
