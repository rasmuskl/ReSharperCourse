<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true"
    CodeFile="Rights.aspx.cs" Inherits="Admin.Users.Rights" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">
    <script type="text/javascript">
        var rights = <%=GetRightsJson()%>;
        var role = "<%=RoleName %>";

        var rightsControls = {};

        $(document).ready(function() {

            var tempIdCount = 0;

            for (var category in rights) {
            
                var catDiv = $("<div class=\"dashboardWidget rounded\">");
                var header = $("<h2 style='border:none;' />");
                header.html(category);

                var catUl = $("<ul class='fl'>");
                catDiv.append(header);
                catDiv.append(catUl);

                for (var key in rights[category]) {

                    tempIdCount += 1;
                    var checkBoxId = "rightCheck" + tempIdCount;

                    var li = $("<li>");
                    var checkBox = $("<input type=\"checkbox\" />");
                    checkBox.attr("id", checkBoxId);
                    if (role.toLowerCase() === "administrators") {
                        checkBox.click(function() {
                            if (!$(this).is(":checked")) {
                                alert("Rights cannot be removed from the Administrators role.");
                                return false;
                            }
                        });
                    }

                    if (rights[category][key] === true) {
                        checkBox.attr("checked", "checked");
                    }

                    li.append(checkBox);
                    li.append($("<label>").attr("for", checkBoxId).text(key));

                    catUl.append(li);

                    rightsControls[key] = {
                        li : li,
                        checkBox : checkBox
                    };
                }

                $("#rightsHolder").append(catDiv);
            }
        });

        function SaveRights() {

            var rightsToSave = {};
            for (var category in rights) {
                for (var key in rights[category]) {
                    if (rightsControls[key].checkBox.attr("checked") === true) {
                        rightsToSave[key] = true;
                    }
                }
            }

            var dto = { 
                roleName: role,
                rightsCollection: rightsToSave
            };

            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "api/RoleService.asmx/SaveRights",
                data: JSON.stringify(dto),
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: onAjaxBeforeSend,
                success: function (result) {
                    var rt = result.d;
                    if(rt.Success) {
                        ShowStatus("success", rt.Message);
                    }
                    else {
                        ShowStatus("warning", rt.Message);
                    }
                }
            });

            return false;
        }

        function setRoleRights(roleName, link) {
            return getAndLoadRoleRights(roleName, "GetRoleRights", $(link).html());
        }

        function setDefaultRoleRights(roleName, link) {
            return getAndLoadRoleRights(roleName, "GetDefaultRoleRights", $(link).html());
        }

        function getAndLoadRoleRights(roleName, serviceName, roleDescription) {

            if (!roleName) {
                ShowStatus('warning', 'Missing role name to retrieve rights for.');
                return false;
            }

            $.ajax({
                url: SiteVars.ApplicationRelativeWebRoot + "api/RoleService.asmx/" + serviceName,
                data: JSON.stringify({ roleName: roleName }),
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    var rt = result.d;
                    if(rt.Success) {
                        if (!rt.Data) {
                            ShowStatus('warning', 'There are no rights defined for the "' + roleDescription + '".');
                            return false;
                        }
                        var defaultRights = rt.Data.split('|');

                        for (var category in rights) {
                            for (var key in rights[category]) {
                                if ($.inArray(key, defaultRights) !== -1) {
                                    rightsControls[key].checkBox.attr('checked', 'checked');
                                } else {
                                    rightsControls[key].checkBox.removeAttr('checked');
                                }
                            }
                        }

                        ShowStatus("success", 'Checkboxes adjusted to match the rights for the "<b>' + roleDescription + '</b>".  Changes have not been saved.');
                    }
                    else {
                        ShowStatus("warning", rt.Message);
                    }
                }
            });

            return false;
        }
    </script>
    <script src="../jquery.masonry.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#rightsHolder').masonry({ singleMode: true, itemSelector: '.dashboardWidget' });
            $("a.toolsAction").click(function () { return false; });
            if (role.toLowerCase() === "administrators") {
                $("ul.assignDefaultRoles").empty();
            }
        });
        
    </script>

    <div class="content-box-outer">
        <div class="content-box-right">
            <ul>
                <li><a href="Users.aspx"><%=Resources.labels.users %></a></li>
                <li class="content-box-selected"><a href="Roles.aspx" class="selected"><%=Resources.labels.roles %></a></li>
            </ul>
        </div>
        <div class="content-box-left">
            
            <div class="topRightTools">
                <ul class="rowTools">
                    <li>
                        <a href="#" class="toolsAction"><span class=""><%=Resources.labels.copyRightsFrom %></span></a>
                        <ul class="rowToolsMenu assignDefaultRoles">
                            <%= RolesForLoading %>
                            <li style="border-top:1px solid #ccc;"><a href="#" onclick="return setDefaultRoleRights('Anonymous',this)"><%=Resources.labels.defaultAnonymousRole %></a></li>
                            <li><a href="#" onclick="return setDefaultRoleRights('Editors',this)"><%=Resources.labels.defaultEditorsRole %></a></li>
                        </ul>
                    </li>
                </ul>
            </div>

            <h1><%=Resources.labels.editingRightsForRole %> <%=Server.HtmlEncode(RoleName) %></h1>
            <div id="rightsHolder"></div>
            <div style="clear:both">&nbsp;</div>
            <input type="submit" class="btn primary rounded" value="<%=Resources.labels.save %>" onclick="return SaveRights();" />
            <%=Resources.labels.or %> <a href="Roles.aspx"><%=Resources.labels.cancel %></a>
        </div>
    </div>
</asp:Content>
