using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Input;
using VTube_IFacial_Link.DataModels;
using VTube_IFacial_Link.Dialogs;

namespace VTube_IFacial_Link.ViewModels.Commands
{
    internal class AddScriptParameterCommandModel : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public MainViewModel ViewModel { get; private set; }

        public AddScriptParameterCommandModel(MainViewModel viewModel)
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
                Title = "New Parameter",
                XamlRoot = ViewModel.View.Content.XamlRoot,
                Value = "NewParameter",
                DefaultButton = ContentDialogButton.Primary
            };

            while (true)
            {
                ContentDialogResult contentDialogResult = await createNameDialog.ShowAsync();
                if (contentDialogResult == ContentDialogResult.Primary)
                {
                    string name = createNameDialog.Value;
                    ScriptParameterModel newParameter = new ScriptParameterModel() { Name = name, Script = string.Empty };
                    string errorMessage = null;
                    lock (ViewModel.ScriptParameters)
                    {
                        try
                        {
                            ViewModel.ScriptParameters.Add(newParameter);
                            break;
                        }
                        catch (Exception ex)
                        {
                            errorMessage = ex.Message;
                            try
                            {
                                ViewModel.ScriptParameters.Remove(newParameter);
                            }
                            catch (Exception)
                            {
                                // Note: Error will triggered again
                            }
                        }
                    }
                    if (errorMessage != null)
                    {
                        await new ContentDialog
                        {
                            Title = "Failed to Add Parameter",
                            Content = $"{errorMessage}",
                            CloseButtonText = "Ok",
                            XamlRoot = ViewModel.View.Content.XamlRoot
                        }.ShowAsync();
                    }
                }
                else
                {
                    ViewModel.IsBusy = false;
                    break;
                }

            }
        }
    }
}
