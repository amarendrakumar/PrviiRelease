using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prvii.BusinessService.Models
{
    public class ShoppingCartDTO
    {
        public long CartID { get; set; }
        public string Token { get; set; }
        public string payerId { get; set; }
        public long ChannelID { get; set; }
        public long UserID { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Price { get; set; }
        public List<long> itemList { get; set; }
        public string ExpressCheckoutURL { get; set; }

        public string clientId { get; set; }

        public string secret { get; set; }
        public string url { get; set; }

        public string code { get; set; }

        public string ResponseJSON { get; set; }
      
    }
}