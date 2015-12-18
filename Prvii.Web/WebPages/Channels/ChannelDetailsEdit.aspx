<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="ChannelDetailsEdit.aspx.cs" Inherits="Prvii.Web.WebPages.Channels.ChannelDetailsEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js"></script>
    <script>
        function PreviewImage(imgPreview, input) {
            var preview = document.getElementById(imgPreview);
            var file = input.files[0];
            var reader = new FileReader();

            reader.onloadend = function () {
                preview.src = reader.result;
            }
            if (file) {
                reader.readAsDataURL(file);
                preview.style.visibility = "visible";
            } else {
                preview.src = "";
                preview.style.visibility = "hidden";
            }
        }

        function isNumber(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <%--    <asp:UpdateProgress ID="upgPage" runat="server" AssociatedUpdatePanelID="upPage">
        <ProgressTemplate>
            <div class="progressBackground"></div>
            <div class="progressBox">
                <img alt="" runat="server" src="~/Images/indicator.gif" style="vertical-align: middle;" />
                <span>&nbsp;&nbsp;Please wait...</span>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>


    <div class="row">
        <div class="col-lg-12">
            <h1>Celebrity Details</h1>
            <hr />
            <div class="col-sm-6">
                <div class="form-group">
                    <label>First Name</label>
                    <asp:TextBox ID="tbFirstName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbFirstName" CssClass="requiredField"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label>Last Name</label>
                    <asp:TextBox ID="tbLastName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Email</label>
                    <asp:TextBox ID="tbEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbEmail" CssClass="requiredField"></asp:RequiredFieldValidator>

                </div>
                <div class="form-group">
                    <label>Mobile</label>
                    <asp:TextBox ID="tbMobile" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Is Active</label>
                    <asp:CheckBox ID="cbxIsActive" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="col-sm-6">
                <div class="form-group">

                    <label>Price($)</label>
                    <asp:TextBox ID="tbPrice" runat="server" CssClass="form-control" MaxLength="50" Width="70"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="tbPrice" CssClass="requiredField"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Only two decimal places allowed!" ValidationExpression="((\d{0,9})((\.\d{1,2})?))$" Display="Dynamic" ControlToValidate="tbPrice" CssClass="requiredField"></asp:RegularExpressionValidator>

                    <asp:UpdatePanel runat="server" ID="upPage111" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="right">
                                <asp:HiddenField ID="hidForModel" runat="server" />
                                <%-- <asp:LinkButton ID="linkbtnPrice" runat="server" Text="Price Management" ></asp:LinkButton>--%>
                                <asp:LinkButton ID="LinkButton1" runat="server" Text="Price Management" OnClick="LinkButton1_Click"></asp:LinkButton>
                            </div>
                            <style type="text/css">
                                .modalBackground {
                                    background-color: Black;
                                    filter: alpha(opacity=90);
                                    opacity: 0.8;
                                }

                                .modalPopup {
                                    background-color: #FFFFFF;
                                    border-width: 3px;
                                    border-style: solid;
                                    border-color: black;
                                    padding-top: 10px;
                                    padding-left: 10px;
                                    padding-right: 10px;
                                    width: 650px;
                                    min-height: 150px;
                                }
                            </style>

                            <!-- ModalPopupExtender -->
                            <ajaxToolkit:ModalPopupExtender ID="ModalPopup1" runat="server" PopupControlID="Panel1" TargetControlID="hidForModel" CancelControlID="btnCancelPupop"
                                BackgroundCssClass="modalBackground">
                            </ajaxToolkit:ModalPopupExtender>
                            <asp:Panel ID="Panel1" runat="server" CssClass="modalPopup" align="center" Style="display: none">
                                <%-- <div style="float: right; margin: -30px -30px 0 0;">
                                    <img id="imgClose" src="data:image/png;base64,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" />
                                </div>--%>
                                <div class="row">
                                    <div class="col-lg-12">
                                       <div class="row">  <h2>
                                            <asp:Literal ID="litCelebrityName" runat="server" Text="Price Management"></asp:Literal>

                                        </h2>
                                        </div> <hr />
                                        <asp:Label ID="lblErrormessage" runat="server" ForeColor="Red"></asp:Label>
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
                                                                        <asp:TextBox ID="txtPrecentage" runat="server" Width="100px" Text='<%# Eval("Distribution") %>' CssClass="form-control" MaxLength="5" onkeypress="return isNumber(event)" />
                                                                    </td>
                                                                    <td><b>Sequence</b> </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSequence" runat="server" Width="100px" Text='<%# Eval("Sequence") %>' CssClass="form-control" MaxLength="2" onkeypress="return isNumber(event)" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="4" style="padding-left: 10px; padding-bottom: 2px;">
                                                                        <asp:GridView ID="gvPrviiAccountSubMaster" ShowHeader="false" runat="server" AutoGenerateColumns="false" CssClass="body_table_content" >
                                                                            <Columns>
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <table style="width: 430px; border: 0.5px solid #808080; line-height: 45px;" class="body_table_content">
                                                                                            <tr>
                                                                                                <td >
                                                                                                    <b>
                                                                                                        <asp:HiddenField ID="hidAccountIDSub" runat="server" Value='<%# Eval("ID") %>' />
                                                                                                        <asp:Label ID="lblAccountNameSub" runat="server" Text='<%# Eval("Account") %>'></asp:Label></b>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtPrecentage" runat="server" Width="100px" Text='<%# Eval("Distribution") %>' CssClass="form-control" MaxLength="5" onkeypress="return isNumber(event)" />
                                                                                                </td>
                                                                                                <td><b>Sequence</b> </td>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSequence" runat="server" Width="100px" Text='<%# Eval("Sequence") %>' CssClass="form-control" MaxLength="2" onkeypress="return isNumber(event)" />
                                                                                                </td>
                                                                                            </tr>

                                                                                        </table>

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

                                        <br>
                                        <br>
                                    </div>
                                </div>
                            </asp:Panel>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="form-group">
                    <label>Billing Cycle</label>
                    <asp:DropDownList ID="ddlBillingCycles" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Select" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="ddlBillingCycles" InitialValue="0" CssClass="requiredField"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label>Timezone</label>
                    <asp:DropDownList ID="ddlTimezones" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Required!" Display="Dynamic" ControlToValidate="ddlTimezones" InitialValue="0" CssClass="requiredField"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label>No. Of billing Period</label>
                    <asp:TextBox ID="tbNoOfBillingPeriod" runat="server" TextMode="Number" min="2" CssClass="form-control" MaxLength="2" Width="70"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Preclude</label>
                    <asp:CheckBox ID="chxPreclude" runat="server" CssClass="form-control" />
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="col-sm-6">

                <div class="form-group">
                    <label>Image</label>
                    <asp:FileUpload ID="fuImage" runat="server" onchange="PreviewImage('imgImagePreview',this);" />
                    <asp:Image ID="imgImagePreview" ClientIDMode="Static" runat="server" Height="60" />
                    <asp:LinkButton ID="lbtnImageDelete" runat="server" ForeColor="Red" Visible="false" OnClick="lbtnImageDelete_Click"
                        OnClientClick="return confirm('Are you sure you want to delete Image?');">Delete</asp:LinkButton>
                </div>
                <div class="form-group">
                    <label>Left Image</label>
                    <asp:FileUpload ID="fuLeftImage" runat="server" onchange="PreviewImage('imgLeftImagePreview',this);" />
                    <asp:Image ID="imgLeftImagePreview" ClientIDMode="Static" runat="server" Height="60" />
                    <asp:LinkButton ID="lbtnLeftImageDelete" runat="server" ForeColor="Red" Visible="false" OnClick="lbtnLeftImageDelete_Click"
                        OnClientClick="return confirm('Are you sure you want to delete Left Image?');">Delete</asp:LinkButton>
                </div>
                <div class="form-group">
                    <label>Right Image</label>
                    <asp:FileUpload ID="fuRightImage" runat="server" onchange="PreviewImage('imgRightImagePreview',this);" />
                    <asp:Image ID="imgRightImagePreview" ClientIDMode="Static" runat="server" Height="60" />
                    <asp:LinkButton ID="lbtnRightImageDelete" runat="server" ForeColor="Red" Visible="false" OnClick="lbtnRightImageDelete_Click"
                        OnClientClick="return confirm('Are you sure you want to delete Right Image?');">Delete</asp:LinkButton>
                </div>
                <div class="form-group">
                    <label>Welcome Media</label>
                    <asp:FileUpload ID="fuWelcomeMedia" AllowMultiple="true" runat="server" />
                    <asp:GridView ID="gvWelcomeMediaList" runat="server" CssClass="table-responsive table-striped" PagerStyle-HorizontalAlign="Right"
                        Width="80%" AutoGenerateColumns="false" AllowPaging="true" PageSize="20" GridLines="None" EmptyDataText="No Item Found" OnRowCommand="gvWelcomeMediaList_RowCommand">

                        <PagerStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:HyperLinkField HeaderText="Name" DataTextField="Name" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/WebPages/ViewChannelMediaURL.aspx?ID={0}"></asp:HyperLinkField>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtnDeleteItem" runat="server" Text="Delete" CommandName="DeleteItem" CommandArgument='<%# Eval("ID")%>'
                                        OnClientClick="return confirm('Are you sure you want to delete this Welcome Media?');" />
                                </ItemTemplate>

                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="col-sm-6">
                <div class="form-group">
                    <label>Center Image</label>
                    <asp:FileUpload ID="fuCenterImage" runat="server" onchange="PreviewImage('imgCenterImage',this);" />
                    <asp:Image ID="imgCenterImage" ClientIDMode="Static" runat="server" Height="60" />
                    <asp:LinkButton ID="lbtnCenterImageDelete" runat="server" ForeColor="Red" Visible="false" OnClick="lbtnCenterImageDelete_Click"
                        OnClientClick="return confirm('Are you sure you want to Center Image?');">Delete</asp:LinkButton>
                </div>
                <div class="form-group">
                    <label>Background Image</label>
                    <asp:FileUpload ID="fuBackgroundImage" runat="server" onchange="PreviewImage('imgBackgroundImage',this);" />
                    <asp:Image ID="imgBackgroundImage" ClientIDMode="Static" runat="server" Height="60" />
                    <asp:LinkButton ID="lbtnBackgroundImageDelete" runat="server" ForeColor="Red" Visible="false" OnClick="lbtnBackgroundImageDelete_Click"
                        OnClientClick="return confirm('Are you sure you want to delete Background Image?');">Delete</asp:LinkButton>
                </div>

                <div class="form-group">
                    <label>Media Urls</label>
                    <asp:FileUpload ID="fuMediaUrls" AllowMultiple="true" runat="server" />
                    <asp:GridView ID="gvMediaUrlList" runat="server" CssClass="table-responsive table-striped" PagerStyle-HorizontalAlign="Right"
                        Width="80%" AutoGenerateColumns="false" AllowPaging="true" PageSize="20" GridLines="None" EmptyDataText="No Item Found" OnRowCommand="gvMediaUrlList_RowCommand">

                        <PagerStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:HyperLinkField HeaderText="Name" DataTextField="Name" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/WebPages/ViewChannelMediaURL.aspx?ID={0}"></asp:HyperLinkField>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtnDeleteItem" runat="server" Text="Delete" CommandName="DeleteItem" CommandArgument='<%# Eval("ID")%>'
                                        OnClientClick="return confirm('Are you sure you want to delete this Media url?');" />
                                </ItemTemplate>

                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </div>
    </div>

    <br />
    <div class="row list-table-body">
        <div class="col-md-6"></div>
        <div class="col-md-6">
            <div class="col-sm-6">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-default solid-btn" OnClick="btnSave_Click" />

            </div>
            <div class="col-sm-6">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default" OnClick="btnCancel_Click" CausesValidation="false" />

            </div>
        </div>
        <br />
        <br />
        <br />
    </div>



</asp:Content>
