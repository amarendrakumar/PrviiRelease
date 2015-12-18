using Prvii.Entities;
using Prvii.Entities.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Prvii.Business
{
    public class EMailTemplateManager
    {
        private static string GetUserName(UserProfile user)
        {
            return String.IsNullOrEmpty(user.NickName) ? user.Firstname : user.NickName;
        }

        public static string GetForgotPasswordEmailBody(UserProfile user)
        {
            string webURL = WebConfigurationManager.AppSettings["ServerUrl"].ToString();

            var valueList = new Dictionary<string,string>();
            valueList.Add("{WEB_URL}", webURL);
            valueList.Add("{USER_FULL_NAME}", GetUserName(user));
            valueList.Add("{RETURN_URL}", webURL + "/WebPages/Users/UserPasswordReset.aspx?ID=" + UtilityManager.Encrypt(user.ID.ToString()));

            return BindTemplate(EMailTemplateType.Forgot_Password, valueList);
        }

        public static string GetWelcomeEmailBody(UserProfile user)
        {
            string webURL = WebConfigurationManager.AppSettings["ServerUrl"].ToString();
            bool appleMobileApp = Convert.ToBoolean(WebConfigurationManager.AppSettings["APPLE_MOBILE_APP_ENABLED"].ToString());
            var valueList = new Dictionary<string, string>();
            valueList.Add("{WEB_URL}", webURL);
            valueList.Add("{USER_FULL_NAME}", GetUserName(user));

            string mobileAppStore = string.Empty;

            DeviceType devType = (DeviceType)user.DeviceTypeID;

            if (devType == DeviceType.Android)
            {
                // for live server 
                mobileAppStore = "<a href='https://play.google.com/store/apps/details?id=io.cordova.PrviiCelebrityServices&hl=en' target='_blank'> Android Mobile play Store</a>";
               
                //for UAT 
                // mobileAppStore = "<a href='https://play.google.com/store/apps/details?id=io.cordova.PrviiMobile&hl=en' target='_blank'> Android Mobile play Store</a>";
            }
            else if (devType == DeviceType.Apple && appleMobileApp)
                mobileAppStore = "<a href='http://store.apple.com/us' target='_blank'> Apple Store</a>";

            valueList.Add("{MOBILE_APP_STORE}", mobileAppStore);

            return BindTemplate(EMailTemplateType.Subscription_Welcome_Message, valueList);
        }

        public static string GetSubscriptionEmailBody(UserProfile user, Channel channel)
        {
            string webURL = WebConfigurationManager.AppSettings["ServerUrl"].ToString();
            var valueList = new Dictionary<string, string>();
            valueList.Add("{WEB_URL}", webURL);
            valueList.Add("{USER_FULL_NAME}", GetUserName(user));
            
            valueList.Add("{CELEBRITY_IMAGE_URL}", webURL + "/WebPages/GetChannelMedia.aspx?ChannelID=" + channel.ID);
            valueList.Add("{CELEBRITY_PAGE_URL}", webURL + "/WebPages/Channels/ChannelDetailsView.aspx?ID=" + channel.ID);

            string channelName = channel.Firstname + " " + channel.Lastname;
            string channelurl = webURL + "/WebPages/Login.aspx" + "?ReturnURL=" + webURL + "/WebPages/Channels/ChannelDetailsView.aspx?ID=" + channel.ID;
            string channelLink = "<a href='" + channelurl + "'>" + channelName + "</a>";

            valueList.Add("{CELEBRITY_NAME}", channelLink);
            //GET MEDIS URL
            var mediaURLList = ChannelManager.GetMediaList(channel.ID, ChannelMediaType.Welcome_Media);

            StringBuilder sbMediaList = new StringBuilder();

            if (mediaURLList.Any())
            {
                sbMediaList.Append("As a privilege member of my " + channelName + " fan Club, please visit the welcome message below<br /> <br />");

                foreach (var item in mediaURLList)
                {
                    sbMediaList.Append("<a href='" + webURL + "/WebPages/ViewChannelMediaURL.aspx?ID=" + item.ID + "'>" + item.Name + "</a><br /><br />");
                }
            }

            valueList.Add("{WELCOME_MESSAGE}", sbMediaList.ToString());
            
            return BindTemplate(EMailTemplateType.Subscription_Message, valueList);
        }

        public static string GetMessageEmailBody(UserProfile user, ChannelMessage message, Channel channel)
        {
            string webURL = WebConfigurationManager.AppSettings["ServerUrl"].ToString();
            List<ChannelMedia> attachments = null;
            using(PrviiEntities context = new PrviiEntities())
            {
                attachments = context.ChannelMedias.Where(x => x.ChannelMessageAttachments.Any(y => y.ChannelMessageID == message.ID && y.IsActive)).ToList();
            }

            string celebrityName = channel.Firstname + " " + channel.Lastname;
            StringBuilder sbMediaList = new StringBuilder();

            foreach (var item in attachments)
            {
                sbMediaList.Append("<a href='" + webURL + "/WebPages/ViewChannelMediaURL.aspx?ID=" + item.ID + "'>" + item.Name + "</a><br /><br />");
            }

            return "Hi " + GetUserName(user) + ",<br /><br />Subject : " + message.Subject + "<br /><br />"
               + "Message from : " + celebrityName + "<br /><br />"
               + message.EmailMessage + "<br /><br />"
                + "Media : " + sbMediaList + "<br /><br />"
               + "With Regards, <br />" + celebrityName;
        }



        public static string GetMessageSMSBody(UserProfile user, ChannelMessage message, Channel channel)
        {
            string webURL = WebConfigurationManager.AppSettings["ServerUrl"].ToString();
            List<ChannelMedia> attachments = null;
            using (PrviiEntities context = new PrviiEntities())
            {
                attachments = context.ChannelMedias.Where(x => x.ChannelMessageAttachments.Any(y => y.ChannelMessageID == message.ID && y.IsActive)).ToList();
            }

            string celebrityName = channel.Firstname + " " + channel.Lastname;
            StringBuilder sbMediaList = new StringBuilder();

            foreach (var item in attachments)
            {
                sbMediaList.Append("<a href='" + webURL + "/WebPages/ViewChannelMediaURL.aspx?ID=" + item.ID + "'>" + item.Name + "</a><br /><br />");
            }

            return "Hi " + GetUserName(user) +  "  Message from : " + celebrityName + "    "
               + message.Message + ""
                //+ "Media : " + sbMediaList + "<br /><br />"
               + "With Regards," + celebrityName;
        }


        public static string[] GetMessageMedia(UserProfile user, ChannelMessage message, Channel channel)
        {
            string webURL = WebConfigurationManager.AppSettings["ServerUrl"].ToString();
            List<ChannelMedia> attachments = null;
            using (PrviiEntities context = new PrviiEntities())
            {
                attachments = context.ChannelMedias.Where(x => x.ChannelMessageAttachments.Any(y => y.ChannelMessageID == message.ID && y.IsActive)).ToList();
            }

            string celebrityName = channel.Firstname + " " + channel.Lastname;
           List<String> sbMediaList = new  List<String>();

            foreach (var item in attachments)
            {
                sbMediaList.Add(webURL + "/WebPages/ViewChannelMediaURL.aspx?ID=" + item.ID + "'>" + item.Name );
            }

            return sbMediaList.ToArray();
        }

        public static string[] GetMMSMessageMedia(UserProfile user, ChannelMessage message, Channel channel)
        {
            string webURL = WebConfigurationManager.AppSettings["ServerUrl"].ToString();
            List<ChannelMedia> attachments = null;
            using (PrviiEntities context = new PrviiEntities())
            {
                attachments = context.ChannelMedias.Where(x => x.ChannelMessageAttachments.Any(y => y.ChannelMessageID == message.ID && y.IsActive)).ToList();
            }

            string celebrityName = channel.Firstname + " " + channel.Lastname;
            List<String> sbMediaList = new List<String>();

            foreach (var item in attachments)
            {
               // sbMediaList.Add(webURL + "/WebPages/MMSChannelMediaURL.aspx?ID=" + item.ID);
                sbMediaList.Add(webURL + "/WebPages/GetChannelMedia.aspx?ID=" + item.ID);
                
            }

            return sbMediaList.ToArray();
        }




        private static string BindTemplate(EMailTemplateType templateType, Dictionary<string,string> valueList)
        {
            string emailBody = ReadTemplate(templateType);

            foreach(var item in valueList)
            {
                emailBody = emailBody.Replace(item.Key, item.Value);
            }

            return emailBody;
        }

        private static string ReadTemplate(EMailTemplateType templateType)
        {
            HttpContext httpcontext = HttpContext.Current;
            string templatePath = GetTemplatePath(templateType);
            using (StreamReader reader = new StreamReader(httpcontext.Server.MapPath(templatePath)))
            {
                return reader.ReadToEnd();
            }
        }

        private static string GetTemplatePath(EMailTemplateType templateType)
        {
            string filePath = "";

            switch (templateType)
            {
                case EMailTemplateType.Feedback:
                    filePath = "~/EmailTemplates/Feedback.htm";
                    break;
                case EMailTemplateType.Forgot_Password:
                    filePath = "~/EmailTemplates/ForgetPassword.htm";
                    break;
                case EMailTemplateType.Scheduled_Message:
                    filePath = "~/EmailTemplates/ScheduledMessage.htm";
                    break;
                case EMailTemplateType.Subscription_Message:
                    filePath = "~/EmailTemplates/SubscriptionMessage.htm";
                    break;
                case EMailTemplateType.Subscription_Welcome_Message:
                    filePath = "~/EmailTemplates/SubscriptionWelcomeMessage.htm";
                    break;
            }

            return filePath;
        }

    }
}
