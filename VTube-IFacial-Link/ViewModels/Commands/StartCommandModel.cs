using System;
using System.Windows.Input;

namespace VTube_IFacial_Link.ViewModels.Commands
{
    internal class StartCommandModel : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public MainViewModel ViewModel { get; private set; }

        public StartCommandModel(MainViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return ViewModel.CanStart;
        }

        public void Execute(object parameter)
        {
            ViewModel.Start();
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
