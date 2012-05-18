<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Advanced.aspx.cs" Inherits="admin.Settings.Advanced" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server"> 
    <script type="text/javascript">
        $(document).ready(function () {
            var frm = document.forms.aspnetForm;
            $(frm).validate({
                onsubmit: false
            });

            $("#btnSave").click(function (evt) {
                if ($(frm).valid())
                    SaveSettings();

                evt.preventDefault();
            });
            $("#btnSave2").click(function (evt) {
                if ($(frm).valid())
                    SaveSettings();

                evt.preventDefault();
            });
        });
        function SaveSettings() {
            $('.loader').show();
			
            var dto = { 
				"enableCompression": $("[id$='_cbEnableCompression']").attr('checked'),
				"removeWhitespaceInStyleSheets": $("[id$='_cbRemoveWhitespaceInStyleSheets']").attr('checked'),
				"compressWebResource": $("[id$='_cbCompressWebResource']").attr('checked'),
				"enableOpenSearch": $("[id$='_cbEnableOpenSearch']").attr('checked'),
				"requireSslForMetaWeblogApi": $("[id$='_cbRequireSslForMetaWeblogApi']").attr('checked'),
				"wwwSubdomain": $('.rblSubdomain input:radio:checked').val(),
				"enableErrorLogging": $("[id$='_cbEnableErrorLogging']").attr('checked'),
				"allowRemoteFileDownloads": $("[id$='_cbAllowRemoteFileDownloads']").attr('checked'),
				"remoteTimeout": $("[id$='_txtRemoteTimeout']").attr('value'),
				"remoteMaxFileSize": $("[id$='_txtRemoteMaxFileSize']").attr('value'),
				"galleryFeedUrl": $("[id$='_txtGalleryFeed']").attr('value'),
				"enablePasswordReset": $("[id$='cbEnablePasswordReset']").attr('checked'),
				"enableSelfRegistration": $("[id$='_cbEnableSelfRegistration']").attr('checked'),
				"selfRegistrationInitialRole": $("[id$='_ddlSelfRegistrationInitialRole']").val()               
			};
			
            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "admin/Settings/Advanced.aspx/Save",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(dto),
                beforeSend: onAjaxBeforeSend,
                success: function (result) {
                    var rt = result.d;
                    if (rt.Success)
                        ShowStatus("success", rt.Message);
                    else
                        ShowStatus("warning", rt.Message);
                }
            });
            $('.loader').hide();
            return false;
        }

        function warnProviderChange() {
            if ($('#ddlProvider').val() == $('#hdnProvider').val()) {
                ShowStatus("warning", "No provider change detected, operation cancelled.");
                return false;
            }
            return confirm('***CRITIAL WARNING***\r\n\r\nAre you sure you wish to modify your Storage Provider?\r\n\r\nWhen you modify your' +
                           ' storage provider all your uploaded files will be archived, and then moved into your selected provider. Once the move has been completed old File Storage will be deleted.' +
                           '\r\n\r\nThis operation could take 2-3 minutes where your application may seem unresponsive.\r\n\r\nIt is advised to backup ' +
                           'your File Storage locally before completing this request.\r\n\r\nDo NOT refresh the page while your provider is being modified. ' +
                           '\r\n\r\nAre you sure you wish to continue?');
        }
        function warnBackupTime() {
            return confirm('***CRITICAL WARNING***\r\n\r\nYou have selected to backup your file system.' +
                           ' This operation will archive all your current files and allow you to download the archive to your local machine.\r\n\r\n' +
                           'This operation (depending on the file storage size) could take 2-3 minutes.' +
                           ' During this time you MUST NOT refresh or reload your browser.\r\n\r\nAre you sure you wish to continue?');
        }
    </script>
     
	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu" runat="server" />
		</div>
		<div class="content-box-left">
            <div class="rightligned-top action_buttons">
                <input type="submit" id="btnSave2" class="btn primary" value="<%=Resources.labels.saveSettings %>" />
            </div>

           <h1><%=Resources.labels.advancedSettings %></h1>

                <ul class="fl leftaligned">
                    <li>
                        <label class="lbl" for="<%=rblWwwSubdomain.ClientID %>"><%=Resources.labels.handleWwwSubdomain %></label>
                            <asp:RadioButtonList runat="server" CssClass="rblSubdomain" ID="rblWwwSubdomain" RepeatLayout="flow" RepeatDirection="horizontal">
                                <asp:ListItem Text="<%$ Resources:labels, remove %>" Value="remove" />
                                <asp:ListItem Text="<%$ Resources:labels, enforce %>" Value="add" />
                                <asp:ListItem Text="<%$ Resources:labels, ignore %>" Value="" Selected="true" />
                            </asp:RadioButtonList>
                    </li>
                    <li>
                        <label class="lbl"><%=Resources.labels.otherSettings %></label>
                        <asp:CheckBox runat="server" ID="cbEnableCompression" />
                        <label for="<%=cbEnableCompression.ClientID %>"><%=Resources.labels.enableHttpCompression %></label>
                        <span class="insetHelp">(<%=Resources.labels.enableHttpCompressionDescription %>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbRemoveWhitespaceInStyleSheets" />
                        <label for="<%=cbRemoveWhitespaceInStyleSheets.ClientID %>"><%=Resources.labels.trimStylesheet %></label>
                        <span class="insetHelp">(<%=Resources.labels.trimStylesheetDescription %>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbCompressWebResource" />
                        <label for="<%=cbCompressWebResource.ClientID %>"><%=Resources.labels.compressWebResource %></label>
                        <span class="insetHelp">(<%=Resources.labels.compressWebResourceDescription%>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbEnableOpenSearch" />
                        <label for="<%=cbEnableOpenSearch.ClientID %>"><%=Resources.labels.enableOpenSearch %></label>
                        <span class="insetHelp">(<%=Resources.labels.enableOpenSearchDescription %>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbRequireSslForMetaWeblogApi" />
                        <label for="<%=cbRequireSslForMetaWeblogApi.ClientID %>"><%=Resources.labels.requireSslForMetaWeblogApi %></label>
                        <span class="insetHelp">(<%=Resources.labels.requireSslForMetaWeblogApiDescription%>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbEnableErrorLogging" />
                        <label for="<%=cbEnableErrorLogging.ClientID %>"><%=Resources.labels.enableErrorLogging %></label>
                        <span class="insetHelp">(<%=Resources.labels.enableErrorLoggingDescription%>)</span>
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtGalleryFeed.ClientID %>"><%=Resources.labels.galleryFeedUrl%></label>
                        <asp:TextBox Width="300" runat="server" ID="txtGalleryFeed" />
                        <span class="belowHelp"><label><%=Resources.labels.galleryFeedUrlDescription%></label></span>
                    </li>
                </ul>
                <h2><%=Resources.labels.securitySettings %></h2>

                <ul class="fl leftaligned">
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbEnablePasswordReset" />
                        <label for="<%=cbEnablePasswordReset.ClientID %>"><%=Resources.labels.enablePasswordReset %></label>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbEnableSelfRegistration" />
                        <label for="<%=cbEnableSelfRegistration.ClientID %>"><%=Resources.labels.enableSelfRegistration %></label>
                    </li>
                    <li>
                        <label class="lbl" for="<%=ddlSelfRegistrationInitialRole.ClientID %>"><%=Resources.labels.selfRegistrationInitialRole%></label>
                        <asp:DropDownList runat="Server" ID="ddlSelfRegistrationInitialRole" Style="text-transform: capitalize">
                            <asp:ListItem Text="Select" />
                        </asp:DropDownList>
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtRemoteTimeout.ClientID %>"><%=Resources.labels.remoteTimeout %></label>
                        <asp:TextBox runat="server" Width="80" ID="txtRemoteTimeout" />
                        <span class="belowHelp"><label for="<%=txtRemoteTimeout.ClientID %>"><%=Resources.labels.remoteTimeoutDescription %></label></span>
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtRemoteMaxFileSize.ClientID %>"><%=Resources.labels.maximumRemoteFileSize%></label>
                        <asp:TextBox runat="server" ID="txtRemoteMaxFileSize" />
                        <span class="belowHelp"><label><%=Resources.labels.maximumRemoteFileSizeDescription%></label></span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbAllowRemoteFileDownloads" /><label for="<%=cbAllowRemoteFileDownloads.ClientID %>"><%=Resources.labels.allowRemoteFileDownloads %></label>
                        <span class="insetHelp"><%=Resources.labels.allowRemoteFileDownloadsDescription%></span>
                    </li>
                </ul>
                <h2><%=Resources.labels.filestorage %></h2>
                <p>
                    Modifying your File Storage settings may cause data loss. This operation will move all your files from one storage provider to another storage provider. It is recommended to backup your File storage into a recoverable archive.
                </p>
                <ul class="fl leftaligned">
                    <li>
                        <label class="lbl">Backup File Storage</label>
                        <asp:Button runat="server" ID="btnDownloadArchive" OnClick="btnDownloadArchive_Click" Text="Start Backup" CssClass="btn rounded" OnClientClick="return warnBackupTime();" />
                    </li>
                    <li>
                        <label class="lbl">Change File Storage Provider</label>
                        <span style="float:left; text-align: left;">
                            <asp:DropDownList runat="server" ID="ddlProvider" ClientIDMode="Static" />
                            <asp:Button runat="server" ID="btnChangeProvider" OnClick="btnChangeProvider_Click" Text="Change Provider" CssClass="btn rounded" OnClientClick="return warnProviderChange();" />
                        </span>
                        <asp:HiddenField runat="server" ID="hdnProvider" ClientIDMode="Static" />
                        <label class="lbl"></label>
                        <asp:Label runat="server" ID="providerError" ForeColor="Red" Visible="false" />
                    </li>
                </ul>
            <div class="rightligned-bottom action_buttons">
                <input type="submit" id="btnSave" class="btn primary rounded" value="<%=Resources.labels.saveSettings %>" />
            </div>
		</div>
	</div>       
</asp:Content>

