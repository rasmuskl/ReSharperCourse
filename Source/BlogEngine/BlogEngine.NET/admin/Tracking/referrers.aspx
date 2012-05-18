<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true"
    CodeFile="referrers.aspx.cs" Inherits="Admin.Tracking.Referrers" Title="Referrers" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">
    <script type="text/javascript" src="../jquery.colorbox.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".showSettings").colorbox({ width: "400px", inline: true, href: "#settings" });
        });

        function closeOverlay() {
            $.colorbox.close();
        }

        function SaveSettings() {

            var dto = {
                "enable": $("[id$='_cbEnableReferrers']").attr('checked'),
                "days": $("[id$='_txtNumberOfDays']").val()
            };

            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "admin/Tracking/referrers.aspx/Save",
                data: JSON.stringify(dto),
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: onAjaxBeforeSend,
                success: function (result) {
                    var rt = result.d;
                    if (rt.Success) {
                        LoadRoles();
                        ShowStatus("success", rt.Message);
                    }
                    else {
                        ShowStatus("warning", rt.Message);
                    }
                }
            });

            closeOverlay();
            return false;
        }
    </script>

	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu" runat="server" />
		</div>
        <div class="content-box-left">
            <h1><%=Resources.labels.referrers %><a href="#" class="showSettings"><%=Resources.labels.settings %></a></h1>

            <div style="display:none;">
            <div id="settings" class="overlaypanel">
                <h2><%=Resources.labels.settings %></h2>
                <ul class="fl" style="margin:0;">
                    <li>
                        <asp:CheckBox runat="Server" ID="cbEnableReferrers" />
                        <label for=""><%=Resources.labels.enableReferrerTracking %></label>
                    </li>
                    <li>
                        <label class="lbl" for="<%= txtNumberOfDays.ClientID %>"><%= Resources.labels.numberOfDaysToKeep %></label>
                        <asp:TextBox ID="txtNumberOfDays" runat="server"></asp:TextBox><br />
                    </li>
                    <li>
                        <input type="submit" class="btn primary rounded" value="<%=Resources.labels.saveSettings %>" onclick="return SaveSettings();" />
                         <%=Resources.labels.or %> <a href="#" onclick="closeOverlay();"><%=Resources.labels.cancel %></a>
                    </li>
                </ul>
            </div>
            </div>

            <div ID="infoPanel" class="info" runat="server"></div>

            <asp:Panel ID="referrersPanel" runat="server">
                <div style="margin-bottom: 10px">
                    <asp:DropDownList runat="server" ID="ddlDays" AutoPostBack="true" Style="text-transform: capitalize"
                        DataTextFormatString="{0:d}">
                    </asp:DropDownList>
                    <label class="lbl" style="width:200px; display:inline" for="<%=ddlDays.ClientID %>"><%=Resources.labels.selectDay %></label>
                </div>
                <h2><%=Resources.labels.referrers%></h2>
                <asp:GridView runat="server" ID="grid" BorderColor="#f8f8f8" BorderStyle="solid"
                BorderWidth="1px" RowStyle-BorderWidth="0" RowStyle-BorderStyle="None" GridLines="None"
                Width="100%" AlternatingRowStyle-BackColor="#f8f8f8" AlternatingRowStyle-BorderColor="#f8f8f8"
                HeaderStyle-BackColor="#F1F1F1" CellPadding="3" ShowFooter="true" AutoGenerateColumns="False"
                EnableViewState="false">
                <Columns>
                    <asp:HyperLinkField HeaderText="<%$ Resources:labels, referrer %>" FooterStyle-HorizontalAlign="left"
                        DataNavigateUrlFields="url" Target="_blank" DataTextField="shortUrl" HeaderStyle-HorizontalAlign="left" />
                    <asp:HyperLinkField HeaderText="<%$ Resources:labels, link %>" FooterStyle-HorizontalAlign="left"
                        DataNavigateUrlFields="target" Target="_blank" DataTextField="shortTarget" HeaderStyle-HorizontalAlign="left" />
                    <asp:BoundField HeaderText="Hits" DataField="hits" HeaderStyle-HorizontalAlign="center"
                        ItemStyle-HorizontalAlign="center" ItemStyle-Width="40" />
                </Columns>
                <FooterStyle Font-Bold="true" HorizontalAlign="center" />
            </asp:GridView>
            </asp:Panel>
            <br />
            <asp:Panel ID="possibleSmapPanel" runat="server">
                <h2><%=Resources.labels.possibleSpam%></h2>
                <asp:GridView runat="server" ID="spamGrid" BorderColor="#f8f8f8" BorderStyle="solid"
                    BorderWidth="1px" RowStyle-BorderWidth="0" RowStyle-BorderStyle="None" GridLines="None"
                    Width="100%" AlternatingRowStyle-BackColor="#f8f8f8" AlternatingRowStyle-BorderColor="#f8f8f8"
                    HeaderStyle-BackColor="#F1F1F1" CellPadding="3" ShowFooter="true" AutoGenerateColumns="False"
                    EnableViewState="false">
                    <Columns>
                        <asp:HyperLinkField HeaderText="<%$ Resources:labels, referrer %>" FooterStyle-HorizontalAlign="left"
                            DataNavigateUrlFields="url" Target="_blank" DataTextField="shortUrl" HeaderStyle-HorizontalAlign="left" />
                        <asp:BoundField HeaderText="Hits" DataField="hits" HeaderStyle-HorizontalAlign="center"
                            ItemStyle-HorizontalAlign="center" ItemStyle-Width="40" />
                    </Columns>
                    <FooterStyle Font-Bold="true" HorizontalAlign="center" />
                </asp:GridView>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
