using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Security.Authentication;
using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using PushSharp.Core;


namespace Prvii.Business
{
    public class AndroidGCMPushNotification
    {

        public static string SendNotification(string deviceId, string message)
        {
            // string deviceId = "aed4a629ee0fb379e6a41d4b32a49b3b12d0d036dc459c357d9fe26a7af6971e";
            string GoogleAppID = "AIzaSyD9q9e6y8cFeyXmmZ7iJWjqC6NapjWCf20";
            var SENDER_ID = "744011430246";
            var value = message;
            WebRequest tRequest;
            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));

            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

            string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" +
                //System.DateTime.Now.ToString() + "®istration_id=" + deviceId + "";
             System.DateTime.Now.ToString() + "&registration_id=" + deviceId + "";
            Console.WriteLine(postData);
            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;

            Stream dataStream = tRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse tResponse = tRequest.GetResponse();

            dataStream = tResponse.GetResponseStream();

            StreamReader tReader = new StreamReader(dataStream);

            String sResponseFromServer = tReader.ReadToEnd();

            tReader.Close();
            dataStream.Close();
            tResponse.Close();
            return sResponseFromServer;
        }

      

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public static void pushMessage(string deviceID)
        {
            int port = 2195;
           // String hostname = "gateway.push.apple.com";
            String hostname = "gateway.sandbox.push.apple.com";
          //  String certificatePath = System.Web.Hosting.HostingEnvironment.MapPath("pushkey.p12");
            String certificatePath = System.Web.HttpContext.Current.Server.MapPath("pushkeyDev.p12");
            X509Certificate2 clientCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificatePath), "Globrin@123");
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

            TcpClient client = new TcpClient(hostname, port);
            SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);

            try
            {
                sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
                MemoryStream memoryStream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(memoryStream);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)32);

                writer.Write(HexStringToByteArray(deviceID.ToUpper()));
                String payload = "{\"aps\":{\"alert\":\"" + "Hi,, This Is a Sample Push Notification For IPhone.." + "\",\"badge\":1,\"sound\":\"default\"}}";
                writer.Write((byte)0);
                writer.Write((byte)payload.Length);
                byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
                writer.Write(b1);
                writer.Flush();
                byte[] array = memoryStream.ToArray();
                sslStream.Write(array);
                sslStream.Flush();
                client.Close();
            }
            catch (System.Security.Authentication.AuthenticationException ex)
            {
                client.Close();
            }
            catch (Exception e)
            {
                client.Close();
            }
        }

       

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }



        

    }
}
