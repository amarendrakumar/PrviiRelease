//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Prvii.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class GroupChannel
    {
        public long ID { get; set; }
        public long GroupID { get; set; }
        public long ChannelID { get; set; }
        public System.DateTime EFD { get; set; }
        public Nullable<System.DateTime> ETD { get; set; }
    
        public virtual Group Group { get; set; }
        public virtual Channel Channel { get; set; }
    }
}
