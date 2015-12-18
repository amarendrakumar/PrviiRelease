using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections;
using Prvii.Business;
using Prvii.Entities;
using Prvii.BusinessService.Models;
using System.IO;
using System.Net.Http.Headers;
using Prvii.Entities.DataEntities;
using Prvii.Entities.Enumerations;
using Newtonsoft.Json;

namespace Prvii.BusinessService.Controllers
{
    public class ShoppingCartController : ApiController
    {

        [HttpPost]
        public long CreateRecurringPaymentsProfile(ShoppingCartDTO shoppingcart)
        {
            PayPalManager paypal = new PayPalManager();
            string retMsg = "";
            string token = "";
            string payerId = "";
            string ShoppingCartID = "";
            long channelID = 0;
            
            token = shoppingcart.Token;
            paypal.GetAccessToken(token);
            string cartID = Convert.ToString(shoppingcart.CartID);
            NVPCodec nvpResponse = new NVPCodec();
            bool result = paypal.GetCheckoutDetails(token, ref payerId, ref nvpResponse, ref retMsg, ref ShoppingCartID);
            if (result)
            {
                shoppingcart.payerId = payerId;
            }
            paypal.DoCheckoutPaymentNew(cartID, token, shoppingcart.payerId, ref nvpResponse, ref retMsg);
            CreateRecurringPaymentsProfile(cartID, token, shoppingcart.payerId);
            List<Channel> channel = ShoppingCartManager.GetItem(Convert.ToInt64(cartID));
            if (channel != null)
            {
                channelID = channel.Select(s => s.ID).FirstOrDefault();
            }
            return channelID;
        }

        protected void CreateRecurringPaymentsProfile(string cartID, string token, string PayerId)
        {
            List<Channel> channel = ShoppingCartManager.GetItem(Convert.ToInt64(cartID));
            PayPalManager paypal = new PayPalManager();
            if (channel != null)
            {
                foreach (var item in channel)
                {
                    string retMsg = "", ProfileStatus = "", ProfileID = "";
                    string shoppingcartID = cartID;
                    string BillingFrequenc = "1";

                    if (item.BillingCycleID == (short)BillingCycleType.Quarterly)
                        BillingFrequenc = "3";

                    bool isSuccess = paypal.CreateRecurringPaymentsProfile(token, ref PayerId, ref ProfileID, ref retMsg, ref ProfileStatus, item);
                    if (isSuccess)
                    {

                        ShoppingCartManager.UpdatePaymentNew(Convert.ToInt64(shoppingcartID), Convert.ToInt64(item.ID), item.Price, PayerId, ProfileID, item.BillingCycleID, Convert.ToInt16(BillingFrequenc), 0, ProfileStatus, retMsg, (short)PaymentStatus.Payment_Completed);
                    }
                    else
                    {
                        ShoppingCartManager.UpdatePaymentNew(Convert.ToInt64(shoppingcartID), Convert.ToInt64(item.ID), item.Price, PayerId, ProfileID, item.BillingCycleID, Convert.ToInt16(BillingFrequenc), 0, ProfileStatus, retMsg, (short)PaymentStatus.Payment_Cancelled);
                    }
                }
            }
        }

        [HttpPost]
        public long AddShoppingCartItem(ShoppingCartDTO shoppingCart)
        {
            long cartID = 0;
            var cart = ShoppingCartManager.GetCart(shoppingCart.UserID);
            if (cart != null)
            {
                if (cart.ID != 0)
                {
                    ShoppingCartManager.AddItem(cart.ID, shoppingCart.ChannelID);
                    cartID = cart.ID;
                }
                else
                {
                    var cartData = new ShoppingCart { UserID = shoppingCart.UserID, SessionID = "Mobile - " + DateTime.Now.Ticks.ToString(), CreatedOn = DateTime.UtcNow };

                    ShoppingCartManager.AddItem(cartData, shoppingCart.ChannelID);
                    cartID = cartData.ID;
                }
               
            }
            else
            {
                var cartData = new ShoppingCart { UserID = shoppingCart.UserID, SessionID = "Mobile - " + DateTime.Now.Ticks.ToString(), CreatedOn = DateTime.UtcNow };

                ShoppingCartManager.AddItem(cartData, shoppingCart.ChannelID);
                cartID = cartData.ID;
            }

            return cartID;
        }
         [HttpPost]
        public void ShoppingCartRemoveItems(ShoppingCartDTO shoppingcart)
        {
            if (shoppingcart.itemList.Count != 0)
                ShoppingCartManager.RemoveItems(shoppingcart.CartID, shoppingcart.itemList);
        }

        [HttpPost]
         public IList BindCart(ShoppingCartDTO shoppingcart)
         {
             var cart = ShoppingCartManager.GetCart(shoppingcart.UserID);
             if (cart!=null)
             {
                 if (cart.ID != 0)
                 {
                     return ShoppingCartManager.GetCartItems(cart.ID);
                 }
                 else
                 {

                     return null;
                 }
             }
            else
             {
                 return null;
             }
             
         }

         [HttpPost]
        public string  createAccessToken(ShoppingCartDTO shoppingcart)
        {
            string postcontents = string.Format("client_id={0}&client_secret={1}&grant_type=authorization_code&redirect_uri={2}&code={3}"
                                      , System.Web.HttpUtility.UrlEncode(shoppingcart.clientId)
                                      , System.Web.HttpUtility.UrlEncode(shoppingcart.secret)
                                      , System.Web.HttpUtility.UrlEncode(shoppingcart.url)
                                      , System.Web.HttpUtility.UrlEncode(shoppingcart.code));


            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://api.sandbox.paypal.com/v1/identity/openidconnect/tokenservice");
            request.Credentials = new NetworkCredential(shoppingcart.clientId, shoppingcart.secret);
            request.Method = "POST";
            byte[] postcontentsArray = System.Text.Encoding.UTF8.GetBytes(postcontents);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postcontentsArray.Length;
            //OAuth.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(postcontentsArray, 0, postcontentsArray.Length);
                requestStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    responseStream.Close();
                    response.Close();
                    // return SerializeToken(responseFromServer);
                    dynamic dynObj = JsonConvert.DeserializeObject(responseFromServer);
                    string token = dynObj["access_token"];
                    //token = ser.Deserialize<ImportContacts._Default.GoogleOAuthToken>(responseFromServer);
                    return token;
                }
            }
        }

         [HttpPost]
         public ShoppingCartDTO ShoppingCartCheckout(ShoppingCartDTO shoppingcart)
         {
             List<Channel> channel = ShoppingCartManager.GetItem(shoppingcart.CartID).ToList();

             string token = "", expressCheckoutURL = "";
             PayPalManager payPal = new PayPalManager();
             bool result = payPal.MarkExpressCheckoutMobile(ref token, ref expressCheckoutURL, channel, Convert.ToString(shoppingcart.CartID));
             if (result)
             {
                 return new ShoppingCartDTO
                 {
                     Token = token,
                     ExpressCheckoutURL = expressCheckoutURL
                 };
               
             }
             else
             {
                 return new ShoppingCartDTO
                 {
                     Token = token,
                     ExpressCheckoutURL = expressCheckoutURL
                 };
             }

            
         }

         [HttpPost]
         public long GetShoppingCartChannelID(ShoppingCartDTO shoppingcart)
         {
             List<Channel> channel = ShoppingCartManager.GetItem(shoppingcart.CartID).ToList();
             if (channel != null)
             {
                 return channel[0].ID;
             }
             else
             {
                 return 0;
             }
         }


         [HttpPost]
         public bool ChannelUnsubscribe(ShoppingCartDTO shoppingcart)
         {
             ChannelManager.ChannelUnsubscribe(Convert.ToInt64(shoppingcart.ChannelID), shoppingcart.UserID);
             return true;
         }


         [HttpPost]
         public bool subscribedByIos(ShoppingCartDTO shoppingcart)
         {
             var cartID = AddShoppingCartItem(shoppingcart);
             List<Channel> channel = ShoppingCartManager.GetItem(Convert.ToInt64(cartID));

            
             if (channel != null)
             {
                 foreach (var item in channel)
                 {
                     
                     string retMsg = "", ProfileStatus = "", ProfileID = "";
                     string shoppingcartID = cartID.ToString();
                     string BillingFrequenc = "1";

                     if (item.BillingCycleID == (short)BillingCycleType.Quarterly)
                         BillingFrequenc = "3";

                     ProfileStatus = "3";
                     ProfileID = "Ios";
                     retMsg = "Ios Payment";
                     ShoppingCartManager.UpdatePaymentNewAPPle(Convert.ToInt64(shoppingcartID), Convert.ToInt64(item.ID), item.Price, "", ProfileID, item.BillingCycleID, Convert.ToInt16(BillingFrequenc), item.BillingCycleID, ProfileStatus, retMsg, (short)PaymentStatus.Payment_Completed);
                     ShoppingCartManager.IosUpdateTransactionDetails(shoppingcart.ChannelID, shoppingcart.UserID, shoppingcart.ResponseJSON);
                 }
             }
             return true;
         }

         [HttpPost]
         public bool IosTransactionDetails(InAppPurchaseDTO objInAppPurchaseDTO)
         {
             ShoppingCartManager.IosUpdateTransactionDetails(objInAppPurchaseDTO.channelID, objInAppPurchaseDTO.subscriberId, objInAppPurchaseDTO.Resultjson);
             return true;
         }
    }
}
