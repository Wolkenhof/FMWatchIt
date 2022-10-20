using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace FMFuckIt
{
    internal class MailReport
    {
        public static void SendFullReport(string LatestLogFilePath, string reason)
        {
            Console.WriteLine("Creating report ...");

            try
            {
                var http = new System.Net.WebClient();
                var ip = http.DownloadString("https://ifconfig.me/ip");

                var mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress("FMFuckIt",
                    System.Configuration.ConfigurationManager.AppSettings["From"]));
                mailMessage.To.Add(new MailboxAddress("User",
                    System.Configuration.ConfigurationManager.AppSettings["To"]));
                mailMessage.To.Add(new MailboxAddress("User",
                    System.Configuration.ConfigurationManager.AppSettings["To2"]));
                mailMessage.Subject = $"FMFuckIt - Report ({ip})";
                var builder = new BodyBuilder();

                // Set the plain-text version of the message text
                builder.TextBody = $"A report was created. Reason: {reason}.\nThe service was restarted on {ip}.";
                builder.Attachments.Add(LatestLogFilePath);

                // Now we just need to set the message body and we're done
                mailMessage.Body = builder.ToMessageBody();

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Connect(System.Configuration.ConfigurationManager.AppSettings["Host"], 465, true);
                    smtpClient.Authenticate(System.Configuration.ConfigurationManager.AppSettings["Username"],
                        System.Configuration.ConfigurationManager.AppSettings["Password"]);
                    smtpClient.Send(mailMessage);
                    smtpClient.Disconnect(true);
                }

                Console.WriteLine("Email sent!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
