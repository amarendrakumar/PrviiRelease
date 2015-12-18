using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prvii.Entities;
using System.Data.Entity;
using System.Collections;
using Prvii.Entities.DataEntities;
using System.Data.Entity.SqlServer;

namespace Prvii.Business
{
    public class ChannelSubscribersManager
    {
        public static List<UserProfileData> GetSubscriberStatistics(long channelID, DateTime? startDate, DateTime? endDate, bool compareWithETD, bool activeOnly)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var query = from cs in context.ChannelSubscribers
                            join s in context.UserProfiles on cs.SubscriberID equals s.ID
                            select new { s.ID, s.Firstname, s.Lastname, s.Mobile, s.Email, s.ZipCode, cs.EFD, cs.ETD, cs.ChannelID, cs.IsActive };

                if (activeOnly)
                    query = query.Where(x => x.IsActive == true);

                if (channelID != 0)
                    query = query.Where(x => x.ChannelID == channelID);

                if (compareWithETD)
                {
                    query = query.Where(x => x.IsActive == true);

                    if (endDate.HasValue)
                        query = query.Where(x => x.EFD <= endDate.Value);
                    //query = query.Where(x => x.EFD.Day <= endDate.Value.Day && x.EFD.Month <= endDate.Value.Month && x.EFD.Year <= endDate.Value.Year);

                    var query1 = from cs in context.ChannelSubscribers
                                 join s in context.UserProfiles on cs.SubscriberID equals s.ID
                                 select new { s.ID, s.Firstname, s.Lastname, s.Mobile, s.Email, s.ZipCode, cs.EFD, cs.ETD, cs.ChannelID, cs.IsActive };
                    query1 = query1.Where(x => x.IsActive == false);

                    if (channelID != 0)
                        query1 = query1.Where(x => x.ChannelID == channelID);

                    if (endDate.HasValue)
                        query1 = query1.Where(x => x.EFD <= endDate.Value);
                    //query1 = query1.Where(x => x.EFD.Day <= endDate.Value.Day && x.EFD.Month <= endDate.Value.Month && x.EFD.Year <= endDate.Value.Year);

                    if (endDate.HasValue)
                        query1 = query1.Where(x => (x.ETD.Year == endDate.Value.Year ? (x.ETD.Month == endDate.Value.Month ? x.ETD.Day >= x.EFD.Day : x.ETD.Month >= endDate.Value.Month) : x.ETD.Year > endDate.Value.Year));


                    query = query.Concat(query1);

                    //query = query.Where(x => (x.ETD.Day == DateTime.MinValue.Day && x.ETD.Month == DateTime.MinValue.Month && x.ETD.Year == DateTime.MinValue.Year)
                    //    || (endDate.Value.Day < x.ETD.Day && endDate.Value.Month < x.ETD.Month && endDate.Value.Year < x.ETD.Year));
                }
                else
                {
                    if (startDate.HasValue)
                        query = query.Where(x => x.EFD >= startDate.Value);
                    // query = query.Where(x => x.EFD.Day >= startDate.Value.Day && x.EFD.Month >= startDate.Value.Month && x.EFD.Year >= startDate.Value.Year);


                    if (endDate.HasValue)
                        query = query.Where(x => x.EFD <= endDate.Value);
                    // query = query.Where(x => x.EFD.Day <= endDate.Value.Day && x.EFD.Month <= endDate.Value.Month && x.EFD.Year <= endDate.Value.Year);
                }

                var result = query.ToList();

