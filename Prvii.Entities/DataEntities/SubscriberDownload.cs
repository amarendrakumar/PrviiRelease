using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.DataEntities
{
    public class SubscriberDownload
    {
        public long ID { get; set; }
        public string Nickname { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string TimeZoneID { get; set; }
        public long CountryID { get; set; }
        public long? StateID { get; set; }
        public bool IsActive { get; set; }
        public string StateName { get; set; }
        public string City { get; set; }
        public string CountryName { get; set; }
        public string Celebrity { get; set; }

    }
}
