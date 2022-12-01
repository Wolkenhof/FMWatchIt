using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using static Setup.MainWindow;

namespace Setup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length == 1)
            {
                MainWindow wnd = new MainWindow();
                wnd.ShowDialog();
                Environment.Exit(0);
            }
            else
            {
                if (args.Contains("--update"))
                {
                    //// Update the service
                    // Stop the service
                    try
                    {
                        ExtendedServiceController xServiceController =
                            new ExtendedServiceController("FMWatchIt Service");
                        xServiceController.Stop();
                    }
                    catch
                    {
                        // ignored
                    }

                    // Fetch FMFuck URL from API
                    const string fmUpdateUrl = "https://dl.exploitox.de/fmfuckit/latest.json";
                    string fmJson;

                    using (var wc = new System.Net.WebClient())
                        fmJson = wc.DownloadString(fmUpdateUrl);

                    // Parsing data
                    var fmJsonInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<FMFuckInfo>(fmJson);

                    // Move old FMFuck to FMFuck.bkg
                    var InstDir = Path.Combine("C:\\", "Program Files", "Wolkenhof GmbH", "FMWatchIt");
                    if (File.Exists(Path.Combine(InstDir, "FMWatchIt.exe")))
                    {
                        if (File.Exists(Path.Combine(InstDir, "FMWatchIt.bkg")))
                            File.Delete("FMFuck.bkg");

                        File.Move(Path.Combine(InstDir, "FMWatchIt.exe"), Path.Combine(InstDir, "FMWatchIt.bkg"));
                    }

                    // Download Service Watcher from JSON URL
                    var fmFileName = System.IO.Path.GetTempFileName();
                    var fmUrl = fmJsonInfo?.url;
                    var fmMd5 = fmJsonInfo?.md5;
                    const string fmTargetFileName = $"FMWatchIt.exe";
                    using (var wc = new System.Net.WebClient())
                        if (fmUrl != null)
                            wc.DownloadFile(fmUrl, fmFileName);
                        else return; // Failed to download

                    // Check MD5
                    var fmMd5Local = CalculateMD5(fmFileName);
                    if (fmMd5Local != fmMd5)
                    {
                        // Restore old file
                        try
                        {
                            File.Delete(Path.Combine(InstDir, "FMWatchIt.exe"));
                            File.Move(Path.Combine(InstDir, "FMWatchIt.bkg"), Path.Combine(InstDir, "FMWatchIt.exe"));
                        }
                        catch
                        {
                            // the end - no file to restore
                        }
                        return;
                    }

                    // Install
                    Directory.CreateDirectory(InstDir);
                    if (File.Exists(Path.Combine(InstDir, fmTargetFileName)))
                        File.Delete(Path.Combine(InstDir, fmTargetFileName));
                    File.Move(fmFileName, Path.Combine(InstDir, fmTargetFileName));

                    // Service installation
                    using (Process p = new Process())
                    {
                        p.StartInfo.FileName = "sc.exe";
                        p.StartInfo.Arguments = "create \"FMWatchIt Service\" binPath=\"" + Path.Combine(InstDir, fmTargetFileName) + "\" start=auto";
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = true;
                        p.Start();
                        p.WaitForExit();
                    }

                    // Update check interval
                    using (Microsoft.Win32.TaskScheduler.TaskService ts = new Microsoft.Win32.TaskScheduler.TaskService())
                    {
                        // Create a new task definition and assign properties
                        Microsoft.Win32.TaskScheduler.TaskDefinition td = ts.NewTask();
                        td.RegistrationInfo.Description = "Checking for updates for FMWatchIt.";

                        // Create a trigger that will fire the task at every login
                        td.Triggers.Add(new Microsoft.Win32.TaskScheduler.LogonTrigger());
                        td.Triggers.Add(new DailyTrigger { DaysInterval = 5 }); // Every 5 days

                        // Create an action that will launch Notepad whenever the trigger fires
                        td.Actions.Add(new Microsoft.Win32.TaskScheduler.ExecAction(Path.Combine(InstDir, fmTargetFileName), "--update", null));

                        // Set the run level to the highest privilege
                        td.Principal.RunLevel = Microsoft.Win32.TaskScheduler.TaskRunLevel.Highest;

                        // These settings will ensure it runs even if on battery power
                        td.Settings.DisallowStartIfOnBatteries = false;
                        td.Settings.StopIfGoingOnBatteries = false;

                        // Register the task in the root folder
                        ts.RootFolder.RegisterTaskDefinition(@"Wolkenhof GmbH\FMWatchIt Updater", td);
                    }

                    // Download configuration
                    try
                    {
                        string configUrl = "https://dl.exploitox.de/fmfuckit/config.xml";
                        string configFullPath = Path.Combine(InstDir, "FMWatchIt.dll.config");
                        using (var wc = new System.Net.WebClient())
                            wc.DownloadFile(configUrl, configFullPath);
                    }
                    catch
                    {
                        // ignore it 
                    }
                }
            }
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
    }
}
