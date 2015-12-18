<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="ChannelMessageDetails.aspx.cs" Inherits="Prvii.Web.WebPages.ChannelMessages.ChannelMessageDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
       <div class="row">
            <div class="col-lg-12 list-table stripped">
                <h1>Message Details <samp style="font-style: normal; font-weight: normal;">for </samp>
        <asp:Literal ID="litCelebrityName" runat="server"></asp:Literal>

                </h1>
                <br/>
    <asp:UpdateProgress ID="upgPage" runat="server" AssociatedUpdatePanelID="upPage">
        <ProgressTemplate>
            <div class="progressBackground"></div>
            <div class="progressBox">
                <img alt="" runat="server" src="~/Images/indicator.gif" style="vertical-align: middle;" />
                <span>&nbsp;&nbsp;Please wait...</span>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel runat="server" ID="upPage">
        <ContentTemplate>
           
			 <style type="text/css">
                .modalBackground {
                    background-color: Black;
                    filter: alpha(opacity=90);
                    opacity: 0.8;
                }

                .modalPopup {
                    background-color: #FFFFFF;
                    border-width: 3px;
                    border-style: solid;
                    border-color: black;
                    padding-top: 10px;
                    padding-left: 10px;
                    padding-right: 10px;
                    width: 500px;
                    min-height: 150px;
                }
            </style>
            <script>
                $(document).ready(function () {
                    MessageCount();
                    EmailCount();
                    ShowValues();

                });

                var prm = Sys.WebForms.PageRequestManager.getInstance();
                if (prm != null) {
                    prm.add_endRequest(function () {
                        MessageCount();
                        EmailCount();
                        ShowValues();
                    });
                }

                function ShowValues() {
                    var text_max = 155;
                    var textlength = $('#tbMessage').val().length;
                    $('#txtCount').html(textlength + ' characters typed so far.');
                    var text_remaining = text_max - textlength;
                    $('#textarea_feedback').html(text_remaining + ' characters remaining');

                    var textlength1 = $('#tbEmailMessage').val().length;
                    $('#txtCountEmail').html(textlength1 + ' characters typed so far.');

                }
                function MessageCount() {
                    $('#textarea_feedback').show();
                    $('#tbMessage').keyup(function (e) {
                        var text_max = 155;
                        var textlength = $('#tbMessage').val().length;
                        $('#txtCount').html(textlength + ' characters typed so far.');
                        var text_remaining = text_max - textlength;
                        $('#textarea_feedback').html(text_remaining + ' characters remaining');
                        //if ($(this).val().length >= text_max) {
                        //    e.preventDefault();
                        //}
                    });

                }

                function EmailCount() {
                    $('#txtCountEmail').show();
                    $('#tbEmailMessage').keyup(function () {
                        var textlength = $('#tbEmailMessage').val().length;
                        $('#txtCountEmail').html(textlength + ' characters typed so far.');

                    });
                }




            </script>
			
            <div  runat="server" id="tblMessage" visible="true">

				<div class="row form-group">
					<div class="col-md-3">
						<label>Send to</label>
					</div>
					<div class="col-md-9">
                        <asp:RadioButtonList ID="rblSendTo" runat="server" CssClass="form-controlOld" Width="300" RepeatDirection="Horizontal" CellPadding="0" CellSpacing="0"
                            AutoPostBack="true" OnSelectedIndexChanged="rblSendTo_SelectedIndexChanged">
                            <asp:ListItem Text="All Subscribers" Value="1" />
                            <asp:ListItem Text="Selected Subscribers" Value="0" />
                        </asp:RadioButtonList>
                        <asp:RequiredFieldValidator ID="rfvSendTo" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="rblSendTo" CssClass="requiredField"></asp:RequiredFieldValidator>
						
					</div>	
				</div>
				
				<div class="row form-group" runat="server" id="trSubscribers" visible="false" >
					<div class="col-md-3">
						<label>Subscribers</label>
					</div>
					<div class="col-md-9 scrollable-div">						 
                            <asp:CheckBoxList ID="cblSubscribers" runat="server" CssClass="form-controlOld"  CellPadding="0" CellSpacing="0" RepeatDirection="Horizontal" RepeatColumns="6" />
                       
					</div>
				</div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Send By</label>
					</div>
					<div class="col-md-9">
						 <asp:CheckBox ID="cblSendBySMS" runat="server" ClientIDMode="Static" Width="80" CellPadding="0" CellSpacing="0" CssClass="form-control" AutoPostBack="true" Text="Text" OnCheckedChanged="cblSendBySMS_CheckedChanged" />
                        <asp:CheckBox ID="cblSendByEMail" runat="server" ClientIDMode="Static" Width="80" CellPadding="0" CellSpacing="0" CssClass="form-control" AutoPostBack="true" Text="Email" OnCheckedChanged="cblSendByEMail_CheckedChanged" />

					</div>	
				</div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Subject</label>
					</div>
					<div class="col-md-9">
                        <asp:TextBox ID="tbSubject" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="efvSubject" runat="server" ClientIDMode="Static" ErrorMessage="Required!"
                             Display="Dynamic" ControlToValidate="tbSubject" CssClass="requiredField"></asp:RequiredFieldValidator>
						
					</div>	
				</div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Email Message</label>
					</div>
					<div class="col-md-9">
                         <asp:TextBox ID="tbEmailMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="8" ClientIDMode="Static"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="efvEmailMessage" runat="server" ClientIDMode="Static" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbEmailMessage" CssClass="requiredField"></asp:RequiredFieldValidator>
                        <div id="txtCountEmail" class="hint text-right"></div>
					
					</div>	
				</div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Text Message (SMS)</label>
					</div>
					<div class="col-md-9">
                        <asp:TextBox ID="tbMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="8" ClientIDMode="Static"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="efvMessage" runat="server" ErrorMessage="Required!" ClientIDMode="Static" Display="Dynamic" ControlToValidate="tbMessage" CssClass="requiredField"></asp:RequiredFieldValidator>
                        <div id="textarea_feedback"  class="hint text-left pull-left" ></div>
                        
                        <div id="txtCount" class="col-sm-6 hint text-right pull-right" ></div>
                        <placeholder runat="server" id="PlaceHolder1"></placeholder>
					</div>	
				</div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Send</label>
					</div>
					<div class="col-md-9">
                            <asp:RadioButtonList ID="rblSendOption" runat="server" CssClass="form-control" Width="200" RepeatDirection="Horizontal" CellPadding="0" CellSpacing="0"
                            AutoPostBack="true" OnSelectedIndexChanged="rblSendOption_SelectedIndexChanged">
                            <asp:ListItem Text="Immediate" Value="0" />
                            <asp:ListItem Text="Schedule" Value="1" />
                        </asp:RadioButtonList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="rblSendOption" CssClass="requiredField"></asp:RequiredFieldValidator>
                   
						
					</div>	
				</div>

                <div  class="row form-group" runat="server" id="trScheduleOn" visible="false"> 
                    <div class="col-md-3">
						<label>Schedule On</label>
					</div>
					<div class="col-md-9">
                         <asp:TextBox ID="tbScheduleDate" runat="server" CssClass="form-control" MaxLength="50" Width="120"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbScheduleDate" CssClass="requiredField"></asp:RequiredFieldValidator>
                        <ajaxToolkit:CalendarExtender
                            ID="CalendarExtender1"
                            TargetControlID="tbScheduleDate"
                            runat="server" />
                        &nbsp;&nbsp;
                        <asp:TextBox ID="tbScheduleTime" runat="server" CssClass="form-control" MaxLength="50" Width="90"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender3" runat="server"
                            TargetControlID="tbScheduleTime"
                            Mask="99:99"
                            MessageValidatorTip="true"
                            MaskType="Time"
                            AcceptAMPM="True"
                            ErrorTooltipEnabled="True" />
                        <ajaxToolkit:MaskedEditValidator ID="MaskedEditValidator3" runat="server"
                            ControlExtender="MaskedEditExtender3"
                            ControlToValidate="tbScheduleTime"
                            IsValidEmpty="False"
                            EmptyValueMessage="Time is required"
                            InvalidValueMessage="Time is invalid"
                            Display="Dynamic"
                            EmptyValueBlurredText="Required!"
                            InvalidValueBlurredMessage="Invalid!"
                            CssClass="requiredField" />
					</div>
                </div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Media Urls</label>
					</div>
					<div class="col-md-9">
						 <asp:CheckBoxList ID="cblMediaURLs" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" />
					</div>
				</div>
				
				<div class="row form-group" runat="server" id="trStatus" visible="false">
					<div class="col-md-3">
						<label>Status</label>
					</div>
					<div class="col-md-9">
						<asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
					</div>
				</div>
				<hr>
				<div class="row list-table-body">
					<div class="col-md-3"></div>
					<div class="col-md-9">
						<div class="col-sm-12">
                            <div class="col-md-4">
                                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-default solid-btn" OnClick="btnSave_Click" />
                            </div>
                            <div class="col-md-4">
                                 <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn btn-default solid-btn" OnClick="btnApprove_Click" />
                                 </div>
                            <div class="col-md-4">
                                  <asp:Button ID="btnCancel" runat="server" Text="Back" CssClass="btn btn-default solid-btn" OnClick="btnCancel_Click" CausesValidation="false" formnovalidate />
                            <asp:Button ID="Button1" runat="server" Visible="false" />
                        <asp:Button ID="Button3" runat="server" Visible="false" />
                            </div>
							
                        
							<br/><br/>
						</div>
					</div>
				</div>


				</div>
				
            <div runat="server" id="tblSubscriberMessage" visible="false">
                				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Send By</label>
					</div>
					<div class="col-md-9">
						<asp:CheckBoxList ID="cblSendBySubscriber" runat="server" RepeatDirection="Horizontal" Width="150" CssClass="form-control" CellPadding="0" CellSpacing="0" Enabled="false" />
					</div>	
				</div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Subject</label>
					</div>
					<div class="col-md-9">
						<asp:TextBox ID="tbSubjectSubscriber" runat="server" CssClass="form-control" MaxLength="50" Enabled="false"></asp:TextBox>
					</div>	
				</div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Email Message</label>
					</div>
					<div class="col-md-9">
					 <asp:TextBox ID="tbEmailMessageSubscriber" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="8" Enabled="false"></asp:TextBox>
					
					</div>	
				</div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Text Message (SMS)</label>
					</div>
					<div class="col-md-9">
					<asp:TextBox ID="tbMEssageSubscriber" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="8" Enabled="false"></asp:TextBox>

					</div>	
				</div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Send On </label>
					</div>
					<div class="col-md-9">
						 <asp:Label ID="lblSendOn" runat="server"></asp:Label>
					</div>	
				</div>
				
				<div class="row form-group">
					<div class="col-md-3">
						<label>Media Urls</label>
					</div>
					<div class="col-md-9">
						<asp:GridView ID="gvWelcomeMediaList" runat="server" CssClass="grid" PagerStyle-HorizontalAlign="Right"
                            Width="100%" AutoGenerateColumns="false" AllowPaging="true" PageSize="20" GridLines="None" EmptyDataText="No Item Found">
                            <RowStyle CssClass="row" />
                            <PagerStyle HorizontalAlign="Center" />
                            <Columns>
                                <asp:HyperLinkField HeaderText="Name" DataTextField="Name" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/WebPages/ViewChannelMediaURL.aspx?ID={0}">
                                    <ItemStyle Width="80%" CssClass="label" />
                                </asp:HyperLinkField>
                            </Columns>
                        </asp:GridView>
                        <asp:CheckBoxList ID="cblMediaURLsSubscriber" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" Enabled="false" />
					</div>
				</div>
				<hr>
				<div class="row list-table-body">
					<div class="col-md-9"></div>
					<div class="col-md-3">
						<div class="col-sm-12">
                            <asp:Button ID="btnCancelSubscriber" runat="server" Text="Back" CssClass="btn btn-default solid-btn" OnClick="btnCancel_Click" CausesValidation="false" formnovalidate />
							
							<br/><br/>
						</div>
					</div>
				</div>

			</div>
        </div>
           
          

            <!-- ModalPopupExtender -->
            <ajaxToolkit:ModalPopupExtender ID="mp1" runat="server" PopupControlID="Panel1" TargetControlID="Button1"
                BackgroundCssClass="modalBackground">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="Panel1" runat="server" CssClass="modalPopup" align="center" Style="display: none">
                <div class="row">
					<div class="col-md-12">
						 <p>The Message length for SMS is exceeding 155 characters. Click, Yes, to send multiple messages else click NO to modify the message length.</p>
					</div>
					<div class="col-md-12">
						<div class="col-sm-6 col-xs-6">
							 <asp:Button ID="btnCloseYes" runat="server" CssClass="btn btn-default solid-btn" Text="Yes" OnClick="btnCloseYes_Click" />
						</div>
						<div class="col-sm-6 col-xs-6">
							<asp:Button ID="btnClose" runat="server" CssClass="btn btn-default solid-btn" Text="No" OnClick="btnClose_Click" />
						</div>
						
					</div>	
				</div>
            </asp:Panel>
            <!-- ModalPopupExtender -->


            <!-- ModalPopupExtender -->
            <ajaxToolkit:ModalPopupExtender ID="mp2" runat="server" PopupControlID="Panel2" TargetControlID="Button3"
               BackgroundCssClass="modalBackground">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="Panel2" runat="server" CssClass="modalPopup" align="center" Style="display: none">
                 <div class="row">
                  <div class="col-md-12" runat="server" id="divEmail"  > 
                <p>
                    <h3>The List of subscribers don't choose delivery mechanisms text.<br /> Can you send Email for the same? </h3>
                    <asp:CheckBoxList ID="ckUserList" runat="server" CssClass="form-control" CellPadding="0" CellSpacing="0" RepeatDirection="Horizontal" RepeatColumns="6"></asp:CheckBoxList>
                </p></div>
                <div class="col-md-12" runat="server" id="divText" > 
                 <p>
                    <h3>The List of subscribers don't choose delivery mechanisms Email.<br /> Can you send Text for the same? </h3>
                    <asp:CheckBoxList ID="ckUserListText" runat="server" CssClass="form-control" CellPadding="0" CellSpacing="0" RepeatDirection="Horizontal" RepeatColumns="6"></asp:CheckBoxList>
                </p>
                    </div>
                <br />
                     <div class="col-md-12">
                         <div class="col-sm-4">
                              <asp:Button ID="tbnSendSMS" runat="server" CssClass="btn btn-default solid-btn" Text="Yes & Continue" OnClick="tbnSendSMS_Click" />
                         </div>
                         <div class="col-sm-4">
                              <asp:Button ID="btnSendSMSNo" runat="server" CssClass="btn btn-default solid-btn" Text="No & Continue" OnClick="btnSendSMSNo_Click" />
                         </div>
                         <div class="col-sm-4">
                              <asp:Button ID="Close" runat="server" CssClass="btn btn-default solid-btn" Text="Close" OnClick="Close_Click" />
                         </div>
                     </div>
                            <br /><br />
                     </div>
            </asp:Panel>
            <!-- ModalPopupExtender -->
            </div>
             </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
