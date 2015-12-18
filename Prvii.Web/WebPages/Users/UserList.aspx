<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="Prvii.Web.WebPages.Users.UserList" %>

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
            <div class="col-lg-12">
                <h1><asp:Label ID="lblPageHeader" runat="server" Text="Users"></asp:Label></h1>
				<br/>
				<div class="row">
					<div class="col-md-3 col-sm-6 col-xs-12">
                       	 <asp:DropDownList ID="ddlFilters" runat="server" CssClass="form-control" ValidationGroup="Search" AutoPostBack="true" OnSelectedIndexChanged="ddlFilters_SelectedIndexChanged">
                            <asp:ListItem Text="-Select Column-" Value="0" />
                        </asp:DropDownList>
                         <asp:RequiredFieldValidator ID="rfvddlFilters" runat="server" ErrorMessage="Please select a search criteria!" SetFocusOnError="true" InitialValue="0"
                             ControlToValidate="ddlFilters" ValidationGroup="Search" Display="Dynamic" Text="Please select a search criteria!" ForeColor="#FF3300" ></asp:RequiredFieldValidator>

					</div>
					<div class="col-md-3 col-sm-6 col-xs-12">
					
                          <asp:TextBox ID="tbFilterBy" runat="server" CssClass="form-control" MaxLength="50"  ValidationGroup="Search"/>
                         <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" MaxLength="50"  ValidationGroup="Search" Visible="false"  >
                             <asp:ListItem Text="true" Value="True"></asp:ListItem>
                             <asp:ListItem Text="false" Value="False"></asp:ListItem>
                         </asp:DropDownList>
                         <asp:RequiredFieldValidator ID="rfvFilterBy" runat="server" ErrorMessage="Please enter a search value!" SetFocusOnError="true" 
                             ControlToValidate="tbFilterBy" ValidationGroup="Search" Display="Dynamic"  Text="Please enter a search value!"  ForeColor="#FF3300"></asp:RequiredFieldValidator>
					</div>
					<div class="col-md-6 col-sm-12 col-xs-12">
                        <div class="row">
                            <div class="col-md-4 col-sm-4"><asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-default solid-btn" OnClick="btnSearch_Click"  ValidationGroup="Search" /></div>
                             <div class="col-md-4 col-sm-4"> 
                                 <asp:Button ID="btnCreateNew" runat="server" Text="Add New User" CssClass="btn btn-default solid-btn" PostBackUrl="~/WebPages/Users/UserDetails.aspx?ID=0" />
                                 <asp:Button ID="btnShowStatistics" runat="server" Text="Show Statistics" CssClass="btn btn-default solid-btn" Visible="false" />
                             </div>
                             <div class="col-md-2 col-sm-2">
                                  <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-default solid-btn" OnClick="btnReset_Click"  />
                                   
                             </div>
                            
                   <div class="col-md-2 col-sm-2">
                        <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-default solid-btn" Visible="false" />
                   </div>
                   
                  
                        </div>
						 
                       
					</div>
				
				</div>
			
				<br/><br/>
				<div class="row" style="overflow-x: auto;">
                     <asp:GridView ID="gvUserList" runat="server" CssClass="table-responsive table-striped" PagerStyle-HorizontalAlign="Right"
                Width="98%" AutoGenerateColumns="false" AllowPaging="true" AllowSorting="true" PageSize="20" GridLines="None"
                EmptyDataText="Record Not Found" EmptyDataRowStyle-ForeColor="Red"
                OnPageIndexChanging="gvUserList_PageIndexChanging" OnSorting="gvUserList_Sorting">               
                <PagerStyle HorizontalAlign="Center" />
                <Columns>
                    <asp:HyperLinkField HeaderText="Name" DataTextField="Name" SortExpression="Name" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/WebPages/Users/UserDetails.aspx?ID={0}">
                        <ItemStyle Width="15%" />
                    </asp:HyperLinkField>
                    <asp:BoundField DataField="Mobile" HeaderText="Mobile" SortExpression="Mobile">
                        <ItemStyle Width="20%"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email">
                        <ItemStyle Width="20%"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="ZipCode" HeaderText="Zip Code" SortExpression="ZipCode">
                        <ItemStyle Width="15%"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="RoleName" HeaderText="Role" SortExpression="RoleName">
                        <ItemStyle Width="15%" />
                    </asp:BoundField>
                    <asp:BoundField DataField="IsActive" HeaderText="Active" SortExpression="IsActive">
                        <ItemStyle Width="10%" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
					
				</div>
			</div>
		</div>

            
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
