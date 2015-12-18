using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Entities;
using Prvii.Business;
using Prvii.Entities.Enumerations;
using Prvii.Entities.DataEntities;


namespace Prvii.Web.WebPages.ChannelMessages
{
    
    public partial class ChannelMessageDetails : BasePage
    {

       
        protected void Page_Load(object sender, EventArgs e)
        {
         
            if(!Page.IsPostBack)
            {
                this.litCelebrityName.Text = ChannelManager.GetCelebrityName(this.ChannelID);

                if (this.UserSesssion.UserRole == Role.Subscriber)
                {
                    this.tblMessage.Visible = false;
                    this.tblSubscriberMessage.Visible = true;
                    this.BindMastersForSubscriber();                    
                    this.BindMessageForSubscriber();
                    
                }
                else
                {
                    this.tblMessage.Visible = true;
                    this.tblSubscriberMessage.Visible = false;
                    //this.BindMasters();
                    this.BindMediaURLs();
                   
                    if (this.MessageID != 0)
                    {
                        this.BindMessage();
                        checkValidation();
                       

                    }
                    else
                    {
                        this.tbScheduleDate.Attributes["min"] = DateTime.Now.ToString("s");
                    }
                }
               
            }
        }

      

        private void BindMasters()
        {
            //this.cblSendBy.DataSource = this.GetEnumList<DeliveryMechanism>();
            //this.cblSendBy.DataBind();
        }

        private void BindMediaURLs()
        {
            var mediaUrlList = ChannelManager.GetMediaList(this.ChannelID, ChannelMediaType.Media_URL);

            this.cblMediaURLs.DataSource = mediaUrlList;
            this.cblMediaURLs.DataTextField = "Name";
            this.cblMediaURLs.DataValueField = "ID";
            this.cblMediaURLs.DataBind();
        }

        private void BindMastersForSubscriber()
        {
            this.cblSendBySubscriber.DataSource = this.GetEnumList<DeliveryMechanism>();
            this.cblSendBySubscriber.DataBind();
        }

       

        protected void rblSendTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if(Convert.ToBoolean(Convert.ToInt16(this.rblSendTo.SelectedValue)))
            {
                this.trSubscribers.Visible = false;
            }
            else
            {
                this.BindSubscribers();
                this.trSubscribers.Visible = true;
            }
        }

        private void BindSubscribers()
        {
            this.cblSubscribers.DataSource = ChannelManager.GetSubscribers(this.ChannelID);
            this.cblSubscribers.DataTextField = "Name";
            this.cblSubscribers.DataValueField = "ID";
            this.cblSubscribers.DataBind();
        }


        private void BindSubscribersGridView(List<long> ListItem, List<long> ListItemText)
        {
            if (ListItem.Count > 0)
                divEmail.Visible = true;
            else
                divEmail.Visible = false;

            var list = ChannelManager.GetSubscribers1(this.ChannelID, ListItem);
            this.ckUserList.DataSource = list;
            this.ckUserList.DataTextField = "Name";
            this.ckUserList.DataValueField = "ID";
            this.ckUserList.DataBind();
            foreach (ListItem item in   this.ckUserList.Items)
            {
                item.Selected = true;
            }

            if (ListItemText.Count > 0)
                divText.Visible = true;
            else
                divText.Visible = false;

            var listText = ChannelManager.GetSubscribers1(this.ChannelID, ListItemText);
            this.ckUserListText.DataSource = listText;
            this.ckUserListText.DataTextField = "Name";
            this.ckUserListText.DataValueField = "ID";
            this.ckUserListText.DataBind();
            foreach (ListItem item in this.ckUserListText.Items)
            {
                item.Selected = true;
            }
            
        }


        protected void rblSendOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.trScheduleOn.Visible = Convert.ToBoolean(Convert.ToInt16(this.rblSendOption.SelectedValue));
        }

        private void BindMessage()
        {
            ChannelMessage message = ChannelMessageManager.GetByID(this.MessageID);

            if (message.StatusID == (short)ChannelMessageStatus.Sent || message.StatusID == (short)ChannelMessageStatus.Sending)
            {
                this.btnApprove.Visible = false;
                this.btnSave.Visible = false;
            }
            else if (message.StatusID == (short)ChannelMessageStatus.Approved)
            {
                this.btnApprove.Visible = false;
            }
            
            this.rblSendTo.SelectedValue = Convert.ToInt16(message.SendToAll).ToString();
            this.tbSubject.Text = message.Subject;
            this.tbEmailMessage.Text = message.EmailMessage;
            this.tbMessage.Text = message.Message;
            this.BindSendMethods(message);
            this.rblSendOption.SelectedValue = Convert.ToInt16(message.IsScheduled).ToString();
            if(message.IsScheduled)
            {
                this.trScheduleOn.Visible = true;

                string timeZone = ChannelManager.GetTimeZone(this.ChannelID);
                var scheduleDate = UtilityManager.GetZoneSpecificTimeFromUTC(message.ScheduledOn, timeZone);
                this.tbScheduleDate.Text = scheduleDate.ToString("yyyy-MM-dd");
                this.tbScheduleTime.Text = scheduleDate.ToString("hh:mm tt");
            }

            //media urls
            var mediaIDList = ChannelMessageManager.GetAttachments(message.ID);
            this.SetMediaURLs(mediaIDList);

            this.lblStatus.Text = ((ChannelMessageStatus)message.StatusID).ToString();
            this.trStatus.Visible = true;

            //subscribers
            if (!message.SendToAll)
            {
                var subscriberIDList = ChannelMessageManager.GetMessageSubscribers(message.ID);
                this.BindSubscribers();
                this.SetSubscribers(subscriberIDList);
                this.trSubscribers.Visible = true;
            }
            else //If Send To All
            {
 
            }
        }

        private void BindMessageForSubscriber()
        {
            ChannelMessage message = ChannelMessageManager.GetByID(this.MessageID);

            this.tbSubjectSubscriber.Text = message.Subject;
            this.tbEmailMessageSubscriber.Text = message.EmailMessage;
            this.tbMEssageSubscriber.Text = message.Message;
            this.BindSendMethodsforSubcriber(message);


            string timeZone = ChannelManager.GetTimeZone(this.ChannelID);
            var scheduleDate = UtilityManager.GetZoneSpecificTimeFromUTC(message.ScheduledOn, timeZone);
            this.lblSendOn.Text = scheduleDate.ToString("yyyy-MM-dd hh:mm tt");

            //media urls
            var mediaIDList = ChannelMessageManager.GetAttachments(message.ID);
            var mediaUrlList = ChannelManager.GetMediaList(this.ChannelID, ChannelMediaType.Media_URL);
            var newlist = mediaUrlList.Where(a => mediaIDList.Contains(a.ID)).ToList();
            this.gvWelcomeMediaList.DataSource = newlist;
            this.gvWelcomeMediaList.DataBind();


        }

        private void BindSendMethods(ChannelMessage message)
        {
            if(message.SendByEmail)
            {
                cblSendByEMail.Checked = true;
            }

            if (message.SendBySMS)
            {
                cblSendBySMS.Checked = true;
            }

            //foreach (ListItem item in this.cblSendBy.Items)
            //{
            //    if (item.Text == DeliveryMechanism.Email.ToString() && message.SendByEmail)
            //        item.Selected = true;

            //    if (item.Text == DeliveryMechanism.Text.ToString() && message.SendBySMS)
            //        item.Selected = true;
            //}
        }
        private void BindSendMethodsforSubcriber(ChannelMessage message)
        {
            foreach (ListItem item in this.cblSendBySubscriber.Items)
            {
                if (item.Text == DeliveryMechanism.Email.ToString() && message.SendByEmail)
                    item.Selected = true;

                if (item.Text == DeliveryMechanism.Text.ToString() && message.SendBySMS)
                    item.Selected = true;
            }
        }
        private void SetMediaURLs(List<long> meadiaIDList)
        {
            foreach(ListItem item in this.cblMediaURLs.Items)
            {
                if (meadiaIDList.Contains(Convert.ToInt64(item.Value)))
                    item.Selected = true;
            }
        }

        //private void SetMediaURLsSubscriber(List<long> meadiaIDList)
        //{
        //    foreach (ListItem item in this.cblMediaURLsSubscriber.Items)
        //    {
        //        if (meadiaIDList.Contains(Convert.ToInt64(item.Value)))
        //            item.Selected = true;
        //    }
        //}



        private void SetSubscribers(List<long> subscriberIDList)
        {
            foreach (ListItem item in this.cblSubscribers.Items)
            {
                if (subscriberIDList.Contains(Convert.ToInt64(item.Value)))
                    item.Selected = true;
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            ViewState["SaveType"] = "Approve";
            if (!Page.IsPostBack)
                return;

            if (!this.ValidateFields())
                return;

           
            int smsLength = Convert.ToInt32(this.tbMessage.Text.Trim().Length);
            if (smsLength >= 156)
            {
                mp1.TargetControlID = "btnApprove";
                mp1.Show();
            }
            else
            {

                List<long> mediaIDList = new List<long>();
                //List<long> subscriberIDList = new List<long>();


                List<long> SMSIDList = new List<long>();
                List<long> EmailIDList = new List<long>();

                List<ChannelMessageSubscriberData> subscriberIDList = new List<ChannelMessageSubscriberData>();

                checkDeliveryMechanisms(ref SMSIDList, ref EmailIDList);
                if ((this.cblSendBySMS.Checked) && (!this.cblSendByEMail.Checked))
                {
                    if (SMSIDList.Count > 0)
                    {
                        this.BindSubscribersGridView(EmailIDList, SMSIDList);
                        mp2.TargetControlID = "btnApprove";
                        mp2.Show();
                    }
                    else
                    {
                        SaveMessage(mediaIDList, SMSIDList, EmailIDList, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
                    }
                }
                else if ((!this.cblSendBySMS.Checked) && (this.cblSendByEMail.Checked))
                {
                    if (EmailIDList.Count > 0)
                    {
                        this.BindSubscribersGridView(EmailIDList, SMSIDList);
                        mp2.TargetControlID = "btnApprove";
                        mp2.Show();
                    }
                    else
                    {
                        SaveMessage(mediaIDList, SMSIDList, EmailIDList, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
                    }
                }
                else
                {
                    SaveMessage(mediaIDList, SMSIDList, EmailIDList, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
                }


                //if ((SMSIDList.Count > 0) || (EmailIDList.Count > 0))
                //{

                //    this.BindSubscribersGridView(SMSIDList, EmailIDList);
                //    mp2.TargetControlID = "btnApprove";
                //    mp2.Show();
                //}
                //else
                //{
                //    SaveMessage(mediaIDList, SMSIDList, EmailIDList, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
                //}
              
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ViewState["SaveType"] = "Save";
            if (!Page.IsPostBack)
                return;

            if (!this.ValidateFields())
                return;

          
            int smsLength = Convert.ToInt32(this.tbMessage.Text.Trim().Length);
            if (smsLength >= 156)
            {
                mp1.TargetControlID = "btnSave";
                mp1.Show();
            }
            else
            {
                List<long> mediaIDList = new List<long>();
                //List<long> subscriberIDList = new List<long>();

              
                List<long> SMSIDList = new List<long>();
                List<long> EmailIDList = new List<long>();

                List<ChannelMessageSubscriberData> subscriberIDList = new List<ChannelMessageSubscriberData>();

                checkDeliveryMechanisms(ref SMSIDList, ref EmailIDList);

                if ((this.cblSendBySMS.Checked) && (!this.cblSendByEMail.Checked))
                {
                    if (SMSIDList.Count > 0)
                    {
                        this.BindSubscribersGridView(EmailIDList, SMSIDList);
                        mp2.TargetControlID = "btnSave";
                        mp2.Show();
                    }
                    else
                    {
                        SaveMessage(mediaIDList, SMSIDList, EmailIDList, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
                    }
                }
                else if ((!this.cblSendBySMS.Checked) && (this.cblSendByEMail.Checked))
                {
                    if (EmailIDList.Count > 0)
                    {
                        this.BindSubscribersGridView(EmailIDList, SMSIDList);
                        mp2.TargetControlID = "btnSave";
                        mp2.Show();
                    }
                    else
                    {
                        SaveMessage(mediaIDList, SMSIDList, EmailIDList, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
                    }
                }
                else
                {
                    SaveMessage(mediaIDList, SMSIDList, EmailIDList, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
                }


                //if ((cblSendBySMS.Checked) || (cblSendByEMail.Checked))
                //{
                //    if( (SMSIDList.Count > 0) || (EmailIDList.Count > 0))
                //    {
                //        if (SMSIDList.Count > 0)
                //            this.BindSubscribersGridView(SMSIDList);
                //        mp2.TargetControlID = "btnSave";
                //        mp2.Show();
                //    }
                //    else
                //    {
                //        SaveMessage(mediaIDList, SMSIDList, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
                //    }
                //}
                //else
                //{

                //    SaveMessage(mediaIDList, SMSIDList, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
                //}



            }
        }

        private void SaveMessage(List<long> mediaIDList, List<long> SMSIDList, List<long> EmailIDList, List<ChannelMessageSubscriberData> subscriberIDList, string SaveType)
        {
            ChannelMessage message = this.PrepareToSave(mediaIDList, subscriberIDList, SMSIDList, EmailIDList);
            if (subscriberIDList.Count>0)
            {
                if (SaveType == "Approve")
                {
                    ChannelMessageManager.Approve(message, mediaIDList, subscriberIDList);
                }
                if (SaveType == "Save")
                {
                    ChannelMessageManager.Save(message, mediaIDList, subscriberIDList);
                }
            }

            this.NavigateBack();
        }

      

        private void checkDeliveryMechanisms(ref List<long> EmailIDList, ref List<long> SMSIDList)
        {
           
            if (Convert.ToBoolean(Convert.ToInt16(this.rblSendTo.SelectedValue)))
            {
                this.BindSubscribers();
                foreach (ListItem item in this.cblSubscribers.Items)
                {
                    var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item.Value));
                    var deliveryMethodList = deliveryMethod.Split(',').ToList();

                    if (cblSendByEMail.Checked)
                    {
                        if (!deliveryMethodList.Contains("Email"))
                            SMSIDList.Add(Convert.ToInt64(item.Value));
                    }
                    if (cblSendBySMS.Checked)
                    {
                        if (!deliveryMethodList.Contains("Text"))
                            EmailIDList.Add(Convert.ToInt64(item.Value));
                    }
                }
            }
            else
            {
                foreach (ListItem item in this.cblSubscribers.Items)
                {
                    if (item.Selected)
                    {
                        var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item.Value));
                        var deliveryMethodList = deliveryMethod.Split(',').ToList();

                        if (cblSendByEMail.Checked)
                        {
                            if (!deliveryMethodList.Contains("Email"))
                                SMSIDList.Add(Convert.ToInt64(item.Value));
                        }
                        if (cblSendBySMS.Checked)
                        {
                            if (!deliveryMethodList.Contains("Text"))
                                EmailIDList.Add(Convert.ToInt64(item.Value));
                        }
                    }
                }
            }

            //foreach (var item in subscriberSMSEmail)
            //{
               
            //    var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(item.SubsciberID);
            //    var deliveryMethodList = deliveryMethod.Split(',').ToList();

            //    if (cblSendByEMail.Checked)
            //    {
            //        if (!deliveryMethodList.Contains("Email"))
            //            EmailIDList.Add(item.SubsciberID);
            //    }
            //    if (cblSendBySMS.Checked)
            //    {
            //        if (!deliveryMethodList.Contains("Text"))
            //            SMSIDList.Add(item.SubsciberID);
            //    }
            //}
        }
        private bool ValidateFields()
        {
            if(!this.ValidateSelectedSubscribers())
            {
                this.DisplayMessageBox("Please select at least one subscriber.", this.upPage);
                return false;
            }

            if (!this.ValidateSendMethods())
            {
                this.DisplayMessageBox("Please select at least one send by option (sms/email).", this.upPage);
                return false;
            }





            //if (this.rblSendTo.SelectedValue == "0")
            //{
            //    bool selected = false;
            //    foreach (ListItem item in this.cblSubscribers.Items)
            //    {
            //        if (item.Selected)
            //            selected = true;
            //    }

            //    if (!selected)
            //    {
            //        this.DisplayMessageBox("Please select at least one subscriber.", this.upPage);
            //        return false;
            //    }
            //}



            if (this.rblSendOption.SelectedValue == "1")
            {
                var dtNow = UtilityManager.GetZoneSpecificTimeFromUTC(DateTime.UtcNow, ChannelManager.GetTimeZone(this.ChannelID));

                if (this.GetScheduleDate() < dtNow)
                {
                    this.DisplayMessageBox("Schedule date & time should be in future.", this.upPage, this.tbScheduleDate);
                    return false;
                }
            }

           

            if(this.MessageID != 0 && ChannelMessageManager.IsSending(this.MessageID))
            {
                this.DisplayMessageBox("Cannot update message as it has started sending messages.", this.upPage, this.tbScheduleDate);
                return false;
            }

            if (this.MessageID != 0 && ChannelMessageManager.IsSMSSending(this.MessageID))
            {
                this.DisplayMessageBox("Cannot update message as it has started sending sms.", this.upPage, this.tbScheduleDate);
                return false;
            }

            
            return true;
        }

        private bool ValidateSelectedSubscribers()
        {
            if (this.trSubscribers.Visible)
            {
                bool selected = false;

                foreach (ListItem item in this.cblSubscribers.Items)
                {
                    if (item.Selected)
                        selected = true;
                }

                if (!selected)
                    return false;
            }

            return true;
        }

        private bool ValidateSendMethods()
        {
            bool selected = false;

            if ((cblSendByEMail.Checked) || (cblSendBySMS.Checked))
                selected = true;

            //foreach (ListItem item in this.cblSendBy.Items)
            //{
            //    if (item.Selected)
            //        selected = true;
            //}

            if (!selected)
                return false;

            return true;
        }

        private ChannelMessage PrepareToSave(List<long> mediaIDList, List<ChannelMessageSubscriberData> subscriberIDList, List<long> SMSsendIds, List<long> EmailsendIds)
        {
            ChannelMessage message = this.MessageID == 0 ? new ChannelMessage() : ChannelMessageManager.GetByID(this.MessageID);

            message.ChannelID = this.ChannelID;

            message.SendToAll = Convert.ToBoolean(Convert.ToInt16(this.rblSendTo.SelectedValue));
            message.Subject = this.tbSubject.Text.Trim();
            message.Message = this.tbMessage.Text.Trim();
            message.EmailMessage = this.tbEmailMessage.Text.Trim();

            message.IsScheduled = Convert.ToBoolean(Convert.ToInt16(this.rblSendOption.SelectedValue));
            message.ScheduledOn = message.IsScheduled ? this.GetScheduleDate() : DateTime.Now;


            //do media urls
            foreach (ListItem item in this.cblMediaURLs.Items)
            {
                if (item.Selected)
                {
                    mediaIDList.Add(Convert.ToInt64(item.Value));
                }
            }

            //do subscriber list
            //Changed Padmaja 14-Mar-2014

            bool SendEmail = false;
            bool SendSMS = false;


            if (message.SendToAll)
            {
                this.BindSubscribers();
                foreach (ListItem item in this.cblSubscribers.Items)
                {
                    //subscriberIDList.Add(new myClass() { SubId = Convert.ToInt64(item.Value) , EmailStatus= 0});
                    //subscriberIDList.Add(Convert.ToInt64(item.Value));
                    if ((cblSendBySMS.Checked) && (cblSendByEMail.Checked))
                    {

                        if (SMSsendIds.Contains(Convert.ToInt64(item.Value)))
                        {
                            SendEmail = true;
                            subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                        }
                        else if (EmailsendIds.Contains(Convert.ToInt64(item.Value)))
                        {
                            SendSMS = true;
                            subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                        }
                        else
                        {
                            var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item.Value));
                            var deliveryMethodList = deliveryMethod.Split(',').ToList();
                            if (deliveryMethodList.Contains("Email"))
                                SendEmail = true;

                            if (deliveryMethodList.Contains("Text"))
                                SendSMS = true;

                            subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = deliveryMethodList.Contains("Email") ? (int)ChannelMessageStatus.EmailSent : (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = deliveryMethodList.Contains("Text") ? (int)SMSStatus.SMSSent : (int)SMSStatus.SMSNotSent });


                        }

                    }
                    else if (cblSendBySMS.Checked)
                    {
                        if (EmailsendIds.Contains(Convert.ToInt64(item.Value)))
                        {
                            SendEmail = true;
                            message.Subject = message.Message;
                            message.EmailMessage = message.Message;
                            subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                        }
                        else
                        {
                            var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item.Value));
                            var deliveryMethodList = deliveryMethod.Split(',').ToList();
                            if (deliveryMethodList.Contains("Text"))
                            {
                                SendSMS = true;
                                subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                            }
                        }

                    }
                    else if (cblSendByEMail.Checked)
                    {

                        if (SMSsendIds.Contains(Convert.ToInt64(item.Value)))
                        {
                            int textlenght = message.Message.Length > 155 ? 155 : message.Message.Length;
                            SendSMS = true;
                            message.Message = message.Message.Substring(0, textlenght);
                            subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                        }
                        else
                        {
                            var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item.Value));
                            var deliveryMethodList = deliveryMethod.Split(',').ToList();
                            if (deliveryMethodList.Contains("Email"))
                            {
                                SendEmail = true;
                                subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                            }
                        }

                    }


                }
            }
            else
            {
                foreach (ListItem item in this.cblSubscribers.Items)
                {
                    if (item.Selected)
                    {
                        //subscriberIDList.Add(Convert.ToInt64(item.Value));
                        // subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = 0, IsSMSSend = 0 });
                        if ((cblSendBySMS.Checked) && (cblSendByEMail.Checked))
                        {


                            if (SMSsendIds.Contains(Convert.ToInt64(item.Value)))
                            {
                                SendEmail = true;
                                subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                            }
                            else if (EmailsendIds.Contains(Convert.ToInt64(item.Value)))
                            {
                                SendSMS = true;
                                subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                            }
                            else
                            {
                                var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item.Value));
                                var deliveryMethodList = deliveryMethod.Split(',').ToList();
                                if (deliveryMethodList.Contains("Email"))
                                    SendEmail = true;

                                if (deliveryMethodList.Contains("Text"))
                                    SendSMS = true;

                                subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = deliveryMethodList.Contains("Email") ? (int)ChannelMessageStatus.EmailSent : (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = deliveryMethodList.Contains("Text") ? (int)SMSStatus.SMSSent : (int)SMSStatus.SMSNotSent });

                            }
                        }
                        else if (cblSendBySMS.Checked)
                        {

                            if (EmailsendIds.Contains(Convert.ToInt64(item.Value)))
                            {
                                SendEmail = true;
                                message.Subject = message.Message;
                                message.EmailMessage = message.Message;
                                subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                            }
                            else
                            {

                                var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item.Value));
                                var deliveryMethodList = deliveryMethod.Split(',').ToList();
                                if (deliveryMethodList.Contains("Text"))
                                {
                                    SendSMS = true;
                                    subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                                }
                            }

                        }
                        else if (cblSendByEMail.Checked)
                        {


                            if (SMSsendIds.Contains(Convert.ToInt64(item.Value)))
                            {
                                SendSMS = true;
                                int textlenght = message.EmailMessage.Length > 155 ? 155 : message.EmailMessage.Length;
                                message.Message = message.EmailMessage.Substring(0, textlenght);
                                subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailNotSent, IsSMSSend = (int)SMSStatus.SMSSent });
                            }
                            else
                            {

                                var deliveryMethod = UserProfileManager.GetSubscriberDeliveryMechanisms(Convert.ToInt64(item.Value));
                                var deliveryMethodList = deliveryMethod.Split(',').ToList();
                                if (deliveryMethodList.Contains("Email"))
                                {
                                    SendEmail = true;
                                    subscriberIDList.Add(new ChannelMessageSubscriberData() { SubsciberID = Convert.ToInt64(item.Value), IsEmailSend = (int)ChannelMessageStatus.EmailSent, IsSMSSend = (int)SMSStatus.SMSNotSent });
                                }

                            }

                        }

                    }
                }
            }
            //this.SetSendMethods(message);
            message.SendBySMS = SendSMS;
            message.SendByEmail = SendEmail;
            return message;
        }

        private DateTime GetScheduleDate()
        {
            var dtTime = DateTime.ParseExact(this.tbScheduleTime.Text, "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

            return Convert.ToDateTime(this.tbScheduleDate.Text).Add(dtTime.TimeOfDay);
        }

        //private void SetSendMethods(ChannelMessage message)
        //{

        //    if (cblSendBySMS.Checked)
        //    {
        //        message.SendBySMS = true;
        //    }
        //    else
        //    {
        //        message.SendBySMS = false;
        //    }

        //    if (cblSendByEMail.Checked)
        //    {
        //        message.SendByEmail = true;
        //    }
        //    else
        //    {
        //        message.SendByEmail = false;
        //    }


        //    //foreach(ListItem item in this.cblSendBy.Items)
        //    //{
        //    //    if (item.Text == DeliveryMechanism.Email.ToString())
        //    //    {
        //    //        if (item.Selected)
        //    //            message.SendByEmail = true;
        //    //        else
        //    //            message.SendByEmail = false;
        //    //    }
                    

        //    //    if (item.Text == DeliveryMechanism.Text.ToString())
        //    //    {
        //    //         if (item.Selected)
        //    //             message.SendBySMS = true;
        //    //        else
        //    //             message.SendBySMS = false;
        //    //    }
                   
        //    //}
        //}

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.NavigateBack();
        }

        private long MessageID
        {
            get
            {
                return Convert.ToInt64(Request["ID"]);
            }
        }

        private long ChannelID
        {
            get
            {
                return Convert.ToInt64(Request["ChannelID"]);
            }
        }

        protected void cblSendBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            //foreach (ListItem item in this.cblSendBy.Items)
            //{
            //    if (item.Text == DeliveryMechanism.Email.ToString())
            //    {
            //        if (item.Selected)
            //        {
            //            efvEmailMessage.Enabled = true;
            //            efvSubject.Enabled = true;
            //        }
            //        else
            //        {
            //            efvEmailMessage.Enabled = false;
            //            efvSubject.Enabled = false;
            //        }
                        
            //    }


            //    if (item.Text == DeliveryMechanism.Text.ToString())
            //    {
            //        if (item.Selected)
            //        {
            //            efvMessage.Enabled = true;
                       
            //        }
            //        else
            //        {
            //            efvMessage.Enabled = false;
            //        }
            //    }

            //}
        }

        protected void cblSendBySMS_CheckedChanged(object sender, EventArgs e)
        {
            checkValidation();

        }

        protected void cblSendByEMail_CheckedChanged(object sender, EventArgs e)
        {
            checkValidation();
        }

        private void checkValidation()
        {
            if (cblSendByEMail.Checked)
            {
                efvEmailMessage.Enabled = true;
                efvSubject.Enabled = true;
            }
            else
            {
                efvEmailMessage.Enabled = false;
                efvSubject.Enabled = false;
            }

            if (cblSendBySMS.Checked)
            {
                efvMessage.Enabled = true;
            }
            else
            {
                efvMessage.Enabled = false;
            }

            if ((!cblSendByEMail.Checked) && (!cblSendBySMS.Checked))
            {
                efvEmailMessage.Enabled = true;
                efvSubject.Enabled = true;
                efvMessage.Enabled = true;
            }
        }

        protected void btnCloseYes_Click(object sender, EventArgs e)
        {
            List<long> mediaIDList = new List<long>();
            List<long> SMSIDList = new List<long>();
            List<ChannelMessageSubscriberData> subscriberIDList = new List<ChannelMessageSubscriberData>();
            List<long> EmailsendIds = new List<long>();
            SaveMessage(mediaIDList,SMSIDList, EmailsendIds, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
            mp1.Hide();
        }

        protected void tbnSendSMS_Click(object sender, EventArgs e)
        {
            List<long> mediaIDList = new List<long>();
          //  List<long> subscriberIDList = new List<long>();
          //  this.cblSendByEMail.Checked = true;
            List<long> SMSIDList = new List<long>();
            List<ChannelMessageSubscriberData> subscriberIDList = new List<ChannelMessageSubscriberData>();
            List<long> EmailsendIds = new List<long>();
            foreach (ListItem item in this.ckUserList.Items)
            {
                if (item.Selected)
                    SMSIDList.Add(Convert.ToInt64(item.Value));
                
            }
            foreach (ListItem item in this.ckUserListText.Items)
            {
                if (item.Selected)
                    EmailsendIds.Add(Convert.ToInt64(item.Value));

            }

            SaveMessage(mediaIDList,SMSIDList, EmailsendIds, subscriberIDList, Convert.ToString(ViewState["SaveType"]));
            mp2.Hide();
        }

      
        protected void btnSendSMSNo_Click(object sender, EventArgs e)
        {
            List<long> mediaIDList = new List<long>();
            List<long> SMSIDList = new List<long>();
            // List<long> subscriberIDList = new List<long>();
            List<ChannelMessageSubscriberData> subscriberIDList = new List<ChannelMessageSubscriberData>();
            List<long> EmailsendIds = new List<long>();
            SaveMessage(mediaIDList, SMSIDList,EmailsendIds, subscriberIDList, Convert.ToString(ViewState["SaveType"]));

            mp2.Hide();
        }

        protected void Close_Click(object sender, EventArgs e)
        {
            mp2.TargetControlID = "Button3";
            mp2.Hide();
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
             mp1.TargetControlID = "Button1";
             mp1.Hide();
        }


    }

   
}