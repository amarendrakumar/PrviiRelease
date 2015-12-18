<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="RevenueReporting.aspx.cs" Inherits="Prvii.Web.WebPages.ChannelSubscribers.RevenueReporting" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../js/jquery-1.11.3.min.js"> </script>
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
            <script type="text/javascript">
            $(function () { InIEvent() });
            function InIEvent()
            {
                $('#<%= chkAll.ClientID%>').click(function () {
                    if ($(this).is(':checked')) {
                        $("[id*=chkblChannels] input:checkbox").prop('checked', true);
                    }
                    else {
                        $("[id*=chkblChannels] input:checkbox").prop('checked', false);
                    }
                });


                $("[id*=chkblChannels] input:checkbox").change(function () {
                    if ($("[id*=chkblChannels] input:checkbox:checked").length < $('#<%= chkblChannels.ClientID%> input').length) {
                        $('#<%= chkAll.ClientID%>').removeAttr("checked");
                    }
                    else if ($("[id*=chkblChannels] input:checkbox:checked").length = $('#<%= chkblChannels.ClientID%> input').length) {
                        $('#<%= chkAll.ClientID%>').attr("checked", true);
                    }
                })
            }

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(InIEvent);
    </script>
            <div class="row">
            <div class="col-lg-12">
                <h1>Revenue Report </h1>
				<hr/>
				
				<div class="row">
					<div class="col-sm-12 col-xs-12">
						<label>Celebrity </label> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;	<label><asp:CheckBox ID="chkAll" runat="server" Text="Check All" /> </label> 
						 <asp:CheckBoxList ID="chkblChannels" runat="server" CssClass="form-controlOld" RepeatDirection="Horizontal" RepeatColumns="6"  ></asp:CheckBoxList>
					</div>

					<%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="chkblChannels" ErrorMessage="Please select celebrity." Display="Dynamic" SetFocusOnError="True" ValidationGroup="report">Please select celebrity.</asp:RequiredFieldValidator>--%>
				</div>
				<br/>
				<div class="row">
					<div class="col-sm-2 col-xs-6">
						<label>Start Date</label>
						<asp:TextBox ID="txtStartDate" runat="server"  CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"  ControlToValidate="txtStartDate" ErrorMessage="Please enter start date." Display="Dynamic" SetFocusOnError="True" ValidationGroup="report" CssClass="requiredField" >Please enter start date.</asp:RequiredFieldValidator>
                         <ajaxToolkit:CalendarExtender ID="CalendarExtender1" TargetControlID="txtStartDate" runat="server" />
					</div>
					<div class="col-sm-2 col-xs-6">
						<label>End Date</label>
                        <asp:TextBox ID="txtEndDate" runat="server"   CssClass="form-control"></asp:TextBox>
						<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtEndDate" ErrorMessage="Please enter end date." Display="Dynamic" SetFocusOnError="True" ValidationGroup="report" CssClass="requiredField">Please enter end date.</asp:RequiredFieldValidator>
					      <ajaxToolkit:CalendarExtender ID="CalendarExtender2" TargetControlID="txtEndDate" runat="server" />
					</div>
					<div class="col-sm-8">
						<div class="col-xs-4">
                            <label>  </label>
                            <asp:Button ID="btnAdminReport" runat="server" Text=" GO " CssClass="btn btn-default solid-btn" OnClick="btnAdminReport_Click" ValidationGroup="report"   />
						 <%--  <asp:DropDownList ID="ddlMode" runat="server"  CssClass="form-control" Visible="false">
                             <asp:ListItem Text="Default" Value="Default"></asp:ListItem>
                            <asp:ListItem Text="PDF" Value="PDF"></asp:ListItem>
                            <asp:ListItem Text="Excel" Value="Excel"></asp:ListItem>
                          </asp:DropDownList>--%>
						</div>
						<div class="col-xs-4">                            
                              <label>  </label> 
						<asp:ImageButton ID="tbnExcelReport" runat="server"  ImageUrl="~/Images/excel-icon.png"  CssClass="btn down"   OnClick="tbnExcelReport_Click" ValidationGroup="report" Visible="false"   />
                             <asp:ImageButton ID="btnPDFReport" runat="server" ImageUrl ="~/Images/PDF-icon.png"    CssClass="btn down"   OnClick="btnPDFReport_Click" ValidationGroup="report" Visible="false"    />
						</div>
                        <div class="col-xs-4">
                              <label>  </label>
                           
                        </div>
					</div>
					
				</div>
                <br/>
				<div class="row" style="overflow-x: auto;">
					  <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="100%">
                            
                        </rsweb:ReportViewer>
				</div>
				
			</div>
		</div>
    

      </ContentTemplate>
    </asp:UpdatePanel>
           
</asp:Content>
