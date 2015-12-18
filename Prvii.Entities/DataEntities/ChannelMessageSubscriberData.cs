using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.DataEntities
{
    public class ChannelMessageSubscriberData
    {
        public long SubsciberID { get; set; }
        public string SubscriberName { get; set; }
        public int IsSMSSend { get; set; }
        public int IsEmailSend { get; set; }
    }
}
