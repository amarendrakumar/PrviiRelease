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
    
    public partial class PrviiTransaction
    {
        public long ID { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<long> SubscriberId { get; set; }
        public Nullable<long> ChannelId { get; set; }
        public string AmountDetails { get; set; }
        public Nullable<System.DateTime> TrnDate { get; set; }
    }
}
