using System;
using System.Windows.Input;

namespace VTube_IFacial_Link.ViewModels.Commands
{
    internal class StopCommandModel : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public MainViewModel ViewModel { get; private set; }

        public StopCommandModel(MainViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return ViewModel.CanStop;
        }

        public void Execute(object parameter)
        {
            ViewModel.Stop();
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
