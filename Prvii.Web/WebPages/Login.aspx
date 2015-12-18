<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Prvii.Web.WebPages.Login" %>

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
                <table runat="server" id="tblLogin" style="width: 100%;">
                    <tr>
                        <td>
                            <div class="col-lg-12 login-container">
                                <h1>
                                    <asp:Label ID="lblPageHeader" runat="server" Text="Log in" /></h1>
                                <div class="col-sm-12" style="text-align:center;"> <asp:Label ID="lblMessage" runat="server" Text="" EnableViewState="false"></asp:Label></div>
                                <div class="col-sm-4"></div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <label for="email">Email</label>
                                        <asp:TextBox ID="tbEmail" runat="server" TextMode="Email" CssClass="form-control" MaxLength="50" TabIndex="1" placeholder="Enter email"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbEmail" CssClass="requiredField"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="form-group">
                                        <label for="pwd">Password</label>
                                        <asp:TextBox ID="tbPassword" runat="server" TextMode="Password" CssClass="form-control" MaxLength="50" TabIndex="2" placeholder="Enter password"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbPassword" CssClass="requiredField"></asp:RequiredFieldValidator>
                                    </div>
                                    <div>
                                        <label>
                                            <asp:LinkButton ID="lbtnForgotPassword" runat="server" Style="float: right; vertical-align: central; line-height: 28px" Text="Forgot Password" CausesValidation="false" OnClick="lbtnForgotPassword_Click" /></label>
                                    </div>
                                    <asp:Button ID="btnLogin" runat="server" Text="Log in" CssClass="btn btn-default solid-btn" OnClick="btnLogin_Click" TabIndex="3" />&nbsp;&nbsp;
                                </div>
                                <div class="col-sm-4"></div>
                            </div>
                        </td>
                    </tr>
                </table>
                <table runat="server" id="tblSelectRole" visible="false" style="width: 100%;">
                    <tr>
                        <td>
                            <div class="col-lg-12 login-container">
                                <h1>Select Role</h1>

                                <div class="col-sm-4"></div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <label>Role</label>

                                        <asp:DropDownList ID="ddlRoles" runat="server" CssClass="form-control" />
                                        <asp:RequiredFieldValidator ID="rfvRoles" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="ddlRoles" InitialValue="0" CssClass="requiredField"></asp:RequiredFieldValidator>
                                    </div>
                                    <asp:Button ID="btnContinue" runat="server" Text="Continue" CssClass="btn btn-default solid-btn" OnClick="btnContinue_Click" />
                                </div>
                                <div class="col-sm-4"></div>
                            </div>
                        </td>
                    </tr>
                </table>
                <table runat="server" id="tblForgotPassword" visible="false" style="width: 100%;">
                    <tr>
                        <td>
                            <div class="col-lg-12 login-container">
                                <h1>Forgot Password</h1>
                                <div class="col-sm-12" style="text-align:center;">
                                    <asp:Label ID="lblMessageForgetpassword" runat="server" Text="" EnableViewState="false"></asp:Label>

                                </div>
                                <div class="col-sm-4"></div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <label for="email">Email</label>
                                        <asp:TextBox ID="tbForgotPasswordEmail" runat="server" TextMode="Email" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbForgotPasswordEmail" CssClass="requiredField"></asp:RequiredFieldValidator>
                                    </div>
                                    <asp:Button ID="btnForgotPasswordSendEmail" runat="server" Text="Send Email" CssClass="btn btn-default solid-btn" OnClick="btnForgotPasswordSendEmail_Click" />&nbsp;&nbsp;
                                    <asp:Button ID="btnForgotPasswordCancel" runat="server" Text="Cancel" CssClass="btn btn-default solid-btn" CausesValidation="false" OnClick="btnForgotPasswordCancel_Click" />
                                    </div>
                                <div class="col-sm-4"></div>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
