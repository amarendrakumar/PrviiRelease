using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prvii.Entities.DataEntities;
using Prvii.Entities;
using Prvii.Entities.Enumerations;
using System.Collections;

namespace Prvii.Business
{
    public class PaymentManager
    {

        public void ReconcilePaypalProfiles()
        {

            using (PrviiEntities context = new PrviiEntities())
            {
                List<PayReconData> listNewProfiles = context.ShoppingCartItems.Where(e => !context.PaymentRecons.Select(m => m.ProfileId)
                                                   .Contains(e.PaymentProfileID)).Select(e => new PayReconData { Id = 0, ProfileId = e.PaymentProfileID}).ToList();
             // List<string> listScheduledProfiles = context.PaymentRecons.Where(e => e.NextBillDate < DateTime.Now).Select(e => e.ProfileId).ToList();
              List<PayReconData> listScheduledProfiles = context.PaymentRecons.Where(e => e.NextBillDate < DateTime.Now && context.ShoppingCartItems.Any(d => d.ProfileStatus != 3 && d.ProfileStatus != 5 && e.ReconPick == true && d.PaymentProfileID == e.ProfileId)).Select(e => new PayReconData { Id = e.Id, ProfileId = e.ProfileId }).ToList();


              foreach (PayReconData recon in listNewProfiles)
              {
                  PayPalManager paypalManager = new PayPalManager();
                  if (recon.ProfileId != null && recon.ProfileId.Trim() != string.Empty)
                  paypalManager.GetRecurringPaymentProfileDetails(recon);
              }

              foreach (PayReconData recon in listScheduledProfiles)
              {
                  PayPalManager paypalManager = new PayPalManager();
                  paypalManager.GetRecurringPaymentProfileDetails(recon);
              }






            }
        }


        /// <summary>
        /// This method gets all Profiles that need to be reconciled in this cycle
        /// </summary>
        public  List<TransactionReconDetail> ReconcilePaypalTransactions()
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                try
                {

                    var profileList = context.PROC_GetProfileIdsforRecon1().ToList<TransactionReconDetail>();
                    return profileList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
           
        }


       


    }
}
