<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edit.ascx.cs" Inherits="Widgets.Twitter.Edit" %>
<%@ Reference Control="~/widgets/Twitter/widget.ascx" %>

<label for="<%=txtAccountUrl %>">Your Twitter account URL</label><br />
<asp:TextBox runat="server" ID="txtAccountUrl" Width="300" />
<asp:RequiredFieldValidator runat="Server" ControlToValidate="txtAccountUrl" ErrorMessage="Please enter a URL" Display="dynamic" /><br /><br />

<label for="<%=txtUrl %>">Twitter RSS feed URL</label><br />
<asp:TextBox runat="server" ID="txtUrl" Width="300" />
<asp:RequiredFieldValidator runat="Server" ControlToValidate="txtUrl" ErrorMessage="Please enter a URL" Display="dynamic" /><br /><br />

<label for="<%=txtTwits %>">Number of displayed Twits</label><br />
<asp:TextBox runat="server" ID="txtTwits" Width="30" />
<asp:RequiredFieldValidator runat="Server" ControlToValidate="txtTwits" ErrorMessage="Please enter a number" Display="dynamic" />
<asp:CompareValidator runat="server" ControlToValidate="txtTwits" Type="Integer" Operator="dataTypeCheck" ErrorMessage="A real number please" /><br /><br />

<label for="<%=txtPolling %>">Polling Interval (#/minutes)</label><br />
<asp:TextBox ID="txtPolling" runat="server" Width="30" />
<asp:CompareValidator runat="server" ControlToValidate="txtPolling" Type="Integer" Operator="dataTypeCheck" ErrorMessage="A real number please" /><br /><br />

<label for="<%=txtFollowMe %>">Follow me text</label><br />
<asp:TextBox runat="server" ID="txtFollowMe" Width="300" />
<asp:RequiredFieldValidator runat="Server" ControlToValidate="txtFollowMe" ErrorMessage="Please enter a valid string" Display="dynamic" /><br /><br />
