using _12DHumanBot.Model;
using GoogleSheetsManager;
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
        return o?.ToString()?.Split(Figure.CodeSeparator).Select(s => s.ToByte()).RemoveNulls().ToList();
    }
}
