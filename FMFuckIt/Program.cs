/*
 * FMFuckIt - A fucking service watcher for FMAudit (ECI DCA)
 * ---
 * Copyright (c) 2018 - 2022 valnoxy. All rights reserved.
 */

using System.ServiceProcess;

namespace FMFuckIt
{
    internal class Program
    {
        private static int trys = 0;
        private static int max_trys = 5;

        static void Main(string[] args)
        {
            Console.WriteLine("FMFuckIt - A fucking service watcher for FMAudit (ECI DCA)");
            Console.WriteLine("Copyright (c) 2018 - 2022 valnoxy. All rights reserved.");
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Report to: " + System.Configuration.ConfigurationManager.AppSettings["To"]);

            ExtendedServiceController xServiceController = new ExtendedServiceController(Configuration.ServiceName);
            xServiceController.StatusChanged += xServiceController_StatusChanged;
            Console.Read();
        }

        // This event handler will catch service status changes externally as well
        private static void xServiceController_StatusChanged(object sender, ServiceStatusEventArgs e)
        {
            Console.WriteLine("Status Changed: " + e.Status);

            if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
            {
                trys = 0;
            }

            if (e.Status == ServiceControllerStatus.Stopped)
            {
                if (trys < max_trys)
                {
                    trys++;

                    // wait 30 sec and restart service if stopped
                    Thread.Sleep(Convert.ToInt32(5000));
                    Console.WriteLine("Service stopped, restart in 5 sec ...");

                    if (e.Status != ServiceControllerStatus.Stopped) return;
                    Console.WriteLine("Try to restart service ...");
                    ExtendedServiceController xServiceController = new ExtendedServiceController(Configuration.ServiceName);
                    xServiceController.Start();
                    xServiceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    Console.WriteLine("Service restarted!");
                }
                else
                {
                    Console.WriteLine("Max trys reached! Creating report ...");
                    MailReport.SendFullReport("C:\\ProgramData\\ECI DCA\\logs\\dca.log", "Max trys reached!");
                    trys = 0;
                }
            }
        }
    }
}