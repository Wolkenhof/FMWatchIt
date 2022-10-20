using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace Setup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly BackgroundWorker installBackgroundWorker = new();
        private readonly string UpdaterAPI = "https://updates.printfleetcdn.com/dca-pulse/latest.json";
        private static string AuthToken;
        
        public class UpdateInfo
        {
            public GeneralAvailable ga { get; set; }
        }

        public class GeneralAvailable
        {
            public WindowsInfo Windows { get; set; }
        }
        
        public class WindowsInfo
        {
            public string version { get; set; }
            public string url { get; set; }
            public string md5 { get; set; }
        }

        public class FMFuckInfo
        {
            public string version { get; set; }
            public string url { get; set; }
            public string md5 { get; set; }
        }
        

        public MainWindow()
        {
            InitializeComponent();
            installBackgroundWorker.WorkerReportsProgress = true;
            installBackgroundWorker.WorkerSupportsCancellation = true;
            installBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            installBackgroundWorker.DoWork += new DoWorkEventHandler(InstallAction);
            InstallBtn.IsEnabled = AuthTokenTb.Text != "";
        }
        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) => ProgrBar.Value = e.ProgressPercentage;
        private void InstallBtn_Click(object sender, RoutedEventArgs e) => installBackgroundWorker.RunWorkerAsync();

        private void InstallAction(object sender, DoWorkEventArgs e)
        {
            // Fetch URL from internal updater api
            ReportAction("Fetching data from server ...");
            string jsonContent;
            using (var wc = new System.Net.WebClient())
                jsonContent = wc.DownloadString(UpdaterAPI);

            // Parse JSON
            ReportAction("Parsing data from server ...");
            var updateInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateInfo>(jsonContent);

            // Download ECI DCA from JSON URL
            ReportAction("Download ECI DCA ...");
            string filename = System.IO.Path.GetTempFileName();
            string url = "https://updates.printfleetcdn.com" + updateInfo.ga.Windows.url;
            string version = updateInfo.ga.Windows.version;
            string md5 = updateInfo.ga.Windows.md5;
            string targetFileName = $"ECI DCA {version} [{AuthToken}].exe";
            using (var wc = new System.Net.WebClient())
                wc.DownloadFile(url, filename);

            // Check MD5
            ReportAction("Verifying download ...");
            string md5Local = CalculateMD5(filename);
            if (md5Local != md5)
            {
                ReportAction("MD5 Checksum failed! Please try again.");
                return;
            }

            // Install
            ReportAction("Installing ECI DCA ...");
            string tempPath = System.IO.Path.GetTempPath();
            File.Move(filename, Path.Combine(tempPath, targetFileName));

            goto lol;
            Process p = new Process();
            p.StartInfo.FileName = targetFileName;
            p.StartInfo.Arguments = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART";
            p.Start();
            p.WaitForExit();

            if (p.ExitCode == 0)
            {
                ReportAction("ECI DCA Installation successful!");
            }
            else
            {
                ReportAction("ECI DCA Installation failed!");
                return;
            }
            lol:
            // Fetch FMFuck URL from API
            ReportAction("Fetch data for FMFuckIt ...");
            string fmUpdateUrl = "https://dl.exploitox.de/fmfuckit/latest.json";
            string fmJson;

            using (var wc = new System.Net.WebClient())
                fmJson = wc.DownloadString(fmUpdateUrl);
            
            ReportAction("Parsing data from server ...");
            var fmJsonInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<FMFuckInfo>(fmJson);

            // Download ECI DCA from JSON URL
            ReportAction("Download ECI DCA ...");
            string InstDir = Path.Combine("C:\\", "Program Files", "Wolkenhof GmbH", "FMFuckIt");
            string fmFileName = System.IO.Path.GetTempFileName();
            string fmUrl = fmJsonInfo.url;
            string fmVersion = fmJsonInfo.version;
            string fmMD5 = fmJsonInfo.md5;
            string fmTargetFileName = $"FMFuckIt.exe";
            using (var wc = new System.Net.WebClient())
                wc.DownloadFile(url, fmFileName);

            // Check MD5
            ReportAction("Verifying download ...");
            string fmMD5Local = CalculateMD5(fmFileName);
            if (fmMD5Local != fmMD5)
            {
                ReportAction("MD5 Checksum failed! Please try again.");
                return;
            }

            // Install
            ReportAction("Installing FMFuckIt ...");
            Directory.CreateDirectory(InstDir);
            File.Move(fmFileName, Path.Combine(InstDir, fmTargetFileName));
            using (Microsoft.Win32.TaskScheduler.TaskService ts = new Microsoft.Win32.TaskScheduler.TaskService())
            {
                // Create a new task definition and assign properties
                Microsoft.Win32.TaskScheduler.TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Watcher Service for ECI DCA.";

                // Create a trigger that will fire the task at every login
                td.Triggers.Add(new Microsoft.Win32.TaskScheduler.LogonTrigger());

                // Create an action that will launch Notepad whenever the trigger fires
                td.Actions.Add(new Microsoft.Win32.TaskScheduler.ExecAction(Path.Combine(), null, null));

                // Set the run level to the highest privilege
                td.Principal.RunLevel = Microsoft.Win32.TaskScheduler.TaskRunLevel.Highest;

                // These settings will ensure it runs even if on battery power
                td.Settings.DisallowStartIfOnBatteries = false;
                td.Settings.StopIfGoingOnBatteries = false;

                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition(@"Wolkenhof GmbH\FMFuckIt", td);
            }

            ReportAction("Installing FMFuckIt ...");
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private void ReportAction(string s)
        {
            Dispatcher.Invoke(new Action(() => {
                ActionLabel.Content = s;
            }), DispatcherPriority.ContextIdle);
        }

        private void AuthTokenTb_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            InstallBtn.IsEnabled = AuthTokenTb.Text != "";
            AuthToken = AuthTokenTb.Text;
        }
    }
}
