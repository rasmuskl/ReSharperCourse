<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Feed.aspx.cs" Inherits="admin.Settings.Feed" %>
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
        });
        function geodeAsk() {
            if (navigator.geolocation)
                navigator.geolocation.getCurrentPosition(geoFound, geoNotFound);
        }
        function geoFound(pos) {
            document.getElementById('<%=txtGeocodingLatitude.ClientID %>').value = pos.latitude;
            document.getElementById('<%=txtGeocodingLongitude.ClientID %>').value = pos.longitude;
        }

        function geoNotFound() {
            alert('You must be on a wifi network for us to determine your location');
        } 
		function SaveSettings() {
            $('.loader').show();
            var dto = { 
				"syndicationFormat": $("[id$='_ddlSyndicationFormat']").val(),
				"postsPerFeed": $("[id$='_txtPostsPerFeed']").val(),
				"dublinCoreCreator": $("[id$='_txtDublinCoreCreator']").val(),
				"feedemail": $("[id$='_txtEmail']").val(),
				"dublinCoreLanguage": $("[id$='_txtDublinCoreLanguage']").val(),
				"geocodingLatitude": $("[id$='_txtGeocodingLatitude']").val(),
				"geocodingLongitude": $("[id$='_txtGeocodingLongitude']").val(),
				"blogChannelBLink": $("[id$='_txtBlogChannelBLink']").val(),
				"alternateFeedUrl": $("[id$='_txtAlternateFeedUrl']").val(),
				"enableEnclosures": $("[id$='_cbEnableEnclosures']").attr('checked')
			};
			
            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "admin/Settings/Feed.aspx/Save",
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
                        <label class="lbl" for="<%=txtDublinCoreCreator.ClientID %>"><%=Resources.labels.author %></label>
                        <asp:TextBox runat="server" ID="txtDublinCoreCreator" Width="300" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtEmail.ClientID %>"><%=Resources.labels.email %></label>
                        <asp:TextBox runat="server" ID="txtEmail" Width="300" CssClass="email" />
                        <span class="belowHelp">Feed author's email address (optional)</span>
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtBlogChannelBLink.ClientID %>"><%=Resources.labels.endorsment %></label>
                        <asp:TextBox runat="server" ID="txtBlogChannelBLink" MaxLength="255" Width="300" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtAlternateFeedUrl.ClientID %>"><%=Resources.labels.alternateFeedUrl %></label>
                        <asp:TextBox runat="server" ID="txtAlternateFeedUrl" Width="300" CssClass="url" />
                        <span class="belowHelp">(http://feeds.feedburner.com/username)</span>
                    </li>
                    <li>
                        <label class="lbl" for="<%=ddlSyndicationFormat.ClientID %>" style="position: relative; top: 4px"><%=Resources.labels.defaultFeedOutput %></label>
                        <asp:DropDownList runat="server" ID="ddlSyndicationFormat">
                            <asp:ListItem Text="RSS 2.0" Value="Rss" Selected="True" />
                            <asp:ListItem Text="Atom 1.0" Value="Atom" />
                        </asp:DropDownList>
                        format.
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtDublinCoreLanguage.ClientID %>"><%=Resources.labels.languageCode %></label>
                        <asp:TextBox runat="server" ID="txtDublinCoreLanguage" Width="60" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtPostsPerFeed.ClientID %>"><%=Resources.labels.postsPerFeed %></label>
                        <asp:TextBox runat="server" ID="txtPostsPerFeed" Width="50" MaxLength="4" CssClass="required number" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtGeocodingLatitude.ClientID %>"><%=Resources.labels.latitude %></label>
                        <asp:TextBox runat="server" ID="txtGeocodingLatitude" Width="150" CssClass="number" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtGeocodingLongitude.ClientID %>"><%=Resources.labels.longtitude %></label>
                        <asp:TextBox runat="server" ID="txtGeocodingLongitude" Width="150" CssClass="number" />&nbsp;
                        <input type="button" class="btn" id="findPosition" onclick="geodeAsk()" value="<%=Resources.labels.findPosition %>" style="display: none" />
                        <script type="text/javascript">
                            if (navigator.geolocation) document.getElementById('findPosition').style.display = 'inline';
                        </script>
                    </li>
                    <li>
                        <label class="lbl"><%=Resources.labels.otherSettings %></label>
                        <asp:CheckBox runat="server" ID="cbEnableEnclosures" />
                        <label for="<%=cbEnableEnclosures.ClientID %>"><%=Resources.labels.enableEnclosures %></label>
                    </li>
                </ul>
       </div>
    </div>
</asp:Content>