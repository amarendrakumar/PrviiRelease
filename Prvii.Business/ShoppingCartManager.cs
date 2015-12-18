using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prvii.Entities;
using Prvii.Entities.Enumerations;
using System.Data.Entity;
using Newtonsoft.Json;
using Prvii.Entities.DataEntities;

namespace Prvii.Business
{
    public class ShoppingCartManager
    {
        public static Channel GetFirstItem(long cartID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Channels.FirstOrDefault(x => x.ShoppingCartItems.Any(y => y.ShoppingCartID == cartID));
            }
        }
        public static List<Channel> GetItem(long cartID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Channels.Where(x => x.ShoppingCartItems.Any(y => y.ShoppingCartID == cartID)).ToList();
            }
        }
        public static ShoppingCart GetCart(string sessionID, long? userID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var query = context.ShoppingCarts.Where(q => q.Status == false);

                if (userID.HasValue)
                    query = query.Where(q => q.UserID == userID.Value);
                else
                    query = query.Where(q => q.SessionID == sessionID);

                return query.FirstOrDefault();
            }
        }
        public static ShoppingCart GetCart(long userID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ShoppingCarts.Where(q => q.UserID == userID && q.Status == false).FirstOrDefault();
            }
        }

        public static decimal GetCartTotalPrice(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ShoppingCarts.Where(x => x.ID == id).Select(f => f.TotalPrice).FirstOrDefault();
            }
        }

        public static IList GetCartItems(long shoppingCartID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var query = from sci in context.ShoppingCartItems
                            join c in context.Channels on sci.ChannelID equals c.ID
                            where sci.ShoppingCartID == shoppingCartID
                            select new
                            {
                                sci.ShoppingCartID,
                                sci.ID,
                                ChannelName = c.Firstname + " " + c.Lastname,
                                sci.Price,
                                c.Firstname,
                                c.Lastname,
                                sci.ChannelID
                            };

                return query.ToList();
            }
        }

        public static void AddItem(ShoppingCart shoppingCart, long channelID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var itemPrice = context.Channels.Where(x => x.ID == channelID).Select(f => f.Price).FirstOrDefault();

                var cartItem = new ShoppingCartItem { ChannelID = channelID, Price = itemPrice, CreatedOn = DateTime.UtcNow };
                shoppingCart.ShoppingCartItems.Add(cartItem);

                shoppingCart.TotalPrice = itemPrice;
                context.Entry(shoppingCart).State =  System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        public static void AddItem(long id, long channelID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var itemExists = context.ShoppingCartItems.Any(x => x.ShoppingCartID == id && x.ChannelID == channelID);

                if (itemExists)
                    return;

                var itemPrice = context.Channels.Where(x => x.ID == channelID).Select(f => f.Price).FirstOrDefault();

                var cartItem = new ShoppingCartItem { ChannelID = channelID, Price = itemPrice, ShoppingCartID = id, CreatedOn = DateTime.UtcNow };
                context.Entry(cartItem).State = System.Data.Entity.EntityState.Added;

                var cart = context.ShoppingCarts.FirstOrDefault(x => x.ID == id);
                cart.TotalPrice = cart.TotalPrice + itemPrice;

                context.SaveChanges();
            }
        }

        public static void RemoveItems(long cartID, List<long> itemList)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var cart = context.ShoppingCarts.FirstOrDefault(x => x.ID == cartID);

                foreach(var id in itemList)
                {
                    var item = context.ShoppingCartItems.FirstOrDefault(x => x.ID == id);

                    cart.TotalPrice = cart.TotalPrice - item.Price;
                    context.ShoppingCartItems.Remove(item);
                }

                if (cart.TotalPrice == 0)
                    context.ShoppingCarts.Remove(cart);

                context.SaveChanges();
            }
        }

        public static bool PaymentTransactionIDExists(string paymentTransactionID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ShoppingCartItems.Any(x => x.PaymentProfileID == paymentTransactionID);
            }
        }

        public static void UpdatePaymentNew(long cartID, long channelId, decimal Price, string PayerId, string paymentProfileID, short BillingPeriodID, short BillingFrequency, short TotalBillingCycles, string ProfileStatus,string ErrorCode,short PaymentStatusID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var shoppingCart = context.ShoppingCarts.Include("ShoppingCartItems").FirstOrDefault(x => x.ID == cartID);

                bool isNewSubscriber = !context.ChannelSubscribers.Any(x => x.SubscriberID == shoppingCart.UserID);

                //shoppingCart.PaymentTransactionID = transactionID;
                //shoppingCart.PaymentProfileID = paymentProfileID;
                shoppingCart.UpdatedOn = DateTime.UtcNow;
                if (PaymentStatusID == (int)PaymentStatus.Payment_Completed)
                {
                    shoppingCart.Status = true; // (int)PaymentStatus.Payment_Completed;
                }
                else
                {
                    shoppingCart.Status = false; 
                }
                //add subscriptions

                var shoppingCartItem = context.ShoppingCartItems.Where(w => w.ChannelID == channelId && w.ShoppingCartID == cartID).FirstOrDefault();
                shoppingCartItem.Price = Price;
                shoppingCartItem.UpdatedOn = DateTime.UtcNow;
                shoppingCartItem.PayerId = PayerId;
                shoppingCartItem.PaymentStatusID = PaymentStatusID;// (int)PaymentStatus.Payment_Completed;
                shoppingCartItem.PaymentProfileID = paymentProfileID;
                shoppingCartItem.BillingStartDate = DateTime.UtcNow;
                shoppingCartItem.BillingPeriodID = BillingPeriodID;
                shoppingCartItem.BillingFrequency = BillingFrequency;
                shoppingCartItem.TotalBillingCycles = TotalBillingCycles;
                //commented by Padmaja
               // shoppingCartItem.ProfileStatus = ProfileStatus;
                shoppingCartItem.ErrorCode = ErrorCode;
                context.Entry(shoppingCartItem).State = System.Data.Entity.EntityState.Modified;
               


                if (PaymentStatusID == (int)PaymentStatus.Payment_Completed)
                {
                    var subscription = new ChannelSubscriber
                    {
                        ChannelID = channelId,
                        SubscriberID = shoppingCart.UserID.Value,
                        EFD = DateTime.UtcNow,
                        ETD = DateTime.MinValue,
                        IsActive = true
                    };

                    context.Entry(subscription).State = System.Data.Entity.EntityState.Added;
                    //context.SaveChanges();
                }

                context.SaveChanges();
               

                if (PaymentStatusID == (int)PaymentStatus.Payment_Completed)
                {

                    //send email
                    var userProfile = context.UserProfiles.FirstOrDefault(x => x.ID == shoppingCart.UserID);

                    //send prvii welcome message for new subscriber
                    if (isNewSubscriber)
                    {
                        var emailBody = EMailTemplateManager.GetWelcomeEmailBody(userProfile);
                        EMailManager emailer = new EMailManager();
                        emailer.MailTo = userProfile.Email;
                        emailer.Subject = "First Welcome to Prvii Celebrity Services";
                        emailer.MailBody = emailBody;
                        emailer.SendMail();
                    }

                    //send channel specific email

                    var channel = context.Channels.FirstOrDefault(x => x.ID == channelId);
                    var emailBodyChannel = EMailTemplateManager.GetSubscriptionEmailBody(userProfile, channel);
                    EMailManager emailerChannel = new EMailManager();
                    emailerChannel.MailTo = userProfile.Email;
                    emailerChannel.Subject = "Welcome to Prvii Celebrity Services";
                    emailerChannel.MailBody = emailBodyChannel;
                    emailerChannel.SendMail();
                }
            }
        }

        public static void UpdatePaymentNewAPPle(long cartID, long channelId, decimal Price, string PayerId, string paymentProfileID, short BillingPeriodID, short BillingFrequency, short TotalBillingCycles, string ProfileStatus, string ErrorCode, short PaymentStatusID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var shoppingCart = context.ShoppingCarts.Include("ShoppingCartItems").FirstOrDefault(x => x.ID == cartID);

                bool isNewSubscriber = !context.ChannelSubscribers.Any(x => x.SubscriberID == shoppingCart.UserID);

                //shoppingCart.PaymentTransactionID = transactionID;
                //shoppingCart.PaymentProfileID = paymentProfileID;
                shoppingCart.UpdatedOn = DateTime.UtcNow;
                if (PaymentStatusID == (int)PaymentStatus.Payment_Completed)
                {
                    shoppingCart.Status = true; // (int)PaymentStatus.Payment_Completed;
                }
                else
                {
                    shoppingCart.Status = false;
                }
                //add subscriptions

                var shoppingCartItem = context.ShoppingCartItems.Where(w => w.ChannelID == channelId && w.ShoppingCartID == cartID).FirstOrDefault();
                shoppingCartItem.Price = Price;
                shoppingCartItem.UpdatedOn = DateTime.UtcNow;
                shoppingCartItem.PayerId = PayerId;
                shoppingCartItem.PaymentStatusID = PaymentStatusID;// (int)PaymentStatus.Payment_Completed;
                shoppingCartItem.PaymentProfileID = paymentProfileID;
                shoppingCartItem.BillingStartDate = DateTime.UtcNow;
                shoppingCartItem.BillingPeriodID = BillingPeriodID;
                shoppingCartItem.BillingFrequency = BillingFrequency;
                shoppingCartItem.TotalBillingCycles = TotalBillingCycles;
                //commented by Padmaja
                // shoppingCartItem.ProfileStatus = ProfileStatus;
                shoppingCartItem.ErrorCode = ErrorCode;
                context.Entry(shoppingCartItem).State = System.Data.Entity.EntityState.Modified;

                var ETDate = DateTime.MinValue;
                if (TotalBillingCycles == (short)BillingCycleType.Yearly)
                    ETDate = DateTime.UtcNow.AddYears(1);
                else if (TotalBillingCycles == (short)BillingCycleType.Monthly)
                    ETDate = DateTime.UtcNow.AddMonths(1);
                else if (TotalBillingCycles == (short)BillingCycleType.Weekly )
                    ETDate = DateTime.UtcNow.AddDays(7);
                else if (TotalBillingCycles == (short)BillingCycleType.Quarterly)
                    ETDate = DateTime.UtcNow.AddMonths(4);

                if (PaymentStatusID == (int)PaymentStatus.Payment_Completed)
                {
                    var subscription = new ChannelSubscriber
                    {
                        ChannelID = channelId,
                        SubscriberID = shoppingCart.UserID.Value,
                        EFD = DateTime.UtcNow,
                        ETD = ETDate,
                        IsActive = true
                    };

                    context.Entry(subscription).State = System.Data.Entity.EntityState.Added;
                    //context.SaveChanges();
                }

                context.SaveChanges();


                //if (PaymentStatusID == (int)PaymentStatus.Payment_Completed)
                //{

                //    //send email
                //    var userProfile = context.UserProfiles.FirstOrDefault(x => x.ID == shoppingCart.UserID);

                //    //send prvii welcome message for new subscriber
                //    if (isNewSubscriber)
                //    {
                //        var emailBody = EMailTemplateManager.GetWelcomeEmailBody(userProfile);
                //        EMailManager emailer = new EMailManager();
                //        emailer.MailTo = userProfile.Email;
                //        emailer.Subject = "First Welcome to Prvii Celebrity Services";
                //        emailer.MailBody = emailBody;
                //        emailer.SendMail();
                //    }

                //    //send channel specific email

                //    var channel = context.Channels.FirstOrDefault(x => x.ID == channelId);
                //    var emailBodyChannel = EMailTemplateManager.GetSubscriptionEmailBody(userProfile, channel);
                //    EMailManager emailerChannel = new EMailManager();
                //    emailerChannel.MailTo = userProfile.Email;
                //    emailerChannel.Subject = "Welcome to Prvii Celebrity Services";
                //    emailerChannel.MailBody = emailBodyChannel;
                //    emailerChannel.SendMail();
                //}
            }
        }

        public static void UpdatePayment(long cartID, string transactionID, string paymentProfileID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var shoppingCart = context.ShoppingCarts.Include("ShoppingCartItems").FirstOrDefault(x => x.ID == cartID);

                bool isNewSubscriber = !context.ChannelSubscribers.Any(x => x.SubscriberID == shoppingCart.UserID);

               // shoppingCart.PaymentTransactionID = transactionID;
               // shoppingCart.PaymentProfileID = paymentProfileID;
                shoppingCart.Status = true; //(int)PaymentStatus.Payment_Completed;
                //add subscriptions
                foreach(var item in shoppingCart.ShoppingCartItems)
                {
                    var subscription = new ChannelSubscriber 
                    {
                        ChannelID = item.ChannelID, 
                        SubscriberID = shoppingCart.UserID.Value, 
                        EFD = DateTime.Today, 
                        ETD = DateTime.MinValue, 
                        IsActive=true 
                    };

                    context.Entry(subscription).State = System.Data.Entity.EntityState.Added;
                }

                context.SaveChanges();

                //send email
                var userProfile = context.UserProfiles.FirstOrDefault(x => x.ID == shoppingCart.UserID);

                //send prvii welcome message for new subscriber
                if(isNewSubscriber)
                {
                    var emailBody = EMailTemplateManager.GetWelcomeEmailBody(userProfile);
                    EMailManager emailer = new EMailManager();
                    emailer.MailTo = userProfile.Email;
                    emailer.Subject = "First Welcome to Prvii Celebrity Services";
                    emailer.MailBody = emailBody;
                    emailer.SendMail();
                }

                //send channel specific email
                foreach(var item in shoppingCart.ShoppingCartItems)
                {
                    var channel = context.Channels.FirstOrDefault(x=>x.ID == item.ChannelID);
                    var emailBody = EMailTemplateManager.GetSubscriptionEmailBody(userProfile, channel);
                    EMailManager emailer = new EMailManager();
                    emailer.MailTo = userProfile.Email;
                    emailer.Subject = "Welcome to Prvii Celebrity Services";
                    emailer.MailBody = emailBody;
                    emailer.SendMail();
                }
            }
        }

        public static void PurchaseChannel(long channelID, long userID, decimal price, string transactionID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var cart = new ShoppingCart { UserID = userID, TotalPrice = price, CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now, SessionID = "Mobile - " + DateTime.Now.Ticks.ToString() };
                var cartItem = new ShoppingCartItem { ChannelID = channelID, Price = price, PaymentStatusID = (int)PaymentStatus.Payment_Completed };
                cart.ShoppingCartItems.Add(cartItem);

                context.Entry(cart).State = System.Data.Entity.EntityState.Added;

                //add subscription
                var subscription = new ChannelSubscriber { ChannelID = channelID, SubscriberID = userID, EFD = DateTime.Today, ETD = DateTime.Now.AddYears(1) };
                context.Entry(subscription).State = System.Data.Entity.EntityState.Added;

                context.SaveChanges();
            }
        }

       

        public static void UpdatePaymentStatus(long cartID, PaymentStatus paymentStatusID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var shoppingCart = context.ShoppingCarts.FirstOrDefault(x => x.ID == cartID);

                //if (shoppingCart != null)
                //{
                //    shoppingCart.PaymentStatusID = (short)paymentStatusID;
                //    context.SaveChanges();
                //}
            }
        }

        public static bool IsSubscriptionInProgress(long channelID, long userID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                // return context.ShoppingCartItems.Any(a=>a.ShoppingCart.UserID==userID && a.ChannelID==channelID && (a.PaymentStatusID==(short)PaymentStatus.Checkout_Complete || a.PaymentStatusID==(short)PaymentStatus.Awaiting_Payment_Confirmation || a.PaymentStatusID==(short)PaymentStatus.Payment_Completed));
                return context.ShoppingCarts.Any(x => x.UserID == userID && x.ShoppingCartItems.Any(y => y.ChannelID == channelID && (y.PaymentStatusID == (short)PaymentStatus.Checkout_Complete || y.PaymentStatusID == (short)PaymentStatus.Awaiting_Payment_Confirmation || y.PaymentStatusID == (short)PaymentStatus.Payment_Completed)));
            }
        }

        public static bool IsSubscribed(long channelID, long userID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                // return context.ShoppingCartItems.Any(a=>a.ShoppingCart.UserID==userID && a.ChannelID==channelID && (a.PaymentStatusID==(short)PaymentStatus.Checkout_Complete || a.PaymentStatusID==(short)PaymentStatus.Awaiting_Payment_Confirmation || a.PaymentStatusID==(short)PaymentStatus.Payment_Completed));
                return context.ShoppingCarts.Any(x => x.UserID == userID && x.ShoppingCartItems.Any(y => y.ChannelID == channelID && y.PaymentStatusID == (short)PaymentStatus.Payment_Completed));
            }
        }

        public static bool IsSubscriptionCancellationInProgress(long channelID, long userID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.ShoppingCarts.Any(x => x.UserID == userID && x.ShoppingCartItems.Any(y => y.ChannelID == channelID && y.PaymentStatusID == (short)PaymentStatus.Awaiting_Cancel_Confirmation));
            }
        }


        public static void IosUpdateTransactionDetails(long channelID, long subscriberId, string Resultjson)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                Channel channel = ChannelManager.GetByID(channelID);

                decimal GAmount = channel.Price;
                decimal NAmount = ((channel.Price * 100) / 70);
                decimal FAmount = ((channel.Price * 100) / 30);
                string priceDistribution = GetPriceDistribution(GAmount, NAmount, FAmount, channel);
                var cardItemID = context.ShoppingCartItems.Where(s => s.ShoppingCart.UserID == subscriberId).Select(r => r.ID).FirstOrDefault();

                var iosTransactionLog = new IosTransactionLog
                {                    
                    TransactionDetails=Resultjson,
                    TransactionDate=DateTime.UtcNow,
                    ShoppingCartItemsID = cardItemID

                };
                context.Entry(iosTransactionLog).State = EntityState.Added;


                RootObject objReceiptJson = JsonConvert.DeserializeObject<RootObject>(Resultjson);


                DateTime purchaseDate = Convert.ToDateTime(objReceiptJson.receipt.purchase_date.Replace("Etc/GMT", ""));
                DateTime endDate;

                    if (channel.BillingCycleID == (short)BillingCycleType.Yearly)
                    {
                        endDate = purchaseDate.AddYears(1);
                    }
                    else if (channel.BillingCycleID == (short)BillingCycleType.Monthly)
                    {
                        endDate = purchaseDate.AddMonths(1);
                    }
                    else if (channel.BillingCycleID == (short)BillingCycleType.Quarterly)
                    {
                        endDate = purchaseDate.AddMonths(4);
                    }
                    else if (channel.BillingCycleID == (short)BillingCycleType.Weekly)
                    {
                        endDate = purchaseDate.AddDays(7);
                    }
                    else
                    {
                        endDate = purchaseDate.AddDays(1);
                    }

                    var transactionReconDetail = new TransactionReconDetail
                    {
                       
                        ProfileId = "",
                        TransactionId = objReceiptJson.receipt.transaction_id,
                        Status = 3,
                        GrossAmount = GAmount,
                        NetAmount = NAmount,
                        FeeAmount = FAmount,
                        EndDate = endDate.ToString(),
                        TimeStamp = null,
                        PayerDisplayName = objReceiptJson.receipt.unique_identifier,
                        Type = 2, // IOS,
                        PriceDistribution = priceDistribution

                    };

                   // context.Entry(transactionReconDetail).State = transactionReconDetail.Id == 0 ? EntityState.Added : EntityState.Modified;
                    context.Entry(transactionReconDetail).State = EntityState.Added;
                    context.SaveChanges();
               


               
            }

        }


        /// <summary>
        /// gets price distribution applicable for this celebrity
        /// </summary>
        /// <param name="strProfileID"></param>
        public static string GetPriceDistribution(decimal decAmt, decimal decNetAmt, decimal decFeeAmt, Channel channel)
        {
            using (PrviiEntities innerContext = new PrviiEntities())
            {
                try
                {


                    long lChannelid = channel.ID;
                  string ChannelName = channel.Firstname + " " + channel.Lastname;
                  List<PriceManagement> priceManagement = JsonConvert.DeserializeObject<List<PriceManagement>>(channel.PriceManagement);

                    string PriceJson = "{\"Gross\":" + decAmt.ToString() + ",\"OperatingCosts\":" + decFeeAmt.ToString() + ",\"NetGross\":" + decNetAmt.ToString() + ",";
                    decimal decCalcAmt = 0.00M;
                    decimal deCelebrityAmtF = 0.00M;

                    decimal deCelebrityAmt = 0.00M;
                    decimal decManagerAmt = 0.00M;
                    decimal decAgentAmt = 0.00M;
                    decimal decAssociateAmt = 0.00M;
                    decimal decRefeAmt = 0.00M; ;
                    decimal decEmpAmt = 0.00M;
                    foreach (PriceManagement manage in priceManagement)
                    {
                        if (manage.AccountId == 1) //Celebrity PAyout
                        {
                            deCelebrityAmtF = (manage.Distribution) * decNetAmt / 100;

                            foreach (PriceDistribution innerManage in manage.InnerDistribution)
                            {
                                if (innerManage.AccountId == 7) //Manager / AgentPayout
                                {
                                    decManagerAmt = (deCelebrityAmtF * innerManage.Distribution) / 100;
                                    deCelebrityAmt = deCelebrityAmtF - decManagerAmt;
                                    PriceJson = PriceJson + "\"CelebrityPayout\":" + deCelebrityAmt.ToString();

                                    PriceJson = PriceJson + ",\"ManagerPayout\":" + decManagerAmt.ToString();


                                }
                            }

                            break;
                        }
                    }

                    foreach (PriceManagement manage in priceManagement)
                    {
                        if (manage.AccountId == 2) //Agent PAyout
                        {
                            decAgentAmt = ((deCelebrityAmtF) * manage.Distribution) / 100;
                            PriceJson = PriceJson + ",\"AgentPayout\":" + decAgentAmt.ToString();
                        }
                    }

                    foreach (PriceManagement manage in priceManagement)
                    {
                        if (manage.AccountId == 3) //Associate PAyout
                        {
                            decAssociateAmt = ((deCelebrityAmtF) * manage.Distribution) / 100;
                            PriceJson = PriceJson + ",\"AssociatePayout\":" + decAssociateAmt.ToString();
                        }
                    }

                    foreach (PriceManagement manage in priceManagement)
                    {
                        if (manage.AccountId == 4) //ReferrerPayout PAyout
                        {
                            decRefeAmt = ((deCelebrityAmtF) * manage.Distribution) / 100;
                            PriceJson = PriceJson + ",\"ReferrerPayout\":" + decRefeAmt.ToString();
                        }
                    }

                    foreach (PriceManagement manage in priceManagement)
                    {
                        if (manage.AccountId == 5) //Associate PAyout
                        {
                            decEmpAmt = ((deCelebrityAmtF) * manage.Distribution) / 100;
                            PriceJson = PriceJson + ",\"EmployeePayout\":" + decEmpAmt.ToString();
                        }
                    }
                    foreach (PriceManagement manage in priceManagement)
                    {
                        if (manage.AccountId == 6) //DirectorPool PAyout
                        {
                            decEmpAmt = (deCelebrityAmtF * manage.Distribution) / 100;
                            PriceJson = PriceJson + ",\"DirectorPool\":" + decEmpAmt.ToString();
                        }
                    }

                    PriceJson = PriceJson + "}";
                    return PriceJson;
                }
                catch (Exception ex)
                {
                    return "";
                    throw;
                }
            }
        }
    }


    public class Receipt
    {
        public string original_purchase_date_pst { get; set; }
        public string purchase_date_ms { get; set; }
        public string unique_identifier { get; set; }
        public string original_transaction_id { get; set; }
        public string bvrs { get; set; }
        public string transaction_id { get; set; }
        public string quantity { get; set; }
        public string unique_vendor_identifier { get; set; }
        public string item_id { get; set; }
        public string product_id { get; set; }
        public string purchase_date { get; set; }
        public string original_purchase_date { get; set; }
        public string purchase_date_pst { get; set; }
        public string bid { get; set; }
        public string original_purchase_date_ms { get; set; }
    }

    public class RootObject
    {
        public Receipt receipt { get; set; }
        public int status { get; set; }
    }

    //public class receiptToken
    //{
    //    public DateTime original_purchase_date_pst { get; set; }
    //    public string purchase_date_ms { get; set; }
    //    public string unique_identifier { set; get; }
    //    public string original_transaction_id { set; get; }
    //    public string bvrs { get; set; }
    //    public string transaction_id { set; get; }
    //    public string quantity { get; set; }
    //    public string unique_vendor_identifier { get; set; }
    //    public string item_id { set; get; }
    //    public string product_id { get; set; }
    //    public string purchase_date { get; set; }
    //    public string original_purchase_date { get; set; }
    //    public string purchase_date_pst { get; set; }
    //    public string bid { get; set; }
    //    public string original_purchase_date_ms { get; set; }
    //}

    //public class ReceiptJson
    //{
    //    public receiptToken receipt { set; get; }
    //    public long status { get; set; } 
    //}

   // "{
//"receipt":{"original_purchase_date_pst":"2015-10-13 06:00:34 America/Los_Angeles", "purchase_date_ms":"1444741234821", 
//    "unique_identifier":"1ebdde446cbb1f675b45f07dff637a81449efcf1", 
 //   "original_transaction_id":"1000000175608695", "bvrs":"1.0.0.316", 
 //   "transaction_id":"1000000175608695", "quantity":"1", "unique_vendor_identifier":"81DF5960-DDD8-40C7-A916-41FB588CF921", 
 //   "item_id":"1040904581", "product_id":"io.cordova.PrviiMobile.NR.6", "purchase_date":"2015-10-13 13:00:34 Etc/GMT",
 //   "original_purchase_date":"2015-10-13 13:00:34 Etc/GMT", "purchase_date_pst":"2015-10-13 06:00:34 America/Los_Angeles",
 //   "bid":"io.cordova.PrviiMobile", "original_purchase_date_ms":"1444741234821"}, "status":0}"

}
