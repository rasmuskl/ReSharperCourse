<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="admin.Settings.Main" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server"> 
    <script type="text/javascript">

        var oShowDescChkBox;
        var oShowDescTagCatChkBox;
        var oDescCharContainer;
        var oDescCharTagCatContainer;

        function syncCharCountBox(oChkBox,oCharContainer) {

            var isChecked = oChkBox.is(":checked");
            if (isChecked) {
                oCharContainer.show();
            } else {
                oCharContainer.hide();
            }
        }

        $(document).ready(function () {
            var frm = document.forms.aspnetForm;
            $(frm).validate({
                onsubmit: false
            });

            oShowDescChkBox = $("#<%=cbShowDescriptionInPostList.ClientID %>");
            oShowDescTagCatChkBox = $("#<%=cbShowDescriptionInPostListForPostsByTagOrCategory.ClientID %>");
            oDescCharContainer = $("#DescriptionCharacters");
            oDescCharTagCatContainer = $("#DescriptionCharactersForPostsByTagOrCategory");

            $(".btn").click(function (evt) {
                if ($(frm).valid())
                    SaveSettings();

                evt.preventDefault();
            });

            oShowDescTagCatChkBox.change(function () {
                syncCharCountBox(oShowDescTagCatChkBox, oDescCharTagCatContainer);
            });

            oShowDescChkBox.change(function () {
                syncCharCountBox(oShowDescChkBox, oDescCharContainer);
            });

            syncCharCountBox(oShowDescChkBox, oDescCharContainer);
            syncCharCountBox(oShowDescTagCatChkBox, oDescCharTagCatContainer);

        });
        function SaveSettings() {
            $('.loader').show();
            var dto = { 
				"name": $("[id$='_txtName']").val(),
				"desc": $("[id$='_txtDescription']").val(),
				"postsPerPage": $("[id$='_txtPostsPerPage']").val(),
				"themeCookieName": $("[id$='_txtThemeCookieName']").val(),
				"useBlogNameInPageTitles": $("[id$='_cbUseBlogNameInPageTitles']").attr('checked'),
				"enableRelatedPosts": $("[id$='_cbShowRelatedPosts']").attr('checked'),
				"enableRating": $("[id$='_cbEnableRating']").attr('checked'),
				"showDescriptionInPostList": oShowDescChkBox.attr('checked'),
				"descriptionCharacters": $("input", oDescCharContainer).val(),
				"showDescriptionInPostListForPostsByTagOrCategory": oShowDescTagCatChkBox.attr('checked'),
				"descriptionCharactersForPostsByTagOrCategory": $("input", oDescCharTagCatContainer).val(),
				"timeStampPostLinks": $("[id$='_cbTimeStampPostLinks']").attr('checked'),
				"showPostNavigation": $("[id$='_cbShowPostNavigation']").attr('checked'),
				"culture": $("[id$='_ddlCulture']").val(),
				"timezone": $("[id$='_txtTimeZone']").val(),
                "enableQuickNotes": $("[id$='_cbEnableQuickNotes']").attr('checked'),
			};
			
            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "admin/Settings/Main.aspx/Save",
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

            <h1 ><%=Resources.labels.basic %> <%=Resources.labels.settings %></h1>

                <ul class="fl leftaligned">
                    <li>
                        <label class="lbl" for="<%=txtName.ClientID %>"><%=Resources.labels.name %></label>
                        <asp:TextBox width="300" runat="server" ID="txtName" CssClass="required" /></li>
                    <li>
                        <label class="lbl" for="<%=txtDescription.ClientID %>"><%=Resources.labels.description %></label>
                        <asp:TextBox width="300" runat="server" ID="txtDescription" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtThemeCookieName.ClientID %>"><%=Resources.labels.themeCookieName %></label>
                        <asp:TextBox CssClass="w300" runat="server" ID="txtThemeCookieName" />
                    </li>
                    <li>
                        <label class="lbl" for="<%=ddlCulture.ClientID %>"><%=Resources.labels.language %></label>
                        <asp:DropDownList runat="Server" ID="ddlCulture" Style="text-transform: capitalize">
                            <asp:ListItem Text="Auto" />
                            <asp:ListItem Text="english" Value="en" />
                        </asp:DropDownList>
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtTimeZone.ClientID %>"><%=Resources.labels.timezone %></label>
                        <asp:TextBox runat="Server" ID="txtTimeZone" Width="30" CssClass="number" />
                        <span>Server time: <%=DateTime.Now.ToShortTimeString() %></span>
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtPostsPerPage.ClientID %>"><%=Resources.labels.postPerPage %></label>
                        <asp:TextBox runat="server" ID="txtPostsPerPage" Width="50" MaxLength="4" CssClass="required number" />
                    </li>
                    <li>
                        <label class="lbl"><%=Resources.labels.appearance %></label>
                        <asp:CheckBox runat="server" ID="cbShowDescriptionInPostList" />
                        <label for="<%=cbShowDescriptionInPostList.ClientID %>"><%=Resources.labels.showDescriptionInPostList %></label>
                        <div class="insetForm" id="DescriptionCharacters" style=" display:none;">
                            <label class="lbl" for="<%=txtDescriptionCharacters.ClientID %>"><%=Resources.labels.numberOfCharacters %></label>
                            <asp:TextBox runat="server" ID="txtDescriptionCharacters" Width="40" CssClass="number" />      
                        </div>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbShowDescriptionInPostListForPostsByTagOrCategory" />
                        <label for="<%=cbShowDescriptionInPostListForPostsByTagOrCategory.ClientID %>"><%=Resources.labels.showDescriptionInPostListForPostsByTagOrCategory %></label>
                        <div class="insetForm" id="DescriptionCharactersForPostsByTagOrCategory" style=" display:none;">
                            <label class="lbl" for="<%=txtDescriptionCharactersForPostsByTagOrCategory.ClientID %>" ><%=Resources.labels.numberOfCharacters %></label>
                            <asp:TextBox runat="server" ID="txtDescriptionCharactersForPostsByTagOrCategory" Width="40" CssClass="number" />
                        </div>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbShowRelatedPosts" />
                        <label for="<%=cbShowRelatedPosts.ClientID %>"><%=Resources.labels.showRelatedPosts %></label>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbShowPostNavigation" />
                        <label for="<%=cbShowPostNavigation.ClientID %>"><%=Resources.labels.showPostNavigation %></label>
                    </li>
                    <li>
                        <label class="lbl"><%=Resources.labels.otherSettings %></label>
                        <asp:CheckBox runat="server" ID="cbUseBlogNameInPageTitles" />
                        <label for="<%=cbUseBlogNameInPageTitles.ClientID %>"><%=Resources.labels.useBlogNameInPageTitles%></label>
                        <span class="insetHelp">(<%=Resources.labels.useBlogNameInPageTitlesDescription%>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbEnableRating" />
                        <label for="<%=cbEnableRating.ClientID %>"><%=Resources.labels.enableRating %></label>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbTimeStampPostLinks" />
                        <label for="<%=cbTimeStampPostLinks.ClientID %>"><%=Resources.labels.timeStampPostLinks %></label>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbEnableQuickNotes" />
                        <label for="<%=cbEnableQuickNotes.ClientID %>"><%=Resources.labels.enableQuickNotes %></label>
                    </li>
                </ul>

            <div class="rightligned-bottom action_buttons">
                <input type="submit" id="btnSave2" class="btn primary" value="<%=Resources.labels.saveSettings %>" />
            </div>
		</div>
	</div>    
</asp:Content>