using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Threading.Tasks;
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
using System.Web.Configuration;
using System.Web;
using System.Configuration;

namespace Prvii.BusinessService.Controllers
{
    public class PushSharpController : ApiController
    {
        //Currently it will raise only for android devices
        static void DeviceSubscriptionChanged(object sender,
        string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            LogMessage("In DeviceSubscriptionChanged : " + newSubscriptionId, true, 1024);

        }

        //this even raised when a notification is successfully sent
        static void NotificationSent(object sender, INotification notification)
        {
            LogMessage("In Notification Sent" , true, 1024);
            using (PrviiEntities context = new PrviiEntities())
            {
                long id = Convert.ToInt64(notification.Tag);
                var channelMessageSubscriber = context.ChannelSubscriberMessages.Where(x => x.ID == id).First();
                channelMessageSubscriber.ModifiedOn = DateTime.UtcNow;
                channelMessageSubscriber.SMSDeliveredOn = DateTime.UtcNow;
                channelMessageSubscriber.SMSStatus = 3; //Failed to deliver the notification
                channelMessageSubscriber.IsMMS = false;
                context.SaveChanges();
            }
        }

        //this is raised when a notification is failed due to some reason
        static void NotificationFailed(object sender,
        INotification notification, Exception notificationFailureException)
        {
            LogMessage("In Notification Failed : " +  notificationFailureException.InnerException +  " $$$$$$$" + notificationFailureException.Message , true, 1024);
            using (PrviiEntities context = new PrviiEntities())
            {
                long id = Convert.ToInt64(notification.Tag);
                var channelMessageSubscriber = context.ChannelSubscriberMessages.Where(x => x.ID == id).First();
                channelMessageSubscriber.SMSErrorCode = 		((NotificationFailureException)notificationFailureException).ErrorStatusCode + "_"  + ((NotificationFailureException)notificationFailureException).ErrorStatusDescription;
                channelMessageSubscriber.ModifiedOn = DateTime.UtcNow;
                channelMessageSubscriber.SMSStatus = 4; //Failed to deliver the notification
                channelMessageSubscriber.IsMMS = false;
                context.SaveChanges();
            }
        }

        //this is fired when there is exception is raised by the channel
        static void ChannelException
            (object sender, IPushChannel channel, Exception exception)
        {
            LogMessage("In ChannelException : " + exception.InnerException + " $$$$$$$" + exception.Message, true, 1024);

        }

        //this is fired when there is exception is raised by the service
        static void ServiceException(object sender, Exception exception)
        {
            LogMessage("In ServiceException : " + exception.InnerException + " $$$$$$$" + exception.Message, true, 1024);

        }

        //this is raised when the particular device subscription is expired
        static void DeviceSubscriptionExpired(object sender,
        string expiredDeviceSubscriptionId,
            DateTime timestamp, INotification notification)
        {
            LogMessage("In DeviceSubscriptionExpired : " + expiredDeviceSubscriptionId, true, 1024);
            using (PrviiEntities context = new PrviiEntities())
            {
                long id = Convert.ToInt64(notification.Tag);
                var channelMessageSubscriber = context.ChannelSubscriberMessages.Where(x => x.ID == id).First();
                channelMessageSubscriber.SMSErrorCode = "101_DeviceSubscriptionExpired : " + expiredDeviceSubscriptionId;
                channelMessageSubscriber.ModifiedOn = DateTime.UtcNow;
                channelMessageSubscriber.SMSStatus = 4; //Failed to deliver the notification
                channelMessageSubscriber.IsMMS = false;
                context.SaveChanges();
            }
        }

        //this is raised when the channel is destroyed
        static void ChannelDestroyed(object sender)
        {
            //Do something here
        }

        //this is raised when the channel is created
        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            LogMessage("In ChannelCreated : " + pushChannel.ToString() , true, 1024);

        }