                return result.Select(x => new UserProfileData
                {
                    ID = x.ID,
                    Firstname = x.Firstname,
                    Lastname = x.Lastname,
                    Mobile = x.Mobile,
                    Email = x.Email,
                    ZipCode = x.ZipCode,
                    Subscription_EFD = x.EFD
                }).OrderBy(o => o.Firstname).ThenBy(o => o.Lastname).ToList();
            }
        }

        public static List<UserProfileData> GetSubscribers(long channelID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var query = from cs in context.ChannelSubscribers
                            join s in context.UserProfiles on cs.SubscriberID equals s.ID
                            where cs.ChannelID == channelID && cs.IsActive
                            select new { s.ID, s.Firstname, s.Lastname, s.Mobile, s.Email, s.ZipCode, cs.EFD, cs.ETD, cs.ChannelID, s.TimeZoneID, s.CountryID, CountryName = s.Country.Name };

                var result = query.ToList();

                return result.Select(x => new UserProfileData
                {
                    ID = x.ID,
                    Firstname = x.Firstname,
                    Lastname = x.Lastname,
                    Mobile = x.Mobile,
                    Email = x.Email,
                    ZipCode = x.ZipCode,
                    TimeZoneID = x.TimeZoneID,
                    CountryID = x.CountryID,
                    CountryName = x.CountryName
                }).OrderBy(o => o.Firstname).ThenBy(o => o.Lastname).ToList();
            }
        }


     public static List<SubscriberDownload> GetSubscribersDownload(long channelID, bool Preclude)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var queryNew = from cs in context.ChannelSubscribers
                               join up in context.UserProfiles on cs.SubscriberID equals up.ID
                               join c in context.Channels on cs.ChannelID equals c.ID
                               select new
                               {
                                   up.Firstname,
                                   up.Lastname,
                                   channelname = c.Firstname + " " + c.Lastname,
                                   c.ID,
                                   up.NickName,
                                   up.Email,
                                   up.Telephone,
                                   up.Mobile,
                                   up.Address1,
                                   up.Address2,
                                   cs.IsActive,
                                   up.TimeZoneID,
                                   up.City,
                                   up.CountryID,
                                   up.StateID,
                                   c.Preclude,
                                   up.ZipCode,
                                   CountryName = up.Country.Name,
                                   StateName = up.State.Name
                               };

                if (channelID != 0)
                    queryNew = queryNew.Where(x => x.ID == channelID);

                if (Preclude)
                    queryNew = queryNew.Where(x => x.Preclude != true);

                var result = queryNew.ToList();


                

                var groupResult = result.GroupBy(g => new
                {
                    g.Firstname,
                    g.Lastname,
                    g.channelname,
                    g.ID,
                    g.NickName,
                    g.Email,
                    g.Telephone,
                    g.Mobile,
                    g.Address1,
                    g.Address2,
                    g.IsActive,
                    g.TimeZoneID,
                    g.City,
                    g.CountryID,
                    g.StateID,
                    g.ZipCode
                }).Select(s => new
                    {
                        
                        ChannelID = s.Key.ID,
                        Firstname = s.Key.Firstname,
                        Lastname = s.Key.Lastname,
                        Nickname = s.Key.NickName,
                        Mobile = s.Key.Mobile,
                        Telephone = s.Key.Telephone,
                        Email = s.Key.Email,
                        Address1 = s.Key.Address1,
                        Address2 = s.Key.Address2,
                        ZipCode = s.Key.ZipCode,
                        TimeZoneID = s.Key.TimeZoneID,
                        CountryID = s.Key.CountryID,
                        CountryName = s.Max(m => m.CountryName),
                        IsActive = s.Key.IsActive,
                        City = s.Key.City,
                        StateName = s.Max(m=> m.StateName),
                        StateID = s.Key.StateID,
                        Celebrity = s.Key.channelname
                    });

                return result.Select(x => new SubscriberDownload
                {                   
                    Firstname = x.Firstname,
                    Lastname = x.Lastname,
                    Nickname = x.NickName,
                    Mobile = x.Mobile,
                    Telephone = x.Telephone,
                    Email = x.Email,
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    ZipCode = x.ZipCode,
                    TimeZoneID = x.TimeZoneID,
                    CountryID = x.CountryID,
                    CountryName = x.CountryName,
                   IsActive = x.IsActive,
                    City = x.City,
                    StateName = x.StateName,
                    StateID = x.StateID,
                    Celebrity = x.channelname
                }).OrderBy(o => o.Celebrity).ThenBy(o => o.Firstname).ThenBy(o => o.Lastname).ToList();
            }
        }


        public static void SaveChannelSubscriber(ChannelSubscriber channelSubscriber)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                context.ChannelSubscribers.Attach(channelSubscriber);
                context.Entry(channelSubscriber).State = EntityState.Added;
                context.SaveChanges();
            }
        }

        public static long GetCelebritySubscriberActivity(long channelID, string periodType, string period, long periodValue)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                long result = context.ProcGetSubscriberStatisticsActivity(channelID, periodType, period, periodValue).FirstOrDefault().Value;
                return result;
            }
        }


        public static IList GetCelevrityRevenueReport(long channelID, DateTime? startDate, DateTime? endDate)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                
                var query = from cs in context.ChannelSubscribers
                            join s in context.UserProfiles on cs.SubscriberID equals s.ID
                            select new { cs.ID, SubscriberName =s.Firstname+" "+ s.Lastname, cs.ChannelID, 
                                CahannelName = cs.Channel.Firstname+" "+cs.Channel.Lastname,
                                cs.Channel.BillingCycleID, cs.Channel.Price, cs.EFD, cs.ETD, cs.SubscriberID, cs.IsActive };
               

                if (channelID != 0)
                    query = query.Where(x => x.ChannelID == channelID);

                if (startDate.HasValue)
                    query = query.Where(x => x.EFD >= startDate.Value);


                var query1 = query.Where(x => x.IsActive == true && x.EFD <= endDate.Value);
                query = query.Where(x => x.IsActive == false && x.ETD <= endDate.Value);


                query = query.Concat(query1);

                var Listresult = query.ToList().Select(x => new
                {
                    Id = x.ID,
                    SubscriberName = x.SubscriberName,
                    NoofPayment = 1 + GetNoofPayment(x.BillingCycleID, x.IsActive, x.EFD, x.ETD),
                    JoingDate = x.EFD,
                    InActiveDate = x.IsActive == false ? x.ETD.ToString() : "",
                    IsActive = x.IsActive,
                    Total = (1 + GetNoofPayment(x.BillingCycleID, x.IsActive, x.EFD, x.ETD)) * x.Price,
                    CelebrityProfit = ((1 + GetNoofPayment(x.BillingCycleID, x.IsActive, x.EFD, x.ETD)) * x.Price) * 60 / 100,
                    AgentProfit = ((1 + GetNoofPayment(x.BillingCycleID, x.IsActive, x.EFD, x.ETD)) * x.Price) * 10 / 100,
                    Communication = ((1 + GetNoofPayment(x.BillingCycleID, x.IsActive, x.EFD, x.ETD)) * x.Price) * 10 / 100,
                    Prvii = ((1 + GetNoofPayment(x.BillingCycleID, x.IsActive, x.EFD, x.ETD)) * x.Price) * 20 / 100
                }).ToList();


                return Listresult.Select(x => new
                {
                    Sno = x.Id,
                    SubscriberName = x.SubscriberName,
                    NoofPayment = x.NoofPayment,
                    JoingDate = x.JoingDate,
                    InActiveDate = x.InActiveDate,
                    IsActive = x.IsActive,
                    Total = x.Total,
                    CelebrityProfit = x.CelebrityProfit,
                    AgentProfit = x.AgentProfit,
                    ComProfit = x.Communication,
                    PrviiProfit = x.Prvii
                }).ToList();              
                
            }
        }
        public static int GetNoofPayment(int BillingCycleID, bool IsActive, DateTime EFD, DateTime ETD)
        {
            int noofpayment = 0;
            if (BillingCycleID == 1)
            {
                noofpayment = IsActive == false ? Convert.ToInt32(EFD.Subtract(ETD).Days / (365.25 / 12)) : Convert.ToInt32(EFD.Subtract(DateTime.Now).Days / (365.25 / 12));
            }
            else if (BillingCycleID == 2)
            {
                noofpayment = IsActive == false ? Convert.ToInt32(EFD.Subtract(ETD).Days / (365.25 / 12)) : Convert.ToInt32(EFD.Subtract(DateTime.Now).Days / (365.25 / 12));
                noofpayment = noofpayment % 3;
            }
            else if (BillingCycleID == 3)
            {
                noofpayment = IsActive == false ? Convert.ToInt32(EFD.Subtract(ETD).Days / (365.25 )) : Convert.ToInt32(EFD.Subtract(DateTime.Now).Days / (365.25 ));
            }
            else if (BillingCycleID == 4)
            {
                noofpayment = IsActive == false ? Convert.ToInt32(EFD.Subtract(ETD).Days) : Convert.ToInt32(EFD.Subtract(DateTime.Now).Days);
            }
            else if (BillingCycleID == 5)
            {
                noofpayment = IsActive == false ? Convert.ToInt32(EFD.Subtract(ETD).Days) : Convert.ToInt32(EFD.Subtract(DateTime.Now).Days);
                noofpayment = noofpayment % 7;
            }
            return noofpayment;
        }


        #region "Commented Code"

        //public static Int32 GetActiveNetSubscribers(long channelID, string periodType, int period)
        //{
        //    using (PrviiEntities context = new PrviiEntities())
        //    {
        //        int fromDate = 0;
        //        int toDate = 0;

        //        if (periodType == "Quarter")
        //        {
        //            if (period == 1)
        //            {
        //                fromDate = 1;
        //                toDate = 3;
        //            }
        //            if (period == 2)
        //            {
        //                fromDate = 4;
        //                toDate = 6;
        //            }
        //            if (period == 3)
        //            {
        //                fromDate = 7;
        //                toDate = 9;
        //            }
        //            if (period == 4)
        //            {
        //                fromDate = 10;
        //                toDate = 12;
        //            }

        //        }
        //        else 
        //        {
        //            fromDate = period;
        //            toDate = period;
        //        }

        //        var query = from u in context.UserProfiles
        //                    join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                    from cs in gcs.DefaultIfEmpty()
        //                    where cs.ChannelID == channelID && u.IsActive == true
        //                    && cs.EFD <= DateTime.Now && cs.ETD >= DateTime.Now
        //                    && cs.EFD.Month >= fromDate && cs.EFD.Month <= toDate
        //                    select u;

        //        return Convert.ToInt32(query.Count());



        //    }
        //}

        //public static Int32 GetSubscriberWhoJoinInPeriod(long channelID, string periodType, string Joinfromdate, string JoinTodate)
        //{
        //    using (PrviiEntities context = new PrviiEntities())
        //    {
        //        int fromDate = 0;
        //        int toDate = 0;
        //        var today = DateTime.Today;
        //        var month = new DateTime(today.Year, today.Month, 1);
        //        var first = month.AddMonths(-1);
        //        var last = month.AddDays(-1);
        //        if (periodType == "Last Month")
        //        {                 
        //            fromDate = last.Month;
        //            toDate = last.Month;

        //        }
        //        else if (periodType == "This Month")
        //        {
        //            fromDate = today.Month;
        //            toDate = today.Month;

        //        }

        //        if (periodType == "Today")
        //        {

        //            var query = from u in context.UserProfiles
        //                        join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                        from cs in gcs.DefaultIfEmpty()
        //                        where cs.ChannelID == channelID && u.IsActive == true
        //                        && cs.EFD == DateTime.Now 
        //                        select u;

        //            return Convert.ToInt32(query.Count());
        //        }
        //        else if (periodType == "Period")
        //        {

        //            var query = from u in context.UserProfiles
        //                        join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                        from cs in gcs.DefaultIfEmpty()
        //                        where cs.ChannelID == channelID && u.IsActive == true
        //                        && cs.EFD >= Convert.ToDateTime(Joinfromdate) && cs.EFD <= Convert.ToDateTime(JoinTodate)
        //                        select u;

        //            return Convert.ToInt32(query.Count());
        //        }
        //        else if (periodType == "Last Week")
        //        {
        //            DayOfWeek weekStart = DayOfWeek.Monday;
        //            DateTime startingDate = DateTime.Today;

        //            while (startingDate.DayOfWeek != weekStart)
        //                startingDate = startingDate.AddDays(-1);

        //            DateTime previousWeekStart = startingDate.AddDays(-7);

        //            var query = from u in context.UserProfiles
        //                        join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                        from cs in gcs.DefaultIfEmpty()
        //                        where cs.ChannelID == channelID && u.IsActive == true
        //                        && cs.EFD >= previousWeekStart && cs.EFD <= startingDate
        //                        select u;

        //            return Convert.ToInt32(query.Count());
        //        }
        //        else if (periodType == "FromToDate")
        //        {
        //            DateTime joinfromdate = Convert.ToDateTime(Joinfromdate);
        //            DateTime joinTodate = Convert.ToDateTime(JoinTodate);
        //            var query = from u in context.UserProfiles
        //                        join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                        from cs in gcs.DefaultIfEmpty()
        //                        where cs.ChannelID == channelID && u.IsActive == true
        //                        && cs.EFD >= joinfromdate && cs.EFD <= joinTodate
        //                        select u;

        //            return Convert.ToInt32(query.Count());
        //        }
        //        else
        //        {
        //            var query = from u in context.UserProfiles
        //                        join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                        from cs in gcs.DefaultIfEmpty()
        //                        where cs.ChannelID == channelID && u.IsActive == true
        //                        && cs.EFD.Month >= fromDate && cs.EFD.Month <= toDate
        //                        select u;

        //            return Convert.ToInt32(query.Count());
        //        }

        //    }
        //}

        //public static Int32 GetSubscriberWhoLostInPeriod(long channelID, string periodType, string Joinfromdate, string JoinTodate)
        //{
        //    using (PrviiEntities context = new PrviiEntities())
        //    {
        //        int fromDate = 0;
        //        int toDate = 0;
        //        var today = DateTime.Today;
        //        var month = new DateTime(today.Year, today.Month, 1);
        //        var first = month.AddMonths(-1);
        //        var last = month.AddDays(-1);
        //        if (periodType == "Last Month")
        //        {
        //            fromDate = last.Month;
        //            toDate = last.Month;

        //        }
        //        else if (periodType == "This Month")
        //        {
        //            fromDate = today.Month;
        //            toDate = today.Month;

        //        }

        //        if (periodType == "Today")
        //        {

        //            var query = from u in context.UserProfiles
        //                        join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                        from cs in gcs.DefaultIfEmpty()
        //                        where cs.ChannelID == channelID 
        //                        && cs.ETD == DateTime.Now
        //                        select u;

        //            return Convert.ToInt32(query.Count());
        //        }
        //        else if (periodType == "Period")
        //        {

        //            var query = from u in context.UserProfiles
        //                        join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                        from cs in gcs.DefaultIfEmpty()
        //                        where cs.ChannelID == channelID
        //                        && cs.ETD >= Convert.ToDateTime(Joinfromdate) && cs.ETD <= Convert.ToDateTime(JoinTodate)
        //                        select u;

        //            return Convert.ToInt32(query.Count());
        //        }
        //        else if (periodType == "Last Week")
        //        {
        //            DayOfWeek weekStart = DayOfWeek.Monday;
        //            DateTime startingDate = DateTime.Today;

        //            while (startingDate.DayOfWeek != weekStart)
        //                startingDate = startingDate.AddDays(-1);

        //            DateTime previousWeekStart = startingDate.AddDays(-7);

        //            var query = from u in context.UserProfiles
        //                        join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                        from cs in gcs.DefaultIfEmpty()
        //                        where cs.ChannelID == channelID
        //                        && cs.ETD >= previousWeekStart && cs.ETD <= startingDate
        //                        select u;

        //            return Convert.ToInt32(query.Count());
        //        }
        //        else if (periodType == "FromToDate")
        //        {
        //            DateTime joinfromdate = Convert.ToDateTime(Joinfromdate);
        //            DateTime joinTodate = Convert.ToDateTime(JoinTodate);
        //            var query = from u in context.UserProfiles
        //                        join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                        from cs in gcs.DefaultIfEmpty()
        //                        where cs.ChannelID == channelID
        //                        && cs.ETD >= joinfromdate && cs.ETD <= joinTodate
        //                        select u;

        //            return Convert.ToInt32(query.Count());
        //        }
        //        else
        //        {
        //            var query = from u in context.UserProfiles
        //                        join cs in context.ChannelSubscribers on u.ID equals cs.SubscriberID into gcs
        //                        from cs in gcs.DefaultIfEmpty()
        //                        where cs.ChannelID == channelID
        //                        && cs.ETD.Month >= fromDate && cs.ETD.Month <= toDate
        //                        select u;

        //            return Convert.ToInt32(query.Count());
        //        }

        //    }
        //}

        #endregion

        public List<CelebrityRevenueReport> GetCelebrityRevenue(string ChannelId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                using (PrviiEntities context = new PrviiEntities())
                {
                    object[] param = new object[3];
                    param[0] = ChannelId;
                    param[1] = startDate;
                    param[2] = endDate;

                    List<CelebrityRevenueReport> report = context.Database.SqlQuery<CelebrityRevenueReport>(
                    "exec ProcGetCelebrityRevenueReport {0} ,{1},{2}", param).ToList<CelebrityRevenueReport>();
                    return report;
                }
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }

    public static class ExtensionMethods
    {
        public static int GetQuarter(this DateTime dt)
        {
            return (dt.Month - 1) / 3 + 1;
        }
    }

    
}