<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Pending.aspx.cs" Inherits="Admin.Comments.Pending" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server"> 

    <script type="text/javascript">
        LoadComments(1);
    </script>

	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu" runat="server" />
		</div>
		<div class="content-box-left">
            <h1><%=Resources.labels.pendingApproval %><span class="Pager"></span></h1>
            <div id="Container"></div>
            <div id="pager-lower" class="Pager"></div>
		</div>
	</div>      
</asp:Content>