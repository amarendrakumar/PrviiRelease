<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="ChannelMessageList.aspx.cs" Inherits="Prvii.Web.WebPages.ChannelMessages.ChannelMessageList" %>

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
                    <h1>
                        <asp:Label ID="lblPageHeader" runat="server" Text="Messages" />
                        For 
                        <asp:Literal ID="litCelebrityName" runat="server"></asp:Literal></h1>
                    <hr />
                    <div runat="server" id="tblChannelMessage" >
                        <div class="row">
                            <div class="col-sm-3 col-xs-12">Show</div>
                            <div class="col-sm-3 col-xs-12">
                                <asp:DropDownList ID="ddlFilters" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilters_SelectedIndexChanged">
                                    <asp:ListItem Text="All" />
                                    <asp:ListItem Text="Scheduled" />
                                    <asp:ListItem Text="Past" />
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 col-xs-12">
                                <asp:Button ID="btnAddNewMessage" runat="server" Text="Add New Message" CssClass="btn btn-default solid-btn" />

                            </div>
                            <div class="col-sm-3 col-xs-12">
                                <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-default" />

                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="row" style="overflow-x: auto;">
                            <asp:GridView ID="gvChannelMessageList" runat="server" CssClass="table-responsive table-striped" PagerStyle-HorizontalAlign="Right"
                                Width="98%" AutoGenerateColumns="false" AllowPaging="true" PageSize="20" GridLines="None" OnRowDataBound="gvChannelMessageList_RowDataBound"
                                EmptyDataText="No Messages Available" EmptyDataRowStyle-ForeColor="Red" OnPageIndexChanging="gvChannelMessageList_PageIndexChanging">

                                <PagerStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:HyperLinkField HeaderText="Subject" DataNavigateUrlFields="ID,ChannelID" DataNavigateUrlFormatString="~/WebPages/ChannelMessages/ChannelMessageDetails.aspx?ID={0}&ChannelID={1}"></asp:HyperLinkField>
                                    <asp:BoundField DataField="ScheduledOn" HeaderText="Scheduled On"></asp:BoundField>
                                    <asp:BoundField DataField="TimeZoneID" HeaderText="Time Zone"></asp:BoundField>
                                    <asp:BoundField DataField="StatusEmail" HeaderText="Status Email"></asp:BoundField>
                                    <asp:BoundField DataField="StatusText" HeaderText="Status SMS"></asp:BoundField>
                                    <asp:HyperLinkField HeaderText="Edit/View" Text="Edit/View" DataNavigateUrlFields="ID,ChannelID" DataNavigateUrlFormatString="~/WebPages/ChannelMessages/ChannelMessageDetails.aspx?ID={0}&ChannelID={1}"></asp:HyperLinkField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                    <div runat="server" id="tblChannelSubscriberMessageList" visible="false">
                        <div class="row">
                            <div class="col-sm-4 col-xs-12">
                                <asp:Literal ID="litMessageTypeHeader" runat="server"></asp:Literal></div>

                            <div class="col-sm-4 col-xs-12">
                                <asp:Button ID="btnPastAllMessage" runat="server" Text="Past Messages" CssClass="btn btn-default solid-btn" OnClick="btnPastAllMessage_Click" />

                            </div>
                            <div class="col-sm-4 col-xs-12">
                                <asp:Button ID="btnBackMessage" runat="server" Text="Current Messages" CssClass="btn btn-default solid-btn" OnClick="btnBackMessage_Click" />

                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="row" style="overflow-x: auto;">
                            <asp:GridView ID="gvChannelSubscriberMessageList" runat="server" CssClass="table-responsive table-striped" PagerStyle-HorizontalAlign="Right"
                                Width="98%" AutoGenerateColumns="false" AllowPaging="true" PageSize="20" GridLines="None" Visible="false"
                                EmptyDataText="No Messages Available" EmptyDataRowStyle-ForeColor="Red" OnPageIndexChanging="gvChannelSubscriberMessageList_PageIndexChanging">

                                <PagerStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="Subject" HeaderText="Subject"></asp:BoundField>
                                    <asp:BoundField DataField="SentOn" HeaderText="Sent On"></asp:BoundField>
                                    <asp:BoundField DataField="TimeZoneID" HeaderText="Time Zone"></asp:BoundField>
                                    <asp:HyperLinkField HeaderText="View" Text="View" DataNavigateUrlFields="ID,ChannelID" DataNavigateUrlFormatString="~/WebPages/ChannelMessages/ChannelMessageDetails.aspx?ID={0}&ChannelID={1}"></asp:HyperLinkField>
                                </Columns>
                            </asp:GridView>

                        </div>

                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
