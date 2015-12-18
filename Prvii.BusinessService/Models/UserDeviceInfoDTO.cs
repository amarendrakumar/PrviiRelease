using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.BusinessService.Models
{
    public class UserDeviceInfoDTO
    {
        public long ID { get; set; }
        public long SubscriberId { set; get; }      
        public string DeviceCordova { get; set; }
        public string DevicePaltform { set; get; }
        public string DeviceId { get; set; }
        public string DeviceVersion { get; set; }
        public string DeviceToken { get; set; }        
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
       
    }
}
