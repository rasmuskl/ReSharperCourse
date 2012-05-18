<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" 
ValidateRequest="false" CodeFile="EditPage.aspx.cs" Inherits="Admin.Pages.EditPage" %>

<%@ Register Src="~/admin/htmlEditor.ascx" TagPrefix="Blog" TagName="TextEditor" %>
<%@ Register Src="~/admin/FileManager/FileManager.ascx" TagName="FileManager" TagPrefix="con" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">
    <script type="text/javascript">
        function GetSlug() {
            var title = document.getElementById('<%=txtTitle.ClientID %>').value;
            WebForm_DoCallback('__Page', title, ApplySlug, 'slug', null, false)
        }

        function ApplySlug(arg, context) {
            var slug = document.getElementById('<%=txtSlug.ClientID %>');
            slug.value = arg;
        }

        function SavePage() {
            $('.loader').show();

            var content = tinyMCE.activeEditor.getContent();

            if (content.length == 0) content = '[No text]';

            var title = $("[id$='txtTitle']").val();
            var description = $("[id$='txtDescription']").val();
            var keywords = $("[id$='txtKeyword']").val();
            var slug = $("[id$='txtSlug']").val();

            var isFrontPage = $("[id$='cbFrontPage']").is(':checked');
            var showInList = $("[id$='cbShowInList']").is(':checked');
            var isPublished = $("[id$='cbPublished']").is(':checked');

            var parent = $("[id$='ddlParent'] option:selected").val();

            var dto = {
                "id": Querystring('id'),
                "content": content,
                "title": title,
                "description": description,
                "keywords": keywords,
                "slug": slug,
                "isFrontPage": isFrontPage,
                "showInList": showInList,
                "isPublished": isPublished,
                "parent": parent
            };

            //alert(JSON.stringify(dto));

            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/SavePage",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(dto),
                beforeSend: onAjaxBeforeSend,
                success: function (result) {
                    var rt = result.d;
                    if (rt.Success) {
                        if (rt.Data) {
                            window.location.href = rt.Data;
                        } else {
                            ShowStatus("success", rt.Message);
                        }
                    }
                    else
                        ShowStatus("warning", rt.Message);
                }
            });

            $('.loader').hide();
            return false;
        }
    </script>

    <script type="text/javascript" src="../jquery.colorbox.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#uploadImage").colorbox({ width: "550px", inline: true, href: "#uploadImagePanel" });
            $("#uploadVideo").colorbox({ width: "550px", inline: true, href: "#uploadVideoPanel" });
            $("#uploadFile").colorbox({ width: "550px", inline: true, href: "#uploadFilePanel" });
        });

        function closeOverlay() {
            $.colorbox.close();
        }
    </script>

    <div style="display:none;">
        <div id="uploadVideoPanel" class="overlaypanel">
            <h2><%=Resources.labels.insertVideo %></h2>
            <ul class="fl" style="margin:0;">
                <li>
                    <asp:Label ID="lblVideoUpload" CssClass="lbl" AssociatedControlID="txtUploadVideo" runat="server" Text='<%$ Resources:labels, uploadVideo %>' />
                    <asp:FileUpload runat="server" ID="txtUploadVideo" Width="400" size="50" ValidationGroup="imageupload" />
                    <asp:RequiredFieldValidator ID="txtUploadVideoValidator" runat="Server" ControlToValidate="txtUploadVideo" ErrorMessage="<%$ Resources:labels, required %>"
                        ValidationGroup="videoupload" />
                </li>
                <li style="margin:0;">
                    <asp:Button runat="server" ID="btnUploadVideo" Text="<%$Resources:labels,upload %>"
                        ValidationGroup="videoupload" CssClass="btn primary" OnClientClick="colorboxDialogSubmitClicked('videoupload', 'uploadVideoPanel');" /> 
                        <%=Resources.labels.or %> <a href="#" onclick="return closeOverlay();"><%=Resources.labels.cancel %></a>
                </li>
            </ul>
        </div>
        <div id="uploadImagePanel" class="overlaypanel">
            <h2><%=Resources.labels.insertImage %></h2>
            <ul class="fl" style="margin:0;">
                <li>
                    <label class="lbl"><%=Resources.labels.uploadImage %></label>
                    <asp:FileUpload runat="server" ID="txtUploadImage" size="50" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="Server" ControlToValidate="txtUploadImage" ErrorMessage="<%$ Resources:labels, required %>"
                        ValidationGroup="imageupload" />
                </li>
                <li style="margin:0;">
                    <asp:Button CssClass="btn primary" runat="server" ID="btnUploadImage" Text="<%$Resources:labels,upload %>"
                        ValidationGroup="imageupload" OnClientClick="colorboxDialogSubmitClicked('imageupload', 'uploadImagePanel');" />
                        <%=Resources.labels.or %> <a href="#" onclick="closeOverlay();"><%=Resources.labels.cancel %></a>
                </li>
            </ul>
        </div>
        <div id="uploadFilePanel" class="overlaypanel">
            <h2><%=Resources.labels.attachFile %></h2>
            <ul class="fl" style="margin:0;">
                <li>
                    <label class="lbl"><%=Resources.labels.uploadFile %></label>
                    <asp:FileUpload runat="server" ID="txtUploadFile" size="50" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtUploadFile" ErrorMessage="<%$ Resources:labels, required %>"
                        ValidationGroup="fileUpload" />
                </li>
                <li style="margin:0;">
                    <asp:Button CssClass="btn primary" runat="server" ID="btnUploadFile" Text="<%$Resources:labels,upload %>"
                        ValidationGroup="fileUpload" OnClientClick="colorboxDialogSubmitClicked('fileUpload', 'uploadFilePanel');" />
                        <%=Resources.labels.or %> <a href="#" onclick="closeOverlay();"><%=Resources.labels.cancel %></a>
                </li>
            </ul>
        </div>
    </div>

    <div class="content-box-outer">
        <con:FileManager runat="server" ID="FileManager1" />
        <div class="content-box-full">
            <div class="rightligned-top action_buttons">
                <input type="button" id="btnSave" value="<%=Resources.labels.savePage %>" class="btn primary" onclick="return SavePage()" /> <%=Resources.labels.or %> 
                <% if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                   { %>
                <a href="<%=PageUrl %>" title="Go to page"><%=Resources.labels.goToPage %></a>
                <%}
                   else
                   {%>
                <a href="Pages.aspx" title="Cancel"><%=Resources.labels.cancel %></a>
                <%} %>
            </div>
            <h1><%=Resources.labels.editPage %></h1>
            <table class="tblForm largeForm" style="width:100%; margin:0;">
                <tr>
                    <td style="vertical-align:top; padding:0 40px 0 0;">
                        <ul class="fl">
                            <li>
                                <label class="lbl" for="<%=txtTitle.ClientID %>">
                                    <%=Resources.labels.title %></label>
                                <asp:TextBox runat="server" ID="txtTitle" Width="600" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtTitle" Display="Dynamic"
                                    ErrorMessage="<%$Resources:labels,enterTitle %>" />
                            </li>
                            <li>
                                <div class="editToolbar">
                                    <a href="#" id="uploadImage" class="image"><%=Resources.labels.insertImage %></a>
                                    <a href="#" id="uploadVideo" class="video"><%=Resources.labels.insertVideo %></a>
                                    <a href="#" id="uploadFile" class="file"><%=Resources.labels.attachFile %></a>
                                     <a href="javascript:;" id="fileManager" class="file">File Manager</a>
                                </div>
                                <Blog:TextEditor runat="server" id="txtContent" TabIndex="4" />
                            </li>
                            <li>
                                <label class="lbl" for="<%=txtSlug.ClientID %>"><%=Resources.labels.slug %></label>
                                <asp:TextBox runat="server" ID="txtSlug" TabIndex="9" Width="600" />
                                <a href="javascript:void(GetSlug());">
                                    <%=Resources.labels.extractFromTitle %></a>
                            </li>
                            <li>
                                <label class="lbl" for="<%=txtDescription.ClientID %>"><%=Resources.labels.excerpt %></label>
                                <asp:TextBox runat="server" ID="txtDescription" Width="600" TextMode="multiLine"
                                    Columns="50" Rows="4" />
                            </li>
                            <li>
                                <asp:CheckBox runat="Server" ID="cbPublished" Checked="true" Text="<%$ Resources:labels, publish %>" />
                            </li>
                        </ul>
                    </td>
                    <td class="secondaryForm" style="padding:0; vertical-align:top;">
                        <ul class="fl">
                            <li>
                                <label class="lbl" for="<%=ddlParent.ClientID %>"><%=Resources.labels.selectParent %></label>
                                <asp:DropDownList runat="server" ID="ddlParent" Width="250" />
                            </li>
                            <li>
                                <label class="lbl" for="<%=txtKeyword.ClientID %>"><%=Resources.labels.keywords %></label>
                                <asp:TextBox runat="server" ID="txtKeyword" TextMode="MultiLine" Rows="5"  />
                            </li>
                            <li>
                                <label class="lbl"><%=Resources.labels.options %></label>
                                <asp:CheckBox runat="Server" ID="cbFrontPage" Text="<%$ Resources:labels, isFrontPage %>" /><br />
                            </li>
                            <li>
                                 <asp:CheckBox runat="Server" ID="cbShowInList" Text="<%$ Resources:labels, showInList %>" Checked="true" />
                            </li>
                        </ul>
                    </td>
                </tr>
            </table>

            <div class="rightligned-bottom action_buttons">
                <input type="button" id="btnSave2" value="<%=Resources.labels.savePage %>" class="btn primary" onclick="return SavePage()" /> <%=Resources.labels.or %> 
                <% if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                   { %>
                <a href="<%=PageUrl %>" title="Go to page"><%=Resources.labels.goToPage %></a>
                <%}
                   else
                   {%>
                <a href="Pages.aspx" title="Cancel"><%=Resources.labels.cancel %></a>
                <%} %>
            </div>
        </div>
    </div>
</asp:Content>