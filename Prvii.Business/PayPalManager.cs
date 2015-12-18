using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using Prvii.Entities;
using Prvii.Entities.Enumerations;
using Prvii.Entities.DataEntities;
using Newtonsoft.Json;

namespace Prvii.Business
{
    public class PayPalManager
    {
        //Flag that determines the PayPal environment (live or sandbox)
      //  private const bool bSandbox = true;
        public string configSandbox = WebConfigurationManager.AppSettings["bSandbox"].ToString();
        private const bool bSandbox = true;

        
        private const string CVV2 = "CVV2";

        // Live strings.
        private string pEndPointURL = "https://api-3t.paypal.com/nvp";
        private string host = "www.paypal.com";

        // Sandbox strings.
        private string pEndPointURL_SB = "https://api-3t.sandbox.paypal.com/nvp";
        private string host_SB = "www.sandbox.paypal.com";

        private const string SIGNATURE = "SIGNATURE";
        private const string PWD = "PWD";
        private const string ACCT = "ACCT";

        //classic api settings
        //public string PAYPAL_BUSINESS_EMAIL = "prvii.t-facilitator@mail.com";
        public string PAYPAL_BUSINESS_EMAIL = WebConfigurationManager.AppSettings["PAYPAL_BUSINESS_EMAIL"].ToString();

        public string PAYPAL_URL
        {
            get
            {
                return bSandbox ? "https://www.sandbox.paypal.com/cgi-bin/webscr" : "https://www.paypal.com/cgi-bin/webscr";
            }
        }

        //Replace <Your API Username> with your API Username
        //Replace <Your API Password> with your API Password
        //Replace <Your Signature> with your Signature
       // public string APIUsername = "prvii.t-facilitator_api1.mail.com";
       // private string APIPassword = "3FPRQHDY7AKSBMRX";
       // private string APISignature = "AdikT1.c7FJUqjIRaoE8tjfaNXdRARX.xU-8SWWf8IPkWz76jG.B0b.y";
      //  private string Subject = "";
      //  private string BNCode = "PP-ECWizard";

        public string APIUsername = WebConfigurationManager.AppSettings["APIUsername"].ToString();
        private string APIPassword = WebConfigurationManager.AppSettings["APIPassword"].ToString();
        private string APISignature = WebConfigurationManager.AppSettings["APISignature"].ToString();
        private string Subject = "";
        private string BNCode = WebConfigurationManager.AppSettings["BNCode"].ToString();

         
                  


        //HttpWebRequest Timeout specified in milliseconds 
        private const int Timeout = 15000;
        private static readonly string[] SECURED_NVPS = new string[] { ACCT, CVV2, SIGNATURE, PWD };

        public void SetCredentials(string Userid, string Pwd, string Signature)
        {
            APIUsername = Userid;
            APIPassword = Pwd;
            APISignature = Signature;
        }


        public bool CreateRecurringPaymentsProfile(string token, ref string PayerId, ref string ProfileID, ref string retMsg, ref string ProfileStatus, Channel cartItems)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
                host = host_SB;
            }
            string BILLINGPERIOD = "Month";
            string BILLINGFREQUENCY = "1";
            if (cartItems.BillingCycleID == (short)BillingCycleType.Yearly)
            {
                BILLINGPERIOD = "Year";                
            }

            if (cartItems.BillingCycleID == (short)BillingCycleType.Quarterly)
            {
                BILLINGFREQUENCY = "4";
            }

            if (cartItems.BillingCycleID == (short)BillingCycleType.Daily)
            {
                BILLINGPERIOD = "Day";
            }

            if (cartItems.BillingCycleID == (short)BillingCycleType.Weekly)
            {
                BILLINGPERIOD = "Week";
            }


            NVPCodec encoder = new NVPCodec();
            encoder["METHOD"] = "CreateRecurringPaymentsProfile";
            encoder["TOKEN"] = token;
            // Profile Info
            //nvpRequest["SUBSCRIBERNAME"]="";
            encoder["PROFILESTARTDATE"] = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'"); //"2015-03-09T05:38:48Z";
            //Schedule Info
            encoder["DESC"] = "RecurringSubscriptionDescription"; //You must ensure that this field matches the corresponding billing agreement description included in the SetExpressCheckout request.
            //Billing Period Details Fields ||  The combination of BillingPeriod and BillingFrequency cannot exceed one year.
            encoder["BILLINGPERIOD"] = BILLINGPERIOD;// cartItems.billingPeriod;  //Day, Week, SemiMonth, Month, Year

            encoder["BILLINGFREQUENCY"] = BILLINGFREQUENCY;  // Number of billing periods that make up one billing cycle. For example, if the billing cycle is Month, the maximum value for billing frequency is 12.


            encoder["TOTALBILLINGCYCLES"] = Convert.ToString(cartItems.NoOfBillingPeriod);

            double Amount = 0.00;
            Amount = Convert.ToDouble(cartItems.Price.ToString("#.##"));
            string amt = Amount.ToString("F");

            encoder["AMT"] = amt;
            encoder["CURRENCYCODE"] = "USD";

            encoder["SHIPPINGAMT"] = "0.00";
            encoder["TAXAMT"] = "0.00";


            // Payment Details Item Fields
            encoder["L_PAYMENTREQUEST_0_ITEMCATEGORY0"] = "Physical"; //line item || (Digital OR Physical) || For digital goods, this field is required and must be set to Digital to get the best rates. 
            encoder["L_PAYMENTREQUEST_0_NAME0"] = cartItems.Firstname + " " + cartItems.Lastname as string; //This field is required when L_PAYMENTREQUEST_n_ITEMCATEGORYm is passed. 
            encoder["L_PAYMENTREQUEST_0_AMT0"] = amt; //This field is required when L_PAYMENTREQUEST_n_ITEMCATEGORYm is passed. 
            encoder["L_PAYMENTREQUEST_0_QTY0"] = "1"; //This field is required when L_PAYMENTREQUEST_n_ITEMCATEGORYm is passed.


            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                //retMsg = "PROFILEID=" + decoder["PROFILEID"] + "&" +
                //"PROFILESTATUS=" + decoder["PROFILESTATUS"];
                retMsg = "";
                ProfileID = decoder["PROFILEID"];
                ProfileStatus = decoder["PROFILESTATUS"];
                return true;
            }
            else
            {
                retMsg = decoder["L_ERRORCODE0"];
                //retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + ";" +
                //   "Desc=" + decoder["L_SHORTMESSAGE0"] + ";" +
                //   "Desc2=" + decoder["L_LONGMESSAGE0"];
                return false;
                //throw new Exception(retMsg);
            }
        }


        //public bool CreateRecurringProfileSubscription(string subscriberID)
        //{
        //    if (bSandbox)
        //    {
        //        pEndPointURL = pEndPointURL_SB;
        //        host = host_SB;
        //    }

        //    NVPCodec encoder = new NVPCodec();
        //    encoder["METHOD"] = "CreateRecurringPaymentsProfile";
        //    //encoder["TOKEN"] = "SetExpressCheckout";
        //    encoder["SUBSCRIBERNAME"] = "Bombi";
        //    encoder["PROFILESTARTDATE"] = "2015-02-03T00:00:00Z";
        //    encoder["PROFILEREFERENCE"] = subscriberID;
        //    encoder["DESC"] = "Prvii Celebrity Services Subscribtion ";
        //    encoder["AUTOBILLOUTAMT"] = "AddToNextBilling";
        //    encoder["BILLINGPERIOD"] = "Month";
        //    encoder["BILLINGFREQUENCY"] = "1";
        //    encoder["TOTALBILLINGCYCLES"] = "0";
        //    encoder["AMT"] = "1.10";
        //    encoder["BILLINGFREQUENCY"] = "1";

        //    string pStrrequestforNvp = encoder.Encode();
        //    string pStresponsenvp = HttpCall(pStrrequestforNvp);

        //    NVPCodec decoder = new NVPCodec();
        //    decoder.Decode(pStresponsenvp);

        //    string strAck = decoder["ACK"].ToLower();
        //    if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        var retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
        //            "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
        //            "Desc2=" + decoder["L_LONGMESSAGE0"];

        //        throw new Exception(retMsg);
        //    }
        //}

        public bool CancelSubscription(string subscriberID)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
                host = host_SB;
            }

            NVPCodec encoder = new NVPCodec();
            encoder["METHOD"] = "ManageRecurringPaymentsProfileStatus";
            encoder["PROFILEID"] = subscriberID;
            encoder["ACTION"] = "Cancel";
            encoder["NOTE"] = "User cancelled subscription at store.";

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                var reTMsg = decoder["PROFILEID"];
                return true;
            }
            else
            {
                var retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                throw new Exception(retMsg);
            }
        }

        public bool UpdateSubscription(string subscriberID, string TotalPrice, string CartID)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
                host = host_SB;
            }

            NVPCodec encoder = new NVPCodec();
            encoder["METHOD"] = "UpdateRecurringPaymentsProfile";
            encoder["PROFILEID"] = subscriberID;
            encoder["NOTE"] = "Update subscription from User";
            encoder["AMT"] = TotalPrice;
            encoder["DESC"] = "User Update subscription at store.";
            encoder["PROFILEREFERENCE"] = CartID;

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                return true;
            }
            else
            {
                var retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                throw new Exception(retMsg);
            }
        }

        public bool ShortcutExpressCheckout(string amt, ref string token, ref string retMsg, Dictionary<string, decimal> cartItems)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
                host = host_SB;
            }

            string baseURL = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;

            string returnURL = baseURL + VirtualPathUtility.ToAbsolute("~/WebPages/Checkout/CheckoutReview.aspx");
            //string returnURL = "http://localhost:50816/WebPages/Checkout/CheckoutReview.aspx";
            //string returnURL = "http://203.92.34.46:81/PrviiWebAppTest/WebPages/Checkout/CheckoutReview.aspx";
            string cancelURL = baseURL + VirtualPathUtility.ToAbsolute("~/WebPages/Checkout/CheckoutCancel.aspx");
            //string cancelURL ="http://localhost:50816/WebPages/Checkout/CheckoutCancel.aspx";
            // string cancelURL = "http://203.92.34.46:81/PrviiWebAppTest/WebPages/Checkout/CheckoutCancel.aspx";

            NVPCodec encoder = new NVPCodec();
            encoder["METHOD"] = "SetExpressCheckout";
            encoder["RETURNURL"] = returnURL;
            encoder["CANCELURL"] = cancelURL;
            encoder["BRANDNAME"] = "Prvii Celebrity Services";
            encoder["PAYMENTREQUEST_0_AMT"] = amt;
            encoder["PAYMENTREQUEST_0_ITEMAMT"] = amt;
            encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";
            encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "USD";

            // Get the Shopping Cart Products
            int i = 0;

            foreach (var entry in cartItems)
            {
                i++;

                encoder["L_PAYMENTREQUEST_0_NAME" + i] = entry.Key;
                encoder["L_PAYMENTREQUEST_0_AMT" + i] = entry.Value.ToString("#.##");
                encoder["L_PAYMENTREQUEST_0_QTY" + i] = "1";
            }

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                token = decoder["TOKEN"];
                string ECURL = "https://" + host + "/cgi-bin/webscr?cmd=_express-checkout" + "&token=" + token;
                retMsg = ECURL;
                return true;
            }
            else
            {
                retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];
                return false;
            }
        }

        public bool MarkExpressCheckoutNew(ref string token, ref string retMsg, List<Channel> cartItems, string ShoppingCartID)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
                host = host_SB;
            }

            string ServerUrl = WebConfigurationManager.AppSettings["ServerUrl"].ToString();

            string returnURL = ServerUrl + "/WebPages/ShoppingCarts/CheckoutComplete.aspx";
            string cancelURL = ServerUrl + "/WebPages/ShoppingCarts/CheckoutCancel.aspx";
            string ipnURL = ServerUrl + "/WebPages/ShoppingCarts/CheckoutIPNHandler.aspx";



            //  encoder["BRANDNAME"] = "";
            NVPCodec encoder = new NVPCodec();

            // Get the Shopping Cart Products
            int i = 0;
            double totalAmount = 0.00;
            foreach (var item in cartItems)
            {

                totalAmount = totalAmount + (Convert.ToDouble(item.Price.ToString("#.##")));
                encoder["L_PAYMENTREQUEST_0_NAME" + i] = item.Firstname + " " + item.Lastname;
                encoder["L_PAYMENTREQUEST_0_DESC" + i] = ((BillingCycleType)item.BillingCycleID).ToString();
                encoder["L_PAYMENTREQUEST_0_AMT" + i] = item.Price.ToString("#.##");
                encoder["L_PAYMENTREQUEST_0_QTY" + i] = "1";
                encoder["L_PAYMENTREQUEST_0_NUMBER" + i] = item.ID.ToString();
                encoder["L_BILLINGAGREEMENTDESCRIPTION" + i] = "RecurringSubscriptionDescription"; //item.Firstname + " " + item.Lastname; //
                encoder["L_BILLINGTYPE" + i] = "RecurringPayments";
                i++;
            }
            string itemTotalAmount = totalAmount.ToString("F");

            encoder["METHOD"] = "SetExpressCheckout";
            encoder["LANDINGPAGE"] = "Login";
            encoder["ALLOWNOTE"] = "0";
            encoder["RETURNURL"] = returnURL;
            encoder["CANCELURL"] = cancelURL;
            encoder["CURRENCYCODE"] = "USD";
            encoder["PAYMENTREQUEST_0_ITEMAMT"] = itemTotalAmount;
            encoder["PAYMENTREQUEST_0_SHIPPINGAMT"] = "0";
            encoder["PAYMENTREQUEST_0_AMT"] = itemTotalAmount;
            encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";
            encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "USD";
            encoder["BRANDNAME"] = "Prvii Celebrity Services";
            encoder["PAYMENTREQUEST_0_CUSTOM"] = ShoppingCartID;
            //encoder["PAYMENTREQUEST_n_INVNUM"] = "1002";
            //encoder["SOLUTIONTYPE"] = "Mark";
            




            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                token = decoder["TOKEN"];
                string ECURL = "https://" + host + "/cgi-bin/webscr?cmd=_express-checkout" + "&token=" + token;
                retMsg = ECURL;
                return true;
            }
            else
            {
                retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];
                return false;
            }
        }

        public bool MarkExpressCheckoutMobile(ref string token, ref string retMsg, List<Channel> cartItems,string ShoppingCartID)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
                host = host_SB;
            }

           // string ServerUrl = "http://203.92.34.35/prviiwebTest";// WebConfigurationManager.AppSettings["ServerUrl"].ToString();
            string ServerUrl = WebConfigurationManager.AppSettings["ServerUrl"].ToString();
            //string ServerUrl = "http://111.93.192.189/prvii";
       
            string returnURL = ServerUrl + "/WebPages/ShoppingCarts/CheckoutCompleteMobile.aspx";
            string cancelURL = ServerUrl + "/WebPages/ShoppingCarts/CheckoutCancelMobile.aspx";
            string ipnURL = ServerUrl + "/WebPages/ShoppingCarts/CheckoutIPNHandler.aspx";



            //  encoder["BRANDNAME"] = "";
            NVPCodec encoder = new NVPCodec();

            // Get the Shopping Cart Products
            int i = 0;
            double totalAmount = 0.00;
            foreach (var item in cartItems)
            {

                totalAmount = totalAmount + (Convert.ToDouble(item.Price.ToString("#.##")));
                encoder["L_PAYMENTREQUEST_0_NAME" + i] = item.Firstname + " " + item.Lastname;
                encoder["L_PAYMENTREQUEST_0_DESC" + i] = ((BillingCycleType)item.BillingCycleID).ToString();
                encoder["L_PAYMENTREQUEST_0_AMT" + i] = item.Price.ToString("#.##");
                encoder["L_PAYMENTREQUEST_0_QTY" + i] = "1";
                encoder["L_PAYMENTREQUEST_0_NUMBER" + i] = item.ID.ToString();
                encoder["L_BILLINGAGREEMENTDESCRIPTION" + i] = "RecurringSubscriptionDescription";//item.Firstname + " " + item.Lastname; //
                encoder["L_BILLINGTYPE" + i] = "RecurringPayments";
                i++;
            }
            string itemTotalAmount = totalAmount.ToString("F");

            encoder["METHOD"] = "SetExpressCheckout";
            encoder["LANDINGPAGE"] = "Billing";
            encoder["ALLOWNOTE"] = "0";
            encoder["RETURNURL"] = returnURL;
            encoder["CANCELURL"] = cancelURL;
            encoder["CURRENCYCODE"] = "USD";
            encoder["PAYMENTREQUEST_0_ITEMAMT"] = itemTotalAmount;
            encoder["PAYMENTREQUEST_0_SHIPPINGAMT"] = "0";
            encoder["PAYMENTREQUEST_0_AMT"] = itemTotalAmount;
            encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";
            encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "USD";
            encoder["BRANDNAME"] = "Prvii Celebrity Services";
            encoder["PAYMENTREQUEST_0_CUSTOM"] = ShoppingCartID;
           // encoder["PAYMENTREQUEST_n_INVNUM"] = "1002";





            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                token = decoder["TOKEN"];
                string ECURL = "https://" + host + "/cgi-bin/webscr?cmd=_express-checkout-mobile" + "&drt=''" + "&token=" + token;
                retMsg = ECURL;
                return true;
            }
            else
            {
                retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];
                return false;
            }
        }

        public bool GetCheckoutDetails(string token, ref string PayerID, ref NVPCodec decoder, ref string retMsg,ref string ShoppingCartID)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
            }

            NVPCodec encoder = new NVPCodec();
            encoder["METHOD"] = "GetExpressCheckoutDetails";
            encoder["TOKEN"] = token;

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                PayerID = decoder["PAYERID"];
                ShoppingCartID = decoder["PAYMENTREQUEST_0_CUSTOM"];
                return true;
            }
            else
            {
                retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                return false;
            }
        }


        public bool DoCheckoutPayment(string finalPaymentAmount, string token, string PayerID, ref NVPCodec decoder, ref string retMsg)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
            }

            NVPCodec encoder = new NVPCodec();
            encoder["METHOD"] = "DoExpressCheckoutPayment";
            encoder["TOKEN"] = token;
            encoder["PAYERID"] = PayerID;
            encoder["PAYMENTREQUEST_0_AMT"] = finalPaymentAmount;
            encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "USD";
            encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                return true;
            }
            else
            {
                retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                return false;
            }
        }

        public bool DoCheckoutPaymentNew(string cartID, string token, string PayerID, ref NVPCodec decoder, ref string retMsg)
        {
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
            }

            List<Channel> channel = ShoppingCartManager.GetItem(Convert.ToInt64(cartID));
            NVPCodec encoder = new NVPCodec();


            int OneTimeItemCount = 0;
            double totalAmount = 0.00;
            if (channel != null)
            {
                int i = 0;
                foreach (var item in channel)
                {
                    int quantity = 1;
                    totalAmount = totalAmount + (Convert.ToDouble(item.Price.ToString("#.##")) * quantity);
                    OneTimeItemCount++;
                    encoder["L_PAYMENTREQUEST_0_NAME" + i] = item.Firstname + ' ' + item.Lastname;
                    encoder["L_PAYMENTREQUEST_0_DESC" + i] = item.Firstname + ' ' + item.Lastname;
                    encoder["L_PAYMENTREQUEST_0_AMT" + i] = item.Price.ToString("#.##");
                    encoder["L_PAYMENTREQUEST_0_QTY" + i] = "1";
                    encoder["L_PAYMENTREQUEST_0_NUMBER" + i] = item.ID.ToString();

                    i++;
                }
            }

            string finalPaymentAmount = totalAmount.ToString("F");

            encoder["METHOD"] = "DoExpressCheckoutPayment";
            encoder["TOKEN"] = token;
            encoder["PAYERID"] = PayerID;
            encoder["ALLOWNOTE"] = "0";
            encoder["PAYMENTREQUEST_0_ITEMAMT"] = "0";
            encoder["PAYMENTREQUEST_0_SHIPPINGAMT"] = "0";
            encoder["PAYMENTREQUEST_0_AMT"] = finalPaymentAmount;
            encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";
            encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "USD";

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                return true;
            }
            else
            {
                retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                return false;
            }
        }



        public bool GetAccessToken(string Authcode)
        {
            NVPCodec encoder = new NVPCodec();
            encoder["Content-Type"] = "application/x-www-form-urlencoded";
            encoder["grant_type"] = "authorization_code";
            encoder["code"] = Authcode;
            encoder["redirect_uri"] = "urn:ietf:wg:oauth:2.0:oob";
            encoder["client_id"] = "AZAm1RAlrdDzXRBC5h8KPThw9BcdHzjtGs4lp1SQgl73PaR0wpVqveJimO0v";
            encoder["Secret"] = "EJ6YYxCbJ0Ny7L_NgqI0GV8Q-qdn85t4hjavpOAxQmau6Uj1nJiY0fYYEnnl";
          
            string pStrrequestforNvp = encoder.Encode();

            string url = "https://api.paypal.com/v1/oauth2/token";

            string strPost = pStrrequestforNvp + "&" + buildCredentialsNVPString();
            strPost = strPost + "&BUTTONSOURCE=" + HttpUtility.UrlEncode(BNCode);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Timeout = Timeout;
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;


            using (StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream()))
            {
                myWriter.Write(strPost);
            }


            //Retrieve the Response returned from the NVP API call to PayPal.
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            string result;
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(result);

            string strAck = decoder["access_token"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                return true;
            }
            else
            {
                var retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                throw new Exception(retMsg);
               
            }
        }

      

        public string HttpCall(string NvpRequest)
        {
            string url = pEndPointURL;

            string strPost = NvpRequest + "&" + buildCredentialsNVPString();
            strPost = strPost + "&BUTTONSOURCE=" + HttpUtility.UrlEncode(BNCode);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Timeout = Timeout;
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;


            using (StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream()))
            {
                myWriter.Write(strPost);
            }


            //Retrieve the Response returned from the NVP API call to PayPal.
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            string result;
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }

        private string buildCredentialsNVPString()
        {
            NVPCodec codec = new NVPCodec();

            if (!IsEmpty(APIUsername))
                codec["USER"] = APIUsername;

            if (!IsEmpty(APIPassword))
                codec[PWD] = APIPassword;

            if (!IsEmpty(APISignature))
                codec[SIGNATURE] = APISignature;

            if (!IsEmpty(Subject))
                codec["SUBJECT"] = Subject;

            codec["VERSION"] = "88.0";

            return codec.Encode();
        }

        public static bool IsEmpty(string s)
        {
            return s == null || s.Trim() == string.Empty;
        }


        public void GetRecurringPaymentProfileDetails(PayReconData recon)
        {
            bool boolAddEntry = true;
            string strProfileId = recon.ProfileId.Trim();
            if (bSandbox)
            {
                pEndPointURL = pEndPointURL_SB;
            }

            NVPCodec encoder = new NVPCodec();
            encoder["METHOD"] = "GetRecurringPaymentsProfileDetails";
            encoder["PROFILEID"] = strProfileId;

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);


            string strAck = decoder["ACK"].ToLower();
            if (strAck == "failure")
            {
                var retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                throw new Exception(retMsg);
            }
            else
            {
                string strResProfileId = decoder["PROFILEID"].ToLower();
                string strStatus = decoder["STATUS"].ToLower();
                short statusId = GetProfileStatus(strStatus);

                string strAmt = decoder["AGGREGATEAMOUNT"];
                decimal decAggAmt = 0.00M;
                if (strAmt != null)
                    decAggAmt = Convert.ToDecimal(strAmt);

                string strFinalPaymentDueDate = decoder["FINALPAYMENTDUEDATE"];
                DateTime dtFinalPayment = DateTime.Now;
                if (strFinalPaymentDueDate != null)
                    dtFinalPayment = Convert.ToDateTime(strFinalPaymentDueDate);

                string strProfileStartDate = decoder["PROFILESTARTDATE"];
                DateTime dtProfileStart = DateTime.Now;
                if (strProfileStartDate != null)
                    dtProfileStart = Convert.ToDateTime(strProfileStartDate);

                string strCycleAmt = decoder["AMT"];
                decimal decAmt = 0.00M;
                if (strAmt != null)
                    decAmt = Convert.ToDecimal(strCycleAmt);

                string strNextBillingDate = decoder["NEXTBILLINGDATE"];
                DateTime dtNextBilling = Convert.ToDateTime("0001-01-01 00:00:00.0000000");
                if (strProfileStartDate != null)
                    dtNextBilling = Convert.ToDateTime(strNextBillingDate);

                string strTotalCycles = decoder["TOTALBILLINGCYCLES"];
                int iTotalCycles = 0;
                if (strTotalCycles != null)
                    iTotalCycles = Convert.ToInt32(strTotalCycles);

                string strCyclesCompleted = decoder["NUMCYLESCOMPLETED"];
                int iCyclesCompleted = 0;
                if (strCyclesCompleted != null)
                    iCyclesCompleted = Convert.ToInt32(strCyclesCompleted);

                string strCyclesRemaining = decoder["NUMCYCLESREMAINING"];
                int iCyclesRemaining = 0;
                if (iTotalCycles > 0)
                {
                    if (strCyclesRemaining != null)
                        iCyclesRemaining = Convert.ToInt32(strCyclesRemaining);
                }


                string strLastPmtDate = decoder["LASTPAYMENTDATE"];
                DateTime dtLastPayment = DateTime.Now;
                if (strLastPmtDate != null)
                    dtLastPayment = Convert.ToDateTime(strLastPmtDate);

                string strLastAmt = decoder["LASTPAYMENTAMT"];
                decimal decLastAmt = 0.00M;
                if (strLastAmt != null)
                    decLastAmt = Convert.ToDecimal(strLastAmt);

                string strFailedPmtCount = decoder["FAILEDPAYMENTCOUNT"];
                int iFailedPmtCount = 0;
                if (strFailedPmtCount != null)
                    iFailedPmtCount = Convert.ToInt32(strFailedPmtCount);


                
                  using (PrviiEntities innerContext = new PrviiEntities())
                    {
                        try
                        {

                            var cart = innerContext.ShoppingCartItems.FirstOrDefault(x => x.PaymentProfileID == strProfileId);
                            cart.ProfileStatus = statusId;

                            if (recon.Id != 0)
                            {
                                var lastRecon = innerContext.PaymentRecons.FirstOrDefault(x => x.Id == recon.Id);
                                if(DateTime.Compare(Convert.ToDateTime(lastRecon.NextBillDate),dtNextBilling) == 0 && lastRecon.Status == statusId)
                                {
                                    boolAddEntry = false;
                                    lastRecon.ReconPick = true;
                                }
                                else
                                {
                                    boolAddEntry = true;
                                    lastRecon.ReconPick = false;
                                }
                                
                            }


                            if (boolAddEntry)
                            {
                                var payRecon = new PaymentRecon
                                {
                                    ProfileId = strProfileId,
                                    Status = statusId,
                                    CreatedOn = DateTime.Now,
                                    Currency = "USD",
                                    AggregateAmount = decAggAmt,
                                    FinalPaymentDueDate = dtFinalPayment
                                    ,
                                    ProfileStartDate = dtProfileStart,
                                    CycleAmount = decAmt,
                                    NextBillDate = dtNextBilling,
                                    TotalCycles = iTotalCycles,
                                    CyclesCompleted = iCyclesCompleted,
                                    CyclesRemaining = iCyclesRemaining
                                    ,
                                    FailedPmtCount = iFailedPmtCount,
                                    LastPmtDate = dtLastPayment,
                                    LastPmtAmt = decLastAmt,
                                    ReconPick = true
                                };

                                innerContext.Entry(payRecon).State = System.Data.Entity.EntityState.Added;
                            }

                          

                           

                            innerContext.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                        }
                    }

                

            }


           


            
            
        }

        public void GetTransactionDetails(TransactionReconDetail inReconDetail)
        {


            try
            {
                // DateTime StartDate = inReconDetail.TimeStamp.ToString();
                string StartDate = inReconDetail.TimeStamp.Value.Year + "-" + inReconDetail.TimeStamp.Value.Month + "-" + inReconDetail.TimeStamp.Value.Day + " " + inReconDetail.TimeStamp.Value.TimeOfDay.ToString().Substring(0, 8);
                String EndDate = DateTime.UtcNow.ToString("o");

                String strProfileID = inReconDetail.ProfileId;

                if (bSandbox)
                {
                    pEndPointURL = pEndPointURL_SB;
                }

                NVPCodec encoder = new NVPCodec();
                encoder["METHOD"] = "TransactionSearch";
                encoder["StartDate"] = StartDate;
                encoder["PROFILEID"] = strProfileID;
                encoder["VERSION"] = "109.0";

                string pStrrequestforNvp = encoder.Encode();
                string pStresponsenvp = HttpCall(pStrrequestforNvp);

                NVPCodec decoder = new NVPCodec();
                decoder.Decode(pStresponsenvp);


                string strAck = decoder["ACK"].ToLower();
                if (strAck == "failure")
                {
                    var retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                        "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                        "Desc2=" + decoder["L_LONGMESSAGE0"];

                    using (PrviiEntities context = new PrviiEntities())
                    {
                        try
                        {
                            //Create a new row for this error and log in database
                            var reconItem = new ReconErrorLog
                            {
                                ProfileId = strProfileID,
                                ErrorCode = decoder["L_ERRORCODE0"],
                                ErrorMessage = retMsg,
                                ReconDate = DateTime.UtcNow
                            };
                            context.Entry(reconItem).State = System.Data.Entity.EntityState.Added;
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                else
                {
                    int iCount = decoder.Count;
                    int iTranCount = 0;
                    for (int i = 0; i < iCount; i++)
                    {
                        if (decoder.Keys[i].Contains("TRANSACTION"))
                            iTranCount++;
                    }
                    for (int i = 0; i < iTranCount; i++)
                    {
                        string strTimeStamp = decoder["L_TIMESTAMP" + i.ToString()].ToLower();
                        DateTime dtTimeStamp = DateTime.UtcNow;
                        if (strTimeStamp != null)
                        {
                            dtTimeStamp = DateTimeOffset.Parse(strTimeStamp).UtcDateTime;

                        }

                        string strStatus = decoder["L_STATUS" + i.ToString()].ToLower();
                        short statusId = GetPaymentStatus(strStatus);

                        string strType = decoder["L_TYPE" + i.ToString()].ToLower();
                        int iType = GetTransactionType(strType);



                        string strTransactionId = decoder["L_TRANSACTIONID" + i.ToString()];


                        string strName = decoder["L_NAME" + i.ToString()];



                        string strAmt = decoder["L_AMT" + i.ToString()];
                        decimal decAmt = 0.00M;
                        if (strAmt != null)
                        {
                            decAmt = Convert.ToDecimal(strAmt);
                        }

                        string strFeeAmt = decoder["L_FEEAMT" + i.ToString()];
                        decimal decFeeAmt = 0.00M;
                        if (strFeeAmt != null)
                        {
                            decFeeAmt = 0 -  Convert.ToDecimal(strFeeAmt);
                        }

                        string strNetAmt = decoder["L_NETAMT" + i.ToString()];
                        decimal decNetAmt = 0.00M;
                        if (strNetAmt != null)
                        {
                            decNetAmt =  Convert.ToDecimal(strNetAmt);

                        }

                        long lChannelId = 0;
                        string ChannelName = "";
                        string PriceJson = GetPriceDistribution(strProfileID, decAmt, decNetAmt,decFeeAmt, ref lChannelId, ref ChannelName);

                        using (PrviiEntities innerContext = new PrviiEntities())
                        {
                            try
                            {

                                var transaction = innerContext.TransactionReconDetails.FirstOrDefault(x => x.TransactionId.Trim() == strTransactionId.Trim() && x.TimeStamp == dtTimeStamp);
                                if (transaction == null)
                                {
                                    //Create a new row for this transaction
                                    var reconItem = new TransactionReconDetail
                                    {
                                        ProfileId = strProfileID,
                                        TransactionId = strTransactionId,
                                        Status = statusId,
                                        GrossAmount = decAmt,
                                        NetAmount = decNetAmt,
                                        FeeAmount = decFeeAmt,
                                        EndDate = EndDate,
                                        TimeStamp = dtTimeStamp,
                                        PayerDisplayName = strName,
                                        Type = iType,
                                        PriceDistribution = PriceJson,
                                        ChannelId = lChannelId,
                                        ChannelName = ChannelName
                                        

                                    };
                                    innerContext.Entry(reconItem).State = System.Data.Entity.EntityState.Added;

                                    //Check for status 
                                    //if (statusId == Convert.ToInt16(TransactionStatus.Canceled) || statusId == Convert.ToInt16(TransactionStatus.Expired))
                                   // {
                                        var cart = innerContext.ShoppingCartItems.FirstOrDefault(x => x.PaymentProfileID == strProfileID);
                                        cart.ProfileStatus = statusId;
                                    //}

                                    //Save all changes
                                    innerContext.SaveChanges();
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }



        
        /// <summary>
        /// gets price distribution applicable for this celebrity
        /// </summary>
        /// <param name="strProfileID"></param>
        private string GetPriceDistribution(string strProfileID, decimal decAmt, decimal decNetAmt, decimal decFeeAmt, ref long ChannelId, ref string ChannelName)
        {
            using (PrviiEntities innerContext = new PrviiEntities())
            {
                try
                {
                    var cart = innerContext.ShoppingCartItems.FirstOrDefault(x => x.PaymentProfileID.Trim() == strProfileID.Trim());
                    long lChannelId = cart.ChannelID;
                    ChannelId = lChannelId;

                    var Channel = innerContext.Channels.FirstOrDefault(x=> x.ID == lChannelId);
                    ChannelName = Channel.Firstname + " " + Channel.Lastname;
                    string i = Channel.Email;
                    string m = Channel.PriceManagement;
                    List<PriceManagement> priceManagement = JsonConvert.DeserializeObject<List<PriceManagement>>(Channel.PriceManagement);


                   // string json ="[{ \"Id\": 0, \"AccountId\": 1,\"Distribution\": 80.00, \"Sequence\": 1, \"InnerDistribution\": [{\"Id\": 0, \"AccountId\": 7,\"Distribution\": 4.00,\"Sequence\": 7, \"ParentID\": 0 }] },{\"Id\": 0,\"AccountId\": 2,\"Distribution\": 2.00,\"Sequence\": 2,\"InnerDistribution\": []},{\"Id\": 0,\"AccountId\": 3,\"Distribution\": 3.00,\"Sequence\": 3,\"InnerDistribution\": []},{\"Id\": 0,\"AccountId\": 4,\"Distribution\": 3.00, \"Sequence\": 4,\"InnerDistribution\": []},{\"Id\": 0, \"AccountId\": 5,\"Distribution\": 2.00,\"Sequence\": 5, \"InnerDistribution\": []},{\"Id\": 0,\"AccountId\": 6,\"Distribution\": 10.00,\"Sequence\": 6,\"InnerDistribution\": []}]";
                    //List<PriceManagement> priceManagement1 = JsonConvert.DeserializeObject<List<PriceManagement>>(json);

                  //  {"Gross":100.00,"OperatingCosts":10.00,"NetGross":90.00,"CelebrityPayout":54.00,"ManagerPayout":10.00,"AgentPayout":6.00,"AssociatePayout":20.00,"ReferrerPayout":10.00,"EmployeePayout":20.00,"DirectorPool":30.50  } 
                    string PriceJson = "{\"Gross\":" + decAmt.ToString() + ",\"OperatingCosts\":"+ decFeeAmt.ToString() +",\"NetGross\":"+ decNetAmt.ToString() + ",";
                   // "CelebrityPayout":54.00,"ManagerPayout":10.00,"AgentPayout":6.00,
                    //"":20.00,"":10.00,"EmployeePayout":20.00,"":30.50  } 
                    decimal decCalcAmt = 0.00M;
                    decimal deCelebrityAmtF = 0.00M;

                    decimal deCelebrityAmt = 0.00M;
                     decimal decManagerAmt = 0.00M;
                    decimal decAgentAmt = 0.00M;
                    decimal decAssociateAmt = 0.00M;
                    decimal decRefeAmt = 0.00M; ;
                    decimal decEmpAmt = 0.00M;
                    foreach (PriceManagement manage in priceManagement)
                    {
                        if(manage.AccountId == 1) //Celebrity PAyout
                        {
                            deCelebrityAmtF = (manage.Distribution) * decNetAmt / 100;

                            foreach (PriceDistribution innerManage in manage.InnerDistribution)
                            {
                                if (innerManage.AccountId == 7) //Manager / AgentPayout
                                {
                                    decManagerAmt = (deCelebrityAmtF * innerManage.Distribution) / 100;
                                    deCelebrityAmt = deCelebrityAmtF - decManagerAmt;
                                    PriceJson = PriceJson + "\"CelebrityPayout\":" + deCelebrityAmt.ToString();

                                    PriceJson = PriceJson + ",\"ManagerPayout\":" + decManagerAmt.ToString();


                                }
                            }

                            break;
                        }
                    }
                    
                    foreach (PriceManagement manage in priceManagement)
                    {
                         if (manage.AccountId == 2) //Agent PAyout
                        {
                            decAgentAmt = ((deCelebrityAmtF) * manage.Distribution) / 100;
                            PriceJson = PriceJson + ",\"AgentPayout\":" + decAgentAmt.ToString();
                        }
                    }
                      
                   foreach (PriceManagement manage in priceManagement)
                    {
                         if (manage.AccountId == 3) //Associate PAyout
                        {
                            decAssociateAmt = ((deCelebrityAmtF) * manage.Distribution) / 100;
                            PriceJson = PriceJson + ",\"AssociatePayout\":" + decAssociateAmt.ToString();
                        }
                   }
                   
                    foreach (PriceManagement manage in priceManagement)
                    {
                       if (manage.AccountId == 4) //ReferrerPayout PAyout
                        {
                            decRefeAmt = ((deCelebrityAmtF ) * manage.Distribution) / 100;
                            PriceJson = PriceJson + ",\"ReferrerPayout\":" + decRefeAmt.ToString();
                        }
                    }
                     
                    foreach (PriceManagement manage in priceManagement)
                    {
                        if (manage.AccountId == 5) //Associate PAyout
                        {
                            decEmpAmt =( (deCelebrityAmtF) * manage.Distribution) / 100;
                            PriceJson = PriceJson + ",\"EmployeePayout\":" + decEmpAmt.ToString();
                        }
                    }
                     foreach (PriceManagement manage in priceManagement)
                    {
                         if (manage.AccountId == 6) //DirectorPool PAyout
                        {
                            decEmpAmt = (deCelebrityAmtF * manage.Distribution) / 100;
                            PriceJson = PriceJson + ",\"DirectorPool\":" + decEmpAmt.ToString();
                        }
                    }

                    PriceJson = PriceJson + "}";
                    return PriceJson;
                }
                catch (Exception ex)
                {
                    return "";
                    throw;
                }
            }
        }
       
        private int GetTransactionType(string strType)
        {
            switch (strType.Trim().ToLower())
            {
                case "recurring payment":
                    return Convert.ToInt16(TransactionType.RecurringPayment);
                case "temporary hold":
                    return Convert.ToInt16(TransactionType.TemporaryHold);

                default:
                    return 0;
            }
        }

        private short GetPaymentStatus(string strStatus)
        {
            switch (strStatus.Trim().ToLower())
            {
                case "pending":
                    return Convert.ToInt16(TransactionStatus.Pending);
                case "processing":
                    return Convert.ToInt16(TransactionStatus.Processing);
                case "completed":
                    return Convert.ToInt16(TransactionStatus.Completed);
                case "denied":
                    return Convert.ToInt16(TransactionStatus.Denied);
                case "reversed":
                    return Convert.ToInt16(TransactionStatus.Reversed);
                case "created":
                    return Convert.ToInt16(TransactionStatus.Created);
                case "canceled":
                    return Convert.ToInt16(TransactionStatus.Canceled);
                case "expired":
                    return Convert.ToInt16(TransactionStatus.Expired);
                default:
                    return 0;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private short GetProfileStatus(string status)
        {
            switch (status.Trim().ToLower())
            {
                case "active":
                    return Convert.ToInt16(PaypalProfileStatus.Active);
                case "cancelled":
                    return Convert.ToInt16(PaypalProfileStatus.Cancelled);
                case "canceled":
                    return Convert.ToInt16(PaypalProfileStatus.Cancelled);
                case "expired":
                    return Convert.ToInt16(PaypalProfileStatus.Expired);
                case "pending":
                    return Convert.ToInt16(PaypalProfileStatus.Pending);
                case "suspended":
                    return Convert.ToInt16(PaypalProfileStatus.Suspended);
                default:
                    return 0;
            }
        }
    }

    public sealed class NVPCodec : NameValueCollection
    {
        private const string AMPERSAND = "&";
        private const string EQUALS = "=";
        private static readonly char[] AMPERSAND_CHAR_ARRAY = AMPERSAND.ToCharArray();
        private static readonly char[] EQUALS_CHAR_ARRAY = EQUALS.ToCharArray();

        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            bool firstPair = true;
            foreach (string kv in AllKeys)
            {
                string name = HttpUtility.UrlEncode(kv);
                string value = HttpUtility.UrlEncode(this[kv]);
                if (!firstPair)
                {
                    sb.Append(AMPERSAND);
                }
                sb.Append(name).Append(EQUALS).Append(value);
                firstPair = false;
            }
            return sb.ToString();
        }

        public void Decode(string nvpstring)
        {
            Clear();
            foreach (string nvp in nvpstring.Split(AMPERSAND_CHAR_ARRAY))
            {
                string[] tokens = nvp.Split(EQUALS_CHAR_ARRAY);
                if (tokens.Length >= 2)
                {
                    string name = HttpUtility.UrlDecode(tokens[0]);
                    string value = HttpUtility.UrlDecode(tokens[1]);
                    Add(name, value);
                }
            }
        }

        public void Add(string name, string value, int index)
        {
            this.Add(GetArrayName(index, name), value);
        }

        public void Remove(string arrayName, int index)
        {
            this.Remove(GetArrayName(index, arrayName));
        }

        public string this[string name, int index]
        {
            get
            {
                return this[GetArrayName(index, name)];
            }
            set
            {
                this[GetArrayName(index, name)] = value;
            }
        }

        private static string GetArrayName(int index, string name)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index cannot be negative : " + index);
            }
            return name + index;
        }
    }
}