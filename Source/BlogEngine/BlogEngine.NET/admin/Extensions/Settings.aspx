<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="Admin.Extensions.Settings" %>
<%@ Reference Control = "Settings.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server">
    <div class="content-box-outer">
		<div class="content-box-right">
            <ul>
                <% foreach (var x in ExtensionList())
                   {
                       var cls = "";
                       if (Request.RawUrl.IndexOf(x.Name, StringComparison.OrdinalIgnoreCase) != -1)
                           cls = "content-box-selected";

                       if (x.Settings.Count > 0 && x.Settings[0] != null && x.ShowSettings != false)
                           Response.Write(string.Format("<li class=\"{1}\"><a href=\"Settings.aspx?ext={0}&enb={2}\">{0}</a></li>", x.Name, cls, x.Enabled));
                   }%>
            </ul>
		</div>
		<div class="content-box-left">
            <%=EnabledLink%>
            <asp:PlaceHolder ID="ucPlaceHolder" runat="server"></asp:PlaceHolder>
            <asp:HiddenField ID="args" runat="server" />
		</div>
	</div>
</asp:Content>
