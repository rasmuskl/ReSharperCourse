<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true"
    CodeFile="Blogroll.aspx.cs" Inherits="admin.Widgets.Blogroll" Title="Blogroll" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">
    <script type="text/javascript" src="../jquery.colorbox.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".showSettings").colorbox({ width: "600px", inline: true, href: "#blogrollSettings" });
            $(".addNew").colorbox({ width: "620px", inline: true, href: "#addBlogroll" });
        });

        function closeOverlay() {
            $.colorbox.close();
        }
    </script>
	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu" runat="server" />
		</div>
		<div class="content-box-left">
            <h1><%=Resources.labels.blogroll %>
            <a href="#" class="showSettings"><%=Resources.labels.settings %></a>
            <a href="#" class="addNew"><%=Resources.labels.addNewBlog %></a></h1>

            <div style="display:none;">
            <div id="blogrollSettings" class="overlaypanel">
                <h2><%=Resources.labels.settings %></h2>
                <ul class="fl" style="overflow:hidden;">
                    <li style="float:left; margin:0 20px 0 0;">
                        <asp:Label runat="server" AssociatedControlID="ddlVisiblePosts" CssClass="lbl" Text='<%$ Code: Resources.labels.numberOfDisplayedItems %>' />
                        <asp:DropDownList runat="server" ID="ddlVisiblePosts">
                            <asp:ListItem Text="0" />
                            <asp:ListItem Text="1" />
                            <asp:ListItem Text="2" />
                            <asp:ListItem Text="3" />
                            <asp:ListItem Text="4" />
                            <asp:ListItem Text="5" />
                            <asp:ListItem Text="6" />
                            <asp:ListItem Text="7" />
                            <asp:ListItem Text="8" />
                            <asp:ListItem Text="9" />
                            <asp:ListItem Text="10" />
                        </asp:DropDownList>
                    </li>
                    <li style="float:left; margin:0 20px 0 0;">
                        <asp:Label runat="server" AssociatedControlID="txtMaxLength" CssClass="lbl" Text='<%$ Code: Resources.labels.maxLengthOfItems %>' />
                        <asp:TextBox runat="server" ID="txtMaxLength" MaxLength="3" Width="50" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMaxLength" Display="Dynamic" ValidationGroup="settings" ErrorMessage="<%$Resources:labels,required %>"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtMaxLength" Display="Dynamic"
                            Operator="dataTypeCheck" Type="integer" ValidationGroup="settings" ErrorMessage="<%$Resources:labels,noValidNumber %>" />
                    </li>
                    <li style="float:left; margin:0 20px 0 0;">
                        <asp:Label runat="server" AssociatedControlID="txtUpdateFrequency" CssClass="lbl"
                            Text='<%$ Code: Resources.labels.updateFrequenzy %>' />
                        <asp:TextBox runat="server" ID="txtUpdateFrequency" MaxLength="3" Width="50" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUpdateFrequency" Display="Dynamic" ValidationGroup="settings" ErrorMessage="<%$Resources:labels,required %>"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtUpdateFrequency"  Display="Dynamic"
                            Operator="dataTypeCheck" Type="integer" ValidationGroup="settings" ErrorMessage="<%$Resources:labels,noValidNumber %>" />
                    </li>
                </ul>
                <asp:Button runat="server" ID="btnSaveSettings" ValidationGroup="settings" CssClass="btn primary" Text="Save settings" OnClientClick="colorboxDialogSubmitClicked('settings', 'blogrollSettings');" /> 
                <%=Resources.labels.or %> <a href="#" onclick="closeOverlay();"><%=Resources.labels.cancel %></a>
            </div>
            </div>

            <div style="display:none;">
                <div id="addBlogroll" class="overlaypanel">
                    <h2><%=Resources.labels.addNewBlog %></h2>
                    <ul class="fl">
                        <li>
                            <asp:Label runat="server" AssociatedControlID="txtTitle" CssClass="lbl" Text='<%$ Code: Resources.labels.title %>' />
                            <asp:TextBox runat="server" ID="txtTitle" Width="500px" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="Server" ControlToValidate="txtTitle"
                                ErrorMessage="<%$Resources:labels,required %>" ValidationGroup="addNew" Display="Dynamic" />
                        </li>
                        <li>
                            <asp:Label runat="server" AssociatedControlID="txtDescription" CssClass="lbl" Text='<%$ Code: Resources.labels.description %>' />
                            <asp:TextBox runat="server" ID="txtDescription" Width="500px" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="Server" ControlToValidate="txtDescription"
                                ErrorMessage="<%$Resources:labels,required %>" ValidationGroup="addNew" Display="Dynamic" />
                        </li>
                        <li>
                            <asp:Label runat="server" AssociatedControlID="txtWebUrl" CssClass="lbl" Text='<%$ Code: Resources.labels.website %>' />
                            <asp:TextBox runat="server" ID="txtWebUrl" Width="500px" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="Server" ControlToValidate="txtWebUrl"
                                ErrorMessage="<%$Resources:labels,required %>" Display="Dynamic" ValidationGroup="addNew" />
                            <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="txtWebUrl"  Display="Dynamic"
                                ErrorMessage="<%$Resources:labels,invalid %>" EnableClientScript="false" OnServerValidate="validateWebUrl"
                                ValidationGroup="addnew"></asp:CustomValidator>
                        </li>
                        <li>
                            <asp:Label runat="server" AssociatedControlID="txtFeedUrl" CssClass="lbl" Text="RSS url" />
                            <asp:TextBox runat="server" ID="txtFeedUrl" Width="500px" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="Server" ControlToValidate="txtFeedUrl"
                                ErrorMessage="<%$Resources:labels,required %>" Display="Dynamic" ValidationGroup="addNew" />
                            <asp:CustomValidator ID="CustomValidator2" runat="server" ControlToValidate="txtFeedUrl"
                                ErrorMessage="<%$Resources:labels,invalid %>" Display="Dynamic" EnableClientScript="false" OnServerValidate="validateFeedUrl"
                                ValidationGroup="addnew"></asp:CustomValidator>
                        </li>
                        <li>
                            <asp:Label runat="server" AssociatedControlID="cblXfn" CssClass="lbl" Text="XFN tag" />
                            <asp:CheckBoxList runat="server" ID="cblXfn" CssClass="nowidth" RepeatLayout="Table" RepeatDirection="Horizontal" RepeatColumns="6" >
                                <asp:ListItem Text="contact" />
                                <asp:ListItem Text="acquaintance " />
                                <asp:ListItem Text="friend " />
                                <asp:ListItem Text="met" />
                                <asp:ListItem Text="co-worker" />
                                <asp:ListItem Text="colleague " />
                                <asp:ListItem Text="co-resident" />
                                <asp:ListItem Text="neighbor " />
                                <asp:ListItem Text="child" />
                                <asp:ListItem Text="parent" />
                                <asp:ListItem Text="sibling" />
                                <asp:ListItem Text="spouse" />
                                <asp:ListItem Text="kin" />
                                <asp:ListItem Text="muse" />
                                <asp:ListItem Text="crush" />
                                <asp:ListItem Text="date" />
                                <asp:ListItem Text="sweetheart" />
                                <asp:ListItem Text="me" />
                            </asp:CheckBoxList>
                        </li>
                    </ul>
                    <asp:Button runat="server" ID="btnSave" ValidationGroup="addNew" CssClass="btn primary" OnClientClick="colorboxDialogSubmitClicked('addNew', 'addBlogroll');" Text="Add blog" /> 
                    <%=Resources.labels.or %> <a href="#" onclick="closeOverlay();"><%=Resources.labels.cancel %></a>

                </div>
            </div>

            <asp:GridView runat="server" ID="grid" CssClass="beTable" BorderStyle="solid"
                RowStyle-BorderWidth="0" RowStyle-BorderStyle="None" GridLines="None"
                Width="100%" AlternatingRowStyle-BackColor="#f8f8f8" AlternatingRowStyle-BorderColor="#f8f8f8"
                HeaderStyle-BackColor="#F1F1F1" CellPadding="3" AutoGenerateColumns="False" OnRowDeleting="grid_RowDeleting"
                OnRowCommand="grid_RowCommand">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink ID="feedLink" runat="server" ImageUrl="~/pics/rssButton.png" NavigateUrl='<%# Eval("FeedUrl").ToString() %>'
                                Text="<%# string.Empty %>"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("BlogUrl").ToString() %>'
                                Text='<%# Eval("Title") %>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Literal ID="Literal1" runat="server" Text='<%# Eval("Description") %>'></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowDeleteButton="True" DeleteText="<%$Resources:labels, delete %>" ControlStyle-CssClass="deleteAction" />
                    <asp:TemplateField ControlStyle-BackColor="Transparent">
                        <ItemTemplate>
                            <asp:ImageButton ID="ibMoveUp" ImageUrl="~/admin/images/action-up.png" runat="server"
                                CommandArgument="<%# ((GridViewRow)Container).RowIndex %>" CommandName="moveUp" />
                            <asp:ImageButton ID="ibMoveDown" ImageUrl="~/admin/images/action-down.png" runat="server"
                                CommandArgument="<%# ((GridViewRow)Container).RowIndex %>" CommandName="moveDown" />
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
