using IFacial;
using System.Net;
using System.Text.Json;
using VTube;
using VTube_IFacial_Link.DataModel;

namespace VTube_IFacial_Link;

public partial class MainPage : ContentPage
{
    readonly string configPath;

    public class ConfigStore
    {
        public string IFacialAddress { get; set; }
        public string VTubeAddress { get; set; }
        public bool StartOnLaunch { get; set; }
    }

    public MainPage()
	{
        configPath = Path.Combine(FileSystem.AppDataDirectory, "config-ui.json");
        this.Loaded += MainPage_Loaded;
        InitializeComponent();
    }

    private void OnBrowseAppDataClicked(object sender, EventArgs e)
    {
        Launcher.Default.OpenAsync(new Uri($"file://{FileSystem.AppDataDirectory}")).Wait();
    }

    private void Window_Destroying(object sender, EventArgs e)
    {
        Stop();
    }

    private void MainPage_Loaded(object sender, EventArgs e)
    {
        Window.Destroying += Window_Destroying;
        Directory.CreateDirectory(FileSystem.AppDataDirectory);
        LoadConfig();
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
        catch (Exception ex)
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
        using (FileStream configStream = File.OpenWrite(configPath))
        {
            JsonSerializer.Serialize<ConfigStore>(configStream, config);
        }
        System.Diagnostics.Debug.WriteLine($"Config saved. ({configPath})");
    }


    public static readonly BindableProperty CapDataModelProperty = BindableProperty.Create(nameof(CapDataModel), typeof(CapturedDataModel), typeof(MainPage), null);
    public CapturedDataModel CapDataModel
    {
        get => (CapturedDataModel)GetValue(CapDataModelProperty);
        set => SetValue(CapDataModelProperty, value);
    }

    public static readonly BindableProperty IFacialAddressProperty = BindableProperty.Create(nameof(IFacialAddress), typeof(string), typeof(MainPage), string.Empty);
    public string IFacialAddress
    {
        get => (string)GetValue(IFacialAddressProperty);
        set => SetValue(IFacialAddressProperty, value);
    }

    public static readonly BindableProperty VTubeAddressProperty = BindableProperty.Create(nameof(VTubeAddress), typeof(string), typeof(MainPage), "ws://127.0.0.1:8001");
    public string VTubeAddress
    {
        get => (string)GetValue(VTubeAddressProperty);
        set => SetValue(VTubeAddressProperty, value);
    }

    public static readonly BindableProperty CanStartProperty = BindableProperty.Create(nameof(CanStart), typeof(bool), typeof(MainPage), true);
    public bool CanStart
    {
        get => (bool)GetValue(CanStartProperty);
        set => SetValue(CanStartProperty, value);
    }

    public static readonly BindableProperty CanStopProperty = BindableProperty.Create(nameof(CanStop), typeof(bool), typeof(MainPage), false);
    public bool CanStop
    {
        get => (bool)GetValue(CanStopProperty);
        set => SetValue(CanStopProperty, value);
    }

    public static readonly BindableProperty StartOnLaunchProperty = BindableProperty.Create(nameof(StartOnLaunch), typeof(bool), typeof(MainPage), false);
    public bool StartOnLaunch
    {
        get => (bool)GetValue(StartOnLaunchProperty);
        set => SetValue(StartOnLaunchProperty, value);
    }

