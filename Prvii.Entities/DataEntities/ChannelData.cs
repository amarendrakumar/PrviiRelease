using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.DataEntities
{
    public class ChannelData
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Price { get; set; }
        public short BillingCycleID { get; set; }
        public int? NoOfBillingPeriod { get; set; }
        public short StatusID { get; set; }
        public bool IsActive { get; set; }
        public bool IsSubscribed { get; set; }
    }
}
