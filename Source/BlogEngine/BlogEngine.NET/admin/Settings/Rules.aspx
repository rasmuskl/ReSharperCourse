<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Rules.aspx.cs" Inherits="admin.Settings.Rules" %>
<%@ Import Namespace="Resources"%>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server">
    <script type="text/javascript">

        function ResetCounters(filterName) {
            var dto = { "filterName": filterName };
            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/ResetCounters",
                data: JSON.stringify(dto),
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: onAjaxBeforeSend,
                success: function (result) {
                    var rt = result.d;
                    if (rt.Success) {
                        LoadCustomFilters();
                        ShowStatus("success", rt.Message);
                    }
                    else {
                        ShowStatus("warning", rt.Message);
                    }
                }
            });
            return false;
        }
    </script>

	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu1" runat="server" />
		</div>
		<div class="content-box-left">
            <div class="rightligned-top action_buttons">
                <asp:Button runat="server" class="btn primary" ID="btnSave" Text="Save settings" />
                <span class="loader">&nbsp;</span>
            </div>
            <h1 style="border:none;"><%=labels.spamProtection %></h1>
            <h2><%=labels.rules%></h2>
            <ul class="fl leftaligned">
                <li>
                    <label class="lbl" <%=ddWhiteListCount.ClientID %>><%=labels.addToWhiteList%></label>
                    <asp:DropDownList runat="server" ID="ddWhiteListCount">
                        <asp:ListItem Text="0" />
                        <asp:ListItem Text="1" />
                        <asp:ListItem Text="2" />
                        <asp:ListItem Text="3" />
                        <asp:ListItem Text="5" />
                    </asp:DropDownList> 
                    <span><%=labels.authorApproved%></span> 
                    <span class="insetHelp"><%=labels.toDisableSetTo0 %></span>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbAddIpToWhitelistFilterOnApproval" />
                    <label for="<%=cbAddIpToWhitelistFilterOnApproval.ClientID %>"><%=labels.whitelistIpOnCommentApproval%></label>
                </li>
                <li>
                    <label class="lbl" for="<%=ddBlackListCount.ClientID %>"><%=labels.commentsBlacklist%></label>
                    <asp:DropDownList runat="server" ID="ddBlackListCount">
                        <asp:ListItem Text="0" />
                        <asp:ListItem Text="1" />
                        <asp:ListItem Text="2" />
                        <asp:ListItem Text="3" />
                        <asp:ListItem Text="5" />
                    </asp:DropDownList> 
                    <span><%=labels.authorRejected%></span>
                    <span class="insetHelp"><%=labels.toDisableSetTo0 %></span>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbTrustAuthenticated" />
                    <label for="<%=cbTrustAuthenticated.ClientID %>"><%=labels.trustAuthenticated%></label>
                    <span class="insetHelp"><%=labels.alwaysTrust%></span>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbBlockOnDelete" />
                    <label for="<%=cbBlockOnDelete.ClientID %>"><%=labels.commentsBlockOnDelete%></label>
                    <span class="insetHelp"><%=labels.authorBlocked%></span>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbAddIpToBlacklistFilterOnRejection" />
                    <label for="<%=cbAddIpToBlacklistFilterOnRejection.ClientID %>"><%=labels.blacklistIpOnCommentRejection%></label>
                </li>
            </ul>

            <h2><%=labels.filters%></h2>
            <ul class="fl">
                <li>
                    <asp:DropDownList ID="ddAction" runat="server" CssClass="txt">
                        <asp:ListItem Text="<%$ Resources:labels, block %>" Value="Block" Selected=true></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, allow %>" Value="Allow" Selected=false></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, delete %>" Value="Delete" Selected=false></asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddSubject" runat="server" CssClass="txt">
                        <asp:ListItem Text="<%$ Resources:labels, ip %>" Value="IP" Selected=true></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, author %>" Value="Author" Selected=false></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, website %>" Value="Website" Selected=false></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, email %>" Value="Email" Selected=false></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, comment %>" Value="Comment" Selected=false></asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddOperator" runat="server" CssClass="txt">
                        <asp:ListItem Text="<%$ Resources:labels, eqls %>" Value="Equals" Selected=true></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, contains %>" Value="Contains" Selected=false></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="txtFilter" runat="server" CssClass="txt" MaxLength="250" Width="300px"></asp:TextBox>
                    <asp:Button ID="btnAddFilter" class="btn" runat="server" Text="<%$ Resources:labels, addFilter %>" OnClick="btnAddFilter_Click"/>
                    <span runat="Server" ID="FilterValidation" style="color:Red"></span>
                </li>
            </ul>

            <div style="border:1px solid #f3f3f3; margin:0 0 20px;">
                <asp:GridView ID="gridFilters" 
                        PageSize="10" 
                        BorderColor="#f8f8f8" 
                        BorderStyle="solid" 
                        BorderWidth="1px"
                        cellpadding="2"
                        runat="server"  
                        width="100%"
                        gridlines="None"
                        AlternatingRowStyle-BackColor="#f8f8f8" 
                        HeaderStyle-BackColor="#f3f3f3"
                        AutoGenerateColumns="False"
                        AllowPaging="True"
                        datakeynames="ID"
                        OnPageIndexChanging="gridView_PageIndexChanging"
                        AllowSorting="True">
                        <Columns>
                        <asp:BoundField DataField = "ID" Visible="false" />
                        <asp:TemplateField HeaderText="<%$ Resources:labels, action %>" HeaderStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <%# Eval("Action") %> 
                            </ItemTemplate>
                        </asp:TemplateField>
                
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <%=labels.commentsWhere %>
                            </ItemTemplate>
                        </asp:TemplateField>
                
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <%# Eval("Subject") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <%# Eval("Operator") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                
                        <asp:TemplateField HeaderText="<%$ Resources:labels, filter %>" HeaderStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                    <%# Eval("Filter") %> 
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField ShowHeader="False" ItemStyle-VerticalAlign="middle" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnDelete" runat="server" ImageAlign="middle" CausesValidation="false" ImageUrl="~/admin/images/action-delete.png" OnClick="btnDelete_Click" CommandName="btnDelete" AlternateText="<%$ Resources:labels, delete%>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        </Columns>
                        <pagersettings Mode="NumericFirstLast" position="Bottom" pagebuttoncount="20" />
                        <PagerStyle HorizontalAlign="Center"/>
                </asp:GridView>
            </div>
            <h2><%=labels.antiSpamServices %></h2>

            <div id="Container"></div>
        
            <div style="margin:0 0 20px;">
                <asp:CheckBox runat="server" ID="cbReportMistakes" />
                <span><%=labels.reportMistakesToService %></span>
            </div>

            <div style="clear:both; display: inline-block; height: 30px; width: 100%;">&nbsp;</div>

            <div class="rightligned-bottom action_buttons">
                <asp:Button runat="server" class="btn primary" ID="btnSave2" Text="Save settings" />
                <span class="loader">&nbsp;</span>
            </div>
            
		</div>
	</div> 

    <script type="text/javascript">
        LoadCustomFilters();
    </script>
</asp:Content>
