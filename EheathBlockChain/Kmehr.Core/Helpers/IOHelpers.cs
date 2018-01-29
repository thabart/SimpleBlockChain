using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Kmehr.Core.Helpers
{
    public static class IOHelpers
    {
        public static IEnumerable<byte> Compress(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            
            byte[] result = null;
            using (var inputStream = new MemoryStream(input))
            {
                using (var compressedStream = new MemoryStream())
                {
                    using (var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress))
                    {
                        inputStream.CopyTo(compressionStream);
                        compressedStream.Close();
                        result = compressedStream.ToArray();
                    }
                }
            }

            return result;
        }
    }
}
