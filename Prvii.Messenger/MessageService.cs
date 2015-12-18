using Prvii.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Prvii.Business;
using Prvii.Entities.Enumerations;

namespace Prvii.Messenger
{
    public partial class MessageService : ServiceBase
    {
        private System.Timers.Timer _serviceTimer;
        private System.Timers.Timer _smsServiceTimer;
        private System.Timers.Timer _smsServiceRecon;
        private bool _serviceCheckInProgress;

        public MessageService()
        {
            InitializeComponent();
            this._serviceCheckInProgress = false;
            _serviceTimer = new System.Timers.Timer();
            _serviceTimer.Elapsed += new System.Timers.ElapsedEventHandler(serviceTimer_Elapsed);

            //Added by Padmaja
            _smsServiceTimer = new System.Timers.Timer();
            _smsServiceTimer.Elapsed += new System.Timers.ElapsedEventHandler(_smsServiceTimer_Elapsed);

            _smsServiceRecon = new System.Timers.Timer();
            _smsServiceRecon.Elapsed += new System.Timers.ElapsedEventHandler(_smsServiceRecon_Elapsed); 



        }

        private void _smsServiceRecon_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (!this._serviceCheckInProgress)
                {
                    this._serviceCheckInProgress = true;

                    ChannelMessageManager.GetSMSStatus();

                    this._serviceCheckInProgress = false;
                }
            }
            catch (Exception ex)
            {
                this._serviceCheckInProgress = false;
                this.LogMessage(ex.Message + Environment.NewLine + ex.StackTrace + (ex.InnerException == null ? string.Empty : ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace),LogFileType.SMSRecon);
            }
        }

        public void _smsServiceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
               
                if (!this._serviceCheckInProgress)
                {
                    this._serviceCheckInProgress = true;
                    this.LogMessage("SendSMS sending Started", LogFileType.SendSMS);
                    ChannelMessageManager.SendSMS();
                    this.LogMessage("SendSMS sending End", LogFileType.SendSMS);
                    this._serviceCheckInProgress = false;
                }
            }
            catch (Exception ex)
            {
                this._serviceCheckInProgress = false;
                this.LogMessage(ex.Message + Environment.NewLine + ex.StackTrace + (ex.InnerException == null ? string.Empty : ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace),LogFileType.SendSMS);
            }
        }

        void serviceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (!this._serviceCheckInProgress)
                {
                    this._serviceCheckInProgress = true;
                    this.LogMessage("Email sending Started");
                    ChannelMessageManager.SendMessage();
                    this.LogMessage("Email sent end");
                    this._serviceCheckInProgress = false;
                }
            }
            catch (Exception ex)
            {
                this._serviceCheckInProgress = false;
                this.LogMessage(ex.Message + Environment.NewLine + ex.StackTrace + (ex.InnerException == null ? string.Empty : ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace),LogFileType.EMail);
            }
        }

        protected override void OnStart(string[] args)
        {
            //set interval
            int interval = 60000;
            //Retrieve interval values from config file
            interval = Convert.ToInt32(ConfigurationManager.AppSettings["RunFrequency"].Trim());
            //enable timer
            this._serviceTimer.Enabled = true;
            //set timer interval
            this._serviceTimer.Interval = interval;
            //start timer
            this._serviceTimer.Start();
            //write to log
            this.LogMessage("Messenger Started.",LogFileType.EMail);

           
            //Retrieve interval values from config file
            int smsinterval = Convert.ToInt32(ConfigurationManager.AppSettings["SMSRunFrequency"].Trim());
            //enable timer
            this._smsServiceTimer.Enabled = true;
            //set timer interval
            this._smsServiceTimer.Interval = smsinterval;
            //start timer
            this._smsServiceTimer.Start();
            //write to log
            this.LogMessage("SMS Service Started.",LogFileType.SendSMS);



            //Retrieve interval values from config file
            int smsreconinterval = Convert.ToInt32(ConfigurationManager.AppSettings["SMSReconFrequency"].Trim());
            //enable timer
            this._smsServiceRecon.Enabled = true;
            //set timer interval
            this._smsServiceRecon.Interval = smsreconinterval;
            //start timer
            this._smsServiceRecon.Start();
            //write to log
            this.LogMessage("SMS Recon Service Started.", LogFileType.SMSRecon);

        }

        protected override void OnStop()
        {
            this._serviceTimer.Stop();
            this.LogMessage("Messenger Stopped.",LogFileType.EMail);
            this.LogMessage("SMS Service Stopped.", LogFileType.SendSMS);
            this.LogMessage("SMS Recon Service  Stopped.", LogFileType.SMSRecon);

        }

        private void LogMessage(string message, LogFileType logFileType)
        {
         var loggingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["LoggingEnabled"].Trim());
            var logFilePath = "";
            switch (logFileType)
            {
                case LogFileType.EMail:
                    logFilePath = ConfigurationManager.AppSettings["EmailLogFilePath"].Trim();
                    break;
                case LogFileType.SendSMS:
                    logFilePath = ConfigurationManager.AppSettings["SMSSendLog"].Trim();
                    break;
                case LogFileType.SMSRecon:
                    logFilePath = ConfigurationManager.AppSettings["SMSReconLog"].Trim();
                    break;
               
            }
            
            var maxLogFileSize = Convert.ToInt64(ConfigurationManager.AppSettings["MaxLogFileSize"].Trim());

           ExceptionHandler.LogMessage(message, loggingEnabled, maxLogFileSize, logFilePath);
        }

        private void LogMessage(string message)
        {
            var loggingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["LoggingEnabled"].Trim());
            var logFilePath = ConfigurationManager.AppSettings["EmailLogFilePath"].Trim();
            var maxLogFileSize = Convert.ToInt64(ConfigurationManager.AppSettings["MaxLogFileSize"].Trim());

            ExceptionHandler.LogMessage(message, loggingEnabled, maxLogFileSize, logFilePath);
        }
    }
}
