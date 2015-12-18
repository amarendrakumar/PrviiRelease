using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Business;
using Prvii.Entities;
using System.IO;
using Prvii.Entities.Enumerations;
using Newtonsoft.Json;
using System.Data;
using System.Collections;
using Prvii.Entities.DataEntities;


namespace Prvii.Web.WebPages.Channels
{
    public partial class ChannelDetailsEdit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                if (!(this.UserSesssion.UserRole == Role.Administrator || (this.UserSesssion.UserRole == Role.Celebrity && this.UserSesssion.ChannelID == this.ChannelID) || this.UserSesssion.UserRole == Role.Group))
                {
                    this.UnAuthorizeAccess();
                    return;
                }

                this.BindMasters();

                if(this.ChannelID != 0)
                    this.BindChannel();
                else
                {
                    var itemList = ChannelManager.GetPrviiAccountMaster(0);
                    var priceMenegementDataList = new List<PriceMenegementData>();

                    foreach (var item in itemList)
                    {
                        PriceMenegementData priceMgmtData = new PriceMenegementData();
                        priceMgmtData.Id = item.ID;
                        priceMgmtData.Account = item.Account;                       
                        priceMgmtData.Distribution = 0;
                        priceMenegementDataList.Add(priceMgmtData);
                    }

                    gvPrviiAccountMaster.DataSource = priceMenegementDataList;
                    gvPrviiAccountMaster.DataBind();
                }

            }
        }

        private void BindMasters()
        {
            this.BindDropDown(this.ddlBillingCycles, this.GetEnumList<BillingCycleType>(),true);
            this.BindDropDown(this.ddlTimezones, TimeZoneInfo.GetSystemTimeZones().Select(x => new { ID = x.Id, Name = x.DisplayName }).ToList());

           
        }

        private void BindChannel()
        {
            Channel channel = ChannelManager.GetByID(this.ChannelID);

            this.tbFirstName.Text = channel.Firstname;
            this.tbLastName.Text = channel.Lastname;
            this.tbEmail.Text = channel.Email;
            this.tbMobile.Text = channel.Phone;
            this.tbPrice.Text = channel.Price.ToString("#.##");
            this.ddlTimezones.SelectedValue = channel.TimeZoneID;
            this.cbxIsActive.Checked = channel.IsActive;
            this.chxPreclude.Checked = channel.Preclude;

            this.ddlBillingCycles.SelectedValue = channel.BillingCycleID.ToString();
            this.tbNoOfBillingPeriod.Text = channel.NoOfBillingPeriod.HasValue ? channel.NoOfBillingPeriod.Value.ToString() : string.Empty;

            var list = ChannelManager.GetMediaByChannelID(this.ChannelID);
            var imageID = list.Where(x => x.MediaTypeID == (short)ChannelMediaType.Image).Select(x => x.ID).FirstOrDefault();
            var imageLeftID = list.Where(x => x.MediaTypeID == (short)ChannelMediaType.Left_Image).Select(x => x.ID).FirstOrDefault();
            var imageCenterID = list.Where(x => x.MediaTypeID == (short)ChannelMediaType.Center_Image).Select(x => x.ID).FirstOrDefault();
            var imageRightID = list.Where(x => x.MediaTypeID == (short)ChannelMediaType.Right_Image).Select(x => x.ID).FirstOrDefault();
            var imageBackgroundID = list.Where(x => x.MediaTypeID == (short)ChannelMediaType.Background_Image).Select(x => x.ID).FirstOrDefault();

            if (imageID != 0)
            {
                this.ImageID = imageID;
                this.lbtnImageDelete.Visible = true;
                this.imgImagePreview.ImageUrl = "~/WebPages/GetChannelMedia.aspx?ID=" + imageID;
            }

            if (imageLeftID != 0)
            {
                this.LeftImageID = imageLeftID;
                this.lbtnLeftImageDelete.Visible = true;
                this.imgLeftImagePreview.ImageUrl = "~/WebPages/GetChannelMedia.aspx?ID=" + imageLeftID;
            }

            if (imageCenterID != 0)
            {
                this.CenterImageID = imageCenterID;
                this.lbtnCenterImageDelete.Visible = true;
                this.imgCenterImage.ImageUrl = "~/WebPages/GetChannelMedia.aspx?ID=" + imageCenterID;
            }

            if (imageRightID != 0)
            {
                this.RightImageID = imageRightID;
                this.lbtnRightImageDelete.Visible = true;
                this.imgRightImagePreview.ImageUrl = "~/WebPages/GetChannelMedia.aspx?ID=" + imageRightID;
            }

            if (imageBackgroundID != 0)
            {
                this.BackgroundImageID = imageBackgroundID;
                this.lbtnBackgroundImageDelete.Visible = true;
                this.imgBackgroundImage.ImageUrl = "~/WebPages/GetChannelMedia.aspx?ID=" + imageBackgroundID;
            }
            this.PriceManagent = channel.PriceManagement;
            this.BindWelcomeMedia();
            this.BindMediaURL();
            this.BindPriceManagment(channel.PriceManagement);
           
        }

        private void BindWelcomeMedia()
        {
            var welcomeMediaList = ChannelManager.GetMediaList(this.ChannelID, ChannelMediaType.Welcome_Media);
            this.gvWelcomeMediaList.DataSource = welcomeMediaList;
            this.gvWelcomeMediaList.DataBind();
        }

        private void BindMediaURL()
        {            
            var mediaUrlList = ChannelManager.GetMediaList(this.ChannelID, ChannelMediaType.Media_URL);
            this.gvMediaUrlList.DataSource = mediaUrlList;
            this.gvMediaUrlList.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!this.ValidateFields())
                return;

            List<ChannelMedia> mediaList = new List<ChannelMedia>();

            Channel channel = this.PrepareToSave(mediaList);

            ChannelManager.Save(channel, mediaList);

            Response.Redirect("~/WebPages/Channels/ChannelDetailsView.aspx?ID=" + channel.ID);
        }

        private bool ValidateFields()
        {
            if (ChannelManager.Exists(this.tbFirstName.Text, this.tbLastName.Text ,this.ChannelID))
            {
                this.DisplayMessageBox("A celebrity exists with this first & last name. Please use another names.", this.upPage111, this.tbEmail);
                return false;
            }

            return true;
        }

        private Channel PrepareToSave(List<ChannelMedia> mediaList)
        {
            Channel channel = this.ChannelID == 0 ? new Channel() : ChannelManager.GetByID(this.ChannelID);

            channel.Firstname = this.tbFirstName.Text;
            channel.Lastname = this.tbLastName.Text;
            channel.Email = this.tbEmail.Text;
            channel.Phone = this.tbMobile.Text;
            channel.Price = Convert.ToDecimal(this.tbPrice.Text);
            channel.BillingCycleID = Convert.ToInt16(this.ddlBillingCycles.SelectedValue);

            channel.NoOfBillingPeriod = this.tbNoOfBillingPeriod.Text != string.Empty ? (int?)Convert.ToInt32(this.tbNoOfBillingPeriod.Text) : null;

            channel.TimeZoneID = this.ddlTimezones.SelectedValue;
            channel.IsActive = this.cbxIsActive.Checked;
            channel.Preclude = this.chxPreclude.Checked;
            channel.StatusID = (short)ChannelStatus.Created;
            if (this.ChannelID == 0)
            {
                channel.PriceManagement = "";
            }
            else
            {
                channel.PriceManagement = channel.PriceManagement;
            }

            if (fuImage.HasFile)
                mediaList.Add(new ChannelMedia { ID = this.ImageID, Content = fuImage.FileBytes, Name = fuImage.FileName, MimeType = fuImage.PostedFile.ContentType, MediaTypeID = (short)ChannelMediaType.Image });

            if (fuLeftImage.HasFile)
                mediaList.Add(new ChannelMedia { ID = this.LeftImageID, Content = fuLeftImage.FileBytes, Name = fuLeftImage.FileName, MimeType = fuLeftImage.PostedFile.ContentType, MediaTypeID = (short)ChannelMediaType.Left_Image });

            if (fuCenterImage.HasFile)
                mediaList.Add(new ChannelMedia { ID = this.CenterImageID, Content = fuCenterImage.FileBytes, Name = fuCenterImage.FileName, MimeType = fuCenterImage.PostedFile.ContentType, MediaTypeID = (short)ChannelMediaType.Center_Image });

            if (fuRightImage.HasFile)
                mediaList.Add(new ChannelMedia { ID = this.RightImageID, Content = fuRightImage.FileBytes, Name = fuRightImage.FileName, MimeType = fuRightImage.PostedFile.ContentType, MediaTypeID = (short)ChannelMediaType.Right_Image });

            if (fuBackgroundImage.HasFile)
                mediaList.Add(new ChannelMedia { ID = this.BackgroundImageID, Content = fuBackgroundImage.FileBytes, Name = fuBackgroundImage.FileName, MimeType = fuBackgroundImage.PostedFile.ContentType, MediaTypeID = (short)ChannelMediaType.Background_Image });

            foreach (var file in fuWelcomeMedia.PostedFiles)
            {
                if (file.ContentLength != 0)
                {
                    var fileStream = new MemoryStream();
                    file.InputStream.CopyTo(fileStream);

                    mediaList.Add(new ChannelMedia { Content = fileStream.ToArray(), Name = file.FileName, MimeType = file.ContentType, MediaTypeID = (short)ChannelMediaType.Welcome_Media });
                }
            }

            foreach (var file in fuMediaUrls.PostedFiles)
            {
                if (file.ContentLength != 0)
                {
                    var fileStream = new MemoryStream();
                    file.InputStream.CopyTo(fileStream);

                    mediaList.Add(new ChannelMedia { Content = fileStream.ToArray(), Name = file.FileName, MimeType = file.ContentType, MediaTypeID = (short)ChannelMediaType.Media_URL });
                }
            }

            return channel;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.NavigateBack();
        }

        protected void gvWelcomeMediaList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                ChannelManager.DeleteMedia(Convert.ToInt64(e.CommandArgument));
                this.BindWelcomeMedia();
            }
        }

        protected void gvMediaUrlList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                ChannelManager.DeleteMedia(Convert.ToInt64(e.CommandArgument));
                this.BindMediaURL();
            }
        }

        protected void lbtnImageDelete_Click(object sender, EventArgs e)
        {
            ChannelManager.DeleteMedia(this.ChannelID, ChannelMediaType.Image);
            this.ImageID = 0;
            this.lbtnImageDelete.Visible = false;
        }

        protected void lbtnLeftImageDelete_Click(object sender, EventArgs e)
        {
            ChannelManager.DeleteMedia(this.ChannelID, ChannelMediaType.Left_Image);
            this.LeftImageID = 0;
            this.lbtnLeftImageDelete.Visible = false;
        }

        protected void lbtnRightImageDelete_Click(object sender, EventArgs e)
        {
            ChannelManager.DeleteMedia(this.ChannelID, ChannelMediaType.Right_Image);
            this.RightImageID = 0;
            this.lbtnRightImageDelete.Visible = false;
        }

        protected void lbtnCenterImageDelete_Click(object sender, EventArgs e)
        {
            ChannelManager.DeleteMedia(this.ChannelID, ChannelMediaType.Center_Image);
            this.CenterImageID = 0;
            this.lbtnCenterImageDelete.Visible = false;
        }

        protected void lbtnBackgroundImageDelete_Click(object sender, EventArgs e)
        {
            ChannelManager.DeleteMedia(this.ChannelID, ChannelMediaType.Background_Image);
            this.BackgroundImageID = 0;
            this.lbtnBackgroundImageDelete.Visible = false;
        }

        private long ChannelID
        {
            get
            {
                return Convert.ToInt64(Request["ID"]);
            }
        }

        private long ImageID
        {
            get
            {
                return Convert.ToInt64(ViewState["ImageID"]);
            }

            set
            {
                ViewState["ImageID"] = value;
            }
        }

        private long LeftImageID
        {
            get
            {
                return Convert.ToInt64(ViewState["LeftImageID"]);
            }

            set
            {
                ViewState["LeftImageID"] = value;
            }
        }

        private long CenterImageID
        {
            get
            {
                return Convert.ToInt64(ViewState["CenterImageID"]);
            }

            set
            {
                ViewState["CenterImageID"] = value;
            }
        }

        private long RightImageID
        {
            get
            {
                return Convert.ToInt64(ViewState["RightImageID"]);
            }

            set
            {
                ViewState["RightImageID"] = value;
            }
        }

        private long BackgroundImageID
        {
            get
            {
                return Convert.ToInt64(ViewState["BackgroundImageID"]);
            }

            set
            {
                ViewState["BackgroundImageID"] = value;
            }
        }


        private string PriceManagent
        {
            get
            {
                return Convert.ToString(ViewState["PriceManagent"]);
            }

            set
            {
                ViewState["PriceManagent"] = value;
            }
        }


        protected void btnPriceSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<PriceManagement> priceDistribution = new List<PriceManagement>();


                lblErrormessage.Text = "";
                decimal totalPercentage = 0;
                foreach (GridViewRow row in gvPrviiAccountMaster.Rows)
                {
                    var accountId = 0;
                    decimal Distribution = 0;
                    int Sequence = 0;

                    TextBox _txtPrecentage = (TextBox)row.FindControl("txtPrecentage");
                    if (_txtPrecentage != null)
                    {
                        totalPercentage += Convert.ToDecimal(_txtPrecentage.Text);
                        Distribution = Convert.ToDecimal(_txtPrecentage.Text);

                    }

                    HiddenField _hidAccountID = (HiddenField)row.FindControl("hidAccountID");
                    if (_hidAccountID != null)
                    {
                        accountId = Convert.ToInt32(_hidAccountID.Value);
                    }

                    TextBox _txtSequence = (TextBox)row.FindControl("txtSequence");
                    if (_txtSequence != null)
                    {
                        Sequence = Convert.ToInt16(_txtSequence.Text);
                    }

                    List<PriceDistribution> innerDistribution = new List<PriceDistribution>();
                    GridView _gvPrviiAccountSubMaster = (GridView)row.FindControl("gvPrviiAccountSubMaster");
                    if (_gvPrviiAccountSubMaster != null)
                    {

                        foreach (GridViewRow innerrow in _gvPrviiAccountSubMaster.Rows)
                        {
                            var accountIdInner = 0;
                            decimal DistributionInner = 0;
                            int SequenceInner = 0;

                            TextBox _txtPrecentageInner = (TextBox)innerrow.FindControl("txtPrecentage");
                            if (_txtPrecentageInner != null)
                            {
                                DistributionInner = Convert.ToDecimal(_txtPrecentageInner.Text);
                            }

                            HiddenField _hidAccountIDSub = (HiddenField)innerrow.FindControl("hidAccountIDSub");
                            if (_hidAccountIDSub != null)
                            {
                                accountIdInner = Convert.ToInt32(_hidAccountIDSub.Value);
                            }

                            TextBox _txtSequenceInner = (TextBox)innerrow.FindControl("txtSequence");
                            if (_txtSequenceInner != null)
                            {
                                SequenceInner = Convert.ToInt16(_txtSequenceInner.Text);
                            }

                            var itemInner = new PriceDistribution { AccountId = accountIdInner, Distribution = DistributionInner, Sequence = SequenceInner };
                            innerDistribution.Add(itemInner);
                        }
                    }

                    var item = new PriceManagement { AccountId = accountId, Distribution = Distribution, InnerDistribution = innerDistribution, Sequence = Sequence };
                    priceDistribution.Add(item);
                }
                string json = JsonConvert.SerializeObject(priceDistribution, Formatting.Indented);               
                ChannelManager.UpdateChannelPrice(this.ChannelID, json);                
                lblErrormessage.Text = "Record Update Successfully.";

                //if (totalPercentage == 100)
                //{
                //    string json = JsonConvert.SerializeObject(priceDistribution, Formatting.Indented);
                //    //  Console.WriteLine(json);
                //    ChannelManager.UpdateChannelPrice(this.ChannelID, json);
                //    // List<PriceManagement> deserializedProduct = JsonConvert.DeserializeObject<List<PriceManagement>>(json);
                //    lblErrormessage.Text = "Record Update Successfully.";
                //}
                //else
                //{
                //    lblErrormessage.Text = "The sum of percentage assigned to stakeholders should be 100. Currently the total percentage is " + totalPercentage.ToString();
                //}                 

            }
            catch(Exception ex)
            {
                lblErrormessage.Text = ex.Message;
            }
           
            
            ModalPopup1.Show();
            

            
        }

        private void BindPriceManagment(string Pricejson)
        {
            List<PriceManagement> deserializedProduct;
            if(Pricejson!=null)
            {
                deserializedProduct = JsonConvert.DeserializeObject<List<PriceManagement>>(Pricejson);
            }
            
            else
            {
                deserializedProduct = null;
            }
            var prviiAccountMasterList = ChannelManager.GetPrviiAccountMaster(0);

            var priceMenegementDataList = new List<PriceMenegementData>();

            foreach (var item in prviiAccountMasterList)
            {
                PriceMenegementData priceMgmtData = new PriceMenegementData();
                priceMgmtData.Id = item.ID;
                priceMgmtData.Account = item.Account;
                if(deserializedProduct!=null)
                {
                    PriceManagement prcMgmt = deserializedProduct.Where(p => p.AccountId == item.ID).FirstOrDefault();
                    priceMgmtData.Distribution = prcMgmt.Distribution;
                    priceMgmtData.Sequence = prcMgmt.Sequence;
                }
                else
                {
                    priceMgmtData.Sequence = 0;
                    priceMgmtData.Distribution = 0;
                }
                priceMenegementDataList.Add(priceMgmtData);
            }

            this.gvPrviiAccountMaster.DataSource = priceMenegementDataList;
             this.gvPrviiAccountMaster.DataBind();


            
        }

        protected void btnCancelPupop_Click(object sender, EventArgs e)
        {
            ModalPopup1.Hide();
        }

        protected void gvPrviiAccountMaster_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                long AccountId = Convert.ToInt64(gvPrviiAccountMaster.DataKeys[e.Row.RowIndex].Value);
                GridView gvPrviiAccountSubMaster = e.Row.FindControl("gvPrviiAccountSubMaster") as GridView;
                List<PriceManagement> deserializedProductInner;

                if (PriceManagent != null)
                {
                    deserializedProductInner = JsonConvert.DeserializeObject<List<PriceManagement>>(PriceManagent);
                }
                else
                {
                    deserializedProductInner = null;
                }

                var prviiAccountMasterListInner = ChannelManager.GetPrviiAccountMaster(AccountId);

                var priceMenegementDataList = new List<PriceMenegementData>();

                foreach (var item in prviiAccountMasterListInner)
                {
                    PriceMenegementData priceMgmtData = new PriceMenegementData();
                    priceMgmtData.Id = item.ID;
                    priceMgmtData.Account = item.Account;
                    if (deserializedProductInner != null)
                    {
                        PriceManagement prcMgmt = deserializedProductInner.Where(p => p.AccountId == item.ParentID).FirstOrDefault();

                        priceMgmtData.Distribution = prcMgmt.InnerDistribution.Where(s => s.AccountId == item.ID).Select(h=>h.Distribution).FirstOrDefault();
                        priceMgmtData.Sequence = prcMgmt.InnerDistribution.Where(s => s.AccountId == item.ID).Select(h => h.Sequence).FirstOrDefault();
                    }
                    else
                    {
                        priceMgmtData.Sequence = 0;
                        priceMgmtData.Distribution = 0;
                    }
                    priceMenegementDataList.Add(priceMgmtData);
                }

                gvPrviiAccountSubMaster.DataSource = priceMenegementDataList;// ChannelManager.GetPrviiAccountMaster(AccountId).ToList();
                gvPrviiAccountSubMaster.DataBind();
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            ModalPopup1.Show();
        }
    }
}