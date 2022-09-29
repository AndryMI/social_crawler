using System.Text.RegularExpressions;

namespace Twitter
{
    public static class TwitterUtils
    {
        /// <summary>Extracts profile part from tweet link (eg twitter.com/some_profile from twitter.com/some_profile/status/...)</summary>
        public static string ExtractProfileLink(string link)
        {
            return Regex.Replace(link, "/status/.*", "");
        }
    }
}
