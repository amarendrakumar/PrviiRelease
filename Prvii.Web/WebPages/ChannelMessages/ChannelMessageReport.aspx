<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="ChannelMessageReport.aspx.cs" Inherits="Prvii.Web.WebPages.ChannelMessages.ChannelMessageReport" %>

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
                    <h1>Message Report For
                        <asp:Literal ID="litCelebrityName" runat="server"></asp:Literal></h1>
                    <hr />

                    <div class="row">
                        <div class="col-sm-3 col-xs-12">
                            <label>Celebrity</label>
                            <asp:DropDownList ID="ddlChannels" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-sm-3 col-xs-6">
                            <label>Start Date</label>
                            <asp:TextBox ID="tbStartDate" runat="server" CssClass="form-control" MaxLength="50" />
                            <ajaxToolkit:CalendarExtender ID="CalendarExtender1" TargetControlID="tbStartDate" runat="server" Format="MM/dd/yyyy" />
                        </div>
                        <div class="col-sm-3 col-xs-6">
                            <label>End Date</label>
                            <asp:TextBox ID="tbEndDate" runat="server" CssClass="form-control" MaxLength="50" />
                            <ajaxToolkit:CalendarExtender ID="CalendarExtender2" TargetControlID="tbEndDate" runat="server" Format="MM/dd/yyyy" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-4">
                            Please enter (subject / celebrity(first name / last name) / message) for search
				
                        </div>
                        <div class="col-sm-5">
                            <div class="col-xs-7">
                                <asp:TextBox ID="tbFilterBy" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            </div>
                            <div class="col-xs-5">
                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-default solid-btn" OnClick="btnSearch_Click" />
                            </div>
                        </div>

                    </div>
                    <br />
                    <br />
                    <div class="row" style="overflow-x: auto;">
                        <asp:GridView ID="gvMessageList" runat="server" CssClass="table-responsive table-striped" PagerStyle-HorizontalAlign="Right"
                            Width="98%" AutoGenerateColumns="false" AllowPaging="true" PageSize="20" GridLines="None" 
                            OnPageIndexChanging="gvMessageList_PageIndexChanging" >

                            <PagerStyle HorizontalAlign="Center" CssClass="pagination" />
                            <Columns>
                                <asp:BoundField DataField="Subject" HeaderText="Subject">
                                    <ItemStyle Width="15%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ChannelName" HeaderText="Celebrity">
                                    <ItemStyle Width="15%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Message" HeaderText="Message">
                                    <ItemStyle Width="55%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ScheduledOn" HeaderText="Date">
                                    <ItemStyle Width="15%" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>


                    </div>

                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
