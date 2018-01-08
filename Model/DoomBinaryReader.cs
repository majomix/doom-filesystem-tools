using System.IO;
using System.Text;

namespace DoomFileSystemTools.Model
{
    internal class DoomBinaryReader : BinaryReader
    {
        public DoomBinaryReader(FileStream fileStream)
            : base(fileStream) { }

        public string ReadFixedSizeString(int size)
        {
            return Encoding.ASCII.GetString(ReadBytes(size));
        }

        public string ReadInt32PrefixedString()
        {
            int stringLength = ReadInt32();
            return stringLength == 0 ? null : ReadFixedSizeString(stringLength);
        }

        public ResourcesIndex ReadResourcesIndex()
        {
            ResourcesIndex index = new ResourcesIndex();

            index.Version = ReadByte();
            index.Signature = ReadFixedSizeString(3);
            index.FileSize = this.ReadUInt32BE();
            index.Padding = ReadBytes(24);
            index.NumberOfEntries = this.ReadUInt32BE();

            return index;
        }

        public ResourcesIndexEntry ReadResourcesIndexEntry()
        {
            ResourcesIndexEntry entry = new ResourcesIndexEntry();

            entry.Identifier = this.ReadUInt32BE();
            entry.ResourceType = ReadInt32PrefixedString();
            entry.InternalName = ReadInt32PrefixedString();
            entry.FileSystemName = ReadInt32PrefixedString();
            entry.Offset = this.ReadUInt64BE();
            entry.UncompressedSize = this.ReadUInt32BE();
            entry.CompressedSize = this.ReadUInt32BE();
            entry.Zero = this.ReadUInt32();
            entry.PatchNumber = ReadByte();

            return entry;
        }
    }
}
