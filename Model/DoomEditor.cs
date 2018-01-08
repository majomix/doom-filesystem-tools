using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
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

        public void SaveDataEntry(DoomBinaryReader reader, DoomBinaryWriter writer)
        {

        }

        public void SaveIndex(DoomBinaryWriter writer)
        {

        }
    }
}
