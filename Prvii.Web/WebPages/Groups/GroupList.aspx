<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="GroupList.aspx.cs" Inherits="Prvii.Web.WebPages.Groups.GroupList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
            <div class="col-lg-12">
                <h1>Groups</h1>
				<hr/>
				
				<div class="row">
					<div class="col-sm-1 col-xs-3">Name</div>
					<div class="col-sm-3 col-xs-9">
						  <asp:TextBox ID="tbFilterByColumnName" runat="server" CssClass="form-control" MaxLength="50"  />
					</div>
					<div class="col-sm-2 col-xs-6">
						<asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-default solid-btn" />                       
					</div>
					<div class="col-sm-2 col-xs-6">
                        <asp:Button ID="btnAddNew" runat="server" Text="Add New" CssClass="btn btn-default solid-btn" PostBackUrl="~/WebPages/Groups/GroupDetails.aspx?ID=0" />
						
					</div>
				</div>
				<br/><br/>
				<div class="row" style="overflow-x: auto;">
                     <asp:GridView ID="gvGroupList" runat="server" CssClass="table-responsive table-striped" PagerStyle-HorizontalAlign="Right"
        Width="98%" AutoGenerateColumns="false" AllowPaging="true" PageSize="20" GridLines="None">        
        <PagerStyle HorizontalAlign="Center" />
        <Columns>
            <asp:HyperLinkField HeaderText="Name" DataTextField="Name" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/WebPages/Groups/GroupDetails.aspx?ID={0}">
               
            </asp:HyperLinkField>
            <asp:BoundField DataField="Mobile" HeaderText="Mobile">
                
            </asp:BoundField>
            <asp:BoundField DataField="Email" HeaderText="Email">
               
            </asp:BoundField>
            <asp:BoundField DataField="ZipCode" HeaderText="ZipCode">
                
            </asp:BoundField>
            <asp:BoundField DataField="IsActive" HeaderText="Active">
               
            </asp:BoundField>
            <asp:BoundField DataField="UserCount" HeaderText="Users">
              
            </asp:BoundField>
            <asp:HyperLinkField HeaderText="Celebrities" DataTextField="CelebrityCount" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/WebPages/Channels/ChannelList.aspx?GroupID={0}">
               
            </asp:HyperLinkField>
        </Columns>
    </asp:GridView>
					
				</div>
			</div>
		</div>
   
   
</asp:Content>
