<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="CheckoutComplete.aspx.cs" Inherits="Prvii.Web.WebPages.ShoppingCarts.CheckoutComplete" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Checkout Complete</h2>
    <p></p>
    <b>Thank you for your purchase!</b>
    <p></p>

    <asp:Button ID="btnContinueShopping" runat="server" Text="Continue Shopping" CssClass="btn-primary" PostBackUrl="~/WebPages/Channels/ChannelList.aspx" />
</asp:Content>
