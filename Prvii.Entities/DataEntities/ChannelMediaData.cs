using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.DataEntities
{
    public class ChannelMediaData
    {
        public long ID { get; set; }
        public long ChannelID { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string MimeType { get; set; }
        public short MediaTypeID { get; set; }
    }
}
