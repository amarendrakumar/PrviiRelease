using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class SubscriptionDTO
    {
        public long ChannelID { get; set; }
        public long UserID { get; set; }
        public decimal Price { get; set; }
        public string PaymentTransactionID { get; set; }
    }
}