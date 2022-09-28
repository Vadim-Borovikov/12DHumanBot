using AbstractBot;

namespace _12DHumanBot;

public sealed class Config : ConfigGoogleSheets
{
    public readonly string GoogleRange;
    public readonly string GoogleRangeAll;
    public readonly string GoogleRangeWorkingTemplate;

    internal readonly byte MaxLength;

    internal readonly Dictionary<byte, string> LengthNames;

    internal long? LogsChatId => SuperAdminId;

    public Config(string token, string systemTimeZoneId, string dontUnderstandStickerFileId,
        string forbiddenStickerFileId, TimeSpan sendMessagePeriodPrivate, TimeSpan sendMessagePeriodGroup,
        TimeSpan sendMessageDelayGlobal, string googleCredentialJson, string applicationName, string googleSheetId,
        string googleRange, string googleRangeAll, string googleRangeWorkingTemplate, byte maxLength,
        Dictionary<byte, string> lengthNames)
        : base(token, systemTimeZoneId, dontUnderstandStickerFileId, forbiddenStickerFileId, sendMessagePeriodPrivate,
            sendMessagePeriodGroup, sendMessageDelayGlobal, googleCredentialJson, applicationName, googleSheetId)
    {
        GoogleRange = googleRange;
        GoogleRangeAll = googleRangeAll;
        GoogleRangeWorkingTemplate = googleRangeWorkingTemplate;
        MaxLength = maxLength;
        LengthNames = lengthNames;
    }
}
