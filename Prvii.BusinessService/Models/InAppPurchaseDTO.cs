using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class InAppPurchaseDTO
    {
        public string receiptData { get; set; }
        public string receiptToken { get; set; }
        public string Resultjson { get; set; }
        public long subscriberId { get; set; }
        public long channelID { get; set; }
    }
}