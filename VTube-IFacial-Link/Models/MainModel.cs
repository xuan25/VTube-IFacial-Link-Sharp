using IFacial;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using VTube;
using VTube_IFacial_Link.DataModels;
using VTube_IFacial_Link.Utils;
using VTube_IFacial_Link.ViewModels;

namespace VTube_IFacial_Link.Models
{
    internal class MainModel
    {
        public MainViewModel ViewModel { get; set; }

        public MainModel(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.View.Loaded += View_Loaded;

            Directory.CreateDirectory(PathUtils.ConfigPath);
            configPath = Path.Combine(PathUtils.ConfigPath, "config-ui.json");
            scriptsPath = Path.Combine(PathUtils.ConfigPath, "scripts.json");

            LoadConfig();
            LoadScripts();
        }

        internal void View_Loaded(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).MainWindow.Closed += MainWindow_Closed;

            if (ViewModel.StartOnLaunch)
            {
                Start();
            }
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            Stop();
            SaveScripts();
        }

        #region Config


        private JsonSerializerOptions configSerializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = false,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

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


        public bool LoadConfig()
        {
            System.Diagnostics.Debug.WriteLine($"Loading config... ({configPath})");
            try
            {
                if (!File.Exists(configPath))
                {
                    return false;
                }

                ConfigStore store;
                using (FileStream configStream = File.OpenRead(configPath))
                {
                    store = JsonSerializer.Deserialize<ConfigStore>(configStream, configSerializeOptions);
                }

                if (store == null)
                {
                    return false;
                }

                ViewModel.IFacialAddress = store.IFacialAddress;
                ViewModel.VTubeAddress = store.VTubeAddress;
                ViewModel.StartOnLaunch = store.StartOnLaunch;

                return true;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine($"Failed to load config. ({configPath})");
                return false;
            }

        }

        public void SaveConfig()
        {
            ConfigStore store = new()
            {
                IFacialAddress = ViewModel.IFacialAddress,
                VTubeAddress = ViewModel.VTubeAddress,
                StartOnLaunch = ViewModel.StartOnLaunch,
            };
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
            using (FileStream configStream = File.OpenWrite(configPath))
            {
                JsonSerializer.Serialize(configStream, store, configSerializeOptions);
            }
            System.Diagnostics.Debug.WriteLine($"Config saved. ({configPath})");
        }

        public bool LoadScripts()
        {
            System.Diagnostics.Debug.WriteLine($"Loading scripts... ({scriptsPath})");
            try
            {
                if (File.Exists(scriptsPath))
                {
                    ScriptStore store;
                    using (FileStream configStream = File.OpenRead(scriptsPath))
                    {
                        store = JsonSerializer.Deserialize<ScriptStore>(configStream, configSerializeOptions);
                    }

                    ViewModel.ScriptParameters = store.Parameters;
                    ViewModel.ScriptGlobals = store.Globals;

                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Scripts not found ({scriptsPath})");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine($"Failed to load scripts. ({scriptsPath})");
            }

            // Load default
            {
                System.Diagnostics.Debug.WriteLine($"Loading default scripts... ({scriptsPath})");
                ScriptStore store;
                using (FileStream configStream = File.OpenRead("default-scripts.json"))
                {
                    store = JsonSerializer.Deserialize<ScriptStore>(configStream, configSerializeOptions);
                }

                if (store != null)
                {
                    ViewModel.ScriptParameters = store.Parameters;
                    ViewModel.ScriptGlobals = store.Globals;
                }
            }

            return false;
        }

        public void SaveScripts()
        {
            ScriptStore store = new()
            {
                Parameters = ViewModel.ScriptParameters,
                Globals = ViewModel.ScriptGlobals,
            };
            if (File.Exists(scriptsPath))
            {
                File.Delete(scriptsPath);
            }
            using (FileStream configStream = File.OpenWrite(scriptsPath))
            {
                JsonSerializer.Serialize(configStream, store, configSerializeOptions);
            }
            System.Diagnostics.Debug.WriteLine($"Scripts saved. ({scriptsPath})");
        }


        #endregion

        #region Client

        IFacialClient facialClient;
        VTubeClient vtubeClient;

        internal async void Start()
        {
            ViewModel.BusyMessage = "Starting...";
            ViewModel.IsBusy = true;
            ViewModel.CanStart = false;

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    ViewModel.BusyMessage = "Initializing...";
                    SaveConfig();

                    facialClient = new IFacialClient(IPAddress.Parse(ViewModel.IFacialAddress));
                    ViewModel.CapDataModel = new CapturedDataModel(facialClient.Data);
                    facialClient.DataUpdated += FacialClient_DataUpdated;
                    facialClient.ExceptionOccurred += FacialClient_ExceptionOccurred;

                    ScriptParameterConverter parameterConverter = new ScriptParameterConverter(ViewModel.ScriptParameters, ViewModel.ScriptGlobals);
                    vtubeClient = new VTubeClient(new Uri(ViewModel.VTubeAddress), facialClient.Data, Path.Combine(PathUtils.ConfigPath, "config-vtube.json"), parameterConverter);
                    vtubeClient.ExceptionOccurred += VtubeClient_ExceptionOccurred;

                }
                catch (Exception ex)
                {
                    ViewModel.ShowMessageDialog("Failed to Initialize", $"{ex.Message}");
                    Stop();
                    return;
                }

                try
                {
                    ViewModel.BusyMessage = " Connecting to Capturing Device...";
                    facialClient.Connect();
                    ViewModel.BusyMessage = " Starting Captures...";
                    facialClient.Start();
                }
                catch (Exception ex)
                {
                    ViewModel.ShowMessageDialog("Failed to Connect Capturing Device", $"{ex.Message}");
                    Stop();
                    return;
                }

                try
                {
                    ViewModel.BusyMessage = "Connecting to VTube Studio...";
                    vtubeClient.Connect((message) =>
                    {
                        ViewModel.BusyMessage = $"Connecting to VTube Studio...\n{message}";
                    });
                    ViewModel.BusyMessage = "Initializing VTube Studio...";
                    vtubeClient.Init();
                    ViewModel.BusyMessage = "Starting VTube Studio Plugin...";
                    vtubeClient.Start();
                }
                catch (Exception ex)
                {
                    ViewModel.ShowMessageDialog("Failed to Connect VTube Studio", $"{ex.Message}");
                    Stop();
                    return;
                }

                ViewModel.CanStop = true;
                ViewModel.IsBusy = false;
            });
        }

        private void VtubeClient_ExceptionOccurred(VTubeClient sender, Exception exception)
        {
            Task.Factory.StartNew(() =>
            {
                Stop();
                ViewModel.ShowMessageDialog("Failed to Connect VTube Studio", $"{exception.Message}");
            });
        }

        private void FacialClient_ExceptionOccurred(IFacialClient sender, Exception exception)
        {
            Task.Factory.StartNew(() =>
            {
                Stop();
                ViewModel.ShowMessageDialog("Error Occurred with Capturing Device", $"{exception.Message}");
            });

        }

        internal async void Stop()
        {
            ViewModel.IsBusy = true;
            ViewModel.CanStop = false;
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
                ViewModel.CanStart = true;
                ViewModel.IsBusy = false;
            }
            catch (Exception ex)
            {
                ViewModel.ShowMessageDialog("Error Occurred with Capturing Device", $"{ex.Message}");
                ViewModel.CanStop = true;
                ViewModel.IsBusy = false;
            }

        }

        private void FacialClient_DataUpdated(object sender, EventArgs e)
        {
            ViewModel.CapDataModel.NotifyDataChanged(ViewModel.View.DispatcherQueue);
        }

        #endregion
    }
}
