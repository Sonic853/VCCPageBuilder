using Newtonsoft.Json;
using VRC.PackageManagement.Core.Types.Packages;

namespace Sonic853.VCCPageBuilder.Models;

// public class Package
// {
//     [JsonProperty("id")]
//     public string Id { get; set; } = string.Empty;

//     [JsonProperty("name")]
//     public string Name { get; set; } = string.Empty;

//     [JsonProperty("version")]
//     public string Version { get; set; } = string.Empty;

//     [JsonProperty("displayName")]
//     public string DisplayName { get; set; } = string.Empty;

//     [JsonProperty("description")]
//     public string Description { get; set; } = string.Empty;

//     [JsonProperty("unity")]
//     public string Unity { get; set; } = string.Empty;

//     [JsonProperty("documentationUrl")]
//     public string? DocumentationUrl { get; set; }

//     [JsonProperty("changelogUrl")]
//     public string? ChangelogUrl { get; set; }

//     [JsonProperty("licensesUrl")]
//     public string? LicensesUrl { get; set; }

//     [JsonProperty("license")]
//     public string? License { get; set; }

//     [JsonProperty("keywords")]
//     public List<string> Keywords { get; set; } = [];

//     [JsonProperty("author")]
//     public PackageAuthor? Author { get; set; }

//     [JsonProperty("hideInEditor")]
//     public bool HideInEditor { get; set; } = false;

//     [JsonProperty("vrchatVersion")]
//     public string? VrchatVersion { get; set; }

//     [JsonProperty("unityRelease")]
//     public string? UnityRelease { get; set; }

//     [JsonProperty("samples")]
//     public List<PackageSample> Samples { get; set; } = [];

//     [JsonProperty("localPath")]
//     public string? LocalPath { get; set; }

//     [JsonProperty("dependencies")]
//     public Dictionary<string, string> Dependencies { get; set; } = [];

//     [JsonProperty("gitDependencies")]
//     public Dictionary<string, string> GitDependencies { get; set; } = [];

//     [JsonProperty("vpmDependencies")]
//     public Dictionary<string, string> VpmDependencies { get; set; } = [];

//     [JsonProperty("headers")]
//     public Dictionary<string, string> Headers { get; set; } = [];

//     [JsonProperty("legacyFolders")]
//     public Dictionary<string, string> LegacyFolders { get; set; } = [];

//     [JsonProperty("legacyFiles")]
//     public Dictionary<string, string> LegacyFiles { get; set; } = [];

//     [JsonProperty("legacyPackages")]
//     public Dictionary<string, string> LegacyPackages { get; set; } = [];
// }

// public class VPMPackage : Package
// {
//     [JsonProperty("url")]
//     public string Url { get; set; } = string.Empty;
//     [JsonProperty("zipSHA256")]
//     public string ZipSHA256 { get; set; } = string.Empty;
//     [JsonProperty("repo")]
//     public string? Repo { get; set; }
// }

// public class PackageAuthor
// {
//     [JsonProperty("name")]
//     public string? Name { get; set; }

//     [JsonProperty("email")]
//     public string? Email { get; set; }

//     [JsonProperty("url")]
//     public string? Url { get; set; }
// }

// public class PackageSample
// {
//     [JsonProperty("displayName")]
//     public string? DisplayName { get; set; }

//     [JsonProperty("description")]
//     public string? Description { get; set; }

//     [JsonProperty("path")]
//     public string? Path { get; set; }
// }
public class VPMPackagesVersions
{

    [JsonProperty("versions")]
    public Dictionary<string, VRCPackageManifest> Versions = [];
}