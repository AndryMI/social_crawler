using Core.Crawling;
using Core.Data;

namespace Facebook.Data
{
    public class RelationInfo : IRelationInfo
    {
        public string Social => "facebook";
        public string Link { get; set; }
        public string TargetLink { get; set; }
        public string Type { get; set; }

        public string Name;

        public static RelationInfo[] Collect(Browser browser)
        {
            browser.InjectUtils("Scripts/ReactUtils.js");
            return browser.RunCollector<RelationInfo[]>("Scripts/Facebook/RelationInfo.js");
        }
    }
}
