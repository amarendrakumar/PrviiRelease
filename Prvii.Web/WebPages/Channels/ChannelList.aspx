<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="ChannelList.aspx.cs" Inherits="Prvii.Web.WebPages.Channels.ChannelList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
                   <div class="col-lg-12 list-table">
                        <h1>Celebrities</h1>
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
            
            <table class="table-responsive table-striped" style="width: 100%" runat="server" id="tblFilter" visible="false">
                <tr>
                    <td style="white-space: nowrap; text-align: center">&nbsp;&nbsp;
                        <div class="row">
					<div class="col-md-3">  </div>
					<div class="col-md-3">
                       <asp:DropDownList ID="ddlFilters" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilters_SelectedIndexChanged">
                   <asp:ListItem Text="All" Value="0" />
                    <asp:ListItem Text="Subscribed" Value="1" />
                    <asp:ListItem Text="Celebrity VIP Clubs" Value="2" />
                     
                </asp:DropDownList>
					</div>
                           
				</div>

               
                    </td>
                </tr>
            </table>
             <br />
                            <br />
            <table class="table-responsive table-striped" style="width: 100%" runat="server" id="tblGroupName" visible="false">
                <tr>
                    <td>
                        <div class="row">
					<div class="col-md-9"> Group:<strong> <asp:Label ID="lblGroupName" runat="server" Text=""></asp:Label></strong></div>
					<div class="col-md-3">
                       
					</div>
				</div>
                       
                    </td>
                </tr>
            </table>
            <table class="table-responsive table-striped" style="width: 100%" runat="server" id="tblAddNewCelebrity" visible="false">
                <tr>
                    <td>
                        <div class="row">
					<div class="col-md-9"></div>
					<div class="col-md-3">
                        <asp:Button ID="btnAddNewCelebrity" runat="server" Text="Add New Celebrity" CssClass="btn btn-default solid-btn" PostBackUrl="~/WebPages/Channels/ChannelDetailsEdit.aspx?ID=0" />
					</div>
				</div>
				<br/> 
                        
                    </td>
                </tr>
            </table>

            <asp:DataList ID="DataList1" runat="server" Width="98%" DataKeyField="ID" OnItemCommand="DataList1_ItemCommand" OnItemDataBound="DataList1_ItemDataBound">
                <HeaderTemplate>
                     <div class="row list-table-head">
                    <div class="col-md-1" runat="server" id="DivSelectHeader" visible="false">
						Select
					</div>
					<div class="col-md-2" runat="server" id="DivPhotoHeader" >
						Photo
					</div>
					<div class="col-md-2" runat="server" id="DivNameHeader" >
						Name
					</div>
					<div class="col-md-1"  runat="server" id="DivPriceHeader" >
						Subscription<br/>Price
					</div>
					<div class="col-md-2"  runat="server" id="DivMobileHeader" >
						Mobile
					</div>
					<div class="col-md-3"  runat="server" id="DivEmailHeader" >
						Email
					</div>
					<div class="col-md-1"  runat="server" id="DivActiveHeader" >
						Active
					</div>
                    	<div class="col-md-2" runat="server" id="DivSubscribeHeader" >
						Subscribe
					</div>
					<div class="col-md-1" runat="server" id="DivEditHeader" >
						Edit
					</div>
				</div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="row list-table-body">
                    <div class="col-md-1"  runat="server" id="DivSelectItem" visible="false">
						<div class="celebrity-name">
							 <asp:CheckBox ID="cbxSelect" runat="server" />
                            <asp:HiddenField ID="hfID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"ID")%>' />
						</div>
					</div>
					<div class="col-md-2"  runat="server" id="DivPhotoItem">
						<div class="celebrity-img">
                            <a href='<%#  "~/WebPages/Channels/ChannelDetailsView.aspx?ID="+ DataBinder.Eval(Container.DataItem,"ID")%>' runat="server" id="aPhoto">
                                <asp:Image runat="server" ID="imgPhoto" ImageUrl='<%# "~/WebPages/GetChannelMedia.aspx?ChannelID="+ DataBinder.Eval(Container.DataItem,"ID")%>'  />
                            </a>
						</div>
					</div>
					<div class="col-md-2"  runat="server" id="DivNameItem">
						<div class="celebrity-name">
							<asp:HyperLink ID="hypCelebrityName" runat="server" NavigateUrl='<%# "~/WebPages/Channels/ChannelDetailsView.aspx?ID="+ DataBinder.Eval(Container.DataItem,"ID")%>'>
                                 <%# DataBinder.Eval(Container.DataItem,"Name")%></asp:HyperLink>
						</div>
					</div>
					<div class="col-md-1"  runat="server" id="DivPriceItem">
						<div class="celebrity-price">
							<span class="priceText">Subscription Price</span><span class="price">
                                <%# DataBinder.Eval(Container.DataItem,"Price","{0:c}")%>
                               </span>
						</div>
					</div>
					<div class="col-md-2"  runat="server" id="DivMobileItem">
						<div class="celebrity-btn">
							<span class="mobileText">Mobile: </span>  <%# DataBinder.Eval(Container.DataItem,"Phone")%>
						</div>
					</div>
					<div class="col-md-3"  runat="server" id="DivEmailItem">
						<div class="celebrity-name">
							 <%# DataBinder.Eval(Container.DataItem,"Email")%>
						</div>
					</div>
					
					<div class="col-md-1"  runat="server" id="DivActiveItem">
						<div class="celebrity-name">
							<span class="activeText">Active: </span>  <%# DataBinder.Eval(Container.DataItem,"IsActive")%>
						</div>
					</div>
					<div class="col-md-2"  runat="server" id="DivSubscribeItem">
						<div class="celebrity-name">
                             <asp:HiddenField ID="hfIsSubscribed" runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"IsSubscribed")%>' />
                            <asp:Button ID="btnAddToCart" runat="server" Text="Add To Cart" CssClass="btn btn-default solid-btn" CommandName="AddToCart" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID")%>' />
                            <asp:LinkButton ID="lbtnUnSubscribe" runat="server" CssClass="btn btn-default" Text="Unsubscribe" CommandName="Unsubscribe" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID")%>'
                                OnClientClick="return confirm('Are you sure you want to Unsubscribe to this celebrity?');" />
						</div>
					</div>
					<div class="col-md-1"  runat="server" id="DivEditItem">
						<div class="celebrity-name">
							<asp:HyperLink ID="HypEdit" runat="server" NavigateUrl='<%# "~/WebPages/Channels/ChannelDetailsEdit.aspx?ID="+ DataBinder.Eval(Container.DataItem,"ID")%>'>
                                 Edit</asp:HyperLink>
						</div>
					</div>
				</div>
                </ItemTemplate>
            </asp:DataList>           
            <table class="body_table_content" style="width: 100%" runat="server" id="tblSaveGroupChannels" visible="false">
                <tr>
                    <td>
                        				<div class="row">
					<div class="col-md-8"></div>
					<div class="col-md-4">
						<div class="col-md-6">
						<asp:Button ID="btnSaveGroupChannels" runat="server" Text="Save" CssClass="btn btn-default solid-btn" OnClick="btnSaveGroupChannels_Click" />&nbsp;&nbsp;
						</div>
						<div class="col-md-6">
						 <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default" OnClick="btnCancel_Click" />
						</div>
					</div>
				</div>

                        
                       
                    </td>
                </tr>
            </table>
              
        </ContentTemplate>
    </asp:UpdatePanel>
                         </div>    

              </div>
</asp:Content>
