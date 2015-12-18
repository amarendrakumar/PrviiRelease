<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="UserPasswordReset.aspx.cs" Inherits="Prvii.Web.WebPages.Users.UserPasswordReset" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Reset Password</h2>
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
            <asp:Label ID="lblMessage" runat="server" Text="" EnableViewState="false"></asp:Label>
            <table class="body_table_content" style="width: 30%" runat="server" id="tblForgotPassword">
                <tr>
                    <td style="width: 20%">Password</td>
                    <td style="width: 80%">
                        <asp:TextBox ID="tbPassword" runat="server" TextMode="Password" CssClass="form-control" MaxLength="50" TabIndex="1"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbPassword" CssClass="requiredField"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Confirm Password</td>
                    <td>
                        <asp:TextBox ID="tbConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" MaxLength="50" TabIndex="2"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbConfirmPassword" CssClass="requiredField"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Password & Confirm Password should be same!" ControlToValidate="tbConfirmPassword" ControlToCompare="tbPassword"></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnResetPassword" runat="server" Text="Log in" CssClass="btn-primary" TabIndex="3"  OnClick="btnResetPassword_Click"/>&nbsp;&nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-primary" CausesValidation="false" PostBackUrl="~/WebPages/Channels/ChannelList.aspx" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
