using AbstractBot;

namespace _12DHumanBot;

public sealed class Config : ConfigGoogleSheets
{
    public readonly string GoogleRange;
    public readonly string GoogleRangeAll;

    internal readonly byte MaxLength;

    internal readonly Dictionary<byte, string> LengthNames;

    internal long? LogsChatId => SuperAdminId;

    public Config(string token, string systemTimeZoneId, string dontUnderstandStickerFileId,
        string forbiddenStickerFileId, TimeSpan sendMessageDelay, string googleCredentialJson, string applicationName,
        string googleSheetId, string googleRange, string googleRangeAll, byte maxLength,
        Dictionary<byte, string> lengthNames)
        : base(token, systemTimeZoneId, dontUnderstandStickerFileId, forbiddenStickerFileId, sendMessageDelay,
            googleCredentialJson, applicationName, googleSheetId)
    {
        GoogleRange = googleRange;
        GoogleRangeAll = googleRangeAll;
        MaxLength = maxLength;
        LengthNames = lengthNames;
    }
}
