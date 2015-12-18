using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.DataEntities
{
    public class PayReconData
    {
        public long Id { get; set; }
        public string ProfileId { get; set; }
        public short Status { get; set; }
        public decimal AggregateAmount { get; set; }
        public System.DateTime FinalPaymentDueDate { get; set; }
        public System.DateTime ProfileStartDate { get; set; }
        public decimal CycleAmount { get; set; }
        public string Currency { get; set; }
        public DateTime? NextBillDate { get; set; }
        public int TotalCycles { get; set; }
        public int CyclesCompleted { get; set; }
        public int CyclesRemaining { get; set; }
        public int FailedPmtCount { get; set; }
        public System.DateTime LastPmtDate { get; set; }
        public decimal LastPmtAmt { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int AlertCode { get; set; }
        public bool ReconPick { get; set; }
    }
}
