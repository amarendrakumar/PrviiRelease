using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Prvii.Business;
using Prvii.Entities;
using Prvii.BusinessService.Models;
using System.Collections;
using Prvii.Entities.Enumerations;

namespace Prvii.BusinessService.Controllers
{
    public class UserProfileController : ApiController
    {
        [HttpPost]
        public UserProfileDTO Authenticate(UserProfileDTO userProfile)
        {           
            var UserDetails = UserProfileManager.Authenticate(userProfile.Email, userProfile.Password);

            if (UserDetails != null)
            {
                if (userProfile.Email == UserDetails.Email && userProfile.Password == UserDetails.Password)
                {
                    var UserDevice = new UserDeviceInfo
                   {
                       Id = userProfile.userDeviceInfo.ID,
                       SubscriberId = UserDetails.ID,                      
                       DeviceCordova = userProfile.userDeviceInfo.DeviceCordova,
                       DevicePaltform = userProfile.userDeviceInfo.DevicePaltform,
                       DeviceId = userProfile.userDeviceInfo.DeviceId,
                       DeviceVersion = userProfile.userDeviceInfo.DeviceVersion,
                       IsActive = userProfile.userDeviceInfo.IsActive,
                       CreatedDate = DateTime.UtcNow,
                       ModifiedDate = DateTime.UtcNow
                   };

                    UserProfileManager.SaveUserDeviceInfo(UserDevice);
               
                    return new UserProfileDTO
                    {
                        ID = UserDetails.ID,
                        Password = UserDetails.Password,
                        Firstname = UserDetails.Firstname,
                        Lastname = UserDetails.Lastname,
                        Telephone = UserDetails.Telephone,
                        Mobile = UserDetails.Mobile,
                        Email = UserDetails.Email,
                        ChannelID = UserDetails.ChannelID,
                        GroupID = UserDetails.GroupID,
                        NickName = UserDetails.NickName,
                        TimeZoneID = UserDetails.TimeZoneID
                    };
                }
            }
            return new UserProfileDTO();
        }

        [HttpPost]
        public IEnumerable<UserProfileTypeDTO> GetUserProfileTypeByUserID(UserProfileDTO userProfile)
        {
            var userProfileTypeList = UserProfileManager.GetRoles(userProfile.ID);
            var objUserType = new List<UserProfileTypeDTO>();
            foreach (var p in userProfileTypeList)
            {
                var items = new UserProfileTypeDTO
                {
                    ID = p.ID,
                    UserID = p.UserID,
                    ProfileTypeID = p.RoleID,
                    ProfileTypeName = Enum.GetName(typeof(Role), p.RoleID)
                };
                if (p.RoleID != 1)
                    objUserType.Add(items);
            }
            return objUserType;
        }

       

      
      
        //[HttpPost]
        //public int GetActiveNetSubscribers(ChannelSubscriberStatisticDOT channelSubscriberStatistic)
        //{
        //    var result = ChannelSubscribersManager.GetActiveNetSubscribers(channelSubscriberStatistic.channelID, channelSubscriberStatistic.periodType, channelSubscriberStatistic.period);
        //   return result;
        //}


      


      
        //Due to change in database have to comment these lines 
        [HttpPost]
        public UserProfileDTO GetUserProfileById(UserProfileDTO userProfile)
        {
            var UserDetails = UserProfileManager.GetByID(userProfile.ID);
            if (UserDetails != null)
            {
                return new UserProfileDTO
                {
                    ID = UserDetails.ID,
                    Firstname = UserDetails.Firstname,
                    Lastname = UserDetails.Lastname,
                    Telephone = UserDetails.Telephone,
                    Mobile = UserDetails.Mobile,
                    Email = UserDetails.Email,
                    Address1 = UserDetails.Address1,
                    Address2 = UserDetails.Address2,
                    Country = UserProfileManager.GetCountryName( UserDetails.CountryID),
                    State = UserProfileManager.GetStateName(UserDetails.StateID),
                    City = UserDetails.City,
                    ZipCode = UserDetails.ZipCode,
                    StateId = UserDetails.StateID,
                    CountryId = UserDetails.CountryID,
                    DeliveryMethod = UserDetails.DeliveryMechanisms,
                    Password = UserDetails.Password,
                    NickName=UserDetails.NickName,
                    TimeZoneID=UserDetails.TimeZoneID,
                    DeviceTypeID = UserDetails.DeviceTypeID,
                    DeviceName = Enum.GetName(typeof(DeviceType), UserDetails.DeviceTypeID) 
                };
            }
            return new UserProfileDTO();
        }

        [HttpPost]
        public HttpResponseMessage channelScubscrbe(ChannelDTO channel)
        {
            ChannelSubscriber objchannelsubscriber = new ChannelSubscriber();
            objchannelsubscriber.ChannelID = channel.ID;
            objchannelsubscriber.SubscriberID = channel.UserID;
            objchannelsubscriber.EFD = DateTime.Now;
            objchannelsubscriber.ETD = DateTime.Now.AddMonths(2);
            ChannelSubscribersManager.SaveChannelSubscriber(objchannelsubscriber);
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }

        [HttpPost]
        public IList GetCountry()
        {
            return MasterManager.GetAllCountry();
        }
       
        [HttpPost]
        public IList GetStateByCountryID(long CountryID)
        {
            return MasterManager.GetStateByCountryID(CountryID);
        }


        //Due to change in database CityID to City error occuring at CityID 
        [HttpPost]
        public long ManageUserMaster(UserProfileDTO userProfile)
        {
            var UserDetails = UserProfileManager.GetByID(userProfile.ID);
            UserDetails.NickName = userProfile.NickName;
            UserDetails.Firstname = userProfile.Firstname;
            UserDetails.Lastname = userProfile.Lastname;
            UserDetails.Telephone = userProfile.Telephone;
            UserDetails.Mobile = userProfile.Mobile;
            UserDetails.Email = userProfile.Email;
            UserDetails.IsActive = true;
            UserDetails.Address1 = userProfile.Address1;
            UserDetails.Address2 = userProfile.Address2;
            UserDetails.CountryID = userProfile.CountryId.Value;
            UserDetails.StateID = userProfile.StateId;
            UserDetails.City = userProfile.City;
            UserDetails.ZipCode = userProfile.ZipCode;
            UserDetails.DeliveryMechanisms = userProfile.DeliveryMethod;
            return UserProfileManager.ManageUserMaster(UserDetails);
        }

        [HttpPost]
        public int changePassword(UserProfileDTO userProfile)
        {
            return UserProfileManager.changePassword(userProfile.ID, userProfile.Password, userProfile.NewPassword);
        }

        [HttpPost]
        public UserProfileDTO GetUserProfileByDeviceID(UserDeviceInfoDTO deviceInfo)
        {
            var UserDetails = UserProfileManager.GetUserByDeviceID(deviceInfo.DeviceId);
            if (UserDetails != null)
            {
                return new UserProfileDTO
                {
                    ID = UserDetails.ID,
                    Password = UserDetails.Password,
                    Firstname = UserDetails.Firstname,
                    Lastname = UserDetails.Lastname,
                    Telephone = UserDetails.Telephone,
                    Mobile = UserDetails.Mobile,
                    Email = UserDetails.Email,
                    ChannelID = UserDetails.ChannelID,
                    GroupID = UserDetails.GroupID,
                    NickName = UserDetails.NickName,
                    TimeZoneID = UserDetails.TimeZoneID
                };
            }
            else
                return new UserProfileDTO();
           
        }

        [HttpPost]
        public HttpResponseMessage logoutDevice(UserDeviceInfoDTO deviceInfo)
        {
            UserProfileManager.LogoutUserDevice(deviceInfo.SubscriberId, deviceInfo.DeviceId);
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }




        [HttpPost]
        public HttpResponseMessage SaveDeviceToken(UserDeviceInfoDTO deviceInfo)
        {
            UserProfileManager.SaveUserDeviceToken(deviceInfo.SubscriberId, deviceInfo.DeviceId, deviceInfo.DeviceToken);
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }


    }
}
