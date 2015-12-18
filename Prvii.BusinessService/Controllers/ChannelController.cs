using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Prvii.Business;
using Prvii.Entities;
using Prvii.BusinessService.Models;
using System.IO;
using System.Net.Http.Headers;
using Prvii.Entities.DataEntities;
using Prvii.Entities.Enumerations;

namespace Prvii.BusinessService.Controllers
{
    public class ChannelController : ApiController
    {
        [HttpPost]
        public IEnumerable<ChannelDTO> GetChannelList(UserProfileDTO userProfile)
        {
            if (UserProfileManager.IsAuthenticateUser(userProfile.ID))
            {
                List<ChannelData> channelList = null;

                if (userProfile.UserProfileTypeID ==(short)Role.Administrator)
                    channelList = ChannelManager.GetCelebrityList();
                else if (userProfile.UserProfileTypeID == (short)Role.Group)
                    channelList = ChannelManager.GetForGroupUser(userProfile.ID);
                else if (userProfile.UserProfileTypeID == (short)Role.Subscriber)
                    channelList = ChannelManager.GetForSubscriberUser(userProfile.ID);
                
                return channelList.Select(ch => new ChannelDTO
                {
                    ID = ch.ID,
                    Name = ch.Name,                   
                    Email = ch.Email,
                    Phone = ch.Phone,
                    Price = ch.Price,
                    StatusID = ch.StatusID,
                    IsSubscribed = ChannelManager.IsChannelSubscrib(ch.ID, userProfile.ID)
                }).ToList();

            }
            else
            {
                List<ChannelData> channelList = null;
                channelList = ChannelManager.GetCelebrityList();
                return channelList.Select(ch => new ChannelDTO
                {
                    ID = ch.ID,
                    Name = ch.Name,
                    Email = ch.Email,
                    Phone = ch.Phone,
                    Price = ch.Price,
                    StatusID = ch.StatusID,
                    IsSubscribed = false
                }).ToList();
            }
           
        }
        [HttpPost]
        public IEnumerable<ChannelDTO> GetIosChannelList(UserProfileDTO userProfile)
        {
            List<ChannelData> channelList = null;
            channelList = ChannelManager.GetCelebrityList();
            return channelList.Select(ch => new ChannelDTO
            {
                ID = ch.ID,
                Name = ch.Name,
                Email = ch.Email,
                Phone = ch.Phone,
                Price = ch.Price,
                StatusID = ch.StatusID
            }).ToList();
        }

        [HttpPost]
        public IEnumerable<ChannelDTO> GetSubscribChannelList(UserProfileDTO userProfile)
        {
            if (UserProfileManager.IsAuthenticateUser(userProfile.ID))
            {
                return ChannelManager.GetForSubscriberUser(userProfile.ID).Where(x => x.IsSubscribed).Select(ch => new ChannelDTO
                {
                    ID = ch.ID,
                    Name = ch.Name,
                    Email = ch.Email,
                    Phone = ch.Phone,
                    Price = ch.Price,
                    BillingCycleID = ch.BillingCycleID,
                    NoOfBillingPeriod = ch.NoOfBillingPeriod,
                    StatusID = ch.StatusID,
                    IsActive = ch.IsActive
                }).ToList();

            }
            else
            {
                return new List<ChannelDTO>();
            }

        }


        [HttpPost]
        public IEnumerable<ChannelDTO> GetUnSubscribChannelList(UserProfileDTO userProfile)
        {
            if (UserProfileManager.IsAuthenticateUser(userProfile.ID))
            {
                return ChannelManager.GetForSubscriberUser(userProfile.ID).Where(x => !x.IsSubscribed).Select(ch => new ChannelDTO
                {
                    ID = ch.ID,
                    Name = ch.Name,
                    Email = ch.Email,
                    Phone = ch.Phone,
                    Price = ch.Price,
                    BillingCycleID = ch.BillingCycleID,
                    NoOfBillingPeriod = ch.NoOfBillingPeriod,
                    StatusID = ch.StatusID,
                    IsActive = ch.IsActive
                }).ToList();

            }
            else
            {
                return new List<ChannelDTO>();
            }

        }

        [HttpPost]
        public IEnumerable<ChannelDTO> GetChannels(UserProfileDTO userProfile)
        {
            if (UserProfileManager.IsAuthenticateUser(userProfile.ID))
            {
                var ChannelList = ChannelManager.GetAllChannelList(userProfile.ID);
               var objchannel = new List<ChannelDTO>();
                foreach (var ch in ChannelList)
                {
                    var items = new ChannelDTO
                    {
                        ID = ch.ID,
                        Name = ch.Name,
                        Firstname = ch.Firstname,
                        Lastname = ch.Lastname,
                        Email = ch.Email,
                        Phone = ch.Phone,
                        Price = ch.Price,
                        StatusID = ch.Status,
                        IsSubscribed = ChannelManager.IsChannelSubscrib(ch.ID, userProfile.ID)
                    };
                    if (!ChannelManager.IsChannelSubscrib(ch.ID, userProfile.ID))
                    {
                        objchannel.Add(items);
                    }
                  
                   
                }
                return objchannel;              
            }
            else
            {
                return new List<ChannelDTO>();
            }
        }

        [HttpPost]
        public ChannelDTO GetChannel(UserProfileDTO userProfile)
        {
            if (UserProfileManager.IsAuthenticateUser(userProfile.ID))
            {
                var channel = ChannelManager.GetChannel(userProfile.ID);
                if (channel!=null)
                {
                    return new ChannelDTO
                    {
                        ID = channel.ID,
                        Name = channel.Firstname + " " + channel.Lastname,
                        Firstname = channel.Firstname,
                        Lastname = channel.Lastname,
                        Email = channel.Email,
                        Phone = channel.Phone,
                        Price = channel.Price,
                        StatusID = channel.StatusID,
                        Details = string.Empty,
                        IsActive = channel.IsActive
                    };
                }
                else
                {
                    return new ChannelDTO();
                }
                
            }
            else
            {
                return new ChannelDTO();
            }
        }

        //[HttpGet]
        //public HttpResponseMessage GetChannelImages(string imageCode, string Position)
        //{
        //    if (imageCode != null)
        //    {
        //        byte[] imageContent = ChannelManager.GetImage(Convert.ToInt64(imageCode), Position);
        //        if (imageContent != null)
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            var stream = new MemoryStream(imageContent);
        //            result.Content = new StreamContent(stream);
        //            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        //            return result;
        //        }
        //        else
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            return result;
        //        }

        //    }
        //    else
        //    {
        //        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //        return result;
        //    }
        //}
        //[HttpGet]
        //public HttpResponseMessage GetChannelImage(string imageCode)
        //{
        //    if (imageCode != null)
        //    {
        //        byte[] imageContent = ChannelManager.GetImage(Convert.ToInt64(imageCode));
        //        if(imageContent!=null)
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            var stream = new MemoryStream(imageContent);
        //            result.Content = new StreamContent(stream);
        //            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        //            return result;
        //        }
        //        else
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            return result;
        //        }
               
        //    }
        //    else
        //    {
        //        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //        return result;
        //    }
        //}
        //[HttpGet]
        //public HttpResponseMessage GetChannelLogo(string imageCode)
        //{
        //    if (imageCode != null)
        //    {
        //        byte[] imageContent = ChannelManager.GetLogo(Convert.ToInt64(imageCode));
        //        if (imageContent != null)
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            var stream = new MemoryStream(imageContent);
        //            result.Content = new StreamContent(stream);
        //            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        //            return result;
        //        }
        //        else
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            return result;
        //        }

        //    }
        //    else
        //    {
        //        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //        return result;
        //    }
        //}
        //[HttpGet]
        //public HttpResponseMessage GetChannelLeftImage(string imageCode)
        //{
        //    if (imageCode != null)
        //    {
        //        byte[] imageContent = ChannelManager.GetLeftImage(Convert.ToInt64(imageCode));
        //        if (imageContent != null)
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            var stream = new MemoryStream(imageContent);
        //            result.Content = new StreamContent(stream);
        //            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        //            return result;
        //        }
        //        else
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            return result;
        //        }

        //    }
        //    else
        //    {
        //        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //        return result;
        //    }
        //}
        //[HttpGet]
        //public HttpResponseMessage GetChannelRightImage(string imageCode)
        //{
        //    if (imageCode != null)
        //    {
        //        byte[] imageContent = ChannelManager.GetRightImage(Convert.ToInt64(imageCode));
        //        if (imageContent != null)
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            var stream = new MemoryStream(imageContent);
        //            result.Content = new StreamContent(stream);
        //            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        //            return result;
        //        }
        //        else
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            return result;
        //        }

