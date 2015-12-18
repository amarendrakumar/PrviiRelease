using Newtonsoft.Json;
using Prvii.BusinessService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Prvii.Entities;
using Prvii.Business;
using Prvii.Entities.Enumerations;
using System.Net.Http.Headers;
using System.Collections;
using System.Globalization;
using Prvii.Entities.DataEntities;

namespace Prvii.BusinessService.Controllers
{
    public class ChannelMessageController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage CreateChannelMessage(ChannelMessageDTO message)
        {
            string result = this.SaveChannelMessage(message);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        private string SaveChannelMessage(ChannelMessageDTO message)
        {
           
            CultureInfo us = new CultureInfo("en-US");
            //string text = "1434-09-23T15:16";
            string format = "yyyy'-'MM'-'dd'T'HH':'mm";
             if (!message.IsScheduled)
             {
                 var dtNow = UtilityManager.GetZoneSpecificTimeFromUTC(DateTime.UtcNow, ChannelManager.GetTimeZone(message.ChannelID));
                
                 var GetScheduleDate = DateTime.ParseExact(message.ScheduledOn, format, us);
                 if (GetScheduleDate < dtNow)
                 {
                     return "Schedule date & time should be in future.";
                 }
             }
           
             //string scheduledDateTimeUTC=string.Empty;

             //if (!message.IsScheduled)
             //    scheduledDateTimeUTC = Convert.ToDateTime(message.ScheduledOn).ToUniversalTime()
             //               .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

             var channelMessage = new ChannelMessage
             {
                 ID = message.ID,
                 ChannelID = message.ChannelID,
                 SendToAll = message.SendToAll,
                 Subject = message.Subject != null ? message.Subject : "",
                 Message = message.Message != null ? message.Message : "",
                 EmailMessage = message.EmailMessage != null ? message.EmailMessage : "",
                 IsScheduled = message.IsScheduled ? false : true,
                 ScheduledOn = message.IsScheduled ? DateTime.Now : DateTime.ParseExact(message.ScheduledOn, format, us),
                 SendByEmail = message.IsEmail,
                 SendBySMS = message.IsSMS,
                 StatusID = message.IsEmail ? message.IsScheduled ? (short)ChannelMessageStatus.Approved : (short)ChannelMessageStatus.Created : (short)ChannelMessageStatus.NA,
                 SMSStatusID = message.IsSMS ? message.IsScheduled ? (short)SMSStatus.Approved : (short)SMSStatus.Created : (short)SMSStatus.NA
             };

            List<long> subscriberIDList = new List<long>();
            List<long> mediaIDList = new List<long>();

            List<ChannelMessageSubscriberData> channelMessageSubscriberList = new List<ChannelMessageSubscriberData>();
            if (message.SubscriberIDs != null)
            {
                foreach (var item in message.SubscriberIDs)
                {
                    if (message.SentOnly == "SMS")
                    {
                        if (message.SIDs.ToList().Contains(Convert.ToInt64(item)))
                            channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                        else
                        {
                            var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item));
                            var deliveryMethodList = deliveryMethod.Split(',').ToList();
                            if (deliveryMethodList.Contains("Text"))
                                channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                        }
                            
                    }
                    else if (message.SentOnly == "Email")
                    {
                        if (message.SIDs.ToList().Contains(Convert.ToInt64(item)))
                            channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                        else
                        {
                            var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item));
                            var deliveryMethodList = deliveryMethod.Split(',').ToList();
                            if (deliveryMethodList.Contains("Email"))
                                channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                        }
                           
                    }
                    else
                    {
                        var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item));
                        var deliveryMethodList = deliveryMethod.Split(',').ToList();
                        //if (deliveryMethodList.Contains("Email"))
                        //    SendEmail = true;

                        //if (deliveryMethodList.Contains("Text"))
                        //    SendSMS = true;

                        channelMessageSubscriberList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item), IsEmailSend = deliveryMethodList.Contains("Email") ? (int)ChannelMessageStatus.EmailSent : (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = deliveryMethodList.Contains("Text") ? (int)SMSStatus.SMSSent : (int)SMSStatus.SMSNotSent });

                       // channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = channelMessage.SendByEmail ? (int)ChannelMessageStatus.EmailSent : (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = channelMessage.SendBySMS ? (int)SMSStatus.SMSSent : (int)SMSStatus.SMSNotSent });
                    }

                   // channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = channelMessage.SendByEmail ? (int)ChannelMessageStatus.EmailSent : (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = channelMessage.SendBySMS ? (int)SMSStatus.SMSSent : (int)SMSStatus.SMSNotSent });
                }
            }
            if (message.MediaURLIds != null)
            {
                foreach (var item in message.MediaURLIds)
                {
                    mediaIDList.Add(Convert.ToInt64(item));
                }
            }
            if (channelMessageSubscriberList.Count>0)
            {
                if (message.IsScheduled)
                {
                    ChannelMessageManager.ApproveMobile(channelMessage, mediaIDList, channelMessageSubscriberList);
                }
                else
                {
                    ChannelMessageManager.SaveMobile(channelMessage, mediaIDList, channelMessageSubscriberList);
                }
            }
            


            return "success";
        }


        private string SaveApproveChannelMessage(ChannelMessageDTO message)
        {
            CultureInfo us = new CultureInfo("en-US");
            //string text = "1434-09-23T15:16";
            string format = "yyyy'-'MM'-'dd'T'HH':'mm";
            if (!message.IsScheduled)
            {
                var dtNow = UtilityManager.GetZoneSpecificTimeFromUTC(DateTime.UtcNow, ChannelManager.GetTimeZone(message.ChannelID));
               
                var GetScheduleDate = DateTime.ParseExact(message.ScheduledOn, format, us);
                if (GetScheduleDate < dtNow)
                {
                    return "Schedule date & time should be in future.";
                }
              
            }

            //string scheduledDateTimeUTC = string.Empty;
            //if (!message.IsScheduled)
            //    scheduledDateTimeUTC = Convert.ToDateTime(message.ScheduledOn).ToUniversalTime()
            //               .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

            var channelMessage = new ChannelMessage
            {
                ID = message.ID,
                ChannelID = message.ChannelID,
                SendToAll = message.SendToAll,
                Subject = message.Subject != null ? message.Subject : "",
                Message = message.Message != null ? message.Message : "",
                EmailMessage = message.EmailMessage != null ? message.EmailMessage : "",
                IsScheduled = message.IsScheduled ? false : true,
                ScheduledOn = message.IsScheduled ? DateTime.Now : DateTime.ParseExact(message.ScheduledOn, format, us),
                SendByEmail = message.IsEmail,
                SendBySMS = message.IsSMS,
                StatusID = message.IsEmail ? message.IsScheduled ? (short)ChannelMessageStatus.Approved : (short)ChannelMessageStatus.Created : (short)ChannelMessageStatus.NA,
                SMSStatusID = message.IsSMS ? message.IsScheduled ? (short)SMSStatus.Approved : (short)SMSStatus.Created : (short)SMSStatus.NA
            };

            List<long> subscriberIDList = new List<long>();
            List<long> mediaIDList = new List<long>();
            List<ChannelMessageSubscriberData> channelMessageSubscriberList = new List<ChannelMessageSubscriberData>();
            if (message.SubscriberIDs != null)
            {
                foreach (var item in message.SubscriberIDs)
                {
                    //if(message.SentOnly=="SMS")
                    //{
                    //    if (message.SIDs.ToList().Contains(Convert.ToInt64(item)))
                    //         channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend =  (int)SMSStatus.SMSSent  });
                    //    else
                    //        channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                    //}
                    //else if (message.SentOnly == "Email")
                    //{
                    //    if (message.SIDs.ToList().Contains(Convert.ToInt64(item)))
                    //        channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                    //    else
                    //        channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                    //}
                    //else
                    //{
                    //    channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = channelMessage.SendByEmail ? (int)ChannelMessageStatus.EmailSent : (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = channelMessage.SendBySMS ? (int)SMSStatus.SMSSent : (int)SMSStatus.SMSNotSent });
                    //}

                    if (message.SentOnly == "SMS")
                    {
                        if (message.SIDs.ToList().Contains(Convert.ToInt64(item)))
                            channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                        else
                        {
                            var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item));
                            var deliveryMethodList = deliveryMethod.Split(',').ToList();
                            if (deliveryMethodList.Contains("Text"))
                                channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                        }

                    }
                    else if (message.SentOnly == "Email")
                    {
                        if (message.SIDs.ToList().Contains(Convert.ToInt64(item)))
                            channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                        else
                        {
                            var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item));
                            var deliveryMethodList = deliveryMethod.Split(',').ToList();
                            if (deliveryMethodList.Contains("Email"))
                                channelMessageSubscriberList.Add(new ChannelMessageSubscriberData { SubsciberID = Convert.ToInt64(item), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                        }

                    }
                    else
                    {
                        var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item));
                        var deliveryMethodList = deliveryMethod.Split(',').ToList();
                        channelMessageSubscriberList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item), IsEmailSend = deliveryMethodList.Contains("Email") ? (int)ChannelMessageStatus.EmailSent : (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = deliveryMethodList.Contains("Text") ? (int)SMSStatus.SMSSent : (int)SMSStatus.SMSNotSent });

                    }
                }
            }

            if (message.MediaURLIds != null)
            {
                foreach (var item in message.MediaURLIds)
                {
                    mediaIDList.Add(Convert.ToInt64(item));
                }
            }
            if (channelMessageSubscriberList.Count > 0)
                ChannelMessageManager.ApproveMobile(channelMessage, mediaIDList, channelMessageSubscriberList);
          

            return "success";
        }

        [HttpPost]
        public HttpResponseMessage CreateChannelMessageApprove(ChannelMessageDTO message)
        {
           string result= this.SaveApproveChannelMessage(message);
           return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage UpdateChannelMessage(ChannelMessageDTO message)
        {
            string result = this.SaveChannelMessage(message);
            return Request.CreateResponse(HttpStatusCode.OK, result);           
        }


        [HttpPost]
        public HttpResponseMessage UpdateChannelMessageApprove(ChannelMessageDTO message)
        {
            string result = this.SaveApproveChannelMessage(message);
            return Request.CreateResponse(HttpStatusCode.OK, result);          
            
        }

        [HttpGet]
        public HttpResponseMessage GetAttachment(long AttachId)
        {
            var attachment = ChannelMessageManager.GetAttachmentByID(AttachId);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //var stream = new MemoryStream(attachment.Content);
            //result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue(attachment.MimeType);
            return result;

            //result = Request.CreateResponse(HttpStatusCode.OK);
            //result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            //result.Content.Headers.ContentDisposition.FileName = attachment.Name;
            //return result;
        }

      

        [HttpPost]
        public IEnumerable<ChannelMessageDTO> GetChannelMessageList(ChannelDTO channel)
        {
            return ChannelMessageManager.GetByChannelID(channel.ID).Select(cm => new ChannelMessageDTO
            {
                ID = cm.ID,
                ChannelID = cm.ChannelID,
                SendToAll = cm.SendToAll,
                Subject = cm.Subject,
                Message = cm.Message,
                EmailMessage = cm.EmailMessage,
                IsSMS = cm.IsSMS,
                IsEmail = cm.IsEmail,
                IsScheduled = cm.IsScheduled,
                ScheduledOn = Convert.ToString(cm.ScheduledOn),
                TimeZoneID = cm.TimeZoneID,
                Status = cm.Status,
                SMSStatus = cm.SMSStatus,
                StatusText = ((SMSStatus)cm.SMSStatus).ToString(),
                StatusEmail = ((ChannelMessageStatus)cm.Status).ToString(),
                SentOn = Convert.ToString(cm.SentOn)

            }).ToList();
          
        }

        [HttpPost]
        public ChannelMessageDTO GetChannelMessageById(ChannelMessageDTO channelMessageId)
        {
            //var channelMessageDetails = ChannelMessageManager.GetChannelMessageById(channelMessageId.ID);
            var channelMessageDetails = ChannelMessageManager.GetByID(channelMessageId.ID);
            long[] mediaIDList = ChannelMessageManager.GetAttachments(channelMessageId.ID).ToArray();
            long[] subscriberIDList = ChannelMessageManager.GetMessageSubscribers(channelMessageId.ID).ToArray();
            
            if (channelMessageDetails != null)
            {
                string timeZone = ChannelManager.GetTimeZone(channelMessageDetails.ChannelID);
                var scheduleDate = UtilityManager.GetZoneSpecificTimeFromUTC(channelMessageDetails.ScheduledOn, timeZone);
                return new ChannelMessageDTO
             {
                 ID = channelMessageDetails.ID,
                 ChannelID = channelMessageDetails.ChannelID,
                 Subject = channelMessageDetails.Subject,
                 Message = channelMessageDetails.Message,
                 EmailMessage = channelMessageDetails.EmailMessage,
                 IsScheduled = channelMessageDetails.IsScheduled,
                 ScheduledOn = Convert.ToString(scheduleDate),
                 Status = channelMessageDetails.StatusID,
                 SMSStatus = channelMessageDetails.SMSStatusID.Value,
                 IsEmail = channelMessageDetails.SendByEmail,
                 IsSMS = channelMessageDetails.SendBySMS,
                 SentOn = Convert.ToString(channelMessageDetails.SentOn),
                 MediaURLIds = mediaIDList,
                 SendToAll = channelMessageDetails.SendToAll,
                 SubscriberIDs = subscriberIDList,
                 TimeZoneID = channelMessageDetails.TimeZoneID
             };
            }

            return new ChannelMessageDTO();
        }

         [HttpPost]
        public IList GetChannelSubscriberMessages(ChannelDTO channel)
        {

            return ChannelMessageManager.GetBySubscriberID(channel.ID, channel.UserID).Select(cm => new ChannelMessageDTO
            {
                ID = cm.ID,
                ChannelID = cm.ChannelID,
                Subject = cm.Subject,
                TimeZoneID = cm.TimeZoneID,
                SentOn = Convert.ToString(cm.SentOn)
            }).ToList();

           // return ChannelMessageManager.GetChannelSubscriberMessages(channel.ID, channel.UserID);
          
        }

          [HttpPost]
        public IList GetChannelSubscriberMessagesPastWeek(ChannelDTO channel)
        {

            return ChannelMessageManager.GetBySubscriberIDPastWeek(channel.ID, channel.UserID,channel.WeekNo).Select(cm => new ChannelMessageDTO
            {
                ID = cm.ID,
                ChannelID = cm.ChannelID,
                Subject = cm.Subject,
                TimeZoneID = cm.TimeZoneID,
                SentOn = Convert.ToString(cm.SentOn)
            }).ToList();

           // return ChannelMessageManager.GetChannelSubscriberMessages(channel.ID, channel.UserID);
          
        }

       

         [HttpPost]
         public HttpResponseMessage ChannelMessageApproved(ChannelMessageDTO channelMessage)
         {
             ChannelMessageManager.UpdateMessageStatusByID(channelMessage.ID);
             return Request.CreateResponse(HttpStatusCode.OK, "success");
         }
         [HttpPost]
         public HttpResponseMessage ChannelMessageDelete(ChannelMessageDTO channelMessage)
         {
             ChannelMessageManager.DeleteChannelMessageByID(channelMessage.ID);
             return Request.CreateResponse(HttpStatusCode.OK, "success");
         }

         [HttpPost]
         public HttpResponseMessage DeleteMessageAttachmentById(ChannelMessageAttachmentDTO channelMessageAttachment)
         {
             ChannelMessageManager.DeleteAttachmentByID(channelMessageAttachment.ID);
             return Request.CreateResponse(HttpStatusCode.OK, "success");
         }


         [HttpPost]
         public IEnumerable<ChannelMessageDTO> GetChannelSubscriberAllMessageByChannelID(ChannelDTO channel)
         {
             return ChannelMessageManager.GetChannelSubscriberAllMessageByChannelID(channel.ID,channel.UserID).Select(cm => new ChannelMessageDTO
             {
                 ID = cm.ID,
                 ChannelID = cm.ChannelID,
                 Subject = cm.Subject,
                 Message = cm.Message,
                 IsScheduled = cm.IsScheduled,
                 ScheduledOn = Convert.ToString(cm.ScheduledOn),
                 Status = cm.Status,
                 IsEmail = cm.IsEmail,
                 IsSMS = cm.IsSMS,
                 SentOn = Convert.ToString(cm.SentOn),
                 TimeZoneID = cm.TimeZoneID

             }).ToList();

         }

        
         [HttpPost]
         public IEnumerable<ChannelMessageDTO> GetChannelSubscriberAllMessageByChannelIDSubscriberSearch(ChannelDTO channel)
         {
             return ChannelMessageManager.GetChannelSubscriberAllMessageByChannelIDSubscriberSearch(channel.ID, channel.UserID, channel.FromDate, channel.ToDate, channel.SearchText).Select(cm => new ChannelMessageDTO
             {
                 ID = cm.ID,
                 ChannelID = cm.ChannelID,
                 Subject = cm.Subject,
                 Message = cm.Message,
                 IsScheduled = cm.IsScheduled,
                 ScheduledOn = Convert.ToString(cm.ScheduledOn),
                 Status = cm.Status,
                 IsEmail = cm.IsEmail,
                 IsSMS = cm.IsSMS,
                 SentOn = Convert.ToString(cm.SentOn),
                 TimeZoneID = cm.TimeZoneID,
                 IsDeleted = false // cm.IsDeleted
             }).ToList();

         }

         [HttpPost]
         public IList checkDeliveryMechanismsSMS(ChannelMessageDTO channelMessage)
         {
             List<long> SMSIDList = new List<long>();
             foreach (var item in channelMessage.SubscriberIDs)
             {
                 var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item));
                 var deliveryMethodList = deliveryMethod.Split(',').ToList();

                 if (!deliveryMethodList.Contains("Email"))
                     SMSIDList.Add(Convert.ToInt64(item));                
             }
             return SMSIDList.ToList();
           
         }

         [HttpPost]
         public IList checkDeliveryMechanismsEmail(ChannelMessageDTO channelMessage)
         {
             List<long> EmailIDList = new List<long>();
             foreach (var item in channelMessage.SubscriberIDs)
             {
                 var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item));
                 var deliveryMethodList = deliveryMethod.Split(',').ToList();

                 if (!deliveryMethodList.Contains("Text"))
                     EmailIDList.Add(Convert.ToInt64(item));
             }
             return EmailIDList.ToList();
         }  
    }
}
