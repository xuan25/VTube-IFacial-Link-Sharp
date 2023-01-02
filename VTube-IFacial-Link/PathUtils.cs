using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTube_IFacial_Link
{
    internal class PathUtils
    {
        public static string ConfigPath
        {
            get
            {
                return FileSystem.AppDataDirectory;
            }
        }
    }
}
