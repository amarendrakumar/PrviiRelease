using Prvii.Entities.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.DataEntities
{
    public class ChannelMessageData
    {
        public long ID { get; set; }
        public long ChannelID { get; set; }
        public bool SendToAll { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string EmailMessage { get; set; }
        public bool IsSMS { get; set; }
        public bool IsEmail { get; set; }
        public bool IsScheduled { get; set; }
        public System.DateTime ScheduledOn { get; set; }
        public short Status { get; set; }
        public short SMSStatus { get; set; }
        public Nullable<System.DateTime> SentOn { get; set; }
        public string TimeZoneID { get; set; }

        public string StatusText { get; set; }
        public string StatusEmail { get; set; }
        public string ChannelName { get; set; }
    }


   
}
