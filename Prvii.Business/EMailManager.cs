using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Prvii.ExceptionHandling;
using System.Web.Configuration;


namespace Prvii.Business
{
    public class EMailManager : IDisposable
    {
        public string SMTPServer { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public int SMTPServerPort { get; set; }
        public bool EnableSsl { get; set; }
        public string Subject { get; set; }
        public string MailTo { get; set; }
        public string MailBody { get; set; }

        public void SendMail()
        {
            this.SendMailBySMTP();
        }

        private void SendMailBySMTP()
        {
            try
            {
                if (string.IsNullOrEmpty(MailTo))
                    throw new Exception("MailTo is empty");

                if (System.Configuration.ConfigurationManager.AppSettings["SMTPADDRESS"] != null)
                    SMTPServer = ConfigurationManager.AppSettings["SMTPADDRESS"].ToString();

                if (System.Configuration.ConfigurationManager.AppSettings["SMTPUSERNAME"] != null)
                    SMTPUserName = ConfigurationManager.AppSettings["SMTPUSERNAME"].ToString();

                if (System.Configuration.ConfigurationManager.AppSettings["SMTPPASSWORD"] != null)
                    SMTPPassword = ConfigurationManager.AppSettings["SMTPPASSWORD"].ToString();

                if (System.Configuration.ConfigurationManager.AppSettings["SMTPPORT"] != null)
                    SMTPServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPORT"]);

                if (System.Configuration.ConfigurationManager.AppSettings["EnableSsl"] != null)
                    EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);

                MailAddress toEmail = new MailAddress(MailTo);
                MailAddress fromEmail = new MailAddress(ConfigurationManager.AppSettings["FromMailID"]);
                MailAddress objFromAndReply = new MailAddress(ConfigurationManager.AppSettings["FromMailID"]);
                System.Net.Mail.MailMessage mailMsg = new System.Net.Mail.MailMessage(fromEmail, toEmail);
               // mailMsg.ReplyTo = objFromAndReply;
                mailMsg.Subject = Subject;
                mailMsg.IsBodyHtml = true;
                mailMsg.Body = MailBody;

                SmtpClient ObjSmtpClient = new SmtpClient(SMTPServer, SMTPServerPort);
                ObjSmtpClient.UseDefaultCredentials = true;
                ObjSmtpClient.Credentials = new System.Net.NetworkCredential(SMTPUserName, SMTPPassword);
                ObjSmtpClient.EnableSsl = EnableSsl;
                ObjSmtpClient.Send(mailMsg);


            }
            catch(Exception ex)
            {
               // throw new Exception("Email Error msg" + ex.Message);
                this.LogMessage("Email Error msg : " + ex.Message + " - " + ex.StackTrace);
            }
           

           
        }

        private void LogMessage(string message)
        {
            string logfilePath = WebConfigurationManager.AppSettings["LogFilePathweb"].ToString();
            ExceptionHandler.LogMessage(message, true, 10240, logfilePath);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

    }
}
