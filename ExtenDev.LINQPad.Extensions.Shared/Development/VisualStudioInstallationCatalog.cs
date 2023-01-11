using Newtonsoft.Json;

namespace ExtenDev.LINQPad.Extensions.Development
{
    public class VisualStudioInstallationCatalog
    {
        [JsonProperty("buildBranch")] public string? BuildBranch { get; set; }
        [JsonProperty("buildVersion")] public string? BuildVersion { get; set; }
        [JsonProperty("id")] public string? Id { get; set; }
        [JsonProperty("localBuild")] public string? LocalBuild { get; set; }
        [JsonProperty("manifestName")] public string? ManifestName { get; set; }
        [JsonProperty("manifestType")] public string? ManifestType { get; set; }
        [JsonProperty("productDisplayVersion")] public string? ProductDisplayVersion { get; set; }
        [JsonProperty("productLine")] public string? ProductLine { get; set; }
        [JsonProperty("productLineVersion")] public string? ProductLineVersion { get; set; }
        [JsonProperty("productMilestone")] public string? ProductMilestone { get; set; }
        [JsonProperty("productMilestoneIsPreRelease")] public string? ProductMilestoneIsPreRelease { get; set; }
        [JsonProperty("productName")] public string? ProductName { get; set; }
        [JsonProperty("productPatchVersion")] public string? ProductPatchVersion { get; set; }
        [JsonProperty("productPreReleaseMilestoneSuffix")] public string? ProductPreReleaseMilestoneSuffix { get; set; }
        [JsonProperty("productSemanticVersion")] public string? ProductSemanticVersion { get; set; }
        [JsonProperty("requiredEngineVersion")] public string? RequiredEngineVersion { get; set; }
    }
}
