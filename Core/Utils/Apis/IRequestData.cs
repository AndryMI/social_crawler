using System.IO;

namespace Core
{
    public interface IRequestData
    {
        string ContentType { get; }
        void Serialize(StreamWriter writer);
    }
}
