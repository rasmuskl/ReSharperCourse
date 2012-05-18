<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Profile.aspx.cs" Inherits="Admin.Users.ProfilePage" %>
<%@ Register Src="~/admin/htmlEditor.ascx" TagPrefix="Blog" TagName="TextEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server">
    <script type="text/javascript">
        function SaveProfile() {
            var vals = new Array();
            var roles = new Array();
            var cnt = 0;

            $.each($('.chkRole:checked'), function (i, v) {
                roles[cnt] = v.id;
                cnt++;
            });

            var displayName = $('#txtDispalayName').val();
            var firstName = $('#txtFirstName').val();
            var middleName = $('#txtMiddleName').val();
            var lastName = $('#txtLastName').val();
            var email = $('#txtEmail').val();
            var birthday = $('#txtBirthday').val();
            var photoURL = $('#txtPhotoURL').val();
            var isPrivate = false;
            if ($('#chkPrivate').attr('checked')) {
                isPrivate = true;
            }
            var mobile = $('#txtMobile').val();
            var phone = $('#txtMainPhone').val();
            var fax = $('#txtFax').val();

            var city = $('#txtCity').val();
            var state = $('#txtState').val();
            var country = $('#txtCountry').val();
            var biography = $('#biography').val();

            if (displayName.length == 0) {
                $('#txtDispalayNameReq').removeClass('hidden');
                $('#txtDispalayName').focus().select();
                ShowStatus("warning", "Display Name is Required.");
                return false;
            }

            $('#txtDispalayNameReq').addClass('hidden');

            vals[0] = displayName;
            vals[1] = firstName;
            vals[2] = middleName;
            vals[3] = lastName;
            vals[4] = email;
            vals[5] = birthday;
            vals[6] = photoURL;
            vals[7] = isPrivate;
            vals[8] = mobile;
            vals[9] = phone;
            vals[10] = fax;

            vals[11] = city;
            vals[12] = state;
            vals[13] = country;
            vals[14] = biography;

            var dto = { "id": Querystring('id'), "vals" : vals, "roles" : roles };

            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "api/Profile.asmx/Save",
                data: JSON.stringify(dto),
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: onAjaxBeforeSend,
                success: function (result) {
                    var rt = result.d;
                    if (rt.Success) {
                        ShowStatus("success", rt.Message);
                    }
                    else {
                        ShowStatus("warning", rt.Message);
                    }
                }
            });
            return false;
        }

        LoadProfile();
    </script>
	<div class="content-box-outer">
        <asp:PlaceHolder ID="phRightContentBox" runat="server">
		    <div class="content-box-right">
                <ul>
			        <li class="content-box-selected"><a href="Users.aspx"><%=Resources.labels.users %></a></li>
			        <li><a href="Roles.aspx" class="selected"><%=Resources.labels.roles %></a></li>
                </ul>
		    </div>
        </asp:PlaceHolder>
		<div class="content-box-left">
            <h1 style="border:none;"><%=Resources.labels.profile %> : <%=Request.QueryString["id"] %></h1>
            <div id="Container"></div>

            <asp:PlaceHolder ID="phRoles" runat="server">
                <h2><%=Resources.labels.roles %></h2>
                <div id="rolelist" style="margin:0 0 20px;"><%=RolesList%></div>
            </asp:PlaceHolder>

            <div id="Container2"></div>
            <div class="action_buttons">
                <input type="submit" id="btnSave" class="btn primary rounded" value="<%=Resources.labels.saveProfile %>" onclick="return SaveProfile()" />
		        <%=Resources.labels.or %> <a href="Users.aspx"><%=Resources.labels.cancel %></a>
            </div>
		</div>
	</div>
</asp:Content>
