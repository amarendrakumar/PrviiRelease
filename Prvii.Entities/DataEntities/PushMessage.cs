using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.DataEntities
{
    public class PushMessage
    {
        public string DeviceToken { set; get; }
        public string Message { set; get; }
        public string DeviceId { set; get; }
        public int SubscriberId { set; get; }
        public string DeviceName { set; get; }
        public long ChannelSubscriberMessageId { set; get; }

    }
}
