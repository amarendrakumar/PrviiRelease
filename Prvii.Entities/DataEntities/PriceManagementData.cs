using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.DataEntities
{
     public class PriceMenegementData
     {
         public long Id { get; set; }
         public string Account { get; set; }
         public decimal Distribution { get; set; }
         public int Sequence { get; set; }
         public long ParentID { get; set; }

     }




     public class PriceDistribution
     {
         public long Id { get; set; }
         public int AccountId { get; set; }
         public decimal Distribution { get; set; }
         public int Sequence { get; set; }
         public long ParentID { get; set; }
     }

     public class PriceManagement
     {
         public long Id { get; set; }
         public int AccountId { get; set; }
         public decimal Distribution { get; set; }
         public int Sequence { get; set; }

         public List<PriceDistribution> InnerDistribution { get; set; }
     }
}
