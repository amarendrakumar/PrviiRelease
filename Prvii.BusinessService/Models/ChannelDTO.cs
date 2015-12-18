using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class ChannelDTO
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Price { get; set; }

        public long UserID { get; set; }

        public string Details { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string UserName { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string SearchText { get; set; }

        public short BillingCycleID { get; set; }
        public int? NoOfBillingPeriod { get; set; }
        public short StatusID { get; set; }
        public bool IsSubscribed { get; set; }
        public long WeekNo { get; set; }


    }
}