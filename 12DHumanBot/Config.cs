using AbstractBot;

namespace _12DHumanBot;

public class Config : ConfigGoogleSheets
{
    public readonly string GoogleRange;

    internal long? LogsChatId => SuperAdminId;

    public Config(string token, string systemTimeZoneId, string dontUnderstandStickerFileId,
        string forbiddenStickerFileId, TimeSpan sendMessageDelay, string googleCredentialJson, string applicationName,
        string googleSheetId, string googleRange)
        : base(token, systemTimeZoneId, dontUnderstandStickerFileId, forbiddenStickerFileId, sendMessageDelay,
            googleCredentialJson, applicationName, googleSheetId)
    {
        GoogleRange = googleRange;
    }
}
