using Prvii.Entities.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.DataEntities
{
    public class UserProfileData
    {
        public long ID { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
        public string ZipCode { get; set; }
        public Nullable<long> GroupID { get; set; }
        public Nullable<long> ChannelID { get; set; }

        public string TimeZoneID { get; set; }
        public string CountryName { get; set; }
        public long CountryID { get; set; }

        public string RoleName { get; set; }
        public Role UserRole { get; set; }
        public DateTime Subscription_EFD { get; set; }

        public string Name
        {
            get
            {
                return this.Firstname + " " + this.Lastname;
            }
        }

    }
}
