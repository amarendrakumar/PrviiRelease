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
    
    public partial class Channel
    {
        public Channel()
        {
            this.ChannelMedias = new HashSet<ChannelMedia>();
            this.ChannelMessages = new HashSet<ChannelMessage>();
            this.ChannelSubscribers = new HashSet<ChannelSubscriber>();
            this.GroupChannels = new HashSet<GroupChannel>();
            this.ShoppingCartItems = new HashSet<ShoppingCartItem>();
            this.PrviiChannelAccountings = new HashSet<PrviiChannelAccounting>();
        }
    
        public long ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Price { get; set; }
        public short BillingCycleID { get; set; }
        public Nullable<int> NoOfBillingPeriod { get; set; }
        public short StatusID { get; set; }
        public string TimeZoneID { get; set; }
        public bool IsActive { get; set; }
        public bool Preclude { get; set; }
        public string PriceManagement { get; set; }
    
        public virtual ICollection<ChannelMedia> ChannelMedias { get; set; }
        public virtual ICollection<ChannelMessage> ChannelMessages { get; set; }
        public virtual ICollection<ChannelSubscriber> ChannelSubscribers { get; set; }
        public virtual ICollection<GroupChannel> GroupChannels { get; set; }
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
        public virtual ICollection<PrviiChannelAccounting> PrviiChannelAccountings { get; set; }
    }
}
