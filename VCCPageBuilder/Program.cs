﻿// dotnet publish VCCPageBuilder/VCCPageBuilder.csproj -c Release -o ./output /p:PublishSingleFile=true /p:RuntimeIdentifier=linux-x64 /p:SelfContained=false
using System.Text;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sonic853.VCCPageBuilder;
using Sonic853.VCCPageBuilder.Models;
using VRC.PackageManagement.Automation.Multi;
using VRC.PackageManagement.Core.Types.Packages;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var startTime = DateTime.Now;
var retValue = await Parser.Default.ParseArguments<Options>(args)
     .MapResult(Run, _ => Task.FromResult(1));
var endTime = DateTime.Now;
var dur = endTime - startTime;
Console.WriteLine($"Conversion complete in [{dur.TotalMilliseconds}ms].");

static async Task<int> Run(Options options)
{
    var sourceFile = options.SourcePath;
    var vpmFile = options.JsonPath;
    var templatePath = options.WebPath;
    var outputPath = options.OutputPath;
    var bannerUrl = options.BannerUrl;

    ListingSource? source = null;

    if (File.Exists(sourceFile))
    {
        source = JsonConvert.DeserializeObject<ListingSource>(await File.ReadAllTextAsync(sourceFile));
    }
    var _bannerUrl = "banner.png";
    if (string.IsNullOrEmpty(bannerUrl)) bannerUrl = source?.bannerUrl;
    if (string.IsNullOrEmpty(bannerUrl) && File.Exists(Path.Combine(templatePath, "banner.png")))
    {
        bannerUrl = _bannerUrl;
    }
    const string WebPageIndexFilename = "index.html";
    // const string VRCAgent = "VCCBootstrap/1.0";
    const string WebPageAppFilename = "app.js";

    // var vpm = JsonConvert.DeserializeObject<VPM>(await File.ReadAllTextAsync(vpmFile));
    JObject? vpmobj = null;

    if (File.Exists(vpmFile))
        vpmobj = JObject.Parse(await File.ReadAllTextAsync(vpmFile));

    if (vpmobj == null) { return 0; }

    var listingInfo = new
    {
        Name = source?.name ?? vpmobj["name"]?.ToString()!,
        Url = source?.url ?? vpmobj["url"]?.ToString()!,
        Description = source?.description ?? vpmobj["description"]?.ToString()!,
        InfoLink = new
        {
            Text = source?.infoLink.text,
            Url = source?.infoLink.url,
        },
        Author = new
        {
            Name = source?.author.name ?? vpmobj["author"]?.ToString(),
            Url = source?.author.url,
            Email = source?.author.email
        },
        BannerImage = !string.IsNullOrEmpty(bannerUrl),
        BannerImageUrl = bannerUrl,
    };

    // var lastpackages = new List<VRCPackageManifest>();
    var lastpackagesobj = new List<JToken>();
    var packagesobj = vpmobj["packages"];
    // Dictionary<string, VPMPackagesVersions>? packages = null;
    Dictionary<string, JToken>? _packagesobj = null;

    if (packagesobj != null && packagesobj.Type == JTokenType.Object)
    {
        // packages = packagesobj.ToObject<Dictionary<string, VPMPackagesVersions>>();
        _packagesobj = packagesobj.ToObject<Dictionary<string, JToken>>();
    }

    if (_packagesobj != null)
    {

        foreach (var package in _packagesobj)
        {
            var versionsobj = package.Value["versions"];
            if (versionsobj == null) { continue; }
            Dictionary<string, JToken>? versions = versionsobj.ToObject<Dictionary<string, JToken>>();
            if (versions == null) { continue; }
            foreach (var version in versions)
            {
                lastpackagesobj.Add(version.Value);
                break;
            }
        }
    }
    // if (packages != null)
    // {
    //     foreach (var package in packages)
    //     {
    //         foreach (var version in package.Value.Versions)
    //         {
    //             lastpackages.Add(version.Value);
    //             break;
    //         }
    //     }
    // }
    var formattedPackagesobj = lastpackagesobj.ConvertAll(p =>
    {
        // Console.WriteLine(p.ToString());
        // Console.WriteLine(p["licensesUrl"]?.ToString());
        var authorobj = p["author"];
        VPMAuthor? author = null;

        if (authorobj != null && authorobj.Type == JTokenType.Object)
        {
            author = authorobj.ToObject<VPMAuthor>();
        }

        var vRCPackageManifest = p.ToObject<VRCPackageManifest>();
        return new
        {
            Name = p["name"]?.ToString(),
            Author = new
            {
                Name = author?.name ?? p["author"]?.ToString(),
                Url = author?.url,
            },
            ZipUrl = p["url"]?.ToString(),
            Homepage = p["homepage"]?.ToString(),
            License = p["license"]?.ToString(),
            LicensesUrl = p["licensesUrl"]?.ToString(),
            Keywords = p["keywords"]?.ToObject<string[]>(),
            Description = p["description"]?.ToString(),
            DisplayName = p["displayName"]?.ToString(),
            Version = p["version"]?.ToString(),
            Type = GetPackageType(vRCPackageManifest),
            Dependencies = vRCPackageManifest?.VPMDependencies.Select(dep => new
            {
                Name = dep.Key,
                Version = dep.Value
            }
            ).ToList()
        };
    });
    // var formattedPackages = lastpackages.ConvertAll(p => new
    // {
    //     Name = p.Id,
    //     Author = new
    //     {
    //         Name = p.author?.name,
    //         Url = p.author?.url,
    //     },
    //     ZipUrl = p.Url,
    //     License = p.license,
    //     LicensesUrl = p.licensesUrl,
    //     Keywords = p.keywords,
    //     Type = GetPackageType(p),
    //     p.Description,
    //     DisplayName = p.Title,
    //     p.Version,
    //     Dependencies = p.VPMDependencies.Select(dep => new
    //     {
    //         Name = dep.Key,
    //         Version = dep.Value
    //     }
    //     ).ToList(),
    // });

    // Console.WriteLine(JsonConvert.SerializeObject(formattedPackagesobj, Formatting.Indented));

    CopyFilesRecursively(templatePath, outputPath);

    var indexTemplatePath = Path.Combine(templatePath, WebPageIndexFilename);
    var appReadPath = Path.Combine(templatePath, WebPageAppFilename);
    var indexTemplateContent = await File.ReadAllTextAsync(indexTemplatePath);
    var appReadContent = await File.ReadAllTextAsync(appReadPath);
    var rendered = await Scriban.Template.Parse(indexTemplateContent, indexTemplatePath).RenderAsync(
        new { listingInfo, packages = formattedPackagesobj }, member => member.Name
    );
    var appJsRendered = await Scriban.Template.Parse(appReadContent, appReadPath).RenderAsync(
        new { listingInfo, packages = formattedPackagesobj }, member => member.Name
    );

    File.WriteAllText(Path.Combine(outputPath, WebPageIndexFilename), rendered);
    File.WriteAllText(Path.Combine(outputPath, WebPageAppFilename), appJsRendered);

    // Console.WriteLine($"Saved Listing to {outputPath}.");
    return 0;
}

static string GetPackageType(IVRCPackage? p)
{
    string result = "Any";
    if (p is not VRCPackageManifest manifest) return result;

    try
    {
        if (manifest.ContainsAvatarDependencies()) result = "Avatar";
        else if (manifest.ContainsWorldDependencies()) result = "World";
    }
    catch (Exception)
    { }

    return result;
}

static void CopyFilesRecursively(string sourcePath, string targetPath)
{
    if (!Directory.Exists(targetPath))
        Directory.CreateDirectory(targetPath);
    //Now Create all of the directories
    foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
    {
        Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
    }

    //Copy all the files & Replaces any files with the same name
    foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
    {
        File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }
}