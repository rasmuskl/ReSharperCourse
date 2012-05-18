<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Email.aspx.cs" Inherits="admin.Settings.Email" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server"> 
    <script type="text/javascript">
        $(document).ready(function () {
            var	frm	= document.forms.aspnetForm;
            $(frm).validate({
                onsubmit: false
            });

            $("#btnSave").click(function (evt) {
                if ($(frm).valid())
                    SaveSettings();
                
                evt.preventDefault();
            });
        });
        function SaveSettings() {
            $('.loader').show();
		
            var dto = { 
				"email": $("[id$='_txtEmail']").val(),
				"smtpServer": $("[id$='_txtSmtpServer']").val(),
				"smtpServerPort": $("[id$='_txtSmtpServerPort']").val(),
				"smtpUserName": $("[id$='_txtSmtpUsername']").val(),
				"smtpPassword": $("[id$='_txtSmtpPassword']").val(),
				"sendMailOnComment": $("[id$='_cbComments']").attr('checked'),
				"enableSsl": $("[id$='_cbEnableSsl']").attr('checked'),
				"emailSubjectPrefix": $("[id$='_txtEmailSubjectPrefix']").val()
			};
			
            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "admin/Settings/Email.aspx/Save",
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
        function TestEmail() {
            $('.loader').show();
            var dto = {
                "email": $("[id$='_txtEmail']").val(),
                "smtpServer": $("[id$='_txtSmtpServer']").val(),
                "smtpServerPort": $("[id$='_txtSmtpServerPort']").val(),
                "smtpUserName": $("[id$='_txtSmtpUsername']").val(),
                "smtpPassword": $("[id$='_txtSmtpPassword']").val(),
                "sendMailOnComment": $("[id$='_cbComments']").attr('checked'),
                "enableSsl": $("[id$='_cbEnableSsl']").attr('checked'),
                "emailSubjectPrefix": $("[id$='_txtEmailSubjectPrefix']").val()
            };

            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "admin/Settings/Email.aspx/TestSmtp",
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
    </script>
     
	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu" runat="server" />
		</div>
		<div class="content-box-left">
            <div class="rightligned-top action_buttons">
                <input type="submit" id="btnSave" class="btn primary" value="<%=Resources.labels.saveSettings %>" />
            </div>
            <h1><%=Resources.labels.settings %></h1>

                <ul class="fl leftaligned">
                    <li>
                        <label class="lbl" for="<%=txtEmail.ClientID %>"><%=Resources.labels.emailAddress %></label>
                        <asp:TextBox CssClass="w300 email" runat="server" ID="txtEmail" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtSmtpServer.ClientID %>"><%=Resources.labels.smtpServer%></label>
                        <asp:TextBox runat="server" ID="txtSmtpServer" CssClass="w300" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtSmtpServerPort.ClientID %>"><%=Resources.labels.portNumber %></label>
                        <asp:TextBox runat="server" ID="txtSmtpServerPort" Width="35" CssClass="number" />
                        <span class="belowHelp"><%=Resources.labels.portNumberDescription %></span>
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtSmtpUsername.ClientID %>"><%=Resources.labels.userName %></label>
                        <asp:TextBox CssClass="txt" runat="server" ID="txtSmtpUsername" Width="300" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtSmtpPassword.ClientID %>"><%=Resources.labels.password %></label>
                        <asp:TextBox TextMode="Password"  runat="server" ID="txtSmtpPassword" Width="300" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtEmailSubjectPrefix.ClientID %>"><%=Resources.labels.emailSubjectPrefix %></label>
                        <asp:TextBox runat="server" ID="txtEmailSubjectPrefix" Width="300" />
                    </li>
                    <li>
                        <label class="lbl"><%=Resources.labels.otherSettings %></label>
                        <asp:CheckBox runat="Server" ID="cbEnableSsl" />
                        <label for="<%=cbEnableSsl.ClientID %>"><%=Resources.labels.enableSsl%></label>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="Server" ID="cbComments" />
                        <label for="<%=cbComments.ClientID %>"><%=Resources.labels.sendCommentEmail %></label>
                    </li>
                </ul>
            <div class="action_buttons">
                <input style="margin-left: 220px" type="submit" class="btn" value="<%=Resources.labels.testEmailSettings %>" onclick="return TestEmail();" />
                <asp:Label runat="Server" ID="lbSmtpStatus" />
            </div>
        </div>
    </div>
</asp:Content>

