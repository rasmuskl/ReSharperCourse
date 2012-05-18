<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Tags.aspx.cs" Inherits="Admin.Posts.Tags" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">
    <script type="text/javascript" src="../jquery.tipsy.js"></script>
    <script type="text/javascript">
        LoadTags(1);
        function LoadTags(page) {
            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/LoadTags",
                data: "{'page':'" + page + "'}",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: onAjaxBeforeSend,
                success: function (msg) {
                    $('#Container').setTemplateURL('../../Templates/tags.htm', null, { filter_data: false });
                    $('#Container').processTemplate(msg);
                    SaveOriginalIdValues('#Container tr', '.editable');
                }
            });
            return false;
        }
        function OnAdminDataSaved() {
            LoadTags(1);
        }
        $(document).ready(function () {
            $(".tableToolBox a").click(function () {
                $(".tableToolBox a").removeClass("current");
                $(this).addClass("current");
            });
        });
    </script>
	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu" runat="server" />
		</div>
		<div class="content-box-left">
            <h1><%=Resources.labels.tags %></h1>
            <div class="tableToolBox">
                <div class="Pager"></div>
            </div>
            <div id="Container"></div>
            <div class="Pager"></div>
        </div>
    </div>
</asp:Content>