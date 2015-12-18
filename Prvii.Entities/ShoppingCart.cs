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
    
    public partial class ShoppingCart
    {
        public ShoppingCart()
        {
            this.ShoppingCartItems = new HashSet<ShoppingCartItem>();
        }
    
        public long ID { get; set; }
        public string SessionID { get; set; }
        public decimal TotalPrice { get; set; }
        public Nullable<long> UserID { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool Status { get; set; }
    
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}