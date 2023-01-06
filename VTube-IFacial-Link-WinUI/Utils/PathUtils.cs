using System;
using System.IO;

namespace VTube_IFacial_Link.Utils
{
    internal class PathUtils
    {
        public static string ConfigPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            }
        }
    }
}
