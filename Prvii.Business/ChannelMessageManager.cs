using Prvii.Entities;
using Prvii.Entities.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Globalization;
using System.Net.Mail;
using Prvii.Entities.DataEntities;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Web.Configuration;
using Newtonsoft.Json;

namespace Prvii.Business
{
    public class ChannelMessageManager
    {
        public static bool IsSending(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMessages.Any(x => x.ID == id && x.StatusID == (short)ChannelMessageStatus.Sending);
            }
        }

        public static bool IsSMSSending(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMessages.Any(x => x.ID == id && x.SMSStatusID == (short)SMSStatus.Sending);
            }
        }

        public static ChannelMessage GetByID(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMessages.FirstOrDefault(x => x.ID == id);
            }
        }

        public static List<long> GetAttachments(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMessageAttachments.Where(x => x.ChannelMessageID == id).Select(x => x.ChannelMediaID).ToList();
            }
        }

        public static List<long> GetMessageSubscribers(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelSubscriberMessages.Where(x => x.ChannelMessageID == id).Select(x => x.ChannelSubscriberID).ToList();
            }
        }

        public static List<ChannelMessageData> GetByChannelID(long channelID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMessages.Where(c => c.ChannelID == channelID).OrderByDescending(o => o.ID).ToList().Select(x => new ChannelMessageData
                {
                    ID = x.ID,
                    ChannelID = x.ChannelID,
                    SendToAll = x.SendToAll,
                    Subject = x.Subject,
                    EmailMessage = x.EmailMessage,
                    Message = x.Message,
                    IsSMS = x.SendBySMS,
                    IsEmail = x.SendByEmail,
                    IsScheduled = x.IsScheduled,
                    ScheduledOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.ScheduledOn, x.TimeZoneID),
                    TimeZoneID = x.TimeZoneID,
                    Status = x.StatusID,
                    SMSStatus = x.SMSStatusID.Value,
                    StatusEmail = ((ChannelMessageStatus)x.StatusID).ToString(),
                    StatusText = ((SMSStatus)x.SMSStatusID).ToString(),
                    SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.ScheduledOn, x.TimeZoneID)// x.SentOn
                }).ToList();
            }
        }

        public static List<ChannelMessageData> GetBySubscriberID(long channelID, long subscriberID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var query = from cm in context.ChannelMessages
                            join csm in context.ChannelSubscriberMessages on cm.ID equals csm.ChannelMessageID
                            join cs in context.ChannelSubscribers on csm.ChannelSubscriberID equals cs.ID
                            where cm.ChannelID == channelID && cs.SubscriberID == subscriberID && (csm.DeliveredOn.HasValue || csm.SMSDeliveredOn.HasValue)
                            select new
                            {
                                ID = cm.ID,
                                Subject = cm.Subject.Length > 0 ? cm.Subject : cm.Message,
                                SentOn = csm.DeliveredOn.HasValue ? csm.DeliveredOn.Value : csm.SMSDeliveredOn.Value,
                                TimeZoneID = csm.TimeZoneID
                            };

                return query.ToList().Select(x => new ChannelMessageData
                {
                    ID = x.ID,
                    Subject = x.Subject,
                    SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.SentOn, x.TimeZoneID),
                    TimeZoneID = x.TimeZoneID,
                    ChannelID = channelID
                }).ToList();
            }
        }
        public static List<ChannelMessageData> GetBySubscriberIDPastWeek(long channelID, long subscriberID, long weekNo)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                DateTime? startDate = null;
                DateTime? endDate = null;

                // DayOfWeek weekStart = DayOfWeek.Monday;
                endDate = DateTime.UtcNow;

                //while (endDate.Value.DayOfWeek != weekStart)
                //    endDate = endDate.Value.AddDays(-1);

                startDate = endDate.Value.AddDays(-7 * weekNo);


                var query = from cm in context.ChannelMessages
                            join csm in context.ChannelSubscriberMessages on cm.ID equals csm.ChannelMessageID
                            join cs in context.ChannelSubscribers on csm.ChannelSubscriberID equals cs.ID
                            where cm.ChannelID == channelID && cs.SubscriberID == subscriberID && (csm.DeliveredOn.HasValue || csm.SMSDeliveredOn.HasValue)
                           && cm.ScheduledOn >= startDate.Value && cm.ScheduledOn <= endDate.Value
                            select new
                            {
                                ID = cm.ID,
                                Subject = cm.Subject,
                                SentOn = cm.ScheduledOn,
                                TimeZoneID = csm.TimeZoneID
                            };

                return query.ToList().Select(x => new ChannelMessageData
                {
                    ID = x.ID,
                    Subject = x.Subject,
                    SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.SentOn, x.TimeZoneID),
                    TimeZoneID = x.TimeZoneID,
                    ChannelID = channelID
                }).ToList();
            }
        }




        public static List<ChannelMessageData> GetMessageReport(long channelID, DateTime? startDate, DateTime? endDate)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var query = from cm in context.ChannelMessages
                            join c in context.Channels on cm.ChannelID equals c.ID
                            select new
                            {
                                cm.ChannelID,
                                cm.Subject,
                                Celebrity = c.Firstname + " " + c.Lastname,
                                cm.Message,
                                cm.ScheduledOn
                            };


                if (channelID != 0)
                    query = query.Where(q => q.ChannelID == channelID);


                if (startDate.HasValue)
                    query = query.Where(q => q.ScheduledOn > startDate.Value);

                //uery = query.Where(q => q.ScheduledOn >= startDate.Value.ToUniversalTime());

                if (endDate.HasValue)
                    query = query.Where(q => q.ScheduledOn <= endDate.Value);

                return query.OrderByDescending(o => o.ScheduledOn).ToList().Select(x => new ChannelMessageData
                {
                    ChannelName = x.Celebrity,
                    Subject = x.Subject,
                    Message = x.Message,
                    ScheduledOn = x.ScheduledOn.ToLocalTime()
                }).ToList(); ;
            }
        }

        public static void Approve(ChannelMessage message, List<long> mediaIDList, List<ChannelMessageSubscriberData> subscriberIDList)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                //message.StatusID = (short)ChannelMessageStatus.Approved;
                // message.SMSStatusID = (short)ChannelMessageStatus.Approved;
                if (message.SendBySMS)
                {
                    message.SMSStatusID = (short)SMSStatus.Approved;
                }
                else
                {
                    message.SMSStatusID = (short)SMSStatus.NA;
                }

                if (message.SendByEmail)
                {
                    message.StatusID = (short)ChannelMessageStatus.Approved;
                }
                else
                {
                    message.StatusID = (short)ChannelMessageStatus.NA;
                }


                Save(message, mediaIDList, subscriberIDList, context);

                context.SaveChanges();
            }
        }

        public static void Save(ChannelMessage message, List<long> mediaIDList, List<ChannelMessageSubscriberData> subscriberIDList)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                if (message.ID == 0 || message.StatusID == (short)ChannelMessageStatus.Approved || message.SMSStatusID == (short)SMSStatus.Approved)
                {
                    if (message.SendBySMS)
                    {
                        message.SMSStatusID = (short)SMSStatus.Created;
                    }
                    else
                    {
                        message.SMSStatusID = (short)SMSStatus.NA;
                    }

                    if (message.SendByEmail)
                    {
                        message.StatusID = (short)ChannelMessageStatus.Created;
                    }
                    else
                    {
                        message.StatusID = (short)ChannelMessageStatus.NA;
                    }
                }
                else
                {
                    if (message.SendBySMS)
                    {
                        message.SMSStatusID = (short)SMSStatus.Created;
                    }
                    else
                    {
                        message.SMSStatusID = (short)SMSStatus.NA;
                    }

                    if (message.SendByEmail)
                    {
                        message.StatusID = (short)ChannelMessageStatus.Created;
                    }
                    else
                    {
                        message.StatusID = (short)ChannelMessageStatus.NA;
                    }
                }



                Save(message, mediaIDList, subscriberIDList, context);

                context.SaveChanges();
            }
        }

        public static void SaveMobile(ChannelMessage message, List<long> mediaIDList, List<ChannelMessageSubscriberData> subscriberIDList)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                if (message.ID == 0 || message.StatusID == (short)ChannelMessageStatus.Approved || message.SMSStatusID == (short)SMSStatus.Approved)
                {
                    if (message.SendBySMS)
                    {
                        message.SMSStatusID = (short)SMSStatus.Created;
                    }
                    else
                    {
                        message.SMSStatusID = (short)SMSStatus.NA;
                    }

                    if (message.SendByEmail)
                    {
                        message.StatusID = (short)ChannelMessageStatus.Created;
                    }
                    else
                    {
                        message.StatusID = (short)ChannelMessageStatus.NA;
                    }
                    // message.StatusID = (short)ChannelMessageStatus.Created;
                    // message.SMSStatusID = (short)SMSStatus.Created;
                }
                else
                {
                    if (message.SendBySMS)
                    {
                        message.SMSStatusID = (short)SMSStatus.Created;
                    }
                    else
                    {
                        message.SMSStatusID = (short)SMSStatus.NA;
                    }

                    if (message.SendByEmail)
                    {
                        message.StatusID = (short)ChannelMessageStatus.Created;
                    }
                    else
                    {
                        message.StatusID = (short)ChannelMessageStatus.NA;
                    }
                }

                context.Entry(message).State = message.ID == 0 ? EntityState.Added : EntityState.Modified;

                message.TimeZoneID = context.Channels.Where(x => x.ID == message.ChannelID).Select(x => x.TimeZoneID).FirstOrDefault();
                //if (message.ID == 0)
                //{
                //    message.TimeZoneID = context.Channels.Where(x => x.ID == message.ChannelID).Select(x => x.TimeZoneID).FirstOrDefault();
                //}


                // message.ScheduledOn = UtilityManager.GetUTC(message.ScheduledOn, message.TimeZoneID);

                MobileSave(message, mediaIDList, subscriberIDList, context);

                context.SaveChanges();
            }
        }


        public static void ApproveMobile(ChannelMessage message, List<long> mediaIDList, List<ChannelMessageSubscriberData> subscriberIDList)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                if (message.SendBySMS)
                {
                    message.SMSStatusID = (short)SMSStatus.Approved;
                }
                else
                {
                    message.SMSStatusID = (short)SMSStatus.NA;
                }

                if (message.SendByEmail)
                {
                    message.StatusID = (short)ChannelMessageStatus.Approved;
                }
                else
                {
                    message.StatusID = (short)ChannelMessageStatus.NA;
                }

                // message.StatusID = (short)ChannelMessageStatus.Approved;
                // message.SMSStatusID = (short)SMSStatus.Approved;
                context.Entry(message).State = message.ID == 0 ? EntityState.Added : EntityState.Modified;
                message.TimeZoneID = context.Channels.Where(x => x.ID == message.ChannelID).Select(x => x.TimeZoneID).FirstOrDefault();
                //if (message.ID == 0)
                //{
                //    message.TimeZoneID = context.Channels.Where(x => x.ID == message.ChannelID).Select(x => x.TimeZoneID).FirstOrDefault();
                //}

                // message.ScheduledOn = UtilityManager.GetUTC(message.ScheduledOn, message.TimeZoneID);

                MobileSave(message, mediaIDList, subscriberIDList, context);

                context.SaveChanges();
            }
        }

        public static void MobileSave(ChannelMessage message, List<long> mediaIDList, List<ChannelMessageSubscriberData> subscriberIDList, PrviiEntities context)
        {
            context.Entry(message).State = message.ID == 0 ? EntityState.Added : EntityState.Modified;

            //if (message.ID == 0)
            //{
            //    message.TimeZoneID = context.Channels.Where(x => x.ID == message.ChannelID).Select(x => x.TimeZoneID).FirstOrDefault();
            //}

            message.ScheduledOn = message.IsScheduled ? UtilityManager.GetUTC(message.ScheduledOn, message.TimeZoneID) : DateTime.UtcNow;

            var mediaExistingIDList = context.ChannelMessageAttachments.Where(x => x.ChannelMessageID == message.ID);

            var itemsToRemove = new List<ChannelMessageAttachment>();

            foreach (var item in mediaExistingIDList)
            {
                if (!mediaIDList.Contains(item.ChannelMediaID))
                    itemsToRemove.Add(item);
            }

            //remove items
            itemsToRemove.ForEach(x => context.ChannelMessageAttachments.Remove(x));

            //add new items
            foreach (var item in mediaIDList)
            {
                if (!mediaExistingIDList.Any(x => x.ChannelMediaID == item))
                {
                    message.ChannelMessageAttachments.Add(new ChannelMessageAttachment { ChannelMediaID = item, IsActive = true });
                }
            }

            ////add specific subscribers
            //var subscribersList = context.ChannelSubscriberMessages.Where(x => x.ChannelMessageID == message.ID).ToList();

            //var subscriberToRemove = new List<ChannelSubscriberMessage>();

            //foreach (var item in subscribersList)
            //{
            //    if (!subscriberIDList.Contains(item.ChannelSubscriberID))
            //        subscriberToRemove.Add(item);
            //}

            //subscriberToRemove.ForEach(x => context.ChannelSubscriberMessages.Remove(x));

            ////add any new
            //foreach (var item in subscriberIDList)
            //{
            //    if (!subscribersList.Any(x => x.ChannelSubscriberID == item))
            //    {
            //        message.ChannelSubscriberMessages.Add(new ChannelSubscriberMessage { ChannelSubscriberID = item });
            //    }
            //}

            //add specific subscribers
            var subscribersList = context.ChannelSubscriberMessages.Where(x => x.ChannelMessageID == message.ID).ToList();

            var subscriberToRemove = new List<ChannelSubscriberMessage>();

            foreach (var item in subscribersList)
            {
                if (!subscriberIDList.Any(a => a.SubsciberID == item.ChannelSubscriberID))
                    subscriberToRemove.Add(item);
            }

            subscriberToRemove.ForEach(x => context.ChannelSubscriberMessages.Remove(x));

            //add any new
            foreach (var item in subscriberIDList)
            {
                if (!subscribersList.Any(x => x.ChannelSubscriberID == item.SubsciberID))
                {
                    message.ChannelSubscriberMessages.Add(new ChannelSubscriberMessage { ChannelSubscriberID = item.SubsciberID, SMSStatus = Convert.ToInt16(item.IsSMSSend), EmailStatus = Convert.ToInt16(item.IsEmailSend) });
                }
            }
        }


        public static void Save(ChannelMessage message, List<long> mediaIDList, List<ChannelMessageSubscriberData> subscriberIDList, PrviiEntities context)
        {
            context.Entry(message).State = message.ID == 0 ? EntityState.Added : EntityState.Modified;

            if (message.ID == 0)
            {
                message.TimeZoneID = context.Channels.Where(x => x.ID == message.ChannelID).Select(x => x.TimeZoneID).FirstOrDefault();
            }

            message.ScheduledOn = message.IsScheduled ? UtilityManager.GetUTC(message.ScheduledOn, message.TimeZoneID) : DateTime.UtcNow;

            var mediaExistingIDList = context.ChannelMessageAttachments.Where(x => x.ChannelMessageID == message.ID);

            var itemsToRemove = new List<ChannelMessageAttachment>();

            foreach (var item in mediaExistingIDList)
            {
                if (!mediaIDList.Contains(item.ChannelMediaID))
                    itemsToRemove.Add(item);
            }

            //remove items
            itemsToRemove.ForEach(x => context.ChannelMessageAttachments.Remove(x));

            //add new items
            foreach (var item in mediaIDList)
            {
                if (!mediaExistingIDList.Any(x => x.ChannelMediaID == item))
                {
                    message.ChannelMessageAttachments.Add(new ChannelMessageAttachment { ChannelMediaID = item, IsActive = true });
                }
            }

            //add specific subscribers
            var subscribersList = context.ChannelSubscriberMessages.Where(x => x.ChannelMessageID == message.ID).ToList();

            var subscriberToRemove = new List<ChannelSubscriberMessage>();

            foreach (var item in subscribersList)
            {
                if (!subscriberIDList.Any(a => a.SubsciberID == item.ChannelSubscriberID))
                    subscriberToRemove.Add(item);
            }

            subscriberToRemove.ForEach(x => context.ChannelSubscriberMessages.Remove(x));

            //add any new
            foreach (var item in subscriberIDList)
            {
                if (!subscribersList.Any(x => x.ChannelSubscriberID == item.SubsciberID))
                {
                    message.ChannelSubscriberMessages.Add(new ChannelSubscriberMessage { ChannelSubscriberID = item.SubsciberID, SMSStatus = Convert.ToInt16(item.IsSMSSend), EmailStatus = Convert.ToInt16(item.IsEmailSend) });
                }
            }
        }

        public static bool Create(ChannelMessage channelMessage)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                context.Entry(channelMessage).State = channelMessage.ID == 0 ? EntityState.Added : EntityState.Modified;
                context.SaveChanges();
            }

            return true;
        }

        public static bool ChannelAttachmentAdd(ChannelMessageAttachment channelMessageAttachment)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                channelMessageAttachment.ChannelMessageID = channelMessageAttachment.ChannelMessageID;
                context.Entry(channelMessageAttachment).State = channelMessageAttachment.ID == 0 ? EntityState.Added : EntityState.Modified;
                context.SaveChanges();
            }

            return true;
        }

        public static bool DeleteChannelMessageMedia(long MessageID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                ChannelMessageAttachment AttachmentDelete = new ChannelMessageAttachment { ChannelMessageID = MessageID };
                if (AttachmentDelete.ID != 0)
                {
                    context.Entry(AttachmentDelete).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
            return true;
        }

        public static ChannelMessageAttachment GetAttachmentByID(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMessageAttachments.FirstOrDefault(x => x.ID == id);
            }
        }

        public static List<ChannelMessage> GetChannelMessagesByChannelId(long channelId)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMessages.Where(c => c.ChannelID == channelId).ToList();
            }
        }

        public static List<ChannelMessageData> GetChannelSubscriberAllMessageByChannelID(long channelID, long UserID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var CurrentMessage = ChannelMessageManager.GetBySubscriberID(channelID, UserID);
                var SubscribDate = context.ChannelSubscribers.Where(s => s.ChannelID == channelID && s.SubscriberID == UserID && s.IsActive == true).Select(c => c.EFD).FirstOrDefault();
                var pastMsgList = context.ChannelMessages.Where(w => w.StatusID == (short)ChannelMessageStatus.Sent && w.ChannelID == channelID && w.ScheduledOn <= SubscribDate
                    && w.SendToAll == true).ToList().Select(x => new ChannelMessageData
                    {
                        ID = x.ID,
                        Subject = x.Subject,                      
                        SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.ScheduledOn, UserProfileManager.GetSubscriberTimeZone(UserID)),
                        TimeZoneID = UserProfileManager.GetSubscriberTimeZone(UserID),//x.TimeZoneID,
                        ChannelID = x.ChannelID
                    }).ToList();


                return CurrentMessage.Union(pastMsgList).ToList();


            }
        }

        public static List<ChannelMessageData> GetChannelSubscriberPastMessageByChannelID(long channelID, long UserID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {

                var SubscribDate = context.ChannelSubscribers.Where(s => s.ChannelID == channelID && s.SubscriberID == UserID && s.IsActive == true).Select(c => c.EFD).FirstOrDefault();
                return context.ChannelMessages.Where(w => w.StatusID == (short)ChannelMessageStatus.Sent && w.ChannelID == channelID && w.ScheduledOn <= SubscribDate
                    && w.SendToAll == true).ToList().Select(x => new ChannelMessageData
                    {
                        ID = x.ID,
                        ChannelID = x.ChannelID,
                        SendToAll = x.SendToAll,
                        Subject = x.Subject,
                        Message = x.Message,
                        IsSMS = x.SendBySMS,
                        IsEmail = x.SendByEmail,
                        IsScheduled = x.IsScheduled,
                        ScheduledOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.ScheduledOn, UserProfileManager.GetSubscriberTimeZone(UserID)),
                        TimeZoneID = UserProfileManager.GetSubscriberTimeZone(UserID),//x.TimeZoneID,
                        Status = x.StatusID,
                        StatusText = ((ChannelMessageStatus)x.StatusID).ToString(),
                        SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.ScheduledOn, UserProfileManager.GetSubscriberTimeZone(UserID))
                    }).ToList();

            }
        }
        public static List<ChannelMessageData> GetChannelSubscriberAllMessageByChannelIDSubscriberSearch(long channelID, long subscriberID, string Fromdate, string ToDate, string SearchText)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var SubscribDate = context.ChannelSubscribers.Where(s => s.ChannelID == channelID && s.SubscriberID == subscriberID && s.IsActive == true).Select(c => c.EFD).FirstOrDefault();

                if ((Fromdate == null) && (ToDate == null) && (SearchText == null))
                {


                    var pastMsgList = context.ChannelMessages.Where(w => w.StatusID == (short)ChannelMessageStatus.Sent && w.ChannelID == channelID && w.ScheduledOn <= SubscribDate
                        && w.SendToAll == true).ToList().Select(x => new ChannelMessageData
                        {
                            ID = x.ID,
                            Subject = x.Subject,
                            SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.ScheduledOn, UserProfileManager.GetSubscriberTimeZone(subscriberID)),
                            TimeZoneID = UserProfileManager.GetSubscriberTimeZone(subscriberID),
                            ChannelID = x.ChannelID
                        }).ToList();


                    var query = from cm in context.ChannelMessages
                                join csm in context.ChannelSubscriberMessages on cm.ID equals csm.ChannelMessageID
                                join cs in context.ChannelSubscribers on csm.ChannelSubscriberID equals cs.ID
                                where cm.ChannelID == channelID && cs.SubscriberID == subscriberID && (csm.DeliveredOn.HasValue || csm.SMSDeliveredOn.HasValue)
                                select new
                                {
                                    ID = cm.ID,
                                    Subject = cm.Subject,
                                    SentOn = cm.ScheduledOn,
                                    TimeZoneID = csm.TimeZoneID
                                };

                    var queryResult1 = query.ToList().Select(x => new ChannelMessageData
                    {
                        ID = x.ID,
                        Subject = x.Subject,
                        SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.SentOn, x.TimeZoneID),
                        TimeZoneID = x.TimeZoneID,
                        ChannelID = channelID
                    }).ToList();

                    return queryResult1.Union(pastMsgList).ToList();
                   
                }
                else if ((Fromdate == null) && (ToDate == null) && (SearchText != null))
                {
                    var pastMsgList = context.ChannelMessages.Where(w => w.StatusID == (short)ChannelMessageStatus.Sent && w.ChannelID == channelID && w.ScheduledOn <= SubscribDate
                        && w.SendToAll == true && w.Subject.Contains(SearchText)).ToList().Select(x => new ChannelMessageData

                        {
                            ID = x.ID,
                            Subject = x.Subject,
                            SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.ScheduledOn, UserProfileManager.GetSubscriberTimeZone(subscriberID)),
                            TimeZoneID = UserProfileManager.GetSubscriberTimeZone(subscriberID),
                            ChannelID = x.ChannelID
                        }).ToList();


                    var query = from cm in context.ChannelMessages
                                join csm in context.ChannelSubscriberMessages on cm.ID equals csm.ChannelMessageID
                                join cs in context.ChannelSubscribers on csm.ChannelSubscriberID equals cs.ID
                                where cm.ChannelID == channelID && cs.SubscriberID == subscriberID && (csm.DeliveredOn.HasValue || csm.SMSDeliveredOn.HasValue) && cm.Subject.Contains(SearchText)
                                select new
                                {
                                    ID = cm.ID,
                                    Subject = cm.Subject,
                                    SentOn = cm.ScheduledOn,
                                    TimeZoneID = csm.TimeZoneID
                                };

                    var queryResult1 = query.ToList().Select(x => new ChannelMessageData
                    {
                        ID = x.ID,
                        Subject = x.Subject,
                        SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.SentOn, x.TimeZoneID),
                        TimeZoneID = x.TimeZoneID,
                        ChannelID = channelID
                    }).ToList();


                    return queryResult1.Union(pastMsgList).ToList();

                    
                   
                }
                else
                {
                    DateTime fromDate = Convert.ToDateTime(Fromdate);
                    DateTime toDate = Convert.ToDateTime(ToDate);


                    var pastMsgList = context.ChannelMessages.Where(w => w.StatusID == (short)ChannelMessageStatus.Sent && w.ChannelID == channelID && w.ScheduledOn <= SubscribDate
                       && w.SendToAll == true && w.Subject.Contains(SearchText) && w.ScheduledOn >= fromDate && w.ScheduledOn <= toDate).ToList().Select(x => new ChannelMessageData

                       {
                           ID = x.ID,
                           Subject = x.Subject,
                           SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.ScheduledOn, UserProfileManager.GetSubscriberTimeZone(subscriberID)),
                           TimeZoneID = UserProfileManager.GetSubscriberTimeZone(subscriberID),
                           ChannelID = x.ChannelID
                       }).ToList();


                    var query = from cm in context.ChannelMessages
                                join csm in context.ChannelSubscriberMessages on cm.ID equals csm.ChannelMessageID
                                join cs in context.ChannelSubscribers on csm.ChannelSubscriberID equals cs.ID
                                where cm.ChannelID == channelID && cs.SubscriberID == subscriberID && (csm.DeliveredOn.HasValue || csm.SMSDeliveredOn.HasValue) && cm.Subject.Contains(SearchText)
                                 && cm.ScheduledOn >= fromDate && cm.ScheduledOn <= toDate
                                select new
                                {
                                    ID = cm.ID,
                                    Subject = cm.Subject,
                                    SentOn = cm.ScheduledOn,
                                    TimeZoneID = csm.TimeZoneID
                                };

                    var queryResult1 = query.ToList().Select(x => new ChannelMessageData
                    {
                        ID = x.ID,
                        Subject = x.Subject,
                        SentOn = UtilityManager.GetZoneSpecificTimeFromUTC(x.SentOn, x.TimeZoneID),
                        TimeZoneID = x.TimeZoneID,
                        ChannelID = channelID
                    }).ToList();


                    return queryResult1.Union(pastMsgList).ToList();
                  
                }

            }
        }

        public static ChannelMessage GetChannelMessageById(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMessages.FirstOrDefault(x => x.ID == id);
            }
        }

        public static List<ChannelMessageAttachment> GetAttachmentByMessageID(long channelMessageID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMessageAttachments.Where(x => x.ChannelMessageID == channelMessageID).ToList();
            }
        }

        public static void SendMessage()
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var dtNow = DateTime.Now.ToUniversalTime();

                var messageList = context.ChannelMessages.Where(x => x.SendByEmail == true && x.ScheduledOn <= dtNow && (x.StatusID == (short)ChannelMessageStatus.Approved || x.StatusID == (short)ChannelMessageStatus.Sending)).ToList();

                foreach (var message in messageList)
                {
                    //update status to sending..
                    if (message.StatusID == (short)ChannelMessageStatus.Approved)
                    {
                        message.StatusID = (short)ChannelMessageStatus.Sending;
                        context.SaveChanges();
                    }



                    //if (message.SendToAll)
                    //{
                    //    var channelSubscriberList = (from cs in context.ChannelSubscribers
                    //                                join u in context.UserProfiles on cs.SubscriberID equals u.ID
                    //                                where cs.ChannelID == message.ChannelID && cs.IsActive
                    //                                select new {cs.ID, cs.SubscriberID, u.TimeZoneID}).ToList();

                    //    foreach (var channelSubscriber in channelSubscriberList)
                    //    {
                    //        if (context.ChannelSubscriberMessages.Any(x => x.ChannelMessageID == message.ID && x.ChannelSubscriberID == channelSubscriber.ID))
                    //            continue;

                    //        using (PrviiEntities innerContext = new PrviiEntities())
                    //        {
                    //            //send email
                    //            SendMessageEmail(innerContext, channelSubscriber.SubscriberID, message);

                    //            ChannelSubscriberMessage subscriberMessage = new ChannelSubscriberMessage
                    //            {
                    //                ChannelSubscriberID = channelSubscriber.ID,
                    //                ChannelMessageID = message.ID,
                    //                DeliveredOn = DateTime.UtcNow,
                    //                TimeZoneID = channelSubscriber.TimeZoneID
                    //            };

                    //            innerContext.Entry(subscriberMessage).State = EntityState.Added;
                    //            innerContext.SaveChanges();
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    using (PrviiEntities innerContext = new PrviiEntities())
                    {
                        var subscriberMessageList = innerContext.ChannelSubscriberMessages.Where(x => x.ChannelMessageID == message.ID && x.EmailStatus == (short)ChannelMessageStatus.EmailSent).ToList();

                        foreach (var item in subscriberMessageList)
                        {
                            if (!item.DeliveredOn.HasValue)
                            {
                                var subscriber = (from cs in innerContext.ChannelSubscribers
                                                  join u in innerContext.UserProfiles on cs.SubscriberID equals u.ID
                                                  where cs.ID == item.ChannelSubscriberID && u.DeliveryMechanisms.Contains("Email") && cs.IsActive
                                                  select new { u.ID, u.TimeZoneID }).FirstOrDefault();


                                if (subscriber != null)
                                {
                                    //send email
                                    SendMessageEmail(innerContext, subscriber.ID, message);
                                    item.EmailStatus = (short)ChannelMessageStatus.Sent;
                                    item.DeliveredOn = DateTime.UtcNow;
                                    item.TimeZoneID = subscriber.TimeZoneID;
                                    innerContext.SaveChanges();
                                }

                            }
                        }
                    }
                    //}

                    bool allMessageSent = false;

                    //check if all message sent
                    //if (message.SendToAll)
                    //{
                    //    var suscriberCount = context.ChannelSubscribers.Where(x => x.ChannelID == message.ChannelID).Count();
                    //    var messageSentCount = context.ChannelSubscriberMessages.Where(x => x.ChannelMessageID == message.ID && x.DeliveredOn.HasValue).Count();

                    //    allMessageSent = suscriberCount == messageSentCount;
                    //}
                    //else
                    //{
                    allMessageSent = !context.ChannelSubscriberMessages.Any(x => x.ChannelMessageID == message.ID && x.EmailStatus == (short)ChannelMessageStatus.EmailSent && !x.DeliveredOn.HasValue);
                    //}

                    if (allMessageSent)
                    {
                        message.StatusID = (short)ChannelMessageStatus.Sent;
                        message.SentOn = DateTime.UtcNow;
                        context.SaveChanges();
                    }

                }
            }
        }

        private static void SendMessageEmail(PrviiEntities innerContext, long subscriberID, ChannelMessage message)
        {
            //send email
            var user = innerContext.UserProfiles.FirstOrDefault(x => x.ID == subscriberID);
            var channel = innerContext.Channels.FirstOrDefault(x => x.ID == message.ChannelID);
            var emailBody = EMailTemplateManager.GetMessageEmailBody(user, message, channel);
            EMailManager emailer = new EMailManager();
            emailer.MailTo = user.Email;
            emailer.Subject = message.Subject;
            emailer.MailBody = emailBody;
            emailer.SendMail();
        }

        public static IList GetChannelSubscriberMessages(long channelId, long SubscriberId)
        {
            using (PrviiEntities context = new PrviiEntities())
            {


                return context.ChannelSubscriberMessages.Include("ChannelMessages").Where(c => c.ChannelSubscriberID == SubscriberId && c.ChannelMessage.ChannelID == channelId)
                    .Select(s => new
                    {
                        ID = s.ChannelMessage.ID,
                        ChannelID = s.ChannelMessage.ChannelID,
                        Subject = s.ChannelMessage.Subject,
                        Message = s.ChannelMessage.Message,
                        DeliveredOn = s.DeliveredOn,
                        IsEmail = s.ChannelMessage.SendByEmail,
                        IsSMS = s.ChannelMessage.SendBySMS
                    }).ToList();

            }
        }


        public static void UpdateMessageStatusByID(long messageID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                //context.Database.ExecuteSqlCommand("Update ChannelMessages Set Status = {0} where ID = {1}", 
                //    ChannelMessageStatus.Approved, messageID);
                ChannelMessage channelMessage = context.ChannelMessages.Where(w => w.ID == messageID).FirstOrDefault();
                channelMessage.StatusID = Convert.ToInt16(ChannelMessageStatus.Approved);
                context.Entry(channelMessage).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void DeleteChannelMessageByID(long messageID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                //context.Database.ExecuteSqlCommand("Delete from ChannelMessages where ID = {0}", messageID);
                ChannelMessage channelMessage = context.ChannelMessages.Where(w => w.ID == messageID).FirstOrDefault();
                context.ChannelMessages.Attach(channelMessage);
                context.Entry(channelMessage).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void DeleteAttachmentByID(long ID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                ChannelMessageAttachment channelMessageAttachment = context.ChannelMessageAttachments
                    .Where(w => w.ID == ID).FirstOrDefault();
                context.ChannelMessageAttachments.Attach(channelMessageAttachment);
                context.Entry(channelMessageAttachment).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public static string GetStringIDs(string[] Ids)
        {
            string Vales = "";
            if (Ids != null)
            {
                foreach (var id in Ids)
                {
                    Vales = Vales + id + ',';
                }
                if (Vales.Length > 0)
                    Vales = Vales.Substring(0, Vales.Length - 1);
            }

            return Vales;
        }

        public static void SendSMS()
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var dtNow = DateTime.Now.ToUniversalTime();

                var messageList = context.ChannelMessages.Where(x => x.SendBySMS == true && x.ScheduledOn <= dtNow && (x.SMSStatusID == (short)SMSStatus.Approved || x.SMSStatusID == (short)SMSStatus.Sending)).ToList();

                foreach (var message in messageList)
                {
                    //update status to sms sending..
                    if (message.SMSStatusID == (short)SMSStatus.Approved)
                    {
                        message.SMSStatusID = (short)SMSStatus.Sending;
                        context.SaveChanges();
                    }


                    using (PrviiEntities innerContext = new PrviiEntities())
                    {
                        var subscriberMessageList = innerContext.ChannelSubscriberMessages.Where(x => x.ChannelMessageID == message.ID && x.SMSStatus == (short)SMSStatus.SMSSent).ToList();

                        foreach (var item in subscriberMessageList)
                        {
                            if (!item.SMSDeliveredOn.HasValue)
                            {
                                var subscriber = (from cs in innerContext.ChannelSubscribers
                                                  join u in innerContext.UserProfiles on cs.SubscriberID equals u.ID
                                                  where cs.ID == item.ChannelSubscriberID && u.DeliveryMechanisms.Contains("Text") && cs.IsActive
                                                  select new { u.ID, u.TimeZoneID }).FirstOrDefault();

                                if (subscriber != null)
                                {
                                    //send sms
                                    long channelsubscribermessageid = item.ID;
                                    try
                                    {
                                        SendSMS(innerContext, subscriber.ID, message, channelsubscribermessageid);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                  

                                    item.SMSDeliveredOn = DateTime.UtcNow;
                                    item.TimeZoneID = subscriber.TimeZoneID;
                                    innerContext.SaveChanges();
                                }

                            }
                        }
                    }


                    bool allMessageSent = false;


                    allMessageSent = !context.ChannelSubscriberMessages.Any(x => x.ChannelMessageID == message.ID && x.SMSStatus == (short)SMSStatus.SMSSent && !x.SMSDeliveredOn.HasValue);


                    if (allMessageSent)
                    {
                        message.SMSStatusID = (short)SMSStatus.Sent;
                        //message.SentOn = DateTime.UtcNow;
                        context.SaveChanges();
                    }

                }
            }
        }

        private static void SendSMS(PrviiEntities innerContext, long subscriberID, ChannelMessage message, long channelSubcriberMessageId)
        {
            //send email
            var user = innerContext.UserProfiles.FirstOrDefault(x => x.ID == subscriberID);
            var channel = innerContext.Channels.FirstOrDefault(x => x.ID == message.ChannelID);
            var messageBody = EMailTemplateManager.GetMessageSMSBody(user, message, channel);
            string[] MediaUrls = EMailTemplateManager.GetMMSMessageMedia(user, message, channel);
            var deviceInfo = innerContext.UserDeviceInfoes.FirstOrDefault(x => x.SubscriberId == subscriberID && x.IsActive == true);

            if (deviceInfo != null)
            {
               
                if (deviceInfo.DeviceToken != null)
                {
                    try
                    {
                        PushMessage pushMessage = new PushMessage();
                        pushMessage.DeviceToken = deviceInfo.DeviceToken;
                        pushMessage.DeviceName = deviceInfo.DevicePaltform;
                        pushMessage.Message = messageBody;
                        pushMessage.ChannelSubscriberMessageId = channelSubcriberMessageId;

                        string RequestJson = JsonConvert.SerializeObject(pushMessage);

                        string webURL = WebConfigurationManager.AppSettings["WebApiUrl"].ToString();
                        string URL = webURL + "/api/PushSharp/SendPushNotification";
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                        request.Method = "POST";
                        request.ContentType = "application/json";
                       // request.ContentLength = 1000;
                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream());
                        requestWriter.Write(RequestJson);
                        requestWriter.Close();
                        HttpWebResponse objResponse1 = (HttpWebResponse)request.GetResponse();
                        using (StreamReader sr = new StreamReader(objResponse1.GetResponseStream()))
                        {
                            string strResponse = sr.ReadToEnd();
                            objResponse1.Close();
                            sr.Close();
                        }

                       // PushSharpNotification.SendPushNotification(deviceInfo.DevicePaltform, deviceInfo.DeviceToken, message.Message);
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                    
                }
            }
           // SMSManager smsManager = new SMSManager();


           // if (MediaUrls.Length > 0)
           //     smsManager.MedialUrls = MediaUrls;
           // else
           //     smsManager.MedialUrls = null;

           //smsManager.TONUMBER = user.Mobile;
           // //smsManager.SMSBody = message.Message;
           // smsManager.SMSBody = messageBody;
           // //smsManager.SMSBody = "Test Message Twilio from Nitish Tiwari. Please forward.";
           // smsManager.ChannelSubscriberMessageId = channelSubcriberMessageId;


           // if (smsManager.MedialUrls == null)
           // {
           //    // smsManager.SendSMS();
           //     PushSharpNotification.SendPushNotification(deviceInfo.DevicePaltform, deviceInfo.DeviceToken, message.Message);
           // }
           // else
           // {
           //    // smsManager.SendMMS();
           //     PushSharpNotification.SendPushNotification(deviceInfo.DevicePaltform, deviceInfo.DeviceToken, message.Message);
           // }



        }


        public static IList GetSubscriberNotDeleverList(long MessageID, string Type)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                Dictionary<long, string> UsersList = new Dictionary<long, string>();

                var subscribersList = context.ChannelSubscriberMessages.Where(x => x.ChannelMessageID == MessageID).ToList();
                if (subscribersList.Count > 0)
                {
                    foreach (var item in subscribersList)
                    {
                        var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item.ChannelSubscriberID));
                        var deliveryMethodList = deliveryMethod.Split(',').ToList();
                        if (!deliveryMethodList.Contains(Type))
                        {
                            UsersList.Add(Convert.ToInt64(item.ChannelSubscriberID), UserProfileManager.GetSubscriberNameByChannelSubscriberID(Convert.ToInt64(item.ChannelSubscriberID)));
                        }
                    }
                }

                return UsersList.Select(s => new { ID = s.Key, Name = s.Value }).ToList();
            }
        }


        public static void GetSMSStatus()
        {
            using (PrviiEntities innerContext = new PrviiEntities())
            {
                var subscriberMessageList = innerContext.ChannelSubscriberMessages.Where(x => x.SMSStatus == 1 || x.SMSStatus == 2).ToList();

                foreach (var item in subscriberMessageList)
                {
                    if (item.MessageSID != null)
                        GetSMSStatus(item.ID, item.MessageSID);
                }
            }


        }
        private static void GetSMSStatus(long Id, string MessageId)
        {
            SMSManager smsManager = new SMSManager();
            smsManager.GetSMSStatusfromTwilio(Id, MessageId);
        }


        public static void SendPushNotification()
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var dtNow = DateTime.Now.ToUniversalTime();

                var messageList = context.ChannelMessages.Where(x => x.SendBySMS == true && x.ScheduledOn <= dtNow && (x.SMSStatusID == (short)SMSStatus.Approved || x.SMSStatusID == (short)SMSStatus.Sending)).ToList();

                foreach (var message in messageList)
                {
                    //update status to sms sending..
                    if (message.SMSStatusID == (short)SMSStatus.Approved)
                    {
                        message.SMSStatusID = (short)SMSStatus.Sending;
                        context.SaveChanges();
                    }


                    using (PrviiEntities innerContext = new PrviiEntities())
                    {
                        var subscriberMessageList = innerContext.ChannelSubscriberMessages.Where(x => x.ChannelMessageID == message.ID && x.SMSStatus == (short)SMSStatus.SMSSent).ToList();

                        foreach (var item in subscriberMessageList)
                        {
                            if (!item.SMSDeliveredOn.HasValue)
                            {
                                var subscriber = (from cs in innerContext.ChannelSubscribers
                                                  join u in innerContext.UserProfiles on cs.SubscriberID equals u.ID
                                                  where cs.ID == item.ChannelSubscriberID && u.DeliveryMechanisms.Contains("Text") && cs.IsActive
                                                  select new { u.ID, u.TimeZoneID }).FirstOrDefault();

                                if (subscriber != null)
                                {
                                    //send sms
                                    long channelsubscribermessageid = item.ID;
                                    SendSMS(innerContext, subscriber.ID, message, channelsubscribermessageid);

                                    item.SMSDeliveredOn = DateTime.UtcNow;
                                    item.TimeZoneID = subscriber.TimeZoneID;
                                    innerContext.SaveChanges();
                                }

                            }
                        }
                    }


                    bool allMessageSent = false;


                    allMessageSent = !context.ChannelSubscriberMessages.Any(x => x.ChannelMessageID == message.ID && x.SMSStatus == (short)SMSStatus.SMSSent && !x.SMSDeliveredOn.HasValue);


                    if (allMessageSent)
                    {
                        message.SMSStatusID = (short)SMSStatus.Sent;
                        //message.SentOn = DateTime.UtcNow;
                        context.SaveChanges();
                    }

                }
            }
        }
    }


}
