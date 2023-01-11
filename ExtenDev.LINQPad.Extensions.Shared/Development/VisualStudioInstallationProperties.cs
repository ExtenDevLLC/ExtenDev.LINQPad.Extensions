using Newtonsoft.Json;

namespace ExtenDev.LINQPad.Extensions.Development
{
    public class VisualStudioInstallationProperties
    {
        [JsonProperty("campaignId")] public string? CampaignId { get; set; }
        [JsonProperty("channelManifestId")] public string? ChannelManifestId { get; set; }
        [JsonProperty("nickname")] public string? Nickname { get; set; }
        [JsonProperty("setupEngineFilePath")] public string? SetupEngineFilePath { get; set; }
    }
}
