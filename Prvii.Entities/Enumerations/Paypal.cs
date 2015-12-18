using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Entities.Enumerations
{
    public enum TransactionStatus
    {
        Pending = 1,
        Processing = 2,
        Completed =3,
        Denied = 4,
        Reversed =5,
        Created = 6,
        Canceled = 7,
        Expired =8
    }

    public enum TransactionType
    {
        RecurringPayment = 1,
        Payment = 2,
        TemporaryHold = 3
    }




}
