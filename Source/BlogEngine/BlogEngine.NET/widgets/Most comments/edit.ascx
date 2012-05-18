<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edit.ascx.cs" Inherits="Widgets.ModeComments.Edit" %>

<label for="<%=cbShowComments.ClientID %>">Show comments</label><br />
<asp:CheckBox runat="Server" ID="cbShowComments" />

<br /><br />

<label for="<%=txtSize.ClientID %>">Avatar size (in pixels)</label><br />
<asp:TextBox runat="Server" ID="txtSize" />
<asp:CompareValidator runat="Server" ControlToValidate="txtSize" Operator="dataTypeCheck" Type="integer" ErrorMessage="Please enter a valid number" />

<br /><br />

<label for="<%=txtNumber.ClientID %>">Number of commenters</label><br />
<asp:TextBox runat="Server" ID="txtNumber" />
<asp:CompareValidator runat="Server" ControlToValidate="txtNumber" Operator="dataTypeCheck" Type="integer" ErrorMessage="Please enter a valid number" />

<br /><br />

<label for="<%=txtDays.ClientID %>">Max age in days</label><br />
<asp:TextBox runat="Server" ID="txtDays" />
<asp:CompareValidator runat="Server" ControlToValidate="txtDays" Operator="dataTypeCheck" Type="integer" ErrorMessage="Please enter a valid number" />