using System;
using System.IO;

namespace DoomFileSystemTools.Model
{
    internal class DoomEditor
    {
        public ResourcesIndex Index { get; private set; }

        public void LoadFileStructure(DoomBinaryReader reader)
        {
            Index = reader.ReadResourcesIndex();

            for(int i = 0; i < Index.NumberOfEntries; i++)
            {
                Index.Entries.Add(reader.ReadResourcesIndexEntry());
            }
        }

        public void ExtractFile(string directory, ResourcesIndexEntry entry, DoomBinaryReader reader)
        {
            string compoundName = directory + @"\" + entry.FileSystemName.Replace(@"/", @"\");
            if (!compoundName.Contains(@"\")) return;

            Directory.CreateDirectory(Path.GetDirectoryName(compoundName));

            using (BinaryWriter writer = new BinaryWriter(File.Open(compoundName, FileMode.Create)))
            {
                writer.Write(CompressionHandler.DecompressIfNecessary(entry, reader));
            }
        }

        public void SaveDataEntry(ResourcesIndexEntry entry, DoomBinaryWriter writer)
        {
            using (DoomBinaryReader reader = new DoomBinaryReader(File.Open(entry.Changed, FileMode.Open)))
            {
                entry.Offset = (UInt64)writer.BaseStream.Position;
                entry.UncompressedSize = (UInt32)new FileInfo(entry.Changed).Length;
                entry.CompressedSize = entry.UncompressedSize;
                writer.Write(reader.ReadBytes((int)entry.UncompressedSize));

                int alignment = Align((int)entry.UncompressedSize, 16);
                writer.Write(new byte[alignment]);
            }
        }

        public void SaveIndex(DoomBinaryWriter writer)
        {
            writer.Write(Index);

            foreach(ResourcesIndexEntry entry in Index.Entries)
            {
                writer.Write(entry);
            }
        }

        private int Align(int size, int alignnment)
        {
            return alignnment - size % alignnment;
        }
    }
}
