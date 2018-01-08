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

        public void ExtractFile(string directory, DoomBinaryReader reader)
        {

        }

        public void SaveDataEntry(DoomBinaryReader reader, DoomBinaryWriter writer)
        {

        }

        public void SaveIndex(DoomBinaryWriter writer)
        {

        }
    }
}
