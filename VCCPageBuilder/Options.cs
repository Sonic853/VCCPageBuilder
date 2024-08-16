using CommandLine;

namespace Sonic853.VCCPageBuilder;

/// <summary>
/// 命令行参数定义
/// </summary>
public sealed class Options
{
    public Options()
    {
        JsonPath = string.Empty;
        WebPath = string.Empty;
        OutputPath = string.Empty;
        BannerUrl = string.Empty;
    }

    /// <summary>
    /// input vpm file path.
    /// </summary>
    [Option('v', "vpm", Default = "vpm.json", HelpText = "input vpm file path.")]
    public string JsonPath
    {
        get;
        set;
    }
    /// <summary>
    /// input webfile path.
    /// </summary>
    [Option('w', "web", Default = "Website", HelpText = "input webfile path.")]
    public string WebPath
    {
        get;
        set;
    }
    /// <summary>
    /// output webfile path.
    /// </summary>
    [Option('o', "out", Default = "output", HelpText = "output webfile path.")]
    public string OutputPath
    {
        get;
        set;
    }
    /// <summary>
    /// Banner url or path.
    /// </summary>
    [Option('b', "banner", Default = "", HelpText = "Banner url or path.")]
    public string BannerUrl
    {
        get;
        set;
    }
}
