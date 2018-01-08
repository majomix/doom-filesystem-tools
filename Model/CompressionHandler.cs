using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;

namespace DoomFileSystemTools.Model
{
    internal static class CompressionHandler
    {
        public static byte[] DecompressIfNecessary(ResourcesIndexEntry entry, DoomBinaryReader reader)
        {
            if (entry.CompressedSize != entry.UncompressedSize)
            {
                byte[] decompressedBuffer = new byte[entry.UncompressedSize];
                InflaterInputStream stream = new InflaterInputStream(reader.BaseStream, new Inflater(noHeader: true), 4096);
                int decompressedBytes = stream.Read(decompressedBuffer, 0, decompressedBuffer.Length);
                if (decompressedBytes != decompressedBuffer.Length)
                {
                    throw new InvalidDataException();
                }
                return decompressedBuffer;
            }
            else
            {
                return reader.ReadBytes((int)entry.UncompressedSize);
            }
        }
    }
}
