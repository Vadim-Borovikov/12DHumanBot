﻿using GoogleSheetsManager;
using GryphonUtilities;
using Newtonsoft.Json;

namespace _12DHumanBot.Web.Models;

public sealed class ConfigJson : IConvertibleTo<Config>
{
    [JsonProperty]
    public string? Token { get; set; }
    [JsonProperty]
    public string? SystemTimeZoneId { get; set; }
    [JsonProperty]
    public string? DontUnderstandStickerFileId { get; set; }
    [JsonProperty]
    public string? ForbiddenStickerFileId { get; set; }
    [JsonProperty]
    public double? UpdatesPerSecondLimitPrivate { get; set; }
    [JsonProperty]
    public double? UpdatesPerMinuteLimitGroup { get; set; }
    [JsonProperty]
    public double? UpdatesPerSecondLimitGlobal { get; set; }

    [JsonProperty]
    public string? Host { get; set; }
    [JsonProperty]
    public List<string?>? About { get; set; }
    [JsonProperty]
    public List<string?>? ExtraCommands { get; set; }
    [JsonProperty]
    public List<long?>? AdminIds { get; set; }
    [JsonProperty]
    public long? SuperAdminId { get; set; }

    [JsonProperty]
    public string? GoogleCredentialJson { get; set; }
    [JsonProperty]
    public string? ApplicationName { get; set; }
    [JsonProperty]
    public string? GoogleSheetId { get; set; }

    [JsonProperty]
    public string? GoogleRange { get; set; }
    [JsonProperty]
    public string? GoogleRangeAll { get; set; }
    [JsonProperty]
    public string? GoogleRangeWorkingTemplate { get; set; }

    [JsonProperty]
    public string? AdminIdsJson { get; set; }
    [JsonProperty]
    public Dictionary<string, string?>? GoogleCredential { get; set; }

    [JsonProperty]
    public byte? MaxLength { get; set; }
    [JsonProperty]
    public Dictionary<string, string?>? LengthNames { get; set; }

    public Config Convert()
    {
        string token = Token.GetValue(nameof(Token));
        string systemTimeZoneId = SystemTimeZoneId.GetValue(nameof(SystemTimeZoneId));
        string dontUnderstandStickerFileId = DontUnderstandStickerFileId.GetValue(nameof(DontUnderstandStickerFileId));
        string forbiddenStickerFileId = ForbiddenStickerFileId.GetValue(nameof(ForbiddenStickerFileId));

        double updatesPerSecondLimitPrivate =
            UpdatesPerSecondLimitPrivate.GetValue(nameof(UpdatesPerSecondLimitPrivate));
        TimeSpan sendMessagePeriodPrivate = TimeSpan.FromSeconds(1.0 / updatesPerSecondLimitPrivate);

        double updatesPerMinuteLimitGroup = UpdatesPerMinuteLimitGroup.GetValue(nameof(UpdatesPerMinuteLimitGroup));
        TimeSpan sendMessagePeriodGroup = TimeSpan.FromMinutes(1.0 / updatesPerMinuteLimitGroup);

        double updatesPerSecondLimitGlobal = UpdatesPerSecondLimitGlobal.GetValue(nameof(UpdatesPerSecondLimitGlobal));
        TimeSpan sendMessagePeriodGlobal = TimeSpan.FromSeconds(1.0 / updatesPerSecondLimitGlobal);

        string googleCredentialJson = string.IsNullOrWhiteSpace(GoogleCredentialJson)
            ? JsonConvert.SerializeObject(GoogleCredential)
            : GoogleCredentialJson;
        string applicationName = ApplicationName.GetValue(nameof(ApplicationName));
        string googleSheetId = GoogleSheetId.GetValue(nameof(GoogleSheetId));

        string googleRange = GoogleRange.GetValue(nameof(GoogleRange));
        string googleRangeAll = GoogleRangeAll.GetValue(nameof(GoogleRangeAll));
        string googleRangeWorkingTemplate = GoogleRangeWorkingTemplate.GetValue(nameof(GoogleRangeWorkingTemplate));

        if (AdminIds is null || (AdminIds.Count == 0))
        {
            string json = AdminIdsJson.GetValue(nameof(AdminIdsJson));
            AdminIds = JsonConvert.DeserializeObject<List<long?>>(json);
        }

        byte maxLength = MaxLength.GetValue(nameof(MaxLength));
        Dictionary<string, string?> lengthNames = LengthNames.GetValue(nameof(LengthNames));
        Dictionary<byte, string> lengthNamesValue =
            lengthNames.ToDictionary(p => p.Key.ToByte().GetValue(), p => p.Value.GetValue());
        IEnumerable<long> adminIds =
            AdminIds is null ? Enumerable.Empty<long>() : AdminIds.Select(id => id.GetValue("Admin id"));

        return new Config(token, systemTimeZoneId, dontUnderstandStickerFileId, forbiddenStickerFileId,
            sendMessagePeriodPrivate, sendMessagePeriodGroup, sendMessagePeriodGlobal, googleCredentialJson,
            applicationName, googleSheetId, googleRange, googleRangeAll, googleRangeWorkingTemplate, maxLength,
            lengthNamesValue)
        {
            Host = Host,
            About = About is null ? null : string.Join(Environment.NewLine, About),
            ExtraCommands = ExtraCommands is null ? null : string.Join(Environment.NewLine, ExtraCommands),
            AdminIds = adminIds.ToList(),
            SuperAdminId = SuperAdminId,
        };
    }
}