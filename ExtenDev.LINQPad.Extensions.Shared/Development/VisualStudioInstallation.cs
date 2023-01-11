using System;
using Newtonsoft.Json;

namespace ExtenDev.LINQPad.Extensions.Development
{
    public class VisualStudioInstallation
    {
        [JsonProperty("instanceId")] public string? InstanceId { get; set; }
        [JsonProperty("installDate")] public DateTime InstallDate { get; set; }
        [JsonProperty("installationName")] public string? InstallationName { get; set; }
        [JsonProperty("installationPath")] public string? InstallationPath { get; set; }
        [JsonProperty("installationVersion")] public string? InstallationVersion { get; set; }
        [JsonProperty("productId")] public string? ProductId { get; set; }
        [JsonProperty("productPath")] public string? ProductPath { get; set; }
        [JsonProperty("state")] public long State { get; set; }
        [JsonProperty("isComplete")] public bool IsComplete { get; set; }
        [JsonProperty("isLaunchable")] public bool IsLaunchable { get; set; }
        [JsonProperty("isPrerelease")] public bool IsPrerelease { get; set; }
        [JsonProperty("isRebootRequired")] public bool IsRebootRequired { get; set; }
        [JsonProperty("displayName")] public string? DisplayName { get; set; }
        [JsonProperty("description")] public string? Description { get; set; }
        [JsonProperty("channelId")] public string? ChannelId { get; set; }
        [JsonProperty("channelUri")] public string? ChannelUri { get; set; }
        [JsonProperty("enginePath")] public string? EnginePath { get; set; }
        [JsonProperty("installedChannelId")] public string? InstalledChannelId { get; set; }
        [JsonProperty("installedChannelUri")] public string? InstalledChannelUri { get; set; }
        [JsonProperty("releaseNotes")] public string? ReleaseNotes { get; set; }
        [JsonProperty("thirdPartyNotices")] public string? ThirdPartyNotices { get; set; }
        [JsonProperty("updateDate")] public DateTime UpdateDate { get; set; }
        [JsonProperty("catalog")] public VisualStudioInstallationCatalog? Catalog { get; set; }
        [JsonProperty("properties")] public VisualStudioInstallationProperties? Properties { get; set; }
    }
}
