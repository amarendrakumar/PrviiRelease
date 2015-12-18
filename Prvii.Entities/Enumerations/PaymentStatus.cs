using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.Enumerations
{
    public enum PaymentStatus
    {
        Checkout_Complete = 1,
        Awaiting_Payment_Confirmation = 2,
        Payment_Completed = 3,
        Payment_Cancelled = 4,
        Awaiting_Cancel_Confirmation = 5
    }


    public enum PaypalProfileStatus
    {
        Active=1,
        Pending=2,
        Cancelled=3,
        Suspended=4,
        Expired=5
    }
}
