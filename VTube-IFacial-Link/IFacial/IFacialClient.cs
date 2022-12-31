using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IFacial
{
    internal class IFacialClient : IDisposable
    {
        public IPAddress CaptureDeviceAddress { get; private set; }
        public CapturedData Data { get; private set; }

        public CancellationTokenSource CTS { get; private set; }

        public event EventHandler DataUpdated;

        Thread PorcessingThread;

        IPEndPoint iPEndPoint;
        UdpClient udpClient;


        public IFacialClient(IPAddress iPAddress)
        {
            CaptureDeviceAddress = iPAddress;
            Data = new CapturedData();

            iPEndPoint = new IPEndPoint(CaptureDeviceAddress, 49983);
            udpClient = new UdpClient(49983);
        }

        public void Connect()
        {
            string connectionHint = "iFacialMocap_sahuasouryya9218sauhuiayeta91555dy3719|sendDataVersion=v2";
            byte[] connectionHintData = Encoding.UTF8.GetBytes(connectionHint);
            int numSent = udpClient.SendAsync(connectionHintData, iPEndPoint, CancellationToken.None).Result;
        }

        public void Start()
        {
            CTS = new CancellationTokenSource();
            PorcessingThread = new (new ThreadStart(ConnectionLoop)) { IsBackground = true };
            PorcessingThread.Start();
        }

        public void Stop()
        {
            if (CTS != null)
            {
                CTS.Cancel();
            }
            if (PorcessingThread != null && PorcessingThread.ThreadState != ThreadState.Stopped)
            {
                PorcessingThread.Join();
            }
        }

        public delegate void ExceptionHandler(IFacialClient sender, Exception exception);

        public event ExceptionHandler ExceptionOccurred;

        private void ConnectionLoop()
        {
            try
            {
                while (!CTS.IsCancellationRequested)
                {
                    ValueTask<UdpReceiveResult> udpReceiveResultTask = udpClient.ReceiveAsync(CTS.Token);
                    UdpReceiveResult udpReceiveResult;
                    try
                    {
                        udpReceiveResult = udpReceiveResultTask.Result;
                    }
                    catch (Exception)
                    {
                        if (udpReceiveResultTask.IsCanceled)
                        {
                            return;
                        }
                        continue;
                    }

                    byte[] payloadData = udpReceiveResult.Buffer;
                    string payload = Encoding.UTF8.GetString(payloadData);

                    DataParser dataParser = new DataParser(Data);
                    dataParser.Parse(payload);
                    DataUpdated?.Invoke(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                ExceptionOccurred?.Invoke(this, ex);
            }
            
        }

        public void Dispose()
        {
            udpClient.Close();
        }
    }
}
