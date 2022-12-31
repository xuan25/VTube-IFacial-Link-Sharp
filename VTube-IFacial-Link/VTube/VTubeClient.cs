using System.Net.WebSockets;
using System.Text.Json;
using VTube.DataModel;
using IFacial;

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

        public VTubeClient(Uri apiAddress, CapturedData captured) {
            ApiAddress = apiAddress;
            Captured = captured;

            configPath = Path.Combine(FileSystem.AppDataDirectory, "config-vtube.json");
            if (!LoadConfig())
            {
                Config = new ConfigStore();
            }
        }

        ClientWebSocket clientWebSocket;

        public bool Connect()
        {
            clientWebSocket = new ClientWebSocket();
            clientWebSocket.ConnectAsync(ApiAddress, CancellationToken.None).Wait();
            bool success;
            if (Config.AuthenticationKey != null)
            {
                success = Authentication(Config.AuthenticationKey);
                if (!success)
                {
                    success = Authentication();
                }
            } 
            else
            {
                success = Authentication();
            }

            return success;
        }

        public CapturedData Captured { get; set; }

        public bool Init()
        {
            System.Diagnostics.Debug.WriteLine($"Requesting Registered Input Parameter List...");
            InputParameterListResponse.DataSection data = Api.RequestInputParameterList(clientWebSocket);

            HashSet<string> registeredParameters = new();
            foreach (InputParameterListResponse.DataSection.Parameter parameter in data.DefaultParameters)
            {
                registeredParameters.Add(parameter.Name);
            }
            foreach(InputParameterListResponse.DataSection.Parameter parameter in data.CustomParameters)
            {
                registeredParameters.Add(parameter.Name);
            }

            System.Diagnostics.Debug.WriteLine($"Registering Missing Parameters...");
            foreach (string paramName in Enum.GetNames(typeof(Params)))
            {
                if (!registeredParameters.Contains(paramName))
                {
                    Api.RequestParameterCreation(clientWebSocket, paramName, "", 0, 1, 0);
                }
            }

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
            try
            {
                while (!CTS.IsCancellationRequested)
                {
                    List<InjectParameterDataRequest.DataSection.ParameterValue> parameterValues = ParameterConverter.Convert(Captured);
                    Api.RequestInjectParameterData(clientWebSocket, true, "set", parameterValues);
                }
            }
            catch (Exception ex)
            {
                ExceptionOccurred?.Invoke(this, ex);
            }
        }

        public void Dispose()
        {
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
            using (FileStream configStream = File.OpenWrite(configPath))
            {
                JsonSerializer.Serialize<ConfigStore>(configStream, Config);
            }
            System.Diagnostics.Debug.WriteLine($"Config saved. ({configPath})");
        }

        private bool Authentication()
        {
            System.Diagnostics.Debug.WriteLine($"Requesting Authentication Token...");
            string token = Api.RequestAuthenticationToken(clientWebSocket);
            System.Diagnostics.Debug.WriteLine($"Authentication token requested. ({token})");
            Config.AuthenticationKey = token;
            SaveConfig();
            return Authentication(token);
        }

        private bool Authentication(string authenticationKey)
        {
            System.Diagnostics.Debug.WriteLine($"Requesting Authentication...");
            return Api.RequestAuthentication(clientWebSocket, authenticationKey);
        }

    }
}
