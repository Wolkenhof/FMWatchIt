using System.Net;
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
        private static readonly string LocalUpdatePath = "https://updates.printfleetcdn.com/dca-pulse/latest.json";

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
        }
    }

}


