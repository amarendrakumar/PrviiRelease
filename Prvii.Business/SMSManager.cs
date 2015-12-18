using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Twilio;
using Prvii.Entities;
using Prvii.Entities.Enumerations;

namespace Prvii.Business
{
    public class SMSManager : IDisposable
    {

        public string ACCOUNT_SID { get; set; }
        public string AUTH_TOKEN { get; set; }
        public string FROMNUMBER { get; set; }
        public string TONUMBER { get; set; }
        public string SMSBody { get; set; }

        public string[] MedialUrls { get; set; }

        public long ChannelSubscriberMessageId { get; set; }


        public void SendSMS()
        {
            this.SendSMSByTwilio();
        }

        public void SendMMS()
        {
            this.SendMMSByTwilio();
        }


        private void SendSMSByTwilio()
        {
            long channelSubcriberMessageId = this.ChannelSubscriberMessageId;
            
            if (string.IsNullOrEmpty(TONUMBER))
                throw new Exception("ToNumber is empty");

            if (System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_SID"] != null)
                ACCOUNT_SID = ConfigurationManager.AppSettings["ACCOUNT_SID"].ToString();

            if (System.Configuration.ConfigurationManager.AppSettings["AUTH_TOKEN"] != null)
                AUTH_TOKEN = ConfigurationManager.AppSettings["AUTH_TOKEN"].ToString();

            if (System.Configuration.ConfigurationManager.AppSettings["FROMNUMBER"] != null)
                FROMNUMBER = ConfigurationManager.AppSettings["FROMNUMBER"].ToString();

            TwilioRestClient client = new TwilioRestClient(ACCOUNT_SID, AUTH_TOKEN);

            SMSMessage smsMessage = client.SendSmsMessage(FROMNUMBER, TONUMBER, SMSBody);
            if (smsMessage.RestException != null)
            {
                using (PrviiEntities context = new PrviiEntities())
                {
                    var channelMessageSubscriber = context.ChannelSubscriberMessages.Where(x => x.ID == channelSubcriberMessageId).First();
                    channelMessageSubscriber.SMSErrorCode = smsMessage.RestException.Code;
                    channelMessageSubscriber.ModifiedOn = DateTime.Now;
                    channelMessageSubscriber.IsMMS = false;
                    context.SaveChanges();
                }
            }
            else
            {
                using (PrviiEntities context = new PrviiEntities())
                {
                    var channelMessageSubscriber = context.ChannelSubscriberMessages.Where(x => x.ID == channelSubcriberMessageId).First();
                    channelMessageSubscriber.MessageSID = smsMessage.Sid;
                    channelMessageSubscriber.SMSStatus = GetSMSStatus(smsMessage.Status);
                    channelMessageSubscriber.SMSCost = smsMessage.Price;
                    channelMessageSubscriber.ModifiedOn = DateTime.Now;
                    channelMessageSubscriber.IsMMS = false;
                    context.SaveChanges();
                }
            }
            


        }


        private void SendMMSByTwilio()
        {
            long channelSubcriberMessageId = this.ChannelSubscriberMessageId;
            if (string.IsNullOrEmpty(TONUMBER))
                throw new Exception("ToNumber is empty");

            if (System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_SID"] != null)
                ACCOUNT_SID = ConfigurationManager.AppSettings["ACCOUNT_SID"].ToString();

            if (System.Configuration.ConfigurationManager.AppSettings["AUTH_TOKEN"] != null)
                AUTH_TOKEN = ConfigurationManager.AppSettings["AUTH_TOKEN"].ToString();

            if (System.Configuration.ConfigurationManager.AppSettings["FROMNUMBER"] != null)
                FROMNUMBER = ConfigurationManager.AppSettings["FROMNUMBER"].ToString();

            TwilioRestClient client = new TwilioRestClient(ACCOUNT_SID, AUTH_TOKEN);

            Message mmsMessage = client.SendMessage(FROMNUMBER, TONUMBER, SMSBody,  MedialUrls);
           // if(mmsMessage.ErrorCode == "")
            if (mmsMessage.RestException != null)
            {
                if (mmsMessage.RestException.Code == "21612")
                {
                    SendSMSByTwilio();
                }
                else
                {
                    using (PrviiEntities context = new PrviiEntities())
                    {
                        var channelMessageSubscriber = context.ChannelSubscriberMessages.Where(x => x.ID == channelSubcriberMessageId).First();
                        channelMessageSubscriber.SMSErrorCode = mmsMessage.RestException.Code;
                        channelMessageSubscriber.ModifiedOn = DateTime.Now;
                        channelMessageSubscriber.IsMMS = true;
                        context.SaveChanges();
                    }
                }
            }
            else
            {
                
                using (PrviiEntities context = new PrviiEntities())
                {
                    var channelMessageSubscriber = context.ChannelSubscriberMessages.Where(x => x.ID == channelSubcriberMessageId).First();
                    channelMessageSubscriber.MessageSID = mmsMessage.Sid;
                    channelMessageSubscriber.SMSStatus = GetSMSStatus(mmsMessage.Status);
                    channelMessageSubscriber.SMSCost = mmsMessage.Price;
                    channelMessageSubscriber.ModifiedOn = DateTime.Now;
                    channelMessageSubscriber.IsMMS = true;
                    context.SaveChanges();
                }
            }



        }



        private short GetSMSStatus(string status)
        {
            switch (status)
            {
                case "queued":
                    return Convert.ToInt16(SMSStatus.Queued);
                case "sending":
                    return Convert.ToInt16(SMSStatus.Sending);
                case "sent":
                    return Convert.ToInt16(SMSStatus.Sent);
                case "failed":
                    return Convert.ToInt16(SMSStatus.Failed);
                default:
                    return Convert.ToInt16(SMSStatus.Queued);
            }
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        internal void GetSMSStatusfromTwilio(long channelSubcriberMessageId, string TwilioMessageId)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_SID"] != null)
                ACCOUNT_SID = ConfigurationManager.AppSettings["ACCOUNT_SID"].ToString();

            if (System.Configuration.ConfigurationManager.AppSettings["AUTH_TOKEN"] != null)
                AUTH_TOKEN = ConfigurationManager.AppSettings["AUTH_TOKEN"].ToString();

            TwilioRestClient client = new TwilioRestClient(ACCOUNT_SID, AUTH_TOKEN);

            SMSMessage smsMessage = client.GetSmsMessage(TwilioMessageId);

            using (PrviiEntities context = new PrviiEntities())
            {
                var channelMessageSubscriber = context.ChannelSubscriberMessages.Where(x => x.ID == channelSubcriberMessageId).First();
                channelMessageSubscriber.SMSStatus = GetSMSStatus(smsMessage.Status);
                if (smsMessage.Status.ToLower() == "sent")
                {
                    channelMessageSubscriber.SMSDeliveredOn = smsMessage.DateSent;
                }
                channelMessageSubscriber.SMSCost = smsMessage.Price;
                channelMessageSubscriber.ModifiedOn = DateTime.Now;
                context.SaveChanges();
            }
            


        }
    }

   
}
