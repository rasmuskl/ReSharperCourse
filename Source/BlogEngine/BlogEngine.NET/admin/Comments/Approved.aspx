<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Approved.aspx.cs" Inherits="Admin.Comments.Approved" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server"> 
    <script type="text/javascript">
        LoadComments(1);
    </script>
    
	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu" runat="server" />
		</div>
		<div class="content-box-left">
            <h1><%=Resources.labels.approvedComments %><span class="Pager"></span></h1>
            <div id="Container"></div>
            <div id="pager-lower" class="Pager"></div>
		</div>
	</div>      
</asp:Content>