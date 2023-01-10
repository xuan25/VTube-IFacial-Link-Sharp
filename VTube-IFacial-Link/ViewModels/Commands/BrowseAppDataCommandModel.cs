using System;
using System.Windows.Input;
using VTube_IFacial_Link.Utils;
using Windows.System;

namespace VTube_IFacial_Link.ViewModels.Commands
{
    internal class BrowseAppDataCommandModel : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public MainViewModel ViewModel { get; private set; }

        public BrowseAppDataCommandModel(MainViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            await Launcher.LaunchFolderPathAsync(PathUtils.ConfigPath);
        }
    }
}
