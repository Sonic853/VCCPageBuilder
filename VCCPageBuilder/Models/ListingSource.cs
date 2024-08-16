namespace VRC.PackageManagement.Automation.Multi
{
    public class ListingSource
    {
        public string name { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public Author author { get; set; } = new();
        public string url { get; set; } = string.Empty;
        public string description { get; set;} = string.Empty;
        public InfoLink infoLink { get; set; } = new();
        public string bannerUrl { get; set; } = string.Empty;
        public List<PackageInfo> packages { get; set; } = [];
		public List<string> githubRepos { get; set; } = [];
    }

    public class InfoLink {
        public string text { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
    }

    public class Author {
        public string name { get; set;} = string.Empty;
        public string url { get; set;} = string.Empty;
        public string email {get; set;} = string.Empty;
    }
    
    public class PackageInfo
    {
        public string id { get; set; } = string.Empty;
        public List<string> releases { get; set; } = [];
    }
}