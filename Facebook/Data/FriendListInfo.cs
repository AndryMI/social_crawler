using Core.Data;
using System.Collections.Generic;

namespace Facebook.Data
{
    public class FriendListInfo : IFriendListInfo
    {
        public string Social => "facebook";

        public string Link { get; set; }

        public string[] TargetLinks { get; set; }

        public class Collector
        {
            private readonly List<string> targets = new List<string>();
            private string link;

            public bool IsEmpty => targets.Count == 0 || string.IsNullOrEmpty(link);

            public void AddRange(RelationInfo[] relations)
            {
                if (string.IsNullOrEmpty(link))
                {
                    link = relations[0].Link;
                }
                foreach (var relation in relations)
                {
                    targets.Add(relation.TargetLink);
                }
            }

            public FriendListInfo Collected()
            {
                return new FriendListInfo
                {
                    Link = link,
                    TargetLinks = targets.ToArray(),
                };
            }
        }
    }
}
