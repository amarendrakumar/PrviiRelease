<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckoutCompleteMobile.aspx.cs" Inherits="Prvii.Web.WebPages.ShoppingCarts.CheckoutCompleteMobile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function Close()
        {
            alert("close");
            window.close();
            alert("close1");
            window.self.close();
            alert("close2");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Literal ID="litDetails" runat="server"></asp:Literal>
        <a onclick="Close();"  > exits </a>
    </div>
    </form>
</body>
</html>
