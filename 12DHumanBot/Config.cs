using System.ComponentModel.DataAnnotations;
using AbstractBot;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace _12DHumanBot;

[PublicAPI]
public class Config : ConfigGoogleSheets
{
    [Required]
    [MinLength(1)]
    public string GoogleSheetId { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleRange { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleRangeAll { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleRangeWorkingTemplate { get; init; } = null!;

    [Required]
    [Range(1, byte.MaxValue)]
    public byte MaxLength { get; init; }

    [Required]
    public Dictionary<string, string> LengthNames { get; init; } = null!;
}