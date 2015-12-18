using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class ChannelMessageDTO
    {
        public long ID { get; set; }
        public long ChannelID { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string EmailMessage { get; set; }
        public bool IsScheduled { get; set; }
        public string ScheduledOn { get; set; }
        public int Status { get; set; }
        public int SMSStatus { get; set; }
        public bool IsSMS { get; set; }
        public bool IsEmail { get; set; }
        public string SentOn { get; set; }
        public long[] MediaURLIds { get; set; }
        public long[] SubscriberIDs { get; set; }
        public bool SendToAll { get; set; }
        public bool IsDeleted { get; set; }
        public string TimeZoneID { get; set; }
        public List<ChannelMessageAttachmentDTO> Attachments { get; set; }
        public string StatusText { get; set; }
        public string StatusEmail { get; set; }

        public List<ChannelMessageSubscriberDTO> channelMessageSubscriberList { get; set; }

        public string SentOnly { get; set; }
        public long[] SIDs { get; set; }
        
    }
}