<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="Prvii.Web.test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btnSendMSMS" runat="server" Text="Sent SMS" OnClick="btnSendMSMS_Click" />

            <asp:Button ID="btnSentEmail" runat="server" Text="Sent Email" OnClick="btnSentEmail_Click" />
            <br />

            <asp:Label ID="lblConfirmationMessage" runat="server" ForeColo="red"></asp:Label>

            <asp:Button ID="btnSendEmail" runat="server" Text="Send Email from Godaddy" OnClick="btnSendEmail_Click" />

            <br />
            <asp:Button ID="btnSubmit" runat="server" Text="Apple Push Message send" OnClick="btnSubmit_Click" />
            <br />
            <asp:TextBox ID="txtNotification" runat="server"></asp:TextBox>
        </div>
    </form>
</body>
</html>
