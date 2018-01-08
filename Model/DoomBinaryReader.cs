using System.IO;

namespace DoomFileSystemTools.Model
{
    internal class DoomBinaryReader : BinaryReader
    {
        public DoomBinaryReader(FileStream fileStream)
            : base(fileStream) { }
    }
}
