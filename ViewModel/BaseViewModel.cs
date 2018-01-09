using DoomFileSystemTools.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace DoomFileSystemTools.ViewModel
{
    internal abstract class BaseViewModel : INotifyPropertyChanged
    {
        private int myCurrentProgress = 100;
        private string myLoadedFilePath;
        private string myCurrentFile;
        private bool myHasError;

        public DoomEditor Model { get; protected set; }
        public string LoadedFilePath
        {
            get { return myLoadedFilePath; }
            set
            {
                if (myLoadedFilePath != value)
                {
                    myLoadedFilePath = value;
                    OnPropertyChanged("LoadedFilePath");
                }
            }
        }
        public string CurrentFile
        {
            get { return myCurrentFile; }
            protected set
            {
                if (myCurrentFile != value)
                {
                    myCurrentFile = value;
                    OnPropertyChanged("CurrentFile");
                }
            }
        }
        public int CurrentProgress
        {
            get { return myCurrentProgress; }
            protected set
            {
                if (myCurrentProgress != value)
                {
                    myCurrentProgress = value;
                    OnPropertyChanged("CurrentProgress");
                }
            }
        }
        public bool HasError
        {
            get { return myHasError; }
            set
            {
                if (myHasError != value)
                {
                    myHasError = value;
                    OnPropertyChanged("HasError");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler RequestClose;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void OnRequestClose(EventArgs e)
        {
            RequestClose(this, e);
        }

        public void LoadStructure()
        {
            using (DoomBinaryReader reader = new DoomBinaryReader(File.Open(LoadedFilePath, FileMode.Open)))
            {
                Model.LoadFileStructure(reader);
                OnPropertyChanged("Model");
            }
        }

        public void ExtractFile(string directory)
        {
            using (DoomBinaryReader reader = new DoomBinaryReader(File.Open(Path.ChangeExtension(LoadedFilePath, ".resources"), FileMode.Open)))
            {
                IEnumerable<ResourcesIndexEntry> entries = Model.Index.Entries.Where(entry => entry.FileSystemName != null);
                long currentSize = 0;
                long totalSize = entries.Sum(_ => _.UncompressedSize);

                foreach (ResourcesIndexEntry entry in entries)
                {
                    if (reader.BaseStream.Position != (long)entry.Offset)
                    {
                        reader.BaseStream.Seek((long)entry.Offset, SeekOrigin.Begin);
                    }

                    Model.ExtractFile(directory, entry, reader);
                    CurrentProgress = (int)(currentSize * 100.0 / totalSize);
                    CurrentFile = entry.InternalName;
                    currentSize += entry.UncompressedSize;
                }
            }
        }

        public void ResolveNewFiles(string directory)
        {
            foreach (string file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
            {
                string internalPath = file.Split(new string[] { directory + @"\" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(@"\", @"/");
                {
                    ResourcesIndexEntry entry = Model.Index.Entries.SingleOrDefault(_ => _.FileSystemName == internalPath);
                    
                    if(entry != null)
                    {
                        entry.Changed = file;
                    }
                }
            }
        }

        public void SaveStructure(string path)
        {
            using (DoomBinaryWriter resourcesWriter = new DoomBinaryWriter(File.Open(Path.ChangeExtension(LoadedFilePath, ".resources"), FileMode.Append)))
            {
                IEnumerable<ResourcesIndexEntry> entries = Model.Index.Entries.Where(_ => _.Changed != null);
                long currentSize = 0;
                long totalSize = entries.Sum(_ => _.UncompressedSize);

                foreach (ResourcesIndexEntry entry in entries)
                {
                    Model.SaveDataEntry(entry, resourcesWriter);
                    CurrentProgress = (int)(currentSize * 100.0 / totalSize);
                    CurrentFile = entry.Changed;
                    currentSize += entry.UncompressedSize;
                }
            }

            using (DoomBinaryWriter indexWriter = new DoomBinaryWriter(File.Open(path, FileMode.Create)))
            {
                Model.SaveIndex(indexWriter);
            }

            OnPropertyChanged("Model");
        }
    }
}
