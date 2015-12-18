using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class ChannelMessageAttachmentDTO
    {
        public long ID { get; set; }
        public long ChannelMessageID { get; set; }
        public long MediaURLID { get; set; }
      
    }
}