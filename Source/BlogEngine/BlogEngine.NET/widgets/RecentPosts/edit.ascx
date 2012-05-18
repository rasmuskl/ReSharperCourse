<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edit.ascx.cs" Inherits="Widgets.RecentPosts.Edit" %>

<style type="text/css">
  #body label {display: block; float:left; width:150px}
  #body input {display: block; float:left; }
</style>

<div id="body">

<label for="<%=txtNumberOfPosts.ClientID %>">Number of posts</label>
<asp:TextBox runat="server" ID="txtNumberOfPosts" Width="30" />
<asp:CompareValidator runat="Server" ControlToValidate="txtNumberOfPosts" Type="Integer" Operator="DataTypeCheck" ErrorMessage="Please enter a valid number" Display="Dynamic" />
<asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumberOfPosts" ErrorMessage="Please enter a valid number" Display="dynamic" /><br /><br />

<label for="<%=cbShowComments.ClientID %>">Show comments</label>
<asp:CheckBox runat="Server" ID="cbShowComments" />
<br /><br />

<label for="<%=cbShowRating.ClientID %>">Show rating</label>
<asp:CheckBox runat="Server" ID="cbShowRating" />

</div>