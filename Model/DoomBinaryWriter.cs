using System.IO;
using System.Text;

namespace DoomFileSystemTools.Model
{
    internal class DoomBinaryWriter : BinaryWriter
    {
        public DoomBinaryWriter(Stream stream)
            : base(stream) { }

        public void WriteInt32PrefixedString(string stringToWrite)
        {
            int length = string.IsNullOrEmpty(stringToWrite) ? 0 : stringToWrite.Length;
            Write(length);
            
            if(length != 0)
            {
                WriteFixedSizeString(stringToWrite);
            }
        }

        public void WriteFixedSizeString(string stringToWrite)
        {
            Write(Encoding.ASCII.GetBytes(stringToWrite));
        }

        public void Write(ResourcesIndex index)
        {
            Write(index.Version);
            WriteFixedSizeString(index.Signature);
            this.WriteUInt32BE(index.FileSize);
            Write(index.Padding);
            this.WriteUInt32BE(index.NumberOfEntries);
        }

        public void Write(ResourcesIndexEntry entry)
        {
            this.WriteUInt32BE(entry.Identifier);
            WriteInt32PrefixedString(entry.ResourceType);
            WriteInt32PrefixedString(entry.InternalName);
            WriteInt32PrefixedString(entry.FileSystemName);
            this.WriteUInt64BE(entry.Offset);
            this.WriteUInt32BE(entry.UncompressedSize);
            this.WriteUInt32BE(entry.CompressedSize);
            Write(entry.Zero);
            Write(entry.PatchNumber);
        }
    }
}
