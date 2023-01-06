// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using IFacial;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using VTube;
using VTube_IFacial_Link.DataModel;
using VTube_IFacial_Link.Dialogs;
using VTube_IFacial_Link.Pages;
using VTube_IFacial_Link.Utils;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VTube_IFacial_Link
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(HomePage)),
            ("data", typeof(DataPage)),
            ("globals", typeof(GlobalsPage)),
            ("parameters", typeof(ParametersPage)),
        };

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                throw new NotImplementedException("No Setting Page");
                //_page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        public class StartCommandModel : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public MainPage Parent { get; private set; }

            public StartCommandModel(MainPage parent)
            {
                Parent = parent;
            }

            public bool CanExecute(object parameter)
            {
                return Parent.CanStart;
            }

            public void Execute(object parameter)
            {
                Parent.Start();
            }

            public void OnCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public class StopCommandModel : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public MainPage Parent { get; private set; }

            public StopCommandModel(MainPage parent)
            {
                Parent = parent;
            }

            public bool CanExecute(object parameter)
            {
                return Parent.CanStop;
            }

            public void Execute(object parameter)
            {
                Parent.Stop();
            }

            public void OnCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public class BrowseAppDataCommandModel : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public MainPage Parent { get; private set; }

            public BrowseAppDataCommandModel(MainPage parent)
            {
                Parent = parent;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                Parent.BrowseAppData();
            }
        }

        public StartCommandModel StartCommand { private set; get; }
        public StopCommandModel StopCommand { private set; get; }
        public BrowseAppDataCommandModel BrowseAppDataCommand { private set; get; }

        public class AddScriptGlobalCommandModel : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public MainPage Parent { get; private set; }

            public AddScriptGlobalCommandModel(MainPage parent)
            {
                Parent = parent;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public async void Execute(object parameter)
            {
                CreateNameDialog createNameDialog = new()
                {
                    Title = "Create New Global Variable",
                    XamlRoot = Parent.Content.XamlRoot,
                    Value = "NEW_GLOBAL",
                    DefaultButton = ContentDialogButton.Primary
                };
                ContentDialogResult contentDialogResult = await createNameDialog.ShowAsync();
                if (contentDialogResult == ContentDialogResult.Primary) {
                    string name = createNameDialog.Value;
                    Parent.ScriptGlobals.Add(new ScriptGlobalModel() { Name = name, Value = 0 });
                }
            }
        }

        public class RemoveScriptGlobalCommandModel : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public MainPage Parent { get; private set; }

            public RemoveScriptGlobalCommandModel(MainPage parent)
            {
                Parent = parent;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                ScriptGlobalModel scriptGlobalModel = (ScriptGlobalModel)parameter;
                Parent.ScriptGlobals.Remove(scriptGlobalModel);
            }
        }

        public class AddScriptParameterCommandModel : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public MainPage Parent { get; private set; }

            public AddScriptParameterCommandModel(MainPage parent)
            {
                Parent = parent;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public async void Execute(object parameter)
            {
                CreateNameDialog createNameDialog = new()
                {
                    Title = "Create New Parameter",
                    XamlRoot = Parent.Content.XamlRoot,
                    Value = "NewParameter",
                    DefaultButton = ContentDialogButton.Primary
                };
                ContentDialogResult contentDialogResult = await createNameDialog.ShowAsync();
                if (contentDialogResult == ContentDialogResult.Primary)
                {
                    string name = createNameDialog.Value;
                    lock (Parent.ScriptParameters)
                    {
                        Parent.ScriptParameters.Add(new ScriptParameterModel() { Name = name, Script = string.Empty });
                    }
                }
            }
        }

        public class RemoveScriptParameterCommandModel : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public MainPage Parent { get; private set; }

            public RemoveScriptParameterCommandModel(MainPage parent)
            {
                Parent = parent;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                ScriptParameterModel scriptGlobalModel = (ScriptParameterModel)parameter;
                lock (Parent.ScriptParameters)
                {
                    Parent.ScriptParameters.Remove(scriptGlobalModel);
                }
            }
        }

        public AddScriptGlobalCommandModel AddScriptGlobalCommand { private set; get; }
        public RemoveScriptGlobalCommandModel RemoveScriptGlobalCommand { private set; get; }
        public AddScriptParameterCommandModel AddScriptParameterCommand { private set; get; }
        public RemoveScriptParameterCommandModel RemoveScriptParameterCommand { private set; get; }

        readonly string configPath;
        readonly string scriptsPath;

        public class ConfigStore
        {
            public string IFacialAddress { get; set; }
            public string VTubeAddress { get; set; }
            public bool StartOnLaunch { get; set; }
        }

        public class ScriptStore
        {
            public ScriptParameterCollection<ScriptParameterModel> Parameters { get; set; }
            public ScriptGlobalCollection<ScriptGlobalModel> Globals { get; set; }
        }

        public MainPage()
        {
            StartCommand = new StartCommandModel(this);
            StopCommand = new StopCommandModel(this);
            BrowseAppDataCommand = new BrowseAppDataCommandModel(this);
            AddScriptGlobalCommand = new AddScriptGlobalCommandModel(this);
            RemoveScriptGlobalCommand = new RemoveScriptGlobalCommandModel(this);
            AddScriptParameterCommand = new AddScriptParameterCommandModel(this);
            RemoveScriptParameterCommand = new RemoveScriptParameterCommandModel(this);

            configPath = Path.Combine(PathUtils.ConfigPath, "config-ui.json");
            scriptsPath = Path.Combine(PathUtils.ConfigPath, "scripts.json");

            this.Loaded += MainPage_Loaded;
            InitializeComponent();

            Directory.CreateDirectory(PathUtils.ConfigPath);

            LoadConfig();
            LoadScripts();
        }

        private void BrowseAppData()
        {
            Launcher.LaunchFolderPathAsync(PathUtils.ConfigPath).AsTask().Wait();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).MainWindow.Closed += MainWindow_Closed;

            if (StartOnLaunch)
            {
                Start();
            }
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            Stop();
            SaveScripts();
        }

        private bool LoadConfig()
        {
            System.Diagnostics.Debug.WriteLine($"Loading config... ({configPath})");
            try
            {
                if (!File.Exists(configPath))
                {
                    return false;
                }

                ConfigStore config;
                using (FileStream configStream = File.OpenRead(configPath))
                {
                    config = JsonSerializer.Deserialize<ConfigStore>(configStream);
                }

                if (config == null)
                {
                    return false;
                }

                IFacialAddress = config.IFacialAddress;
                VTubeAddress = config.VTubeAddress;
                StartOnLaunch = config.StartOnLaunch;

                return true;

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine($"Failed to load config. ({configPath})");
                return false;
            }

        }

        private void SaveConfig()
        {
            ConfigStore config = new()
            {
                IFacialAddress = IFacialAddress,
                VTubeAddress = VTubeAddress,
                StartOnLaunch = StartOnLaunch,
            };
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
            using (FileStream configStream = File.OpenWrite(configPath))
            {
                JsonSerializer.Serialize<ConfigStore>(configStream, config);
            }
            System.Diagnostics.Debug.WriteLine($"Config saved. ({configPath})");
        }

        private bool LoadScripts()
        {
            System.Diagnostics.Debug.WriteLine($"Loading scripts... ({scriptsPath})");
            try
            {
                if (File.Exists(scriptsPath))
                {
                    ScriptStore config;
                    using (FileStream configStream = File.OpenRead(scriptsPath))
                    {
                        config = JsonSerializer.Deserialize<ScriptStore>(configStream);
                    }

                    if (config != null)
                    {
                        ScriptParameters = config.Parameters;
                        ScriptGlobals = config.Globals;
                    }
                }

                if (ScriptParameters == null)
                {
                    using (FileStream configStream = File.OpenRead("default-parameters.json"))
                    {
                        JsonSerializerOptions serializeOptions = new()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = false
                        };
                        ScriptParameters = JsonSerializer.Deserialize<ScriptParameterCollection<ScriptParameterModel>>(configStream, serializeOptions);
                    }
                }

                if (ScriptGlobals == null)
                {
                    using (FileStream configStream = File.OpenRead("default-globals.json"))
                    {
                        JsonSerializerOptions serializeOptions = new()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = false
                        };
                        ScriptGlobals = JsonSerializer.Deserialize<ScriptGlobalCollection<ScriptGlobalModel>>(configStream, serializeOptions);
                    }
                }

                return true;

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine($"Failed to load scripts. ({scriptsPath})");
                return false;
            }
        }

        private void SaveScripts()
        {
            ScriptStore config = new()
            {
                Parameters = ScriptParameters,
                Globals = ScriptGlobals,
            };
            if (File.Exists(scriptsPath))
            {
                File.Delete(scriptsPath);
            }
            using (FileStream configStream = File.OpenWrite(scriptsPath))
            {
                JsonSerializer.Serialize<ScriptStore>(configStream, config);
            }
            System.Diagnostics.Debug.WriteLine($"Scripts saved. ({scriptsPath})");
        }

        public static readonly DependencyProperty ScriptParametersProperty = DependencyProperty.Register(nameof(ScriptParameters), typeof(ScriptParameterCollection<ScriptParameterModel>), typeof(MainPage), new PropertyMetadata(null));
        public ScriptParameterCollection<ScriptParameterModel> ScriptParameters
        {
            get => (ScriptParameterCollection<ScriptParameterModel>)GetValue(ScriptParametersProperty);
            set => SetValue(ScriptParametersProperty, value);
        }

        public static readonly DependencyProperty ScriptGlobalsProperty = DependencyProperty.Register(nameof(ScriptGlobals), typeof(ScriptGlobalCollection<ScriptGlobalModel>), typeof(MainPage), new PropertyMetadata(null));
        public ScriptGlobalCollection<ScriptGlobalModel> ScriptGlobals
        {
            get => (ScriptGlobalCollection<ScriptGlobalModel>)GetValue(ScriptGlobalsProperty);
            set => SetValue(ScriptGlobalsProperty, value);
        }


        public static readonly DependencyProperty CapDataModelProperty = DependencyProperty.Register(nameof(CapDataModel), typeof(CapturedDataModel), typeof(MainPage), new PropertyMetadata(null));
        public CapturedDataModel CapDataModel
        {
            get => (CapturedDataModel)GetValue(CapDataModelProperty);
            set => SetValue(CapDataModelProperty, value);
        }

        public static readonly DependencyProperty IFacialAddressProperty = DependencyProperty.Register(nameof(IFacialAddress), typeof(string), typeof(MainPage), new PropertyMetadata(string.Empty));
        public string IFacialAddress
        {
            get => (string)GetValue(IFacialAddressProperty);
            set => SetValue(IFacialAddressProperty, value);
        }

        public static readonly DependencyProperty VTubeAddressProperty = DependencyProperty.Register(nameof(VTubeAddress), typeof(string), typeof(MainPage), new PropertyMetadata("ws://127.0.0.1:8001"));
        public string VTubeAddress
        {
            get => (string)GetValue(VTubeAddressProperty);
            set => SetValue(VTubeAddressProperty, value);
        }

        public static readonly DependencyProperty CanStartProperty = DependencyProperty.Register(nameof(CanStart), typeof(bool), typeof(MainPage), new PropertyMetadata(true, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            MainPage mainPage = (MainPage)d;
            mainPage.StartCommand.OnCanExecuteChanged();
        }));
        public bool CanStart
        {
            get => (bool)GetValue(CanStartProperty);
            set => SetValue(CanStartProperty, value);
        }

        public static readonly DependencyProperty CanStopProperty = DependencyProperty.Register(nameof(CanStop), typeof(bool), typeof(MainPage), new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            MainPage mainPage = (MainPage)d;
            mainPage.StopCommand.OnCanExecuteChanged();
        }));
        public bool CanStop
        {
            get => (bool)GetValue(CanStopProperty);
            set => SetValue(CanStopProperty, value);
        }

        public static readonly DependencyProperty StartOnLaunchProperty = DependencyProperty.Register(nameof(StartOnLaunch), typeof(bool), typeof(MainPage), new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            MainPage mainPage = (MainPage)d;
            if (mainPage.IsLoaded)
            {
                mainPage.SaveConfig();
            }
        }));
        public bool StartOnLaunch
        {
            get => (bool)GetValue(StartOnLaunchProperty);
            set => SetValue(StartOnLaunchProperty, value);
        }

        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(MainPage), new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            MainPage mainPage = (MainPage)d;
            TimeSpan animationTime = new(250 * 10000);
            if ((bool)e.NewValue)
            {
                TimeSpan holdTime = new(1000 * 10000);
                Storyboard storyboard = new();
                {
                    DoubleAnimation doubleAnimation = new()
                    {
                        EnableDependentAnimation = true,
                        BeginTime = holdTime,
                        Duration = animationTime,
                        From = 1.15,
                        To = 1,
                        EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                    };
                    Storyboard.SetTarget(doubleAnimation, mainPage);
                    Storyboard.SetTargetProperty(doubleAnimation, nameof(BusyMessageScale));
                    storyboard.Children.Add(doubleAnimation);
                }
                {
                    DoubleAnimation doubleAnimation = new()
                    {
                        EnableDependentAnimation = true,
                        BeginTime = holdTime,
                        Duration = animationTime,
                        From = 0,
                        To = 1,
                        EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                    };
                    Storyboard.SetTarget(doubleAnimation, mainPage);
                    Storyboard.SetTargetProperty(doubleAnimation, nameof(BusyMessageOpacity));
                    storyboard.Children.Add(doubleAnimation);
                }
                storyboard.Begin();
            }
            else
            {
                Storyboard storyboard = new();
                {
                    DoubleAnimation doubleAnimation = new()
                    {
                        EnableDependentAnimation = true,
                        Duration = animationTime,
                        To = 1.15,
                        EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                    };
                    Storyboard.SetTarget(doubleAnimation, mainPage);
                    Storyboard.SetTargetProperty(doubleAnimation, nameof(BusyMessageScale));
                    storyboard.Children.Add(doubleAnimation);
                }
                {
                    DoubleAnimation doubleAnimation = new()
                    {
                        EnableDependentAnimation = true,
                        Duration = animationTime,
                        To = 0,
                        EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                    };
                    Storyboard.SetTarget(doubleAnimation, mainPage);
                    Storyboard.SetTargetProperty(doubleAnimation, nameof(BusyMessageOpacity));
                    storyboard.Children.Add(doubleAnimation);
                }
                storyboard.Begin();
            }
        }));

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set
            {
                SetValue(IsBusyProperty, value);
            }
        }

        public static readonly DependencyProperty BusyMessageOpacityProperty = DependencyProperty.Register(nameof(BusyMessageOpacity), typeof(double), typeof(MainPage), new PropertyMetadata(0d));
        public double BusyMessageOpacity
        {
            get => (double)GetValue(BusyMessageOpacityProperty);
            set => SetValue(BusyMessageOpacityProperty, value);
        }

        public static readonly DependencyProperty BusyMessageScaleProperty = DependencyProperty.Register(nameof(BusyMessageScale), typeof(double), typeof(MainPage), new PropertyMetadata(1d));
        public double BusyMessageScale
        {
            get => (double)GetValue(BusyMessageScaleProperty);
            set => SetValue(BusyMessageScaleProperty, value);
        }

        public static readonly DependencyProperty BusyMessageProperty = DependencyProperty.Register(nameof(BusyMessage), typeof(string), typeof(MainPage), new PropertyMetadata(string.Empty));
        public string BusyMessage
        {
            get => (string)GetValue(BusyMessageProperty);
            set => SetValue(BusyMessageProperty, value);
        }

        IFacialClient facialClient;
        VTubeClient vtubeClient;

        private void Start()
        {
            BusyMessage = "Starting...";
            IsBusy = true;
            CanStart = false;

            try
            {
                BusyMessage = "Initializing...";
                SaveConfig();

                facialClient = new IFacialClient(IPAddress.Parse(IFacialAddress));
                CapDataModel = new CapturedDataModel(facialClient.Data);
                facialClient.DataUpdated += FacialClient_DataUpdated;
                facialClient.ExceptionOccurred += FacialClient_ExceptionOccurred;

                ScriptParameterConverter parameterConverter = new ScriptParameterConverter(ScriptParameters, ScriptGlobals);
                vtubeClient = new VTubeClient(new Uri(VTubeAddress), facialClient.Data, Path.Combine(PathUtils.ConfigPath, "config-vtube.json"), parameterConverter);
                vtubeClient.ExceptionOccurred += VtubeClient_ExceptionOccurred;

            }
            catch (System.Exception ex)
            {
                BusyMessage = string.Empty;
                new ContentDialog
                {
                    Title = "Failed to Initialize",
                    Content = $"{ex.Message}",
                    CloseButtonText = "Ok",
                    XamlRoot = this.Content.XamlRoot
                }.ShowAsync().GetResults();
                Stop();
                CanStart = true;
                IsBusy = false;

                return;
            }

            Task.Factory.StartNew(() =>
            {
                try
                {
                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        BusyMessage = " Connecting to Capturing Device...";
                    });
                    facialClient.Connect();
                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        BusyMessage = " Starting Captures...";
                    });
                    facialClient.Start();
                }
                catch (System.Exception ex)
                {

                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        BusyMessage = string.Empty;
                        new ContentDialog
                        {
                            Title = "Failed to Connect Capturing Device",
                            Content = $"{ex.Message}",
                            CloseButtonText = "Ok",
                            XamlRoot = this.Content.XamlRoot
                        }.ShowAsync().GetResults();
                        Stop();
                        CanStart = true;
                        IsBusy = false;
                    });
                    return;
                }

                try
                {
                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        BusyMessage = "Connecting to VTube Studio...";
                    });
                    vtubeClient.Connect((string message) =>
                    {
                        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                        {
                            BusyMessage = $"Connecting to VTube Studio...\n{message}";
                        });
                    });
                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        BusyMessage = "Initializing VTube Studio...";
                    });
                    vtubeClient.Init();
                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        BusyMessage = "Starting VTube Studio Plugin...";
                    });
                    vtubeClient.Start();
                }
                catch (System.Exception ex)
                {
                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        BusyMessage = string.Empty;
                        new ContentDialog
                        {
                            Title = "Failed to Connect VTube Studio",
                            Content = $"{ex.Message}",
                            CloseButtonText = "Ok",
                            XamlRoot = this.Content.XamlRoot
                        }.ShowAsync().GetResults();
                        Stop();
                        CanStart = true;
                        IsBusy = false;
                    });
                    return;
                }

                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    BusyMessage = string.Empty;
                    CanStop = true;
                    IsBusy = false;
                });
            });
        }

        private void VtubeClient_ExceptionOccurred(VTubeClient sender, System.Exception exception)
        {
            Task.Factory.StartNew(() =>
            {
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    Stop();
                    new ContentDialog
                    {
                        Title = "Error Occurred with VTube Studio",
                        Content = $"{exception.Message}",
                        CloseButtonText = "Ok",
                        XamlRoot = this.Content.XamlRoot
                    }.ShowAsync().GetResults();
                });
            });

        }

        private void FacialClient_ExceptionOccurred(IFacialClient sender, System.Exception exception)
        {
            Task.Factory.StartNew(() =>
            {
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    Stop();
                    new ContentDialog
                    {
                        Title = "Error Occurred with Capturing Device",
                        Content = $"{exception.Message}",
                        CloseButtonText = "Ok",
                        XamlRoot = this.Content.XamlRoot
                    }.ShowAsync().GetResults();
                });
            });

        }

        private void Stop()
        {
            IsBusy = true;
            CanStop = false;
            try
            {
                if (facialClient != null)
                {
                    facialClient.Stop();
                    facialClient.Dispose();
                    facialClient = null;
                }
                if (vtubeClient != null)
                {
                    vtubeClient.Stop();
                    vtubeClient.ParamConverter.Dispose();
                    vtubeClient.Dispose();
                    vtubeClient = null;
                }
                CanStart = true;
                IsBusy = false;
            }
            catch (System.Exception ex)
            {
                new ContentDialog
                {
                    Title = "Failed to Stop",
                    Content = $"{ex.Message}",
                    CloseButtonText = "Ok",
                    XamlRoot = this.Content.XamlRoot
                }.ShowAsync().GetResults();
                CanStop = true;
                IsBusy = false;
            }

        }

        private void FacialClient_DataUpdated(object sender, EventArgs e)
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                CapDataModel.NotifyDataChanged();
            });
        }

    }
}
