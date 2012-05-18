<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Import.aspx.cs" Inherits="admin.Settings.Import" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server">    
	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu" runat="server" />
		</div>
		<div class="content-box-left">
            <h1><%=Resources.labels.import %> &amp; <%=Resources.labels.export %></h1>
            <div style="padding: 10px 0">
                <label><%=Resources.labels.blogMLDescription %></label>
            </div>

            <h2><%=Resources.labels.import %></h2>
            <ul class="fl leftaligned">
                <li>
                    <label class="lbl"><%=Resources.labels.selectSavedBlogMlFle %></label>
                    <asp:FileUpload runat="server" ID="txtUploadFile" Width="300" style="margin:0 0 5px;" /><br />
                    <asp:Button ID="btnBlogMLImport" runat="server" CssClass="btn rounded" Text="<%$Resources:labels,importFromFile %>" OnClick="BtnBlogMlImportClick" OnClientClick="$('#ImportLoader').addClass('loader')" />
                    <span id="ImportLoader">&nbsp;</span>
                </li>
                <li><strong><%=Resources.labels.or %></strong></li>
                <li>
                    <label class="lbl"><%=Resources.labels.runClickOnceApplicationToImport %></label>
                    <input type="button" class="btn rounded" value="<%=Resources.labels.importWithClickOnce %>" onclick="location.href='http://dotnetblogengine.net/clickonce/blogimporter/blog.importer.application?url=<%=Utils.AbsoluteWebRoot %>&username=<%=Page.User.Identity.Name %>'" />
                </li>
            </ul>
            
            <h2><%=Resources.labels.export %></h2>
            <ul class="fl leftaligned">
                <li>
                    <label class="lbl"><%=Resources.labels.exportIntoBlogML %></label>
                    <input type="button" class="btn rounded" value="<%=Resources.labels.export %>" onclick="location.href='blogml.axd'" />
                </li>
            </ul>

        </div>
    </div>
</asp:Content>
