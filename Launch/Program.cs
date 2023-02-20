using System.Diagnostics;
using System.Reflection;

namespace Launch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "bin");

            Process process = new()
            {
                StartInfo = new ProcessStartInfo("VTube-IFacial-Link.exe")
            };
            process.Start();
        }
    }
}