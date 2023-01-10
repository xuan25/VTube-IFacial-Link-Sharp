using System.Runtime.CompilerServices;
using System;
using VTube_IFacial_Link.DataModels;
using VTube_IFacial_Link.Models;
using VTube_IFacial_Link.ViewModels.Commands;
using VTube_IFacial_Link.Views;

namespace VTube_IFacial_Link.ViewModels
{
    internal class MainViewModel : BindableBase
    {
        internal MainPage View { private set; get; }
        internal MainModel Model { private set; get; }

        public StartCommandModel StartCommand { private set; get; }
        public StopCommandModel StopCommand { private set; get; }
        public BrowseAppDataCommandModel BrowseAppDataCommand { private set; get; }
        public AddScriptGlobalCommandModel AddScriptGlobalCommand { private set; get; }
        public RemoveScriptGlobalCommandModel RemoveScriptGlobalCommand { private set; get; }
        public AddScriptParameterCommandModel AddScriptParameterCommand { private set; get; }
        public RemoveScriptParameterCommandModel RemoveScriptParameterCommand { private set; get; }

        public MainViewModel(MainPage view)
        {
            View = view;
            Model = new MainModel(this);

            StartCommand = new StartCommandModel(this);
            StopCommand = new StopCommandModel(this);
            BrowseAppDataCommand = new BrowseAppDataCommandModel(this);
            AddScriptGlobalCommand = new AddScriptGlobalCommandModel(this);
            RemoveScriptGlobalCommand = new RemoveScriptGlobalCommandModel(this);
            AddScriptParameterCommand = new AddScriptParameterCommandModel(this);
            RemoveScriptParameterCommand = new RemoveScriptParameterCommandModel(this);
        }

        private ScriptParameterCollection<ScriptParameterModel> _scriptParameters = null;
        public ScriptParameterCollection<ScriptParameterModel> ScriptParameters
        {
            get => _scriptParameters;
            set => Set(ref _scriptParameters, value, nameof(ScriptParameters));
        }

        private ScriptGlobalCollection<ScriptGlobalModel> _scriptGlobals = null;
        public ScriptGlobalCollection<ScriptGlobalModel> ScriptGlobals
        {
            get => _scriptGlobals;
            set => Set(ref _scriptGlobals, value, nameof(ScriptGlobals));
        }

        public CapturedDataModel _capDataModel = null;
        public CapturedDataModel CapDataModel
        {
            get => _capDataModel;
            set => Set(ref _capDataModel, value, nameof(CapDataModel));
        }

        private string _iFacialAddress = string.Empty;
        public string IFacialAddress
        {
            get => _iFacialAddress;
            set => Set(ref _iFacialAddress, value, nameof(IFacialAddress));
        }

        private string _vTubeAddress = "ws://127.0.0.1:8001";
        public string VTubeAddress
        {
            get => _vTubeAddress;
            set => Set(ref _vTubeAddress, value, nameof(VTubeAddress));
        }

        private bool _canStart = true;
        public bool CanStart
        {
            get => _canStart;
            set
            {
                if (Set(ref _canStart, value, nameof(CanStart)))
                {
                    StartCommand.OnCanExecuteChanged();
                }
            }
        }

        private bool _canStop = false;
        public bool CanStop
        {
            get => _canStop;
            set
            {
                if (Set(ref _canStop, value, nameof(CanStop)))
                {
                    StopCommand.OnCanExecuteChanged();
                }
            }
        }

        private bool _startOnLaunch = false;
        public bool StartOnLaunch
        {
            get => _startOnLaunch;
            set
            {
                if (Set(ref _startOnLaunch, value, nameof(StartOnLaunch)))
                {
                    if (View.IsLoaded)
                    {
                        Model.SaveConfig();
                    }
                }
            }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value, nameof(IsBusy));
        }

        private string _busyMessage = string.Empty;
        public string BusyMessage
        {
            get => _busyMessage;
            set => Set(ref _busyMessage, value, nameof(BusyMessage));
        }

        public void Start() => Model.Start();

        public void Stop() => Model.Stop();

        public void SaveConfig() => Model.SaveConfig();

        public void SaveScripts() => Model.SaveScripts();

        public void ShowMessageDialog(string title, string content) => View.ShowMessageDialog(title, content);

    }
}
