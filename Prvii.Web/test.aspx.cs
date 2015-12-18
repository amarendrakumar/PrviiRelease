using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Entities;
using Prvii.Business;
using Prvii.Entities.Enumerations;
using System.Drawing;
using Prvii.Entities.DataEntities;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Mail;
using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using PushSharp.Core;
using System.IO;


namespace Prvii.Web
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ////AndroidGCMPushNotification.SendNotification("aed4a629ee0fb379e6a41d4b32a49b3b12d0d036dc459c357d9fe26a7af6971e", "This si my first push notification");
            //AndroidGCMPushNotification.pushMessage("aed4a629ee0fb379e6a41d4b32a49b3b12d0d036dc459c357d9fe26a7af6971e");
            ////PaymentManager manager = new PaymentManager();
            ////manager.ReconcilePaypalTransactions();
            //DateTime dtStartDate = Convert.ToDateTime("2012-03-15");
            //DateTime dtEndDate = Convert.ToDateTime("2015-07-14");
            //string strYear = "";
            //string strMonth = "";
            //string strDay = "";
            //strYear = (dtEndDate.Year - dtStartDate.Year).ToString();
            //strMonth = (dtEndDate.Month - dtStartDate.Month).ToString();
            //strDay = (dtEndDate.Day - dtStartDate.Day).ToString();
            //var ss= "Year = " + strYear + " " + "Month = " + strMonth + " Days =" + strDay;
           // PushSharpNotification.SendPushNotification("ios", "2ad26f1bbed27462bec2339f566c0fb19241d2dee4892e26a8f3452f9871d2c7", "This is test message from code");
           // PushSharpNotification.SendPushNotification("ios", "3240a39528201cd4d729ed0b7de9fcf4fd823f2cfd4992119b207379ad50bdd0", "This is test message from code");
            
        }

        protected void btnSendMSMS_Click(object sender, EventArgs e)
        {
            ChannelMessageManager.SendSMS();
        }

        protected void btnSentEmail_Click(object sender, EventArgs e)
        {
            ChannelMessageManager.SendMessage();
        }


        protected void btnSendEmail_Click(object sender, EventArgs e)
        {

            MailAddress objFrom = new MailAddress("prviiuat@verifysmartcorp.com");
            MailAddress objTo = new MailAddress("amaren1982@gmail.com");
            MailAddress objFromAndReply = new MailAddress("prviiuat@verifysmartcorp.com");
            MailMessage mailMsg = new MailMessage(objFrom, objTo);
            mailMsg.ReplyTo = objFromAndReply;
            mailMsg.From = objFrom;
            mailMsg.Subject = "Test By Amar";
            mailMsg.IsBodyHtml = true;
            string Body = "<body> sdsds</body>";
            mailMsg.Body = Body;

            SmtpClient ObjSmtpClient = new SmtpClient("smtpout.asia.secureserver.net", 25);
            ObjSmtpClient.UseDefaultCredentials = true;
            ObjSmtpClient.Credentials = new System.Net.NetworkCredential("prviiuat@verifysmartcorp.com", "UATprvii");
           // ObjSmtpClient.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
            ObjSmtpClient.EnableSsl = false;
            ObjSmtpClient.Send(mailMsg);

          
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {



            if (!string.IsNullOrEmpty(txtNotification.Text))
            {
                //create the puchbroker object
                var push = new PushBroker();
                //Wire up the events for all the services that the broker registers
                push.OnNotificationSent += NotificationSent;
                push.OnChannelException += ChannelException;
                push.OnServiceException += ServiceException;
                push.OnNotificationFailed += NotificationFailed;
                push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                push.OnChannelCreated += ChannelCreated;
                push.OnChannelDestroyed += ChannelDestroyed;

                //Here I am looping on my device collection. 
                //Use your own way to handle this
                //List<Device> rows = 
                //    new List<Device>(CommonMethods.entity.Devices.ToList());
                List<Device> rows = new List<Device>();
                //var dv1 = new Device { deviceid = "fEN39HeJdUs:APA91bFqFAKwa5iHZJyptEVGcToJ9DFPSyzttqEGe0gr4MehXydcnGqyZwhu_kOCjh-qMa7Xtdnf7w6RaHR7uUkLpwRogcvrbPCxL2l4bNCD29SxVr3pfTEMhWo-XtEyWsjsBQler_Ih", devicename = "android" };
               // var dv = new Device { deviceid = "c2a58c13a08b7762e7ffe9d85a94ee2de7045f243a75a0f05bc1f21efef31a65", devicename = "ios" };
                var dv2 = new Device { deviceid = "ac6f20b63dccde7e77f6638c1a4a38f88ff756177d953a5eb260760936da63a7", devicename = "ios" };
                 //var dv3 = new Device { deviceid = "2ad26f1bbed27462bec2339f566c0fb19241d2dee4892e26a8f3452f9871d2c7", devicename = "ios" };
                rows.Add(dv2);
               // rows.Add(dv1);
              //  rows.Add(dv2);
              // rows.Add(dv3);
                foreach (Device row in rows)
                {

                    if (row.devicename == "ios")
                    {
                        //-------------------------
                        // APPLE NOTIFICATIONS
                        //-------------------------
                        //Configure and start Apple APNS
                        // IMPORTANT: Make sure you use the right Push certificate.  Apple allows you to
                        //generate one for connecting to Sandbox, and one for connecting to Production.  You must
                        // use the right one, to match the provisioning profile you build your
                        //   app with!  pushkeyDistribution,pushkeyDev
                        try
                        {
                            var appleCert = File.ReadAllBytes(Server.MapPath("pushkeyDistribution.p12"));
                            //IMPORTANT: If you are using a Development provisioning Profile, you must use
                            // the Sandbox push notification server 
                            //  (so you would leave the first arg in the ctor of ApplePushChannelSettings as
                            // 'false')
                            //  If you are using an AdHoc or AppStore provisioning profile, you must use the 
                            //Production push notification server
                            //  (so you would change the first arg in the ctor of ApplePushChannelSettings to 
                            //'true')
                            push.RegisterAppleService(new ApplePushChannelSettings(true, appleCert, "Globrin@123"));
                            //Extension method
                            //Fluent construction of an iOS notification
                            //IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets
                            // generated within your iOS app itself when the Application Delegate
                            //  for registered for remote notifications is called, 
                            // and the device token is passed back to you
                            push.QueueNotification(new AppleNotification()
                                                        .ForDeviceToken(row.deviceid)//the recipient device id
                                                        .WithAlert(txtNotification.Text)//the message
                                                        .WithBadge(1)
                                                        .WithSound("sound.caf")
                                                        );


                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }


                    if (row.devicename == "android")
                    {
                        //---------------------------
                        // ANDROID GCM NOTIFICATIONS
                        //---------------------------
                        //Configure and start Android GCM
                        //IMPORTANT: The API KEY comes from your Google APIs Console App, 
                        //under the API Access section, 
                        //  by choosing 'Create new Server key...'
                        //  You must ensure the 'Google Cloud Messaging for Android' service is 
                        //enabled in your APIs Console
                        push.RegisterGcmService(new
                         GcmPushChannelSettings("AIzaSyDJIMGEMY7C4Ukj6cimgrKLd9Mf4nRSP_o"));
                        //Fluent construction of an Android GCM Notification
                        //IMPORTANT: For Android you MUST use your own RegistrationId 
                        //here that gets generated within your Android app itself!
                        push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(row.deviceid).WithJson("{\"alert\":\"Hello World!\",\"badge\":7,\"sound\":\"sound.caf\"}"));
                    }

                    push.StopAllServices(waitForQueuesToFinish: true); 
                }
            }
        }

        //Currently it will raise only for android devices
        static void DeviceSubscriptionChanged(object sender,
        string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Do something here
        }

        //this even raised when a notification is successfully sent
        static void NotificationSent(object sender, INotification notification)
        {
            //Do something here
        }

        //this is raised when a notification is failed due to some reason
        static void NotificationFailed(object sender,
        INotification notification, Exception notificationFailureException)
        {
            //Do something here
            PushSharp.Apple.NotificationFailureException ss = (PushSharp.Apple.NotificationFailureException)notificationFailureException;
            string  aa = notificationFailureException.Data.Values.ToString();
        }

        //this is fired when there is exception is raised by the channel
        static void ChannelException
            (object sender, IPushChannel channel, Exception exception)
        {
            //Do something here
        }

        //this is fired when there is exception is raised by the service
        static void ServiceException(object sender, Exception exception)
        {
            //Do something here
        }

        //this is raised when the particular device subscription is expired
        static void DeviceSubscriptionExpired(object sender,
        string expiredDeviceSubscriptionId,
            DateTime timestamp, INotification notification)
        {
            //Do something here
        }

        //this is raised when the channel is destroyed
        static void ChannelDestroyed(object sender)
        {
            //Do something here
        }

        //this is raised when the channel is created
        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            //Do something here
        }

    }
}