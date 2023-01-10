using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Input;
using VTube_IFacial_Link.DataModels;
using VTube_IFacial_Link.Dialogs;

namespace VTube_IFacial_Link.ViewModels.Commands
{
    internal class AddScriptGlobalCommandModel : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public MainViewModel ViewModel { get; private set; }

        public AddScriptGlobalCommandModel(MainViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            CreateNameDialog createNameDialog = new()
            {
                Title = "New Global Variable",
                XamlRoot = ViewModel.View.Content.XamlRoot,
                Value = "NEW_GLOBAL",
                DefaultButton = ContentDialogButton.Primary
            };
            ContentDialogResult contentDialogResult = await createNameDialog.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                string name = createNameDialog.Value;
                ViewModel.ScriptGlobals.Add(new ScriptGlobalModel() { Name = name, Value = 0 });
            }
        }
    }
}
