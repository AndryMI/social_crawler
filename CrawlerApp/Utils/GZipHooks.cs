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
            private const long Treshold = short.MaxValue;
            private readonly Stream memory = new MemoryStream();
            private readonly Stream output;

            private long position;
            private Stream buffer;

            public GZip(Stream stream)
            {
                output = stream;
                buffer = new GZipStream(memory, CompressionLevel.Optimal, true);
                position = output.Position;
            }

            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;

            public override void Write(byte[] data, int offset, int count)
            {
                using (var gzip = new GZipStream(output, CompressionLevel.Fastest, true))
                {
                    gzip.Write(data, offset, count);
                }
                buffer.Write(data, offset, count);
            }

            public override void Flush()
            {
                if (memory.Length > Treshold)
                {
                    buffer.Close();
                    output.SetLength(position);

                    memory.Position = 0;
                    memory.CopyTo(output);

                    output.Flush();
                    memory.SetLength(0);

                    buffer = new GZipStream(memory, CompressionLevel.Optimal, true);
                    position = output.Position;
                }
                else
                {
                    output.Flush();
                }
            }

            public override long Length => throw new NotImplementedException();
            public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
            public override void SetLength(long value) => throw new NotImplementedException();
        }
    }
}