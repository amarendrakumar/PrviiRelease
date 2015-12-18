using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Prvii.Business;
using System.Configuration;
using Prvii.ExceptionHandling;
using Prvii.Entities;

namespace Prvii.Messenger
{
   public partial class PaymentRecon : ServiceBase
    {
       bool _serviceCheckInProgress ;
       private System.Timers.Timer _reconServiceTimer;
       private System.Timers.Timer _processStartTimer;


        public PaymentRecon()
        {
            InitializeComponent();
            this._serviceCheckInProgress = false;
            _reconServiceTimer = new System.Timers.Timer();
            _reconServiceTimer.Elapsed += _reconServiceTimer_Elapsed;


            _processStartTimer = new System.Timers.Timer();
            _processStartTimer.Elapsed += _processStartTimer_Elapsed;
            
        }

        void _processStartTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.LogMessage("Process Timer Started");
            _processStartTimer.Stop();
            ReconcilePaypalProfiles();
            _processStartTimer.Enabled = false;
        }

        void _reconServiceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            ReconcilePaypalProfiles();
        }

       
        public void ReconcilePaypalProfiles()
        {
            try
            {
                if (!this._serviceCheckInProgress)
                {
                    this._serviceCheckInProgress = true;

                    PaymentManager payment = new PaymentManager();
                    this.LogMessage("Reconciliation Started");
                    List<TransactionReconDetail> ReconDetailList = payment.ReconcilePaypalTransactions();
                    this.LogMessage("ReconDetailList");
                    this.LogMessage("ReconDetailList : " + ReconDetailList.Count.ToString());


                    if (ReconDetailList.Count > 0)
                    {
                        foreach (TransactionReconDetail inReconDetail in ReconDetailList)
                        {
                            PayPalManager manager = new PayPalManager();
                            manager.GetTransactionDetails(inReconDetail);
                        }

                    }
                    else
                    {
                        this.LogMessage("No Profiles Found for Reconciliation");
                    }


                    this.LogMessage("Reconciliation Done");

                    this._serviceCheckInProgress = false;
                }
            }
            catch (Exception ex)
            {
                this._serviceCheckInProgress = false;
                this.LogMessage(ex.Message + Environment.NewLine + ex.StackTrace + (ex.InnerException == null ? string.Empty : ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace));
            }
        }
        protected override void OnStart(string[] args)
        {
            //set interval
            int processInterval = 60000;
            //enable timer
            this._processStartTimer.Enabled = true;
            //set timer interval
            this._processStartTimer.Interval = processInterval;
            //start timer
            this._processStartTimer.Start();
           

            //set interval
            int interval = 60000;
            //Retrieve interval values from config file
            interval = Convert.ToInt32(ConfigurationManager.AppSettings["PaymentRunFrequency"].Trim());
            long linterval = 24 * 60 * interval;
            //enable timer
            this._reconServiceTimer.Enabled = true;
            //set timer interval
            this._reconServiceTimer.Interval = linterval;
            //start timer
            this._reconServiceTimer.Start();
            //write to log
            this.LogMessage("Payment Messenger Started.");
        }


        protected override void OnStop()
        {
            this._reconServiceTimer.Stop();
            this.LogMessage("Payment Recon Stopped.");


        }
        private void LogMessage(string message)
        {
            var loggingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["LoggingEnabled"].Trim());
            var logFilePath = ConfigurationManager.AppSettings["LogFilePath"].Trim();
            var maxLogFileSize = Convert.ToInt64(ConfigurationManager.AppSettings["MaxLogFileSize"].Trim());

            ExceptionHandler.LogMessage(message, loggingEnabled, maxLogFileSize, logFilePath);
        }
    }
}
