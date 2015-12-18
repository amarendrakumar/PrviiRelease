using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Business;
using Prvii.ExceptionHandling;

namespace Prvii.Web.WebPages.ShoppingCarts
{
    public partial class CheckoutIPNHandler : System.Web.UI.Page
    {
        private void LogMessage(string message)
        {
            string logfilePath = WebConfigurationManager.AppSettings["PayPal_IPNLogFilePath"].ToString();
            ExceptionHandler.LogMessage(message, true, 10240, logfilePath);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HandleIPNFromPaypal();
        }

        private void HandleIPNFromPaypal()
        {
            PayPalManager payPal = new PayPalManager();

            string strFormValues = Encoding.ASCII.GetString(Request.BinaryRead(Request.ContentLength));
            dynamic strNewValue = null;

            this.LogMessage("IPNHandler: All payment's parameters. Request: " + strFormValues);


            if (Request["txn_type"].ToString().Trim() == "subscr_signup")
                return;

            // Create the request back
            HttpWebRequest req = WebRequest.Create(payPal.PAYPAL_URL) as HttpWebRequest;

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            strNewValue = strFormValues + "&cmd=_notify-validate";
            req.ContentLength = strNewValue.Length;

            // Write the request back IPN strings
            StreamWriter stOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            stOut.Write(strNewValue);
            stOut.Close();

            //send the request, read the response
            HttpWebResponse strResponse = (HttpWebResponse)req.GetResponse();
            Stream IPNResponseStream = strResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(IPNResponseStream, encode);

            Char[] read = new Char[257];
            // Reads 256 characters at a time.
            int count = readStream.Read(read, 0, 256);

            while (count > 0)
            {
                // Dumps the 256 characters to a string
                String IPNResponse = new String(read, 0, count);
                count = readStream.Read(read, 0, 256);

                string amount = null;
                try
                {
                    amount = GetRequestPrice(Request["custom"].ToString());
                    if (string.IsNullOrEmpty(amount))
                    {
                        this.LogMessage("Error in IPNHandler: amount = \"");
                        readStream.Close();
                        strResponse.Close();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    this.LogMessage("Error in IPNHandler 1: " + ex.Message + " - " + ex.StackTrace);
                    readStream.Close();
                    strResponse.Close();
                    return;
                }

                if (IPNResponse == "VERIFIED")
                {
                    if (Request["receiver_email"].ToString().Trim() != payPal.PAYPAL_BUSINESS_EMAIL || Request["txn_type"].ToString().Trim() != "subscr_payment")
                    {
                        try
                        {
                            if (Request["receiver_email"].ToString().Trim() != payPal.PAYPAL_BUSINESS_EMAIL || Request["txn_type"].ToString().Trim() == "subscr_cancel")
                            {
                                string shoppingcartID = Request["custom"].ToString();
                                string profileID = Request["subscr_id"].ToString();
                                ChannelManager.ChannelUnsubscribePayPal(Convert.ToInt64(shoppingcartID), profileID);
                                this.LogMessage(" Cancel Success in IPNHandler for CART_ID: " + shoppingcartID + ", PaymentProfileID  :  " + profileID);
                            }
                            else
                            {
                                string paymentTransactionID = Request["txn_id"].ToString();
                                //string shoppingcartID = Request["custom"].ToString();
                                this.LogMessage("Error in IPNHandler: INVALID payment's parameters (receiver_email or txn_type)");
                            }

                        }
                        catch (Exception ex)
                        {
                            this.LogMessage("Error in IPNHandler 2: " + ex.Message + " - " + ex.StackTrace);
                        }
                        readStream.Close();
                        strResponse.Close();
                        return;
                    }

                    if (IsDuplicateID(Request["txn_id"].ToString()))
                    {
                        string paymentTransactionID = Request["txn_id"].ToString();
                        string shoppingcartID = Request["custom"].ToString();
                        this.LogMessage("Error in IPNHandler: Duplicate txn_id found");
                        readStream.Close();
                        strResponse.Close();
                        return;
                    }

                    if (Convert.ToDecimal(Request["mc_gross"]) != Convert.ToDecimal(amount) || Request["mc_currency"].ToString() != "USD" || Request["payment_status"].ToString() != "Completed")
                    {
                        this.LogMessage("Error in IPNHandler: INVALID payment's parameters. Request: " + strFormValues);
                        readStream.Close();
                        strResponse.Close();
                        return;
                    }

                    try
                    {
                        string paymentTransactionID = Request["txn_id"].ToString();
                        string shoppingcartID = Request["custom"].ToString();
                        string profileID = Request["subscr_id"].ToString();
                        ShoppingCartManager.UpdatePayment(Convert.ToInt64(shoppingcartID), paymentTransactionID, profileID);
                        this.LogMessage("Success in IPNHandler for CART_ID: " + shoppingcartID);
                    }
                    catch (Exception ex)
                    {
                        this.LogMessage("Error in IPNHandler 3: " + ex.Message + ex.StackTrace + (ex.InnerException != null ? ex.InnerException.Message + ex.InnerException.StackTrace : string.Empty));
                    }
                }
                else
                {
                    this.LogMessage("Error in IPNHandler. IPNResponse = 'INVALID'");
                }
            }

            readStream.Close();
            strResponse.Close();
        }

        public string GetRequestPrice(string request_id)
        {
            try
            {
                return ShoppingCartManager.GetCartTotalPrice(Convert.ToInt64(request_id)).ToString();
            }
            catch (Exception ex)
            {
                this.LogMessage("Error in IPNHandler.GetRequestPrice(): " + ex.Message);
            }

            return "";
        }

        public bool IsDuplicateID(string txn_id)
        {
            try
            {
                return ShoppingCartManager.PaymentTransactionIDExists(txn_id);
            }
            catch (Exception ex)
            {
                this.LogMessage("Error in IPNHandler.IsDuplicateID(): " + ex.Message);
                return false;
            }
        }
    }
}