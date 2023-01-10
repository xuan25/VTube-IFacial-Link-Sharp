using System;
using System.Windows.Input;
using VTube_IFacial_Link.DataModels;

namespace VTube_IFacial_Link.ViewModels.Commands
{
    internal class RemoveScriptParameterCommandModel : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public MainViewModel ViewModel { get; private set; }

        public RemoveScriptParameterCommandModel(MainViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ScriptParameterModel scriptGlobalModel = (ScriptParameterModel)parameter;
            lock (ViewModel.ScriptParameters)
            {
                ViewModel.ScriptParameters.Remove(scriptGlobalModel);
            }
        }
    }
}
