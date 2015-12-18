using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class ChannelSubscriberStatisticDOT
    {
        public long channelID { get; set; }
        public short period { get; set; }
        public string periods { get; set; }
        public string periodType { get; set; }
        public long periodValue { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

      
    }
}