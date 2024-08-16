using System.Text;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sonic853.VCCPageBuilder;
using Sonic853.VCCPageBuilder.Models;
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
    var vpmFile = options.JsonPath;
    var templatePath = options.WebPath;
    var outputPath = options.OutputPath;
    var bannerUrl = options.BannerUrl;
    var _bannerUrl = Path.Combine(templatePath, "banner.png");
    if (string.IsNullOrEmpty(bannerUrl) && File.Exists(_bannerUrl))
    {
        bannerUrl = _bannerUrl;
    }
    const string WebPageIndexFilename = "index.html";
    // const string VRCAgent = "VCCBootstrap/1.0";
    const string WebPageAppFilename = "app.js";

    // var vpm = JsonConvert.DeserializeObject<VPM>(await File.ReadAllTextAsync(vpmFile));
    var vpmobj = JObject.Parse(await File.ReadAllTextAsync(vpmFile));

    if (vpmobj == null) { return 0; }

    var authorobj = vpmobj["author"];
    VPMAuthor? author = null;

    if (authorobj != null && authorobj.Type == JTokenType.Object)
    {
        author = authorobj.ToObject<VPMAuthor>();
    }

    var infoLink = vpmobj["infoLink"]?.ToObject<VPMInfoLink>();

    var listingInfo = new
    {
        Name = vpmobj["name"]?.ToString()!,
        Url = vpmobj["url"]?.ToString()!,
        Description = vpmobj["description"]?.ToString()!,
        InfoLink = new
        {
            Text = infoLink?.text,
            Url = infoLink?.url,
        },
        Author = new
        {
            Name = author?.name,
            Url = author?.url,
            Email = author?.email
        },
        BannerImage = !string.IsNullOrEmpty(bannerUrl),
        BannerImageUrl = bannerUrl,
    };

    var lastpackages = new List<VRCPackageManifest>();
    var packagesobj = vpmobj["packages"];
    Dictionary<string, VPMPackagesVersions>? packages = null;

    if (packagesobj != null && packagesobj.Type == JTokenType.Object)
    {
        packages = packagesobj.ToObject<Dictionary<string, VPMPackagesVersions>>();
    }

    if (packages == null)
    {
        return 0;
    }

    foreach (var package in packages)
    {
        foreach (var version in package.Value.Versions)
        {
            lastpackages.Add(version.Value);
            break;
        }
    }
    var formattedPackages = lastpackages.ConvertAll(p => new
    {
        Name = p.Id,
        Author = new
        {
            Name = p.author?.name,
            Url = p.author?.url,
        },
        ZipUrl = p.Url,
        License = p.license,
        LicenseUrl = p.licensesUrl,
        Keywords = p.keywords,
        Type = GetPackageType(p),
        p.Description,
        DisplayName = p.Title,
        p.Version,
        Dependencies = p.VPMDependencies.Select(dep => new
        {
            Name = dep.Key,
            Version = dep.Value
        }
        ).ToList(),
    });

    CopyFilesRecursively(templatePath, outputPath);

    var indexTemplatePath = Path.Combine(templatePath, WebPageIndexFilename);
    var appReadPath = Path.Combine(templatePath, WebPageAppFilename);
    var indexTemplateContent = await File.ReadAllTextAsync(indexTemplatePath);
    var appReadContent = await File.ReadAllTextAsync(appReadPath);
    var rendered = await Scriban.Template.Parse(indexTemplateContent, indexTemplatePath).RenderAsync(
        new { listingInfo, packages = formattedPackages }, member => member.Name
    );
    var appJsRendered = await Scriban.Template.Parse(appReadContent, appReadPath).RenderAsync(
        new { listingInfo, packages = formattedPackages }, member => member.Name
    );

    File.WriteAllText(Path.Combine(outputPath, WebPageIndexFilename), rendered);
    File.WriteAllText(Path.Combine(outputPath, WebPageAppFilename), appJsRendered);

    Console.WriteLine($"Saved Listing to {outputPath}.");
    return 0;
}

static string GetPackageType(IVRCPackage p)
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