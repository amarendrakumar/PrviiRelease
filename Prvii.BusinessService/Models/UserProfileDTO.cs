using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class UserProfileDTO
    {
        public long ID { get; set; }
        public string Email { set; get; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Telephone { set; get; }
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public long? CountryId { get; set; }
        public string ZipCode { get; set; }
        public string DeliveryMethod { get; set; }
        public int? DeviceTypeID { get; set; }
        public string NickName { get; set; }
        public string TimeZoneID { get; set; }
        public long? StateId { get; set; }
        public string City { get; set; }
        public Nullable<long> GroupID { get; set; }
        public Nullable<long> ChannelID { get; set; }

        public short UserProfileTypeID { get; set; }
       
        public string Country { get; set; }
        public string State { get; set; }
        public string UserProfileType { get; set; }
      
        public string NewPassword { get; set; }

        public string DeviceName { get; set; }

        public List<UserProfileTypeDTO> ProfileType { get; set; }

        public List<UserDeviceInfoDTO> userDeviceInfoList { get; set; }

        public UserDeviceInfoDTO userDeviceInfo { get; set; }
       
       

    }
}