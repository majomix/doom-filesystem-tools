using System.IO;

namespace DoomFileSystemTools.Model
{
    internal class DoomBinaryWriter : BinaryWriter
    {
        public DoomBinaryWriter(Stream stream)
            : base(stream) { }
    }
}
