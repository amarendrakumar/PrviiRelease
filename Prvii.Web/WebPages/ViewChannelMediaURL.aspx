<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="ViewChannelMediaURL.aspx.cs" Inherits="Prvii.Web.WebPages.ViewChannelMediaURL" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>View Media</h2>
    <table class="body_table_content" style="width: 100%">
        <tr>
            <td style="width: 30%"></td>
            <td style="width: 40%;text-align: center">
                <asp:Label ID="lblMessage" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>
                <asp:Image ID="imgMedia" runat="server" Height="500px" Visible="false" />
                <a class="player" runat="server" id="vdoMedia" visible="false" style="height: 500px; width: 500px; display: block"></a>
                <br />
                <br />
                <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn-primary" OnClick="btnBack_Click" Visible="false" />

                <script src="../Flowplayer/flowplayer-3.2.13.min.js"></script>
                <script type="text/javascript">
                    flowplayer("a.player", "../Flowplayer/flowplayer-3.2.18.swf", {
                        plugins: {
                            pseudo: { url: "../Flowplayer/flowplayer.pseudostreaming-3.2.13.swf" }
                        },
                        clip: { provider: 'pseudo', autoPlay: false },
                    });
                </script>
            </td>
            <td style="width: 30%"></td>
        </tr>
    </table>
</asp:Content>
