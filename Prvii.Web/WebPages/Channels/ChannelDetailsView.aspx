<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="ChannelDetailsView.aspx.cs" Inherits="Prvii.Web.WebPages.Channels.ChannelDetailsView" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .popup_box {
            display: none;
            position: absolute;
            top: 25%;
            left: 34%;
            width: 30%;
            height: 15%;
            padding: 16px;
            border: 2px solid #1C8BCE;
            background-color: white;
            z-index: 1002;
            overflow: auto;
        }

        .popup_background {
            display: none;
            position: absolute;
            top: 0%;
            left: 0%;
            width: 100%;
            height: 100%;
            background-color: black;
            z-index: 1001;
            -moz-opacity: 0.8;
            opacity: .80;
            filter: alpha(opacity=80);
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="upPage" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="row">
        <div class="col-lg-12" id="tblChannelDetails" runat="server">
            <div class="col-md-1"></div>
            <div class="col-sm-3">
                <asp:Image ID="imgLeft" runat="server" Width="100%" />
            </div>
            <div class="col-sm-4">
                <div class="celebrity-detail">

                    <div class="row" runat="server" id="tblMenus">
                        <div class="col-md-6">                            
                                <asp:Button ID="btnMessageProcessing" runat="server" Text="Messages" CssClass="tabs text-center" />                           
                        </div>
                        <div class="col-md-6">
                                <asp:Button ID="btnEventsAndCommitments" runat="server" Text="Event & Commitment" CssClass="tabs text-center" />
                        </div>
                        <div class="col-md-6" runat="server" id="trSubscribers">                            
                                <asp:Button ID="btnSubscribers" runat="server" Text="Subscribers" CssClass="tabs text-center" />                            
                        </div>
                        <div class="col-md-6">                           
                                <asp:Button ID="btnURLs" runat="server" Text="URL" CssClass="tabs text-center popup-open" OnClientClick="document.getElementById('popup').style.display='block'; return false;" />
                          </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-12">
                            
                            <asp:Image ID="imgDescription" runat="server" Width="100%" />
                            <br />
                            <asp:Label ID="lblPricingInfo" runat="server" Text="PRICE" Style="text-align:center;"></asp:Label><br />
                            <br />
                            <div class="row">
                                <div class="col-md-6">
                                    <asp:Button ID="btnAddToCartNewUser" runat="server" Text="Add To Cart New Subscriber" CssClass="btn btn-default solid-btn" Style="float: left" OnClick="btnAddToCartNewUser_Click" />
                                </div>
                                <div class="col-md-6">
                                    <asp:Button ID="btnAddToCartExistingUser" runat="server" Text="Add To Cart Existing Subscriber" CssClass="btn btn-default solid-btn" Style="float: right" OnClick="btnAddToCartExistingUser_Click" />
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-default solid-btn" Width="100px" Style="float: right" Visible="false" />
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
            <div class="col-sm-3">
                <asp:Image ID="imgRight" runat="server" Width="100%" />
            </div>
            <div class="col-md-1"></div>
        </div>
        <br />
        <div class="col-lg-12" id="tblError" runat="server" visible="false">
            <div class="col-sm-12">
                <p>
                    <b>
                        <asp:Label ID="lblError" runat="server" Text="" /></b>
                </p>
                <br />
            </div>
        </div>
    </div>
    <div id="popup" class="popup-wrapper">
			<div class="popup-container">
				<label>URL:</label><br/>
				<asp:Label ID="lblURL" runat="server" Text="" />
				<div class="button-div">
					<div class="col-md-4">
                           <a href="javascript:void(0)" class="btn btn-default solid-btn popup-close" onclick="document.getElementById('popup').style.display='none';">Close</a>
					</div>
					<div class="col-md-8">
					</div>
				</div>
			</div>
		</div>

                           

</asp:Content>
