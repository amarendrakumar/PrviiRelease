<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="ShoppingCartList.aspx.cs" Inherits="Prvii.Web.WebPages.ShoppingCarts.ShoppingCartList" %>

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
                <div class="col-lg-12 list-table">
                    <h1>
                        <asp:Label ID="lblPageHeader" runat="server" Text="Shopping Cart"></asp:Label></h1>
                    <br />
                    <br />
                    <asp:DataList ID="DataList1" runat="server" Width="98%">
                        <HeaderTemplate>
                            <div class="row list-table-head">
                                <div class="col-md-3">Photo</div>
                                <div class="col-md-4">Celebrity</div>
                                <div class="col-md-2">Price</div>
                                <div class="col-md-1">Total</div>
                                <div class="col-md-2">Remove Item</div>
                            </div>
                            <div id="itemPlaceholder" runat="server"></div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="row list-table-body">
                                <div class="col-md-3">
                                    <div class="celebrity-img">
                                        <asp:Image runat="server" ImageUrl='<%# "~/WebPages/GetChannelMedia.aspx?ChannelID="+ DataBinder.Eval(Container.DataItem,"ChannelID")%>' />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="celebrity-name">
                                        <asp:HyperLink ID="hypCelebrityName" runat="server" NavigateUrl='<%# "~/WebPages/Channels/ChannelDetailsView.aspx?ID="+ DataBinder.Eval(Container.DataItem,"ChannelID")%>'>
                                 <%# DataBinder.Eval(Container.DataItem,"ChannelName")%></asp:HyperLink>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="celebrity-price">
                                        <span class="priceText">Price</span><span class="price"> <%# DataBinder.Eval(Container.DataItem,"Price","{0:c}")%></span>
                                    </div>
                                </div>
                                <div class="col-md-1">
                                    <div class="celebrity-price">
                                        <span class="priceText">Total</span><span class="price"> <%# DataBinder.Eval(Container.DataItem,"Price","{0:c}")%></span>
                                    </div>
                                </div>

                                <div class="col-md-2">
                                    <div class="celebrity-name">
                                        <span class="activeText">Remove Item: </span>
                                        <asp:HiddenField ID="hfItemID" runat="server" Value='<%#Bind("ID") %>' />
                                        <asp:CheckBox ID="cbxChannel" runat="server" />
                                    </div>
                                </div>

                            </div>
                        </ItemTemplate>
                    </asp:DataList>

                    <div class="row list-table-body">
                        <div class="col-md-7"></div>
                        <div class="col-md-5 text-center">
                            Total Cart: <strong>
                                <asp:Label ID="lblTotalPrice" runat="server" Text=""></asp:Label></strong>
                        </div>
                    </div>

                    <div class="row list-table-body" runat="server" id="tblCheckoutStart">
                        <div class="col-sm-6">
                            <div class="col-sm-5">
                                <asp:Button ID="btnCheckout" runat="server" Text="Checkout" CssClass="btn btn-default solid-btn" OnClick="btnCheckout_Click" />

                            </div>
                            <div class="col-sm-7">
                                <asp:Button ID="btnContinueShopping" runat="server" Text="Continue Shopping" CssClass="btn btn-default" PostBackUrl="~/WebPages/Channels/ChannelList.aspx?val=2" />

                            </div>
                        </div>
                        <div class="col-sm-4"></div>
                        <div class="col-sm-2">
                            <div class="col-sm-12">
                                <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-default solid-btn" OnClick="btnUpdate_Click" />

                            </div>
                        </div>
                    </div>
                    <div runat="server" id="tblReviewOrder" visible="false">


                        <div class="row">
                            <h3>Payment</h3>
                        </div>

                        <div class="row list-table-body">
                            <div class="col-md-12">
                                <input type="radio" />
                                <asp:Image ID="imgPaypal" runat="server" ImageUrl="~/images/paypal.png" />
                                Paypal - pay securely without sharing your financial information<br />
                                <br />
                                &nbsp; &nbsp; &nbsp; &nbsp; <asp:Image ID="imgpayment" runat="server" ImageUrl="~/images/payment.png" />
                                <br />
                                <br />
                            </div>
                        </div>

                        <div class="row list-table-body">
                            <div class="col-md-6"></div>
                            <div class="col-md-6">
                                <div class="col-sm-7">
                                    <asp:Button ID="btnCompleteOrder" runat="server" Text="Complete Order" CssClass="btn btn-default solid-btn" OnClick="btnCompleteOrder_Click" />

                                </div>
                                <div class="col-sm-5">
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default" OnClick="btnCancel_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>


        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