        //    }
        //    else
        //    {
        //        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //        return result;
        //    }
        //}
        //[HttpGet]
        //public HttpResponseMessage GetChannelCenterImage(string imageCode)
        //{
        //    if (imageCode != null)
        //    {
        //        byte[] imageContent = ChannelManager.GetCenterImage(Convert.ToInt64(imageCode));
        //        if (imageContent != null)
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            var stream = new MemoryStream(imageContent);
        //            result.Content = new StreamContent(stream);
        //            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        //            return result;
        //        }
        //        else
        //        {
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            return result;
        //        }

        //    }
        //    else
        //    {
        //        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //        return result;
        //    }
        //}


        [HttpPost]
        public ChannelDTO GetChannelByID(ChannelDTO channel)
        {
            if (UserProfileManager.IsAuthenticateUser(channel.UserID))
            {
                var channelDetails = ChannelManager.GetByID(channel.ID);

                if (channelDetails != null)
                {
                    return new ChannelDTO
                    {
                        ID = channelDetails.ID,
                        Name = channelDetails.Firstname + " " + channelDetails.Lastname,
                        Firstname = channelDetails.Firstname,
                        Lastname = channelDetails.Lastname,
                        Email = channelDetails.Email,
                        Phone = channelDetails.Phone,
                        Price = channelDetails.Price,
                        StatusID = channelDetails.StatusID,
                        Details = string.Empty,
                        IsActive = channelDetails.IsActive
                    };
                }
            }

            return new ChannelDTO();
        }


        //[HttpPost]
        //public IEnumerable<ChannelMediaURLsDTO> GetChannelMediaURLs(ChannelDTO channel)
        //{
        //    if (UserProfileManager.IsAuthenticateUser(channel.UserID))
        //    {
        //        var MediaList = ChannelManager.GetMediaListWithContent(channel.ID, Entities.Enumerations.ChannelMediaType.Media_URL);
        //        var objMedia = new List<ChannelMediaURLsDTO>();
        //        foreach (var m in MediaList)
        //        {
        //            var items = new ChannelMediaURLsDTO
        //            {
        //                ID = m.ID,
        //                ChannelID = m.ChannelID,
        //                Name = m.Name,
        //                Content = m.Content.ToString(),
        //                MimeType = m.MimeType,
        //                IsActive = true

        //            };
        //            objMedia.Add(items);
        //        }
        //        return objMedia;

        //    }
        //    else
        //    {
        //        return new List<ChannelMediaURLsDTO>();
        //    }
        //}

        [HttpPost]
        public void Subscribe(SubscriptionDTO subscription)
        {
            ShoppingCartManager.PurchaseChannel(subscription.ChannelID, subscription.UserID, subscription.Price, subscription.PaymentTransactionID);
        }


        ////////////////////////////// Statrt New Code for Mobile App /////////////////////////////////////////////////
        #region Getting subscriber 

        [HttpPost]
        public IList GetSubscribers(ChannelDTO channel)
        {
            return ChannelManager.GetSubscribers(channel.ID);
        }

        [HttpPost]
        public IEnumerable<ChannelMediaURLsDTO> GetMediaURLs(ChannelDTO channel)
        {
            if (UserProfileManager.IsAuthenticateUser(channel.UserID))
            {
                var MediaList = ChannelManager.GetMediaList(channel.ID, ChannelMediaType.Media_URL);
                var objMedia = new List<ChannelMediaURLsDTO>();
                foreach (var m in MediaList)
                {
                    var items = new ChannelMediaURLsDTO
                    {
                        ID = m.ID,                      
                        Name = m.Name
                    };
                    objMedia.Add(items);
                }
                return objMedia;

            }
            else
            {
                return new List<ChannelMediaURLsDTO>();
            }
         
        }
        #endregion

        #region Getting Celebrity Media

        /// <summary>
        /// Get Channel Media by channel Id & Media Type Id
        /// </summary>
        /// <param name="channelID"></param>
        /// <param name="mediaTypeID"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetMedia(long channelID, ChannelMediaType mediaTypeID)
        {
            ChannelMedia media = null;
            media = ChannelManager.GetMedia(channelID, (ChannelMediaType)mediaTypeID);
            if (media != null)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new MemoryStream(media.Content);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(media.MimeType);
                return result;
            }
            else
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                return result;
            }
        }

        /// <summary>
        /// Get All media by media ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetMediaByID(long ID)
        {
            ChannelMedia media = null;
            media = ChannelManager.GetMediaByID(ID);
            if (media != null)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new MemoryStream(media.Content);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(media.MimeType);
                return result;
            }
            else
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                return result;
            }
        }
        #endregion
        ////////////////////////////// End New Code for Mobile App /////////////////////////////////////////////////


    }
}
