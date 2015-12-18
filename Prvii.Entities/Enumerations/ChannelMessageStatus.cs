using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.Enumerations
{
    public enum ChannelMessageStatus
    {
        NA = 0,
        Created = 1,
        Sent = 2,
        Approved = 3,
        Sending = 4,
        EmailSent = 11,
        EmailNotSent = 12
        
    }

    public enum SMSStatus
    {
        NA = 0,
        Queued = 1,
        Sending = 2,
        Sent = 3,
        Failed = 4,
        Created = 5,
        Approved = 6,
         SMSSent=11,
        SMSNotSent = 12
      
    }
}