          [HttpPost]
        public  string SendPushNotification(PushMessage pushMessage)
        {
            try
            {
                LogMessage("In SendPushNotification : " + pushMessage.Message, true, 1024);

                string deveiceName = pushMessage.DeviceName.ToUpper();
                string deviceID = pushMessage.DeviceToken;
                string message = pushMessage.Message;
                long tag = pushMessage.ChannelSubscriberMessageId;
                LogMessage("Device Name : " + deveiceName + "  ## Device Id :" + deviceID,true,1024);

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


                if (deveiceName.ToUpper() == "IOS")
                {
                    //-------------------------
                    // APPLE NOTIFICATIONS
                    //-------------------------
                    //Configure and start Apple APNS
                    // IMPORTANT: Make sure you use the right Push certificate.  Apple allows you to
                    //generate one for connecting to Sandbox, and one for connecting to Production.  You must
                    // use the right one, to match the provisioning profile you build your
                    //   app with!
                    try
                    {
                        //string pushkey = ConfigurationManager.AppSettings["pushkeyPath"].ToString();
                        // System.Reflection.Assembly.GetEntryAssembly().Location
                        // var appleCert = File.ReadAllBytes(HttpContext.Current.Server.MapPath("pushkeyDev.p12"));
                        //var appleCert = File.ReadAllBytes("C:\\push\\pushkeyDev.p12");
                        var appleCert = File.ReadAllBytes("C:\\push\\pushkeyDistribution.p12");
                        // var appleCert = File.ReadAllBytes(pushkey);pushkeyDistribution
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
                        AppleNotificationAlert alert = new AppleNotificationAlert();
                        //alert.LaunchImage =
                        push.QueueNotification(new AppleNotification()
                                                    .ForDeviceToken(deviceID)//the recipient device id
                                                    .WithAlert(message)//the message
                                                    .WithBadge(1)
                                                    .WithSound("sound.caf")
                                                    .WithTag(tag)
                                                    
                                                    );


                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }


                if (deveiceName.ToUpper() == "ANDROID")
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
                    push.RegisterGcmService(new GcmPushChannelSettings("AIzaSyD9q9e6y8cFeyXmmZ7iJWjqC6NapjWCf20"));
                    //Fluent construction of an Android GCM Notification
                    //IMPORTANT: For Android you MUST use your own RegistrationId 
                    //here that gets generated within your Android app itself!
                    push.QueueNotification(new GcmNotification()
                        .ForDeviceRegistrationId(deviceID)
                        .WithJson("{\"alert\":\"" + message + "!\",\"badge\":7,\"sound\":\"sound.caf\"}")
                        .WithTag(tag)
                        
                        )
                        ;
                }

                push.StopAllServices(waitForQueuesToFinish: true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "Success";
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="loggingEnabled"></param>
        /// <param name="maxLogFileSize"></param>
        /// <param name="logFilePath"></param>
          public static void LogMessage(string message, bool loggingEnabled, long maxLogFileSize)
          {
              FileInfo fileInfo;
              StreamWriter streamWriter;
              long currentfileSize = 0;
              Decimal currentFileSizeInMB = 0;

              if (loggingEnabled)
              {
                  string logFilePath = "C:\\Inetpub\\vhosts\\verifysmartcorp.com\\apiuat.prvii.org\\bin\\LOGS\\Prvii_mssenger_push_logs.txt";
                  fileInfo = new FileInfo(logFilePath);

                  if (fileInfo.Exists)
                  {
                      currentfileSize = fileInfo.Length;
                      currentFileSizeInMB = currentfileSize / (1024 * 1024);
                  }

                  if (!fileInfo.Exists || currentFileSizeInMB > maxLogFileSize)
                      streamWriter = fileInfo.CreateText();
                  else
                      streamWriter = new StreamWriter(logFilePath, true);

                  streamWriter.WriteLine("==================================================================");
                  streamWriter.WriteLine(DateTime.Now.ToString());
                  streamWriter.WriteLine(message);
                  streamWriter.WriteLine("==================================================================");
                  streamWriter.Flush();
                  streamWriter.Close();
                  streamWriter.Dispose();
              }
          }
    }
}
