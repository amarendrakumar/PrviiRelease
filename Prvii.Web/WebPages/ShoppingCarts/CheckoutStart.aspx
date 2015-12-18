<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckoutStart.aspx.cs" Inherits="Prvii.Web.WebPages.ShoppingCarts.CheckoutStart" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~/Styles/Site.css" rel="stylesheet" id="cssFile" runat="server" />
</head>
<body>

    <table class="header_table">
        <tr>
            <td style="width: 18%; vertical-align: top; text-align: right">
                <img runat="server" alt="Prvii Celebrity Service" src="~/Images/logo.png" style="width: 130px; height: 49px; margin-top: 5px;" />
            </td>
            <td style="width: 82%; white-space: nowrap">
                &nbsp;
                </td>
        </tr>
    </table>
    <table class="body_table">
        <tr>
            <td>
                <h2>Payment</h2>
                <br />
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/indicator.gif" /> &nbsp;
                Please wait while you are redirected to the payment server.
            </td>
        </tr>
    </table>
    <table class="body_table">
        <tr>
            <td>
                <p></p>
                <hr />
                <p class="footer_style">
                    &copy; <%: DateTime.Now.Year %> - Verify Smart Corp - All Rights Reserved
                </p>
            </td>
        </tr>
    </table>

    <form id="payForm" method="post" action="<%= payPalURL %>">
        <!-- Identify your business so that you can collect the payments. -->
        <input type="hidden" name="business" value="<%= business %>">

        <!-- Specify a Subscribe button. -->
        <input type="hidden" name="cmd" value="_xclick-subscriptions">

        <!-- Identify the subscription. -->
        <input type="hidden" name="item_name" value="<%= item_name %>">
        <input type="hidden" name="item_number" value="<%= item_number %>">

        <!-- Set the terms of the regular subscription. -->
        <input type="hidden" name="currency_code" value="<%= currency_code %>">
        <input type="hidden" name="a3" value="<%= a3 %>">
        <input type="hidden" name="p3" value="<%= p3 %>">
        <input type="hidden" name="t3" value="<%= t3 %>">

        <!-- Set recurring payments until canceled. -->
        <input type="hidden" name="src" value="<%= src %>">

        <%= srt %>

        <input type="hidden" name="no_shipping" value="<%= no_shipping %>">
        <input type="hidden" name="return" value="<%= return_url %>">
        <input type="hidden" name="rm" value="<%= rm %>">
        <input type="hidden" name="notify_url" value="<%= notify_url %>">
        <input type="hidden" name="cancel_return" value="<%= cancel_url %>">
        <input type="hidden" name="custom" value="<%= custom %>">
    </form>

    <script language="javascript">
        document.forms["payForm"].submit();
    </script>
</body>
</html>