    public static new readonly BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(MainPage), false, propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
    {
        MainPage mainPage = (MainPage)bindable;
        mainPage.AbortAnimation("BusyMessageFadeAnimation");
        uint duration = 250;
        if ((bool)newValue)
        {
            // show Busy Indicator if the process is longer than one second
            uint startDelay = 1000;
            Animation animation = new Animation();
            Animation ch_animation = new Animation(v => mainPage.BusyMessageOpacity = v, mainPage.BusyMessageOpacity, 1);
            animation.Add((double)startDelay / (startDelay + duration), 1, ch_animation);
            animation.Commit(mainPage, "BusyMessageFadeAnimation", 16, (startDelay + duration), Easing.CubicOut, (v, c) => { if (!c) mainPage.BusyMessageOpacity = 1; }, () => false);
        }
        else
        {
            // hide Busy Indicator immediately
            Animation animation = new Animation(v => mainPage.BusyMessageOpacity = v, mainPage.BusyMessageOpacity, 0);
            animation.Commit(mainPage, "BusyMessageFadeAnimation", 16, duration, Easing.CubicIn, (v, c) => { if (!c) mainPage.BusyMessageOpacity = 0; }, () => false);
        }
    });

    public new bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set
        {
            base.IsBusy = value;
            SetValue(IsBusyProperty, value);
        }
    }

    public static readonly BindableProperty BusyMessageOpacityProperty = BindableProperty.Create(nameof(BusyMessageOpacity), typeof(double), typeof(MainPage), 0.0);
    public double BusyMessageOpacity
    {
        get => (double)GetValue(BusyMessageOpacityProperty);
        set => SetValue(BusyMessageOpacityProperty, value);
    }

    public static readonly BindableProperty BusyMessageProperty = BindableProperty.Create(nameof(BusyMessage), typeof(string), typeof(MainPage), string.Empty);
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
        Task.Factory.StartNew(() =>
        {
            try
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BusyMessage = "Initializing...";
                }).Wait();
                SaveConfig();

                facialClient = new IFacialClient(IPAddress.Parse(IFacialAddress));
                CapDataModel = new CapturedDataModel(facialClient.Data);
                facialClient.DataUpdated += FacialClient_DataUpdated;
                facialClient.ExceptionOccurred += FacialClient_ExceptionOccurred;

                vtubeClient = new VTubeClient(new Uri(VTubeAddress), facialClient.Data);
                vtubeClient.ExceptionOccurred += VtubeClient_ExceptionOccurred;

            }
            catch (Exception ex)
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BusyMessage = string.Empty;
                    DisplayAlert("Failed to Initialize", $"{ex.Message}", "OK");
                    Stop();
                    CanStart = true;
                    IsBusy = false;
                }).Wait();
                
                return;
            }

            try
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BusyMessage = " Connecting to Capturing Device...";
                }).Wait();
                facialClient.Connect();
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BusyMessage = " Starting Captures...";
                }).Wait();
                facialClient.Start();
            }
            catch (Exception ex)
            {
                
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BusyMessage = string.Empty;
                    DisplayAlert("Failed to Connect Capturing Device", $"{ex.Message}", "OK");
                    Stop();
                    CanStart = true;
                    IsBusy = false;
                }).Wait();
                return;
            }

            try
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BusyMessage = "Connecting to VTube Studio...";
                }).Wait();
                vtubeClient.Connect((string message) =>
                {
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        BusyMessage = $"Connecting to VTube Studio...\n{message}";
                    }).Wait();
                });
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BusyMessage = "Initializing VTube Studio...";
                }).Wait();
                vtubeClient.Init();
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BusyMessage = "Starting VTube Studio Plugin...";
                }).Wait();
                vtubeClient.Start();
            }
            catch (Exception ex)
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BusyMessage = string.Empty;
                    DisplayAlert("Failed to Connect VTube Studio", $"{ex.Message}", "OK");
                    Stop();
                    CanStart = true;
                    IsBusy = false;
                }).Wait();
                return;
            }

            MainThread.InvokeOnMainThreadAsync(() =>
            {
                BusyMessage = string.Empty;
                CanStop = true;
                IsBusy = false;
            }).Wait();
        });
    }

    private void VtubeClient_ExceptionOccurred(VTubeClient sender, Exception exception)
    {
        Task.Factory.StartNew(() =>
        {
            Stop();
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                DisplayAlert("Error Occurred with VTube Studio", $"{exception.Message}", "OK");
            }).Wait();
        });
        
    }

    private void FacialClient_ExceptionOccurred(IFacialClient sender, Exception exception)
    {
        Task.Factory.StartNew(() =>
        {
            Stop();
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                DisplayAlert("Error Occurred with Capturing Device", $"{exception.Message}", "OK");
            }).Wait();
        });
        
    }

    private void OnStartClicked(object sender, EventArgs e)
	{
        Start();
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
                vtubeClient.Dispose();
                vtubeClient = null;
            }
            CanStart = true;
            IsBusy = false;
        }
        catch (Exception ex)
        {
            DisplayAlert("Failed to Stop", $"{ex.Message}", "OK");
            CanStop = true;
            IsBusy = false;
        }
        
    }

    private void OnStopClicked(object sender, EventArgs e)
    {
        Stop();
    }

    private void FacialClient_DataUpdated(object sender, EventArgs e)
    {
        CapDataModel.NotifyDataChanged();
    }

}


