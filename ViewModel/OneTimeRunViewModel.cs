using DoomFileSystemTools.Model;
using DoomFileSystemTools.ViewModel.Commands;
using NDesk.Options;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace DoomFileSystemTools.ViewModel
{
    internal class OneTimeRunViewModel : BaseViewModel
    {
        private string myTargetDirectory;
        public bool? Export { get; set; }
        public ICommand ExtractByParameterCommand { get; private set; }
        public ICommand ImportByParameterCommand { get; private set; }

        public OneTimeRunViewModel()
        {
            ParseCommandLine();
            Model = new DoomEditor();

            ImportByParameterCommand = new ImportByParameterCommand();
            ExtractByParameterCommand = new ExtractByParameterCommand();
        }

        public void ParseCommandLine()
        {
            OptionSet options = new OptionSet()
                .Add("export", value => Export = true)
                .Add("import", value => Export = false)
                .Add("index=", value => LoadedFilePath = CreateFullPath(value, false))
                .Add("dir=", value => myTargetDirectory = CreateFullPath(value, true));

            options.Parse(Environment.GetCommandLineArgs());
        }

        public void Extract()
        {
            if (myTargetDirectory != null && LoadedFilePath != null)
            {
                LoadStructure();
                ExtractFile(myTargetDirectory);
            }
        }

        public void Import()
        {
            if (myTargetDirectory != null && Directory.Exists(myTargetDirectory) && LoadedFilePath != null)
            {
                LoadStructure();
                ResolveNewFiles(myTargetDirectory);

                string randomName = LoadedFilePath + "_tmp" + new Random().Next().ToString();
                SaveStructure(randomName);

                File.Delete(LoadedFilePath);
                File.Move(randomName, LoadedFilePath);
            }
        }

        private string CreateFullPath(string path, bool isDirectory)
        {
            if (!String.IsNullOrEmpty(path) && !path.Contains(':'))
            {
                path = Directory.GetCurrentDirectory() + @"\" + path.Replace('/', '\\');
            }
            
            return (isDirectory || File.Exists(path)) ? path : null;
        }
    }
}
