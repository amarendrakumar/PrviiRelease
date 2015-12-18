using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class UserProfileTypeDTO
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long ProfileTypeID { get; set; }
        public string ProfileTypeName { get; set; }
    }
}