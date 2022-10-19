using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            ActionLabel.Content = "d";
            string jsoncontent;
            using (var wc = new System.Net.WebClient())
                jsoncontent = wc.DownloadString(UpdaterAPI);

            // Parse JSON
            var updateInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateInfo>(jsoncontent);

            // Download file
            string filename = System.IO.Path.GetTempFileName();
            string url = "https://updates.printfleetcdn.com" + updateInfo.ga.Windows.url;
            string version = updateInfo.ga.Windows.version;
            string md5 = updateInfo.ga.Windows.md5;
            string TargetFileName = $"ECI DCA {version} [{AuthToken}].exe";
            using (var wc = new System.Net.WebClient())
                wc.DownloadFile(url, filename);
            
            // Check MD5
            string md5local = CalculateMD5(filename);
            if (md5local != md5)
            {
                MessageBox.Show("MD5 Checksum failed! Please try again.");
                return;
            }

            // Install
            string TempPath = System.IO.Path.GetTempPath();
            File.Move(filename, Path.Combine(TempPath, TargetFileName));
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

        private void AuthTokenTb_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            InstallBtn.IsEnabled = AuthTokenTb.Text != "";
            AuthToken = AuthTokenTb.Text;
        }
    }
}
