using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWatchIt.Service
{
    public class Configuration
    {
        public static readonly string ServiceInstallationPath = @"C:\Program Files (x86)\ECI DCA\";
        public static readonly string ServiceExecutable = "DCA.Edge.Console.exe";
        public static readonly string ServiceName = "DCAPulse";
        public static readonly string ServiceArguments = "--config C:\\ProgramData\\ECI DCA\\dca.config";
    }
}
