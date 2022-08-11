using Serilog.Sinks.File;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CrawlerApp.Utils
{
    public class GZipHooks : FileLifecycleHooks
    {
        public override Stream OnFileOpened(Stream underlyingStream, Encoding encoding)
        {
            return new GZip(underlyingStream);
        }

        private class GZip : Stream
        {
            private readonly Stream stream;

            public GZip(Stream stream)
            {
                this.stream = stream;
            }

            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;

            public override void Write(byte[] buffer, int offset, int count)
            {
                using (var gzip = new GZipStream(stream, CompressionLevel.Fastest, true))
                {
                    gzip.Write(buffer, offset, count);
                }
            }

            public override void Flush()
            {
                stream.Flush();
            }

            public override long Length => throw new NotImplementedException();
            public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
            public override void SetLength(long value) => throw new NotImplementedException();
        }
    }
}
