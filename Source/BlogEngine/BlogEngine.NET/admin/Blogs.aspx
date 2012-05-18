<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Blogs.aspx.cs" Inherits="Admin.Blogs" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">

    <script type="text/javascript" src="jquery.colorbox.js"></script>
    <script type="text/javascript" src="jquery.tipsy.js"></script>
    <script type="text/javascript">

        function GetBlogDataJson() {

            // note: blogId might be empty if this is a new blog being added.

            var data = {
                "blogId": $.trim($('#hdnEditBlogId').val()),
                "copyFromExistingBlogId": $.trim($('#existingBlogToCreateNewBlogFrom').val()),
                "blogName": $.trim($('#txtBlogName').val()),
                "storageContainerName": $.trim($('#txtStorageContainerName').val()),
                "hostname": $.trim($('#txtHostname').val()),
                "isAnyTextBeforeHostnameAccepted": $('#cbAcceptAnyTextBeforeHostname').is(':checked'),
                "virtualPath": $.trim($('#txtVirtualPath').val()),
                "isActive": $('#cbActive').is(':checked'),
                "isSiteAggregation": $('#cbIsSiteAggregation').is(':checked')
            };

            return data;
        }

        function IsValidChars(value) {
            return /^[a-z0-9-_]+$/i.test(value);
        }

        function OnlyAllowedMsg() {
            return "Only letters, numbers, hyphens and underscores are allowed.";
        }

        var validIpAddressRegex = /^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])/i;
        var validHostnameRegex = /^(([a-z]|[a-z][a-z0-9\-]*[a-z0-9])\.)*([a-z]|[a-z][a-z0-9\-]*[a-z0-9])$/i;

        function IsEditingPrimary() {
            
        }

        function IsBlogDataValid() {

            // hide all validators.
            $('.overlaypanel req').addClass('hidden');

            var data = GetBlogDataJson();

            if (data.blogName.length === 0) {
                $('#txtBlogNameReq').removeClass('hidden');
                $('#txtUserName').focus().select();
                return false;
            }

            var storageContainerNameIsReadonly = $('#txtStorageContainerName').attr('readonly');
            if (!storageContainerNameIsReadonly) {
                if (data.storageContainerName.length === 0) {
                    $('#txtStorageContainerNameReq').removeClass('hidden');
                    $('#txtStorageContainerName').focus().select();
                    return false;
                } else {

                    if (!IsValidChars(data.storageContainerName)) {
                        $('#txtStorageContainerNameReq').removeClass('hidden');
                        $('#txtStorageContainerName').focus().select();
                        alert('The Storage Container Name contains invalid characters.  ' + OnlyAllowedMsg());
                        return false;
                    }
                }
            }

            if (data.hostname.length > 0) {

                if (!validIpAddressRegex.test(data.hostname) &&
                    !validHostnameRegex.test(data.hostname)) {

                    $('#txtHostnameInvalid').removeClass('hidden');
                    $('#txtHostname').focus().select();
                    alert('Host name is invalid.  It should contain either an IP address or domain name, e.g. example.com');
                    return false;
                }
            }

            if (data.virtualPath.length === 0) {
                $('#txtVirtualPathReq').removeClass('hidden');
                $('#txtVirtualPath').focus().select();
                return false;
            } else {

                if (data.virtualPath.indexOf('~/') !== 0) {
                    $('#txtVirtualPathReq').removeClass('hidden');
                    $('#txtVirtualPath').focus().select();
                    alert('Virtual path must begin with ~/\n\nPlease add ~/ to the beginning of your virtual path.');
                    return false;
                }

                // note: a virtual path of ~/ without anything after it is allowed.  this would
                // typically be for the primary blog, but can also be for blogs that are using
                // subdomains, where each instance might be ~/
                var vpath = data.virtualPath.substr(2);
                if (vpath.length > 0) {

                    if (!IsValidChars(vpath)) {
                        $('#txtVirtualPathReq').removeClass('hidden');
                        $('#txtVirtualPath').focus().select();
                        alert('The Virtual Path after ~/ contains invalid characters.  ' + OnlyAllowedMsg());
                        return false;
                    }

                }
            }

            // if this is a new blog...
            if (!data.blogId) {

                if (!data.copyFromExistingBlogId) {
                    $('#existingBlogToCreateNewBlogFrom').focus();
                    alert('An existing blog to create the new blog from must be selected.');
                    return false;
                }

            }

            return true;
        }

        function BlogDataReceived(data) {

            if (!data.Success) {
                ShowStatus("warning", "Unable to retrieve the blog data to edit.  Message: " + (data.Message || "(none)"));
                return;
            }

            resetForm();

            $('#newBlogData').hide();
            $('#addNewBlogHeader').hide();
            $('#editBlogHeader').show();

            data = data.Data;

            $('#hdnEditBlogId').val(data.Id);
            $('#txtBlogName').val(data.Name);
            $('#txtStorageContainerName').val(data.StorageContainerName).attr('readonly', 'readonly').css('background-color', '#d8d8d8').css('color', '#787878');
            $('#txtHostname').val(data.Hostname);
            $('#txtVirtualPath').val(data.VirtualPath);

            SetCheckbox($('#cbAcceptAnyTextBeforeHostname'), data.IsAnyTextBeforeHostnameAccepted);
            SetCheckbox($('#cbActive'), data.IsActive);
            SetCheckbox($('#cbIsSiteAggregation'), data.IsSiteAggregation);

            OpenColorbox();
        }

        function SetCheckbox(cb, isChecked) {

            if (isChecked) {
                cb.attr('checked', 'checked');
            } else {
                cb.removeAttr('checked');
            }
        }

        function EditBlog(btn) {

            var row = $(btn).closest('tr');
            var id = row.attr('id');
            if (!id || id.length !== 36) {
                ShowStatus("warning", "Sorry, cannot determine the ID of the Blog you would like to edit.");
                return false;
            }

            GetBlog(id, BlogDataReceived);
            return false;
        }

        function SaveBlog() {
            
            // this same SaveBlog() is called for both Adding and Updating blogs.

            if (!IsBlogDataValid()) {
                return;
            }

            var data = GetBlogDataJson();

            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "api/Blogs.asmx/SaveBlog",
                data: JSON.stringify(data),
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: onAjaxBeforeSend,
                success: function (result) {
                    var rt = result.d;
                    if (rt.Success) {
                        resetForm();
                        LoadBlogs();
                        ShowStatus("success", rt.Message);
                    }
                    else {
                        ShowStatus("warning", rt.Message);
                    }

                    closeOverlay();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    closeOverlay();
                    ShowStatus("warning", "An unexpected error occurred: " + (thrownError || ""));
                }
            });

            
            return false;
        }

        function onAddNewClicked() {

            resetForm();

            $('#newBlogData').show();
            $('#addNewBlogHeader').show();
            $('#editBlogHeader').hide();
            $('#txtStorageContainerName').removeAttr('readonly').css('background-color', '').css('color', '');

            var copyFrom = $('#existingBlogToCreateNewBlogFrom');

            if ($('option', copyFrom).length > 0) {
                return true;
            } else {

                /*  make a synchronous call to get the available blogs to copy from */
                $.ajax({
                    url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/GetCopyFromBlogs",
                    data: JSON.stringify({ }),
                    type: "POST",
                    async: false,  /* note: this is synchronous */
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    beforeSend: onAjaxBeforeSend,
                    success: function (result) {
                        var rt = result.d;
                        if (rt.Success) {

                            copyFrom.append($(document.createElement("option")).attr("value", "").text("Select One"));

                            for (var i = 0; i < rt.Data.length; i++) {
                                copyFrom.append($(document.createElement("option")).attr("value", rt.Data[i].Key).text(rt.Data[i].Value));
                            }

                            return true;
                        }
                        else {
                            ShowStatus("warning", rt.Message);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        ShowStatus("warning", "An unexpected error occurred: " + (thrownError || ""));
                    }
                });                

            }

            return false;
        }

        function OnAdminDataSaved() {
            LoadBlogs();
        }

        function closeOverlay() {
            $.colorbox.close();
        }

        function resetForm() {

            $('.tblForm .req').addClass('hidden');

            $('#hdnEditBlogId').val('');
            $('#txtBlogName').val('');
            $('#txtStorageContainerName').val('');
            $('#txtHostname').val('');
            $('#cbAcceptAnyTextBeforeHostname').attr('checked', 'checked');
            $('#txtVirtualPath').val('');
            $('#cbActive').attr('checked', 'checked');
            $('#cbIsSiteAggregation').removeAttr('checked');
        }

        function OpenColorbox() {
            var colorboxOptions = GetColorboxOptions();
            colorboxOptions.open = true;
            $('<a/>').colorbox(colorboxOptions);
        }

        function GetColorboxOptions() {
            var opts = { width: "550px", inline: true, href: '#frmAddNew' };
            return opts;
        }

        $(function () {

            var colorboxOptions = GetColorboxOptions();
            colorboxOptions.onOpen = onAddNewClicked;

            $(".addNew").colorbox(colorboxOptions);

            LoadBlogsForPage(1);
            $(".tableToolBox a").click(function () {
                $(".tableToolBox a").removeClass("current");
                $(this).addClass("current");
            });

            $("#txtVirtualPath").focus(function () {

                if ($.trim($(this).val()).length === 0) {
                    $(this).val('~/');
                }

            });

        });

    </script>


    <div style="display:none;">
    <div id="frmAddNew" class="overlaypanel" >
        <h2>
            <span id="addNewBlogHeader"><%=Resources.labels.addNewBlog%></span>
            <span id="editBlogHeader"><%=Resources.labels.editExistingBlog%></span>
        </h2>
        <input type="hidden" id="hdnEditBlogId" />
        <table class="tblForm">
            <tr>
                <td>
				    <label for="txtBlogName" class="lbl"><%=Resources.labels.name %></label>
				    <input type="text" id="txtBlogName" class="txt200"/>
				    <span id="txtBlogNameReq" class="req hidden">*</span>
				</td>
				<td>
				    <label for="txtStorageContainerName" class="lbl"><%=Resources.labels.storageContainerName%></label>
				    <input type="text" id="txtStorageContainerName" class="txt200"/>
				    <span id="txtStorageContainerNameReq" class="req hidden">*</span>
                </td>
            </tr>
            <tr>
                <td>
				    <label for="txtHostname" class="lbl"><%=Resources.labels.hostName%></label>
				    <input type="text" id="txtHostname" class="txt200"/>
                    <span id="txtHostnameInvalid" class="req hidden">*</span>
				</td>
				<td>
				    <label for="cbAcceptAnyTextBeforeHostname" class="lbl"><%=Resources.labels.acceptAnyTextBeforeHostname%></label>
				    <input type="checkbox" id="cbAcceptAnyTextBeforeHostname" />
                </td>
            </tr>
            <tr>
                <td>
				        <label for="txtVirtualPath" class="lbl"><%=Resources.labels.virtualPath %></label>
				        <input type="text" id="txtVirtualPath" class="txt200"/>
                        <span id="txtVirtualPathReq" class="req hidden">*</span>
                </td>
                <td>
                        <label for="cbActive" class="lbl"><%=Resources.labels.active %></label>
				        <input type="checkbox" id="cbActive"/>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                        <label for="cbIsSiteAggregation" class="lbl"><%=Resources.labels.isForSiteAggregation%></label>
				        <input type="checkbox" id="cbIsSiteAggregation"/>
                </td>
            </tr>
        </table>
        <div id="newBlogData" style="margin:0 0 20px;">
            <h2><%=Resources.labels.existingBlogToCreateNewBlogFrom %></h2>
            <select id="existingBlogToCreateNewBlogFrom"></select>
        </div>
		<input type="submit" class="btn primary rounded" value="<%=Resources.labels.save %>" onclick="return SaveBlog();" id="btnNewBlog" />
		<%=Resources.labels.or %> <a href="#" onclick="closeOverlay();"><%=Resources.labels.cancel %></a>
        <br /><br />
	</div>
    </div>


	<div class="content-box-outer">
		<div class="content-widgets-box-full">
            <h1><%=Resources.labels.blogs%><a href="#" class="addNew"><%=Resources.labels.addNewBlog%></a></h1>
            <div class="tableToolBox">
                <div class="Pager"></div>
                <div class="PageSize">
                    <label for="pageSizeTop"><%=Resources.labels.itemsPerPage%></label>
                    <select id="pageSizeTop" name="pageSizeTop" onchange="return ChangeBlogsPage(this)">
                        <option value="10">10</option>
                        <option value="25" selected="selected">25</option>
                        <option value="50">50</option>
                        <option value="100">100</option>
                    </select>
                </div>
            </div>
            <div id="Container"></div>
            <div class="Pager"></div>
            <div class="PageSize">
                <label for="pageSizeBottom"><%=Resources.labels.itemsPerPage%></label>
                <select id="pageSizeBottom" name="pageSizeBottom" onchange="return ChangeBlogsPage(this)">
                    <option value="10">10</option>
                    <option value="25" selected="selected">25</option>
                    <option value="50">50</option>
                    <option value="100">100</option>
                </select>
            </div>
        </div>
    </div>
</asp:Content>
