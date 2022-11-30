using System;
using System.ServiceProcess;
namespace FMWatchIt.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.WriteLine("FMWatchIt - A service watcher for FMAudit (ECI DCA)");
            Console.WriteLine("Copyright (c) 2018 - 2022 valnoxy. All rights reserved.");
            Console.WriteLine("Report to: " + System.Configuration.ConfigurationManager.AppSettings["To"]);
            Console.WriteLine("Report to: " + System.Configuration.ConfigurationManager.AppSettings["To2"]);
            Console.WriteLine("Report to: " + System.Configuration.ConfigurationManager.AppSettings["To3"]);
            Console.WriteLine();
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            ExtendedServiceController xServiceController = new ExtendedServiceController(Configuration.ServiceName);
            xServiceController.StatusChanged += xServiceController_StatusChanged;
            await Task.Delay(1000, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
    }

    // This event handler will catch service status changes externally as well
    private void xServiceController_StatusChanged(object sender, ServiceStatusEventArgs e)
    {
        _logger.LogInformation("Status Changed: " + e.Status);

        if (e.Status == ServiceControllerStatus.Stopped)
        {
            // wait 10 sec and restart service if stopped
            _logger.LogInformation("Service stopped, restart in 10 sec ...");
            Thread.Sleep(Convert.ToInt32(10000));

            if (e.Status != ServiceControllerStatus.Stopped) return;
            _logger.LogInformation("Try to restart service ...");

            try
            {
                ExtendedServiceController xServiceController = new ExtendedServiceController(Configuration.ServiceName);
                xServiceController.Start();
                xServiceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                _logger.LogInformation("Service restarted!");
                MailReport.SendFullReport("C:\\ProgramData\\ECI DCA\\logs\\dca.log", "The FMAudit service has been restarted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                MailReport.SendFullReport("C:\\ProgramData\\ECI DCA\\logs\\dca.log", "The FMAudit service cannot be restarted: " + ex.Message);
            }
        }
    }
}
