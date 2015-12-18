using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.DataEntities
{
    public class CelebrityRevenueReportOld
    {
        public long ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string SubscriberName { get; set; }
        public int NoofPayment { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? InActiveDate { get; set; }
        public bool IsActive { get; set; }
        public decimal Total { get; set; }
        public decimal CelebrityProfit { get; set; }
        public decimal AgentProfit { get; set; }
        public decimal ComProfit { get; set; }
        public decimal PrviiProfit { get; set; }

       
    }

    public class CelebrityRevenueReport
    {
        public long ChannelId { get; set; }
        public string ChannelName { get; set; }
        public decimal Total { get; set; }
        public decimal GROSSAMOUNT { get; set; }
        public decimal OPERATINGCOST { get; set; }
        public decimal CELEBRITYPAYOUT { get; set; }
        public decimal NETGROSS { get; set; }
        public decimal MANAGERPAYOUT { get; set; }
        public decimal AGENTPAYOUT { get; set; }
        public decimal ASSOCIATEPAYOUT { get; set; }
        public decimal REFERRERPAYOUT { get; set; }
        public decimal EMPLOYEEPAYOUT { get; set; }
        public decimal DIRECTORPOOL { get; set; }


    }

    public class CelebrityRevenueReportList
    {
        public List<CelebrityRevenueReport> ListCelebrityRevenueReport;
        public List<CelebrityRevenueReport> GetRevenueReport()
        {
            return ListCelebrityRevenueReport;
        }
    }
}
