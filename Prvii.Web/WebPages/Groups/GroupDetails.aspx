<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="GroupDetails.aspx.cs" Inherits="Prvii.Web.WebPages.Groups.GroupDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
            <div class="col-lg-12">
                <h1>Group Details</h1>
				<hr/>
				<div class="col-sm-6">
						<div class="form-group">
						  <label>Name</label>
						   <asp:TextBox ID="tbName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbName" CssClass="requiredField"></asp:RequiredFieldValidator>
						</div>
						<div class="form-group">
						  <label>Email</label>
						   <asp:TextBox ID="tbEmail" runat="server" CssClass="form-control" MaxLength="50" TextMode="Email"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbEmail" CssClass="requiredField"></asp:RequiredFieldValidator>

						</div>
						<div class="form-group">
						  <label>Mobile</label>
						  <asp:TextBox ID="tbMobile" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
						</div>  
						<div class="form-group">
						  <label>Telephone</label>
						  <asp:TextBox ID="tbTelephone" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
						</div>  
				</div>
				
				<div class="col-sm-6">	
						<div class="form-group">
						  <label>Address 1</label>
						  <asp:TextBox ID="tbAddress1" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
						</div>
						<div class="form-group">
						  <label>Address 2</label>
						   <asp:TextBox ID="tbAddress2" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
						</div>
						<div class="form-group">
						  <label>ZipCode</label>
						  <asp:TextBox ID="tbZipCode" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
						</div>
						<div class="form-group">
						  <label>Is Active</label><br/>
						   <asp:CheckBox ID="cbxIsActive" runat="server" Text=" Active " /> 
						</div>
						<br/>
						<div class="row">
							<div class="col-lg-12">
								<div class="col-sm-6">
                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-default solid-btn" OnClick="btnSave_Click" />
									
								</div>
								<div class="col-sm-6">
									 <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default" CausesValidation="false" OnClick="btnCancel_Click" />
								</div>
							</div>
						</div>
				</div>
			</div>
		</div>
    
</asp:Content>
