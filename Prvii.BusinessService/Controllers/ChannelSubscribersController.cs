using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections;
using Prvii.Business;
using Prvii.Entities;
using Prvii.BusinessService.Models;
using System.IO;
using System.Net.Http.Headers;
using Prvii.Entities.DataEntities;
using Prvii.Entities.Enumerations;

namespace Prvii.BusinessService.Controllers
{
    public class ChannelSubscribersController : ApiController
    {
        [HttpPost]
        public IEnumerable<UserProfileDTO> GetChannelSubscriberList(ChannelDTO channel)
        {
            if (UserProfileManager.IsAuthenticateUser(channel.UserID))
            {
                var result = ChannelSubscribersManager.GetSubscribers(channel.ID);

                return result.Select(up => new UserProfileDTO
                {
                    ID = up.ID,
                    Firstname = up.Firstname,
                    Lastname = up.Lastname,
                    Telephone = up.Telephone,
                    Mobile = up.Mobile,
                    Email = up.Email,
                    ZipCode = up.ZipCode,
                    Country = up.CountryName,
                    ChannelID = up.CountryID,
                    TimeZoneID = up.TimeZoneID
                }).ToList();
            }
            else
            {
                return new List<UserProfileDTO>();
            }

        }

        [HttpPost]
        public int GetActiveSubscribers(ChannelDTO channel)
        {
            return ChannelSubscribersManager.GetSubscribers(channel.ID).Count();
        }

      
        [HttpPost]
        public long GetCelebritySubscriberActivity(ChannelSubscriberStatisticDOT css)
        {
            var result = ChannelSubscribersManager.GetCelebritySubscriberActivity(css.channelID, css.periodType, css.periods, css.periodValue);
            return result;
        }

        //[HttpPost]
        //public int GetSubscriberWhoJoinInPeriod(ChannelSubscriberStatisticDOT channelSubscriberStatistic)
        //{
        //    var result = ChannelSubscribersManager.GetSubscriberWhoJoinInPeriod(channelSubscriberStatistic.channelID, channelSubscriberStatistic.periodType, channelSubscriberStatistic.FromDate, channelSubscriberStatistic.ToDate);
        //    return result;
        //}

        //[HttpPost]
        //public int GetSubscriberWhoLostInPeriod(ChannelSubscriberStatisticDOT channelSubscriberStatistic)
        //{
        //    var result = ChannelSubscribersManager.GetSubscriberWhoLostInPeriod(channelSubscriberStatistic.channelID, channelSubscriberStatistic.periodType, channelSubscriberStatistic.FromDate, channelSubscriberStatistic.ToDate);
        //    return result;
        //}
        

    }
}
