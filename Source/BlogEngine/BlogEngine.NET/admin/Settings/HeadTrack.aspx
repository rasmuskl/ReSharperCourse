<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="HeadTrack.aspx.cs" Inherits="admin.Settings.HeadTrack" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server">  
    <script type="text/javascript">
        function SaveSettings() {
            $('.loader').show();
            var hdr = $("[id$='_txtHtmlHeader']").val();
            var ftr = $("[id$='_txtTrackingScript']").val();
            var dto = { "hdr": hdr, "ftr": ftr };

            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "admin/Settings/HeadTrack.aspx/Save",
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
                <input type="submit" class="btn primary" value="<%=Resources.labels.saveSettings %>" onclick="return SaveSettings();" />
            </div>
            <h1><%=Resources.labels.settings %></h1>

                <ul class="fl leftaligned">
                    <li>
                        <label class="lbl"><%=Resources.labels.addCustomCodeToHeader %></label>
                        <asp:TextBox runat="server" ID="txtHtmlHeader" TextMode="multiLine" Rows="9" Columns="30" Width="500" />
                    </li>
                    <li>
                        <label class="lbl"><%=Resources.labels.trackingScript %></label>
                        <asp:TextBox runat="server" ID="txtTrackingScript" TextMode="multiLine" Rows="9" Columns="30" Width="500" />
                        <%--<span class="belowHelp" style="width:500px;">Will be added in the bottom of each page regardless of the theme. 
                            You can insert JavaScript code from i.e. Google Analytics (remember to add the &lt;script&gt; tags).</span>--%>
                    </li>
                </ul>

             
        </div>
    </div>
</asp:Content>