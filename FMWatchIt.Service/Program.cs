using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceProcess;
using FMWatchIt.Service;

namespace FMWatchIt.Service
{
    public class Update
    {
        public string version { get; set; }
        public string url { get; set; }
        public string md5 { get; set; }
    }

    public class Program
    {
        private static readonly string LocalUpdatePath = "https://dl.exploitox.de/fmfuckit/latest.json";

        public static void Main(string[] args)
        {
            // Get all args
            var arguments = Environment.GetCommandLineArgs();
            
            if (arguments.Contains("--update"))
            {
                // Update the service
                UpdateService();
            }
            else
            {
                // Run the service
                var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                    .UseWindowsService(options =>
                    {
                        options.ServiceName = "FMWatchIt Service";
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddHostedService<Worker>();
                    })
                    .Build();

                host.Run();
            }
        }

        public static void UpdateService()
        {
            // Fetch URL from internal updater api
            string jsonContent;
            using (var wc = new System.Net.WebClient())
                jsonContent = wc.DownloadString(LocalUpdatePath);

            // Parse JSON
            var updateInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Update>(jsonContent);

            // Check if version is newer
            var newVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (updateInfo.version != newVersion)
            {
                // Download new version
                string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".exe"; 
                using (var wc = new System.Net.WebClient())
                    wc.DownloadFile("https://vnxy.one/fmaudit", fileName);

                // Start new version
                Process.Start(fileName, "--update");
            }
        }
    }

}


