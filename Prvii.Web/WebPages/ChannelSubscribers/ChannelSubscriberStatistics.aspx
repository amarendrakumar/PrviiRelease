<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="ChannelSubscriberStatistics.aspx.cs" Inherits="Prvii.Web.WebPages.ChannelSubscribers.ChannelSubscriberStatistics" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
    <asp:UpdateProgress ID="upgPage" runat="server" AssociatedUpdatePanelID="upPage">
        <ProgressTemplate>
            <div class="progressBackground"></div>
            <div class="progressBox">
                <img alt="" runat="server" src="~/Images/indicator.gif" style="vertical-align: middle;" />
                <span>&nbsp;&nbsp;Please wait...</span>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="upPage" runat="server">
        <ContentTemplate>
             <div class="row">
            <div class="col-lg-12">
                <h1>Subscriber Statistics For <asp:Literal  ID="litCelebrityName" runat="server"></asp:Literal></h1>
				<hr/>
				<div class="row" runat="server" id="tblChannelFilter" visible="false">
                    <div class="col-sm-3 col-xs-6">
                           <asp:DropDownList ID="ddlChannels" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="ddlChannels" InitialValue="0" CssClass="requiredField"></asp:RequiredFieldValidator>
                    </div>
				</div>
                <br />
				<div class="row">
					<div class="col-sm-3 col-xs-6">
						 <asp:DropDownList ID="ddlSubscriber" runat="server" CssClass="form-control"  AutoPostBack="true" OnSelectedIndexChanged="ddlSubscriber_SelectedIndexChanged">
                            <asp:ListItem Text="Total Active Subscriber" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Paying Subscribers by the end of" Value="2"></asp:ListItem>
                            <asp:ListItem Text="The number of subscribers who join in" Value="3"></asp:ListItem>
                        </asp:DropDownList>
					</div>
					<div class="col-sm-2 col-xs-6">
						 <asp:DropDownList ID="ddlYears" runat="server" CssClass="form-control" Enabled="false" />
                        <asp:RequiredFieldValidator ID="rfvYears" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="ddlYears" Visible="false" InitialValue="0" CssClass="requiredField"></asp:RequiredFieldValidator>
					</div>
					<div class="col-sm-2 col-xs-6">
					<asp:DropDownList ID="ddlPeriodTypes" runat="server" CssClass="form-control"  Enabled="false" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriodTypes_SelectedIndexChanged" />
                        <asp:RequiredFieldValidator ID="rfvPeriodTypes" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="ddlPeriodTypes" Visible="false" InitialValue="0" CssClass="requiredField"></asp:RequiredFieldValidator>
                      
					</div>
					<div class="col-sm-2 col-xs-6">
						<asp:DropDownList ID="ddlPeriods" runat="server" CssClass="form-control"  Enabled="false" />
                        <asp:RequiredFieldValidator ID="rfvPeriods" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="ddlPeriods" Visible="false" InitialValue="0" CssClass="requiredField"></asp:RequiredFieldValidator>

					</div>
					<div class="col-sm-2 col-xs-6">
						<asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-default solid-btn" OnClick="btnSearch_Click"  />
                       
					</div>
					<div class="col-sm-1 col-xs-6">
                         <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-default" OnClick="btnBack_Click" />
						
					</div>
				</div>
				<br/><br/>
				<div class="row" style="overflow-x: auto;">
				<div class="col-md-12">
					<h4>Total No of Subscribers:   <asp:Label ID="lblSubscriberCount" runat="server" Text=""></asp:Label></h4><br/>
					 <asp:GridView ID="gvUserList" runat="server" CssClass="table-responsive table-striped" PagerStyle-HorizontalAlign="Right"
                Width="98%" AutoGenerateColumns="false" AllowPaging="true" PageSize="20" GridLines="None"  EmptyDataText="No record" EmptyDataRowStyle-ForeColor="Red" >             
                <PagerStyle HorizontalAlign="Center" />
                <Columns>
                    <asp:HyperLinkField HeaderText="Name" DataTextField="Name" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/WebPages/Users/UserDetails.aspx?ID={0}">
                        <ItemStyle Width="30%" />
                    </asp:HyperLinkField>
                    <asp:BoundField DataField="Mobile" HeaderText="Mobile">
                        <ItemStyle Width="20%" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Email" HeaderText="Email">
                        <ItemStyle Width="25%"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="ZipCode" HeaderText="Zip Code">
                        <ItemStyle Width="15%"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="Subscription_EFD" HeaderText="Start Date" DataFormatString="{0:d}">
                        <ItemStyle Width="10%" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
				</div>
				</div>
				
			</div>
		</div>
		
           
        </ContentTemplate>
    </asp:UpdatePanel>

   <%-- <asp:Button ID="btn_DownLoadAll" runat="server" Text="Download All" CssClass="btnStyle" OnClick="btn_DownLoadAll_Click" ValidationGroup="SaveScore" />--%>
</asp:Content>
