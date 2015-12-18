<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="UserDetails.aspx.cs" Inherits="Prvii.Web.WebPages.Users.UserDetails" %>

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
    <asp:UpdatePanel runat="server" ID="upPage">
        <ContentTemplate>
            <div class="row">
                <div class="col-lg-12 login-container">
                    <h1>

                        <asp:Label ID="lblTitle" runat="server" Text="Register"></asp:Label></h1>
                    <div class="col-sm-2"></div>
                    <div class="col-sm-4">
                        <div class="form-group">
                            <label for="email">Email</label>
                            <asp:TextBox ID="tbEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="50" AutoPostBack="true" OnTextChanged="tbEmail_TextChanged"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbEmail" CssClass="requiredField"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <label>Nickname</label>
                            <asp:TextBox ID="tbNickName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label>First Name</label>
                            <asp:TextBox ID="tbFirstName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbFirstName" CssClass="requiredField"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <label>Last Name</label>
                            <asp:TextBox ID="tbLastName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbLastName" CssClass="requiredField"></asp:RequiredFieldValidator>
                        </div>
                        <asp:Panel ID="tblrowPassword" runat="server" Width="100%">
                            <div class="form-group">
                                <label>Password</label>
                                <asp:TextBox ID="tbPassword" runat="server" CssClass="form-control" MaxLength="50" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbPassword" CssClass="requiredField"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <label>Confirm Password</label>
                                <asp:TextBox ID="tbConfirmPassword" runat="server" CssClass="form-control" MaxLength="50" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbConfirmPassword" CssClass="requiredField"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Password & Confirm Password should be same!" Display="Dynamic" ControlToValidate="tbConfirmPassword" ControlToCompare="tbPassword" CssClass="requiredField"></asp:CompareValidator>
                            </div>
                        </asp:Panel>
                        <div class="form-group">
                            <label>Mobile</label>
                            <asp:TextBox ID="tbMobile" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbMobile" CssClass="requiredField"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <label>Telephone</label>
                            <asp:TextBox ID="tbTelephone" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-sm-4">
                        <div class="form-group">
                            <label>Delivery Mechanism</label>
                            <asp:CheckBoxList ID="cblDeliveryMechanisms" runat="server" RepeatDirection="Horizontal" CssClass="form-control normal" CellPadding="0" CellSpacing="0" >
                            </asp:CheckBoxList>
                        </div>
                        <div class="form-group">
                            <label>Device Type</label>
                            <asp:RadioButtonList ID="rblDeviceTypes" runat="server" CssClass="form-control normal" RepeatDirection="Horizontal" CellPadding="0" CellSpacing="0" AutoPostBack="true" OnSelectedIndexChanged="rblDeviceTypes_SelectedIndexChanged" />
                            <asp:RequiredFieldValidator ID="rfvSendTo" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="rblDeviceTypes" CssClass="requiredField"></asp:RequiredFieldValidator>
                        </div>
                        <asp:Panel ID="trAddress" runat="server" Width="100%">

                            <div class="form-group">
                                <label>Address 1</label>
                                <asp:TextBox ID="tbAddress1" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>Address 2</label>
                                <asp:TextBox ID="tbAddress2" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            </div>
                        </asp:Panel>
                        <div class="form-group">
                            <label>Zipcode</label>
                            <asp:TextBox ID="tbZipCode" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbZipCode" CssClass="requiredField"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <label>Country</label>
                            <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="ddlCountry" InitialValue="0" CssClass="requiredField"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <label>State / Terrotory / Province</label>
                            <asp:DropDownList ID="ddlStates" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>City</label>
                            <asp:TextBox ID="tbCity" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label>Timezone</label>
                            <asp:DropDownList ID="ddlTimezones" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvddlTimezones" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="ddlTimezones" InitialValue="0" CssClass="requiredField"></asp:RequiredFieldValidator>
                        </div>

                        <div class="form-group">
                            <label>
                                <asp:Label ID="lblRolesLabel" runat="server" Text="Roles"></asp:Label>
                            </label>
                            <asp:CheckBoxList ID="cblRoles" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" CssClass="form-control" CellPadding="0" CellSpacing="0"  OnSelectedIndexChanged="cblRoles_SelectedIndexChanged" />
                        </div>
                        <asp:Panel ID="trIsActive" runat="server" Width="100%">
                            <div class="form-group">
                                <label>Is Active</label>
                                <asp:CheckBox ID="cbxIsActive" runat="server" />
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblGroupLabel" runat="server" Text="Group" Visible="false"></asp:Label>
                                <asp:Label ID="lblChannelLabel" runat="server" Text="Celebrity" Visible="false"></asp:Label>

                            </div>
                            <div class="form-group">
                                <asp:DropDownList ID="ddlGroups" runat="server" CssClass="form-control" Visible="false" />
                                <asp:DropDownList ID="ddlChannels" runat="server" CssClass="form-control" Visible="false" />

                            </div>
                        </asp:Panel>
                    </div>
                    <div class="col-sm-2"></div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12">
                    <div class="col-sm-2"></div>
                    <div class="col-sm-4">
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-default solid-btn" OnClick="btnSave_Click" />&nbsp;&nbsp;
					
			
                    </div>
                    <div class="col-sm-4">
                        <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-default" OnClick="btnBack_Click" CausesValidation="false" />

                    </div>
                    <div class="col-sm-2"></div>
                </div>
            </div>
            <h2></h2>

        </ContentTemplate>
    </asp:UpdatePanel>



</asp:Content>
