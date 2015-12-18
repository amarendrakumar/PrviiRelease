using Prvii.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Prvii.Entities.Enumerations;
using System.Collections;
using Prvii.Entities.DataEntities;

namespace Prvii.Business
{
    public class ChannelManager
    {
        public static string GetTimeZone(long id)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.Channels.Where(x => x.ID == id).Select(x => x.TimeZoneID).FirstOrDefault();
            }
        }

        public static long GetByNames(string firstName, string lastName)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.Channels.Where(x => x.Firstname.ToLower() == firstName.ToLower() && x.Lastname.ToLower() == lastName.ToLower()).Select(x => x.ID).FirstOrDefault();
            }
        }

        public static bool CanViewMedia(UserProfileData userSession, long mediaID)
        {
            if(userSession.UserRole == Role.Administrator)
                return true;

            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                if (userSession.UserRole == Role.Group)
                {
                    return context.ChannelMedias.Any(x => x.ID == mediaID && x.Channel.GroupChannels.Any(y => y.GroupID == userSession.GroupID));
                }
                else if (userSession.UserRole == Role.Celebrity)
                {
                    return context.ChannelMedias.Any(x => x.ID == mediaID && x.ChannelID == userSession.ChannelID);
                }
                else if (userSession.UserRole == Role.Subscriber)
                {
                    return context.ChannelMedias.Any(x => x.ID == mediaID && x.Channel.ChannelSubscribers.Any(y => y.SubscriberID == userSession.ID && y.IsActive));
                }
            }

            return false;
        }

        public static IList GetForDropdown()
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.Channels.Select(x => new { x.ID, Name = x.Firstname + " " + x.Lastname }).OrderBy(o=>o.Name).ToList();
            }
        }
        public static IList GetForDropdownByChannel(long ChannelId)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.Channels.Where(w => w.ID == ChannelId).Select(x => new { x.ID, Name = x.Firstname + " " + x.Lastname }).OrderBy(o => o.Name).ToList();
            }
        }

        public static IList GetForDropdown(long GroupID)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.GroupChannels.Include("Channels").Where(g => g.GroupID == GroupID).Select(x => new { x.Channel.ID, Name = x.Channel.Firstname + " " + x.Channel.Lastname }).OrderBy(o => o.Name).ToList();
            }
        }


        public static List<ChannelData> GetForGroupUser(long userID)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                var query = from c in context.Channels
                            join gc in context.GroupChannels on c.ID equals gc.ChannelID
                            join u in context.UserProfiles on gc.GroupID equals u.GroupID
                            where u.ID == userID && c.IsActive == true
                            select new ChannelData
                            {
                                ID = c.ID,
                                Name = c.Firstname + " " + c.Lastname,
                                Email = c.Email,
                                Phone = c.Phone,
                                Price = c.Price,
                                BillingCycleID = c.BillingCycleID,
                                NoOfBillingPeriod = c.NoOfBillingPeriod,
                                StatusID = c.StatusID,
                                IsActive = c.IsActive
                            };

                return query.ToList();
            }
        }

        public static List<ChannelData> GetForSubscriberUser(long userID)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                var channelList = context.Channels.Where(c => c.IsActive == true).Select(x => new ChannelData
                {
                    ID = x.ID,
                    Name = x.Firstname + " " + x.Lastname,
                    Email = x.Email,
                    Phone = x.Phone,
                    Price = x.Price,
                    BillingCycleID = x.BillingCycleID,
                    NoOfBillingPeriod = x.NoOfBillingPeriod,
                    StatusID = x.StatusID,
                    IsActive = x.IsActive
                }).ToList();

                var subscribedChannels = context.ChannelSubscribers.Where(x => x.SubscriberID == userID && x.IsActive).Select(x => x.ChannelID).ToList();

                channelList.ForEach(x => x.IsSubscribed = subscribedChannels.Contains(x.ID));

                return channelList;
            }
        }

        public static bool isSubscribed(long channelID, long userID)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.ChannelSubscribers.Any(x => x.ChannelID == channelID && x.SubscriberID == userID && x.IsActive);
            }
        }

        public static bool Exists(string firstName, string lastName, long id)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.Channels.Any(x => x.Firstname.ToLower() == firstName.ToLower() && x.Lastname.ToLower() == lastName.ToLower() && x.ID != id);
            }
        }

        public static IList GetSubscribers(long id)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                var query = from cs in context.ChannelSubscribers
                            join u in context.UserProfiles on cs.SubscriberID equals u.ID
                            where cs.ChannelID == id && cs.IsActive
                            select new
                            {
                                ID = cs.ID,
                                Name = u.Firstname + " " + u.Lastname,
                                DeliverType = u.DeliveryMechanisms
                            };

                return query.OrderBy(o=>o.Name).ToList();
            }
        }


        public static IList GetSubscribers1(long channelID, List<long> ids)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                var query = from cs in context.ChannelSubscribers
                            join u in context.UserProfiles on cs.SubscriberID equals u.ID
                            where cs.ChannelID == channelID && cs.IsActive
                            select new
                            {
                                ID = cs.ID,
                                Name = u.Firstname + " " + u.Lastname
                            };

                return query.Where(w => ids.Contains(w.ID)).OrderBy(o => o.Name).ToList();
            }
        }

        public static List<ChannelMediaData> GetMediaList(long channelID, ChannelMediaType mediaType)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.ChannelMedias.Where(e => e.ChannelID == channelID && e.MediaTypeID == (short)mediaType).Select(x => new ChannelMediaData { ID= x.ID, Name= x.Name }).ToList();
            }
        }

        public static List<ChannelMedia> GetMediaListWithContent(long channelID, ChannelMediaType mediaType)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.ChannelMedias.Where(e => e.ChannelID == channelID && e.MediaTypeID == (short)mediaType).ToList();
            }
        }

        public static void DeleteMedia(long mediaID)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                var media = context.ChannelMedias.FirstOrDefault(x => x.ID == mediaID);
                context.ChannelMedias.Remove(media);
                context.SaveChanges();
            }
        }

        public static void DeleteMedia(long channelID, ChannelMediaType mediaType)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                var mediaList = context.ChannelMedias.Where(x => x.ChannelID == channelID && x.MediaTypeID == (short)mediaType).ToList();
                mediaList.ForEach(x => context.ChannelMedias.Remove(x));
                context.SaveChanges();
            }
        }

        public static List<ChannelData> GetCelebrityListUser()
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Channels.Where(a => a.IsActive == true).Select(x => new ChannelData
                {
                    ID = x.ID,
                    Name = x.Firstname + " " + x.Lastname,
                    Email = x.Email,
                    Phone = x.Phone,
                    Price = x.Price,
                    BillingCycleID = x.BillingCycleID,
                    NoOfBillingPeriod = x.NoOfBillingPeriod,
                    StatusID = x.StatusID,
                    IsActive = x.IsActive
                }).OrderBy(o => o.Name).ToList();
            }
        }

        public static List<ChannelData> GetCelebrityList()
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Channels.Select(x => new ChannelData
                {
                    ID = x.ID,
                    Name = x.Firstname + " " + x.Lastname,
                    Email = x.Email,
                    Phone = x.Phone,
                    Price = x.Price,
                    BillingCycleID = x.BillingCycleID,
                    NoOfBillingPeriod = x.NoOfBillingPeriod,
                    StatusID = x.StatusID,
                    IsActive = x.IsActive
                }).OrderBy(o => o.Name).ToList();
            }
        }

        public static void Save(Channel channel, List<ChannelMedia> mediaList)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                context.Entry(channel).State = channel.ID == 0 ? EntityState.Added : EntityState.Modified;
                foreach (var item in mediaList)
                {
                    item.Channel = channel;
                    item.ChannelID = channel.ID;
                    context.Entry(item).State = item.ID == 0 ? EntityState.Added : EntityState.Modified;
                }

                context.SaveChanges();
            }
        }



        public static void UpdateChannelPrice(Int64 channelID, string priceManagement)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                if (channelID > 0)
                {
                    Channel channel = GetByID(channelID);
                    if (channel != null)
                    {
                        channel.PriceManagement = priceManagement;
                        context.Entry(channel).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
        }

        public static List<PrviiAccountMaster> GetPrviiAccountMaster(long parentId)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var prviiAccount = context.PrviiAccountMasters.Where(p => p.ParentID == parentId && p.IsActive == true).OrderBy(o => o.LevelID).ToList();
                return prviiAccount;
            }
        }


        public static List<Celebrity> GetAllChannelList(long userId)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var ChannelList = CelebrityList();
                var channelId = context.ChannelSubscribers.Where(cs => cs.SubscriberID == userId).Select(s => s.ChannelID).FirstOrDefault();
                return ChannelList.Where(ch => ch.ID != channelId).ToList();
            }
        }

        public static Channel GetChannel(long userId)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var channelId = context.ChannelSubscribers.Where(cs => cs.SubscriberID == userId).Select(s => s.ChannelID).FirstOrDefault();
                var channel = context.Channels.Where(c => c.ID == channelId).FirstOrDefault();
                return channel;
            }
        }

        public static Channel GetByID(long id)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.Channels.FirstOrDefault(e => e.ID == id);
            }
        }

        public static string GetCelebrityName(long id)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {

                return context.Channels.Where(W => W.ID == id).Select(s => s.Firstname + "  " + s.Lastname ).FirstOrDefault();

            }
        }


        public static List<Celebrity> SubscribChannel(long userId)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var query = from c in context.Channels
                            join cs in context.ChannelSubscribers on c.ID equals cs.ChannelID into gcs
                            from cs in gcs.DefaultIfEmpty()
                            where cs.SubscriberID == userId && c.IsActive == true
                            && cs.EFD <= DateTime.Now // && cs.ETD >= DateTime.Now
                            select new Celebrity
                            {
                                ID = c.ID,
                                Name = c.Firstname + " " + c.Lastname,
                                Firstname = c.Firstname,
                                Lastname = c.Lastname,
                                Email = c.Email,
                                Details = string.Empty,
                                IsActive = c.IsActive,
                                Phone = c.Phone,
                                Price = c.Price,
                                Status = c.StatusID
                            };
                return query.ToList<Celebrity>();
            }
        }
        public static void ChannelUnsubscribe(long channelID, long UserID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                //find payment profileid
                // var shoppingCart = context.ShoppingCarts.FirstOrDefault(x => x.UserID == UserID && x.ShoppingCartItems.Any(y => y.ChannelID == channelID));

                var shoppingCartItems = context.ShoppingCartItems.FirstOrDefault(x => x.ShoppingCart.UserID == UserID && x.ChannelID == channelID && x.PaymentStatusID == (short)PaymentStatus.Payment_Completed);

                if (shoppingCartItems.PaymentProfileID == null)
                    throw new Exception("Payment Profile ID not found.");

                //cancel at paypal
                new PayPalManager().CancelSubscription(shoppingCartItems.PaymentProfileID);

                //set shop cart payment status
                shoppingCartItems.PaymentStatusID = (short)PaymentStatus.Awaiting_Cancel_Confirmation;

                //save changes to db
                context.SaveChanges();
                ChannelUnsubscribePayPal(shoppingCartItems.ShoppingCartID, shoppingCartItems.PaymentProfileID);
            }
        }


        public static void ChannelUnsubscribePayPal(long shoppingCartID, string PaymentProfileID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                //find payment profileid
                var shoppingCartItems = context.ShoppingCartItems.FirstOrDefault(x => x.ShoppingCartID == shoppingCartID && x.PaymentProfileID == PaymentProfileID);

                //set shop cart payment status
                shoppingCartItems.PaymentStatusID = (short)PaymentStatus.Payment_Cancelled;
                var shoppingCart = context.ShoppingCarts.Where(s => s.ID == shoppingCartItems.ShoppingCartID).FirstOrDefault();

                //var shoppingCartItems = context.ShoppingCartItems.Where(s => s.ShoppingCartID == shoppingCart.ID).ToList();

                ////unsubscribe user from channel
                //foreach (var item in shoppingCartItems)
                //{
                //    var subscription = context.ChannelSubscribers.FirstOrDefault(x => x.ChannelID == item.ChannelID && x.SubscriberID == shoppingCart.UserID);
                //    subscription.ETD = DateTime.Today;
                //    subscription.IsActive = false;
                //}

                var subscription = context.ChannelSubscribers.FirstOrDefault(x => x.ChannelID == shoppingCartItems.ChannelID && x.SubscriberID == shoppingCart.UserID && x.IsActive == true);
                subscription.ETD = DateTime.UtcNow;
                subscription.IsActive = false;

                //save changes to db
                context.SaveChanges();
            }
        } 

      
        public static List<Celebrity> GetCelebrityListBySubscriberID(long userId)
        {
            using (PrviiEntities context = new PrviiEntities())
            {

                var query = from c in context.Channels
                            join cs in context.ChannelSubscribers on c.ID equals cs.ChannelID into gcs
                            from cs in gcs.DefaultIfEmpty()
                            where cs.SubscriberID == userId && c.IsActive == true
                            select new Celebrity
                            {
                                ID = c.ID,
                                Name = c.Firstname + " " + c.Lastname,
                                Firstname = c.Firstname,
                                Lastname = c.Lastname,
                                Email = c.Email,
                                Details = string.Empty,
                                IsActive = c.IsActive,
                                Phone = c.Phone,
                                Price = c.Price,
                                Status = c.StatusID
                            };

                return query.ToList<Celebrity>();

            }
        }


        #region Old Code 

     
        public static byte[] GetImage(long channelID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Image).Select(x => x.Content).FirstOrDefault();
            }
        }

        public static byte[] GetLogo(long channelID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Image).Select(x => x.Content).FirstOrDefault();
            }
        }

        public static byte[] GetLeftImage(long channelID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Left_Image).Select(x => x.Content).FirstOrDefault();
            }
        }

        public static byte[] GetRightImage(long channelID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Right_Image).Select(x => x.Content).FirstOrDefault();
            }
        }

        public static byte[] GetCenterImage(long channelID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Background_Image).Select(x => x.Content).FirstOrDefault();
            }
        }

        public static byte[] GetImage(long channelID, string Position)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                if (Position == "Logo")
                    return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Image).Select(x => x.Content).FirstOrDefault();
                else if (Position == "CenterImage")
                    return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Background_Image).Select(x => x.Content).FirstOrDefault();
                else if (Position == "RightImage")
                    return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Right_Image).Select(x => x.Content).FirstOrDefault();
                else if (Position == "LeftImage")
                    return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Left_Image).Select(x => x.Content).FirstOrDefault();
                else if (Position == "Description")
                    return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Center_Image).Select(x => x.Content).FirstOrDefault();
                else
                    return context.ChannelMedias.Where(x => x.ID == channelID && x.MediaTypeID == (short)ChannelMediaType.Image).Select(x => x.Content).FirstOrDefault();
            }
        }


        #endregion
        public static bool IsChannelSubscrib(long ChannelID, long UserId)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                bool Status = false;
                var ChannelSubscrib = context.ChannelSubscribers.Where(c => c.ChannelID == ChannelID && c.SubscriberID == UserId);
                if (ChannelSubscrib != null)
                {
                    if (ChannelSubscrib.Count() > 0)
                    {
                        Status = true;
                    }
                }

                return Status;
            }
        }

        public static List<ChannelMedia> GetMediaByChannelID(long channelID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMedias.Where(x => x.ChannelID == channelID).ToList();
            }
        }

        public static ChannelMedia GetMedia(long channelID, ChannelMediaType mediaTypeID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMedias.FirstOrDefault(x => x.ChannelID == channelID && x.MediaTypeID == (short)mediaTypeID);
            }
        }

        public static ChannelMedia GetMediaByID(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMedias.FirstOrDefault(x => x.ID == id);
            }
        }

        public static string GetMediaMimeType(long mediaID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ChannelMedias.Where(x => x.ID == mediaID).Select(x=>x.MimeType).FirstOrDefault();
            }
        }

        public static List<Celebrity> CelebrityList()
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Channels.Select(s => new Celebrity
                {
                    ID = s.ID,
                    Name = s.Firstname + " " + s.Lastname,
                    Firstname = s.Firstname,
                    Lastname = s.Lastname,
                    Phone = s.Phone,
                    Price = s.Price,
                    Email = s.Email,
                    IsActive = s.IsActive,
                    Status = s.StatusID,
                    Details = string.Empty
                }).ToList<Celebrity>();
            }
        }
    }

    public  class Celebrity
    {
        public long ID { set; get; }
        public string Name { set; get; }
        public string Firstname { set; get; }
        public string Lastname { set; get; }
        public string Phone { set; get; }
        public decimal Price { set; get; }
        public string Email { set; get; }
        public bool IsActive { set; get; }
        public short Status { set; get; }
        public string Details { set; get; }
    }
}
