using System;
using System.Windows.Input;
using VTube_IFacial_Link.DataModels;

namespace VTube_IFacial_Link.ViewModels.Commands
{
    internal class RemoveScriptGlobalCommandModel : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public MainViewModel ViewModel { get; private set; }

        public RemoveScriptGlobalCommandModel(MainViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ScriptGlobalModel scriptGlobalModel = (ScriptGlobalModel)parameter;
            ViewModel.ScriptGlobals.Remove(scriptGlobalModel);
        }
    }
}
