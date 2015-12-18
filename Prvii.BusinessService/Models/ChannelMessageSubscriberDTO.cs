using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class ChannelMessageSubscriberDTO
    {
        public long SubsciberID { get; set; }
        public string SubscriberName { get; set; }
        public int IsSMSSend { get; set; }
        public int IsEmailSend { get; set; }
    }
}