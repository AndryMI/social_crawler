using Core.Data;
using System.Collections.Generic;

namespace Core.Storages
{
    public class StorageApiClient : ApiServerClient
    {
        public void StoreProfiles(List<IProfileInfo> data)
        {
            if (data.Count > 0)
            {
                Request("POST", "/crawler/profiles", data);
            }
        }

        public void StorePosts(List<IPostInfo> data)
        {
            if (data.Count > 0)
            {
                Request("POST", "/crawler/posts", data);
            }
        }

        public void StoreComments(List<ICommentInfo> data)
        {
            if (data.Count > 0)
            {
                Request("POST", "/crawler/comments", data);
            }
        }
    }
}
