using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.Utils
{
    public static class ConfigFile
    {
        public static String AutosaveDirectory {get;private set;}


        public static void loadConfig()
        {
            AutosaveDirectory = ConfigurationManager.AppSettings.Get("AutosaveDirectory");

            if (!Directory.Exists(AutosaveDirectory))
            {
                Directory.CreateDirectory(AutosaveDirectory);
            }
        }
    }
}
