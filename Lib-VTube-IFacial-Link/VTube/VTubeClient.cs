using System.Net.WebSockets;
using System.Text.Json;
using VTube.DataModel;
using IFacial;
using System.Collections.Specialized;
using VTube.Interfaces;

namespace VTube
{
    public class VTubeClient
    {

        readonly string configPath;

        public class ConfigStore
        {
            public string AuthenticationKey { get; set; }
        }

        ConfigStore Config;
        public Uri ApiAddress { get; private set; }

        public IParameterConverter ParamConverter { get; private set; }

        public VTubeClient(Uri apiAddress, CapturedData captured, string configPath, IParameterConverter parameterConverter) {
            ApiAddress = apiAddress;
            Captured = captured;

            this.configPath = configPath;
            if (!LoadConfig())
            {
                Config = new ConfigStore();
            }

            ParamConverter = parameterConverter;
            ParamConverter.Parameters.CollectionChanged += Parameters_CollectionChanged;
        }

        private void Parameters_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if(IsConnected)
            {
                if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
                {
                    foreach (IParameter param in e.NewItems)
                    {
                        Api.RequestParameterCreation(clientWebSocket, param.Name, "", 0, 1, 0);
                        CustomParameters.Add(param.Name);
                    }
                }
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
                {
                    foreach (IParameter param in e.OldItems)
                    {
                        var a = !DefaultParameters.Contains(param.Name) && CustomParameters.Contains(param.Name);
                        if (!DefaultParameters.Contains(param.Name) && CustomParameters.Contains(param.Name))
                        {
                            Api.RequestParameterDeletion(clientWebSocket, param.Name);
                            CustomParameters.Remove(param.Name);
                        }
                    }
                }
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    Init();
                }
            }
        }

        ClientWebSocket clientWebSocket;
        public bool IsConnected = false;

        public void Connect(Action<string> messageHandler = null)
        {
            clientWebSocket = new ClientWebSocket();
            clientWebSocket.ConnectAsync(ApiAddress, CancellationToken.None).Wait();
            if (Config.AuthenticationKey != null)
            {
                try
                {
                    Authentication(Config.AuthenticationKey, messageHandler);
                }
                catch (Exception)
                {
                    Authentication(messageHandler);
                }
            } 
            else
            {
                Authentication(messageHandler);
            }
            IsConnected = true;
        }

        public CapturedData Captured { get; set; }

        private HashSet<string> DefaultParameters { get; set; }
        private HashSet<string> CustomParameters { get; set; }

        public bool Init()
        {
            System.Diagnostics.Debug.WriteLine($"Requesting Registered Input Parameter List...");
            InputParameterListResponse.DataSection data = Api.RequestInputParameterList(clientWebSocket);

            HashSet<string> requiredParameters = new();
            foreach (IParameter parameter in ParamConverter.Parameters)
            {
                requiredParameters.Add(parameter.Name);
            }

            HashSet<string> defaultParameters = new();
            foreach (InputParameterListResponse.DataSection.Parameter parameter in data.DefaultParameters)
            {
                defaultParameters.Add(parameter.Name);
            }
            DefaultParameters = defaultParameters;

            HashSet<string> customParameters = new();
            foreach (InputParameterListResponse.DataSection.Parameter parameter in data.CustomParameters)
            {
                if (!requiredParameters.Contains(parameter.Name))
                {
                    Api.RequestParameterDeletion(clientWebSocket, parameter.Name);
                } 
                else
                {
                    customParameters.Add(parameter.Name);
                }
            }

            System.Diagnostics.Debug.WriteLine($"Registering Missing Parameters...");
            foreach(IParameter parameter in ParamConverter.Parameters)
            {
                if (!customParameters.Contains(parameter.Name) && !defaultParameters.Contains(parameter.Name))
                {
                    Api.RequestParameterCreation(clientWebSocket, parameter.Name, "", 0, 1, 0);
                    customParameters.Add(parameter.Name);
                }
            }
            CustomParameters = customParameters;

            System.Diagnostics.Debug.WriteLine("Successfully Initialized");

            return true;
        }

        public CancellationTokenSource CTS { get; private set; }

        public Thread PorcessingThread { get; private set; }

        public void Start()
        {
            CTS = new CancellationTokenSource();
            PorcessingThread = new(new ThreadStart(ConnectionLoop)) { IsBackground = true };
            PorcessingThread.Start();
        }

        public void Stop()
        {
            CTS?.Cancel();
            if (PorcessingThread != null && PorcessingThread.ThreadState != ThreadState.Stopped)
            {
                PorcessingThread.Join();
            }
        }

        public delegate void ExceptionHandler(VTubeClient sender, Exception exception);

        public event ExceptionHandler ExceptionOccurred;

        private void ConnectionLoop()
        {
            long frameCount = 0;
            long statInterval = 60;
            long lastStatsTick = 0;
            System.Diagnostics.Stopwatch stopwatch = new();
            stopwatch.Start();

            try
            {
                while (!CTS.IsCancellationRequested)
                {
                    List<InjectParameterDataRequest.DataSection.ParameterValue> parameterValues = ParamConverter.Convert(Captured);
                    if (parameterValues.Count > 0)
                    {
                        Api.RequestInjectParameterData(clientWebSocket, true, "set", parameterValues);
                    }

                    frameCount++;
                    if (frameCount % statInterval == 0)
                    {
                        long currentTick = stopwatch.ElapsedTicks;
                        long statsDeltaTicks = currentTick - lastStatsTick;
                        double fps = 1 / (((double)statsDeltaTicks / TimeSpan.TicksPerMillisecond / 1000) / statInterval);
                        System.Diagnostics.Debug.WriteLine($"FPS: {fps:0.##}");
                        lastStatsTick = currentTick;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOccurred?.Invoke(this, ex);
            }
        }

        public void Dispose()
        {
            ParamConverter.Parameters.CollectionChanged -= Parameters_CollectionChanged;
            if (clientWebSocket != null && clientWebSocket.State == WebSocketState.Open)
            {
                clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).Wait();
                clientWebSocket.Dispose();
            }
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

                if (config != null)
                {
                    Config = config;
                    return true;
                }
                else
                {
                    return false;
                }
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
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
            using (FileStream configStream = File.OpenWrite(configPath))
            {
                JsonSerializer.Serialize<ConfigStore>(configStream, Config);
            }
            System.Diagnostics.Debug.WriteLine($"Config saved. ({configPath})");
        }

        private void Authentication(Action<string> messageHandler = null)
        {
            System.Diagnostics.Debug.WriteLine($"Requesting Authentication Token...");
            messageHandler?.Invoke("Requesting Authentication Token...\n\nPlease Allow the plugin in VTube Studio");
            string token = Api.RequestAuthenticationToken(clientWebSocket);
            System.Diagnostics.Debug.WriteLine($"Authentication token requested. ({token})");
            Config.AuthenticationKey = token;
            SaveConfig();
            Authentication(token);
        }

        private void Authentication(string authenticationKey, Action<string> messageHandler = null)
        {
            System.Diagnostics.Debug.WriteLine($"Requesting Authentication...");
            messageHandler?.Invoke("Requesting Authentication...");
            Api.RequestAuthentication(clientWebSocket, authenticationKey);
        }

    }
}
