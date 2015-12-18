<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="CelebrityPriceManagement.aspx.cs" Inherits="Prvii.Web.WebPages.Channels.CelebrityPriceManagement" %>

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
                    <h1>Price Management </h1>
                    <hr />
                    <asp:Label ID="lblErrormessage" runat="server" ForeColor="Red"></asp:Label>
                    <div class="row">
                        <div class="col-sm-12 col-xs-12">
                            <label>Celebrity </label>
                            <asp:DropDownList ID="chkblChannels" runat="server" CssClass="form-controlOld"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="chkblChannels" ErrorMessage="Please select celebrity." Display="Dynamic" SetFocusOnError="True" ValidationGroup="report">Please select celebrity.</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row" style="overflow-x: auto;">
                        <div class="row">
                            <asp:GridView ID="gvPrviiAccountMaster" runat="server" AutoGenerateColumns="false" CssClass="body_table_content"
                                DataKeyNames="ID" OnRowDataBound="gvPrviiAccountMaster_RowDataBound" ShowHeader="false" CellSpacing="5" CellPadding="5">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <table style="width: 450px; border: 0.5px solid #808080; line-height: 45px;" class="body_table_content">
                                                <tr>
                                                    <td style="width: 150px;">
                                                        <b>
                                                            <asp:HiddenField ID="hidAccountID" runat="server" Value='<%# Eval("ID") %>' />
                                                            <asp:Label ID="lblAccountName" runat="server" Text='<%# Eval("Account") %>'></asp:Label></b>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPrecentage" runat="server" Width="100px" Text='<%# Eval("Distribution") %>' CssClass="form-control" MaxLength="5" onkeypress="return isNumber(event)" /></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" style="padding-left: 20px; padding-bottom: 5px;">
                                                        <asp:GridView ID="gvPrviiAccountSubMaster" ShowHeader="false" runat="server" AutoGenerateColumns="false" CssClass="body_table_content" Width="350px">
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidAccountIDSub" runat="server" Value='<%# Eval("ID") %>' />
                                                                        <asp:Label ID="lblAccountNameSub" runat="server" Text='<%# Eval("Account") %>'></asp:Label></b>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtPrecentage" runat="server" Width="100px" Text="0" CssClass="form-control" MaxLength="5" onkeypress="return isNumber(event)" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>

                                                    </td>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="row">
                            <br>
                            <div class="row">
                                <div class="col-sm-12">
                                    <div class="col-xs-6">
                                        <asp:Button ID="btnPriceSave" runat="server" Text="Update" ValidationGroup="pop1" CssClass="btn btn-default solid-btn" OnClick="btnPriceSave_Click" />
                                    </div>
                                    <div class="col-xs-6">
                                        <asp:Button ID="btnCancelPupop" runat="server" Text="Cancel" ValidationGroup="pop" CssClass="btn btn-default solid-btn" OnClick="btnCancelPupop_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
