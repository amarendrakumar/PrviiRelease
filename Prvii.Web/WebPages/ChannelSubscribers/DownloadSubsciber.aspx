<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="DownloadSubsciber.aspx.cs" Inherits="Prvii.Web.WebPages.ChannelSubscribers.DownloadSubsciber" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="row">
        <div class="col-lg-12">
            <h1>Download Subscriber</h1>
            <hr />

            <div class="row">

                <div class="col-sm-4 col-xs-12">
                    <asp:DropDownList ID="ddlChannels" runat="server" CssClass="form-control" />
                </div>
                <div class="col-sm-2 col-xs-12">
                    <asp:Label ID="lblSubscriberCount" runat="server" Text=""></asp:Label></div>
                <div class="col-sm-3 col-xs-12">
                    <asp:Button ID="btnAdminDownload" runat="server" Text=" Admin Download " CssClass="btn btn-default solid-btn" OnClick="btnAdminDownload_Click" />

                </div>
                <div class="col-sm-3 col-xs-12">
                    <asp:Button ID="btnAdvertiserDownload" runat="server" Text=" Advertiser Download " CssClass="btn btn-default solid-btn" OnClick="btnAdvertiserDownload_Click" />

                </div>
            </div>


        </div>
    </div>


</asp:Content>
