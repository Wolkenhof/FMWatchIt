﻿/*
 * FMFuckIt - A fucking service watcher for FMAudit (ECI DCA)
 * ---
 * Copyright (c) 2018 - 2022 valnoxy. All rights reserved.
 */

using System.ServiceProcess;

namespace FMFuckIt
{
    internal class Program
    {
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
            if (e.Status == ServiceControllerStatus.Stopped)
            {
                Console.WriteLine("Service stopped, restart in 5 sec ...");

                // wait 30 sec and restart service if stopped
                Thread.Sleep(Convert.ToInt32(5000));
                if (e.Status == ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("Restaring service ...");
                    ExtendedServiceController xServiceController = new ExtendedServiceController(Configuration.ServiceName);
                    xServiceController.Start();
                    xServiceController.WaitForStatus(ServiceControllerStatus.Running);
                    Console.WriteLine("Service restarted.");
                    MailReport.SendFullReport("C:\\ProgramData\\ECI DCA\\dca.config", "C:\\ProgramData\\ECI DCA\\logs\\dca.log");
                }
            }
        }
    }
}