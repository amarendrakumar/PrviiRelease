using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using System.IO;
using Prvii.ExceptionHandling;
using Prvii.BusinessService.Models;
using System.Collections;
using Prvii.Business;
using Prvii.Entities;
using System.Net.Http.Headers;
using Prvii.Entities.DataEntities;
using Prvii.Entities.Enumerations;

namespace Prvii.BusinessService.Controllers
{
    public class InAppPurchaseController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage ValidateReceipt(InAppPurchaseDTO objInAppPurchaseDTO)
        {
            //string returnmessage1 = "API call";
            //string logfilePath1 = "C:\\Inapp\\log1.txt";
            //ExceptionHandler.LogMessage(returnmessage1, true, 10240, logfilePath1);

            string returnmessage = "";
            try
            {
                //var json = new JObject(new JProperty("receipt-data", objInAppPurchaseDTO.receiptData), new JProperty("password", "e4c4e01d7a9b43e9953cfa4b421e35ca")).ToString();
                var json = new JObject(new JProperty("receipt-data", objInAppPurchaseDTO.receiptData), new JProperty("password", "b8e6330c86b54970a9943adbe4c1c7ee")).ToString();
                
                ASCIIEncoding ascii = new ASCIIEncoding();
                byte[] postBytes = Encoding.UTF8.GetBytes(json);

                //  HttpWebRequest request;
                var request = System.Net.HttpWebRequest.Create("https://sandbox.itunes.apple.com/verifyReceipt");
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = postBytes.Length;

                //Stream postStream = request.GetRequestStream();
                //postStream.Write(postBytes, 0, postBytes.Length);
                //postStream.Close();

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postBytes, 0, postBytes.Length);
                    stream.Flush();
                }

                //  var sendresponse = (HttpWebResponse)request.GetResponse();

                var sendresponse = request.GetResponse();

                string sendresponsetext = "";
                using (var streamReader = new StreamReader(sendresponse.GetResponseStream()))
                {
                    sendresponsetext = streamReader.ReadToEnd().Trim();
                }
                returnmessage = sendresponsetext;
                string logfilePath = "C:\\Inapp\\log.txt";
                ExceptionHandler.LogMessage(returnmessage, true, 10240, logfilePath);
            }
            catch (Exception ex)
            {
                returnmessage = ex.Message.ToString();
            }
           
            return Request.CreateResponse(HttpStatusCode.OK, returnmessage);
        }

        [HttpGet]
        public HttpResponseMessage ValidateReceipt1()
        {
            //string returnmessage = "API call";
            //string logfilePath = "C:\\Inapp\\log.txt";
            //ExceptionHandler.LogMessage(returnmessage, true, 10240, logfilePath);
            return Request.CreateResponse(HttpStatusCode.OK, "Good Result");
        }

        [HttpPost]
        public IEnumerable<ChannelDTO> GetIosChannelList(UserProfileDTO userProfile)
        {
            List<ChannelData> channelList = null;
            channelList = ChannelManager.GetCelebrityList();
            return channelList.Select(ch => new ChannelDTO
            {
                ID = ch.ID,
                Name = ch.Name,
                Email = ch.Email,
                Phone = ch.Phone,
                Price = ch.Price,
                StatusID = ch.StatusID
            }).ToList();
        }
    }
}
