using VRC.PackageManagement.Core.Types.Packages;

namespace Sonic853.VCCPageBuilder.Models;

// public class VPM : VRCRepoList 
// {
//     public string description = string.Empty;
//     public VPMInfoLink? infoLink;
// }
public class VPMInfoLink
{
    public string text = string.Empty;
    public string url = string.Empty;
}
public class VPMAuthor
{
    public string name = string.Empty;
    public string url = string.Empty;
    public string email = string.Empty;
}