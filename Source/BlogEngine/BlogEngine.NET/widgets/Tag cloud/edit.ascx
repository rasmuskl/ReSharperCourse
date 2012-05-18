<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edit.ascx.cs" Inherits="Widgets.TagCloud.Edit" %>
<%@ Reference VirtualPath="~/widgets/Tag cloud/widget.ascx" %>

<label for="<%=ddlMinimumPosts.ClientID %>">Minimum posts in each tag</label><br />
<asp:DropDownList runat="server" ID="ddlMinimumPosts">
  <asp:ListItem Value="1" Text="1 (default)" />
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
<br /><br />
<label for="<%=ddlCloudSize.ClientID %>">Tag cloud maximum size (Sorted by Recent Posts)</label><br />
<asp:DropDownList runat="server" ID="ddlCloudSize">
  <asp:ListItem Value="-1" Text="Unlimited (default)" />
  <asp:ListItem Text="10" />
  <asp:ListItem Text="25" />
  <asp:ListItem Text="50" />
  <asp:ListItem Text="75" />
  <asp:ListItem Text="100" />
  <asp:ListItem Text="125" />
  <asp:ListItem Text="150" />
</asp:DropDownList>