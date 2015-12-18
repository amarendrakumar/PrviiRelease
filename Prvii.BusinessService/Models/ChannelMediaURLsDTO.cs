using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class ChannelMediaURLsDTO
    {
        public long ID { get; set; }
        public long ChannelMessageID { get; set; }
        public long ChannelID { get; set; }

        public string Name { get; set; }
        public string Content { get; set; }
        public string MimeType { get; set; }
        public bool IsActive { get; set; }
    }
}