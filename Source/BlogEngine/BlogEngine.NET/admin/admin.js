
$(document).ready(function () {
   $('.editButton').live("click", function () { return EditRow(this); });
   $('.deleteButton').live("click", function () { return DeleteRow(this); });
   $('.loader').hide();
});

//-------------		EDITING


function EditRow(obj) {
    var row = $(obj).closest("tr");
    var revert = $(row).html();
    
    if (revert != null) {        
        var button = '<td><input type="button" value="Save" class="saveButton btn" /> <a href="#" class="cancelButton">Cancel</a></td>';

        $('.editable', row).each(function () {
            var _this = $(this);
            var _thisHtml = _this.html();
            var txt = '<td><input id="' + _thisHtml + '" type=\"text\" class=\"txt200\" value=\"' + _thisHtml + '"/></td>';
            _this.after(txt).remove();
        });

        // replace tools menu with save/cancel buttons
        $(row).find('.rowTools').closest("td").replaceWith(button);

        var cancelButton = $('.cancelButton');
        var saveButton = $('.saveButton');

        cancelButton.unbind('click');
        saveButton.unbind('click');

        cancelButton.bind("click", function () { return CancelChanges(this, revert); });
        saveButton.bind("click", function () { return SaveChanges(this, revert); });
    }
   return false;
}

function SaveChanges(obj, str) {

   var jQobj = $(obj);
   var row = jQobj.closest("tr");
   var id = row.data("recordId");
   var srv = jQobj.closest("table").attr("id");
   var editVals = [];
   var bg = ((row.prevAll().length + 1) % 2 === 0) ? 'fefefe' : 'fff';

   $(':text', row).each(function () {
      editVals.push($(this).val());
   });

   var dto = { "id": id, "bg": bg, "vals": editVals };
   $.ajax({
       type: "post",
       contentType: "application/json; charset=utf-8",
       dataType: "json",
       url: SiteVars.ApplicationRelativeWebRoot + "api/" + srv + ".asmx/Edit",
       data: JSON.stringify(dto),
       beforeSend: onAjaxBeforeSend,
       success: function (result) {
           var rt = result.d;
           if (rt.Success) {
               $(obj).parent().parent().parent().after(rt.Data).remove();
               // the admin page may define a OnAdminDataSaved() function.  if so,
               // call that function so data loading takes place.
               if (typeof OnAdminDataSaved !== 'undefined') {
                   OnAdminDataSaved();
               }
               ShowStatus("success", rt.Message);
           }
           else {
               ShowStatus("warning", rt.Message);
           }
       }
   });

   return false;
}

function CancelChanges(obj, str) {
   $(obj).closest("tr").html(str);
   return false;
}

function DeleteRow(obj) {
   var row = $(obj).closest("tr");
   var id = row.data("recordId");
   var srv = $(obj).closest("table").attr("id");
   var dto = { "id": id };
   var toolsId = row.find(".rowToolsMenu").attr('id');

   $.ajax({
       url: SiteVars.ApplicationRelativeWebRoot + "api/" + srv + ".asmx/Delete",
       data: JSON.stringify(dto),
       type: "POST",
       contentType: "application/json; charset=utf-8",
       dataType: "json",
       beforeSend: onAjaxBeforeSend,
       success: function (result) {
           var rt = result.d;
           if (rt.Success) {
               if (toolsId === 'users') {
                   LoadUsers($.cookie('GenericCurrentPage'));
               }
               else {
                   row.fadeOut(500, function () {
                       var tbody = row.closest('tbody');
                       row.remove();
                       $('tr:odd', tbody).addClass('alt');
                       $('tr:even', tbody).removeClass('alt');
                   });
               }
               ShowStatus("success", rt.Message);
           }
           else {
               ShowStatus("warning", rt.Message);
           }
       }
   });
   return false;
}

//--------------	LOAD DATA VIEWS

function LoadComments(page) {
    var pg = 1;
    if (page > 0) {
        pg = page;
    } else {
        // page is 0 - we just deleted/approved etc. comment
        // don't know what page we are on, just need to reload
        pg = $.cookie('CommentPagerCurrentPage');
    }
    $.cookie('CommentPagerCurrentPage', pg, { expires: 7 });
    var srvs = CommentPage();
    $.ajax({
      url: srvs + "/LoadComments",
      data: "{'page':'" + pg + "'}",
      type: "POST",
      contentType: "application/json",
      beforeSend: onAjaxBeforeSend,
      success: function (msg) {
         $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/comments.htm', null, { filter_data: false });
         $('#Container').processTemplate(msg);
         LoadPager();
      }
   });
   return false;
}

function LoadPager() {
    var srvs = CommentPage();
    var pg = $.cookie('CommentPagerCurrentPage');
   $.ajax({
      url: srvs + "/LoadPager",
      data: "{'page':'" + pg + "'}",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      beforeSend: onAjaxBeforeSend,
      success: function (msg) {
         $('.Pager').html(msg.d);
      }
   });
   return false;
}

function CommentPage() {
    var page = SiteVars.ApplicationRelativeWebRoot + 'admin/Comments/Approved.aspx';
    if (location.href.indexOf('Comments\/Spam.aspx') > 0) { page = SiteVars.ApplicationRelativeWebRoot + 'admin/Comments/Spam.aspx'; }
    if (location.href.indexOf('Comments\/Pending.aspx') > 0) { page = SiteVars.ApplicationRelativeWebRoot + 'admin/Comments/Pending.aspx'; }
    if (location.href.indexOf('Tracking\/Pingbacks.aspx') > 0) { page = SiteVars.ApplicationRelativeWebRoot + 'admin/Tracking/Pingbacks.aspx'; }
    return page;
}

function SaveOriginalIdValues(containerSelector, recordIdSelector) {
    $(containerSelector).each(function () {
        var val = $(recordIdSelector, $(this)).html();
        $(this).data("recordId", val);
    });
}

function LoadRoles() {
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/Users/Roles.aspx/GetRoles",
        data: "{ }",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/roles.htm', null, { filter_data: false });
            $('#Container').processTemplate(msg);
            SaveOriginalIdValues('#Container tr', '.editable');
        }
    });
}

function LoadUsers(page) {
   var dto = { "page": page };
   $.ajax({
       url: SiteVars.ApplicationRelativeWebRoot + "admin/Users/Users.aspx/GetUsers",
       data: JSON.stringify(dto),
       type: "POST",
       contentType: "application/json; charset=utf-8",
       dataType: "json",
       beforeSend: onAjaxBeforeSend,
       success: function (msg) {
           $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/users.htm', null, { filter_data: false });
           $('#Container').processTemplate(msg);
           SaveOriginalIdValues('#Container tr', '.username');
           LoadGenericPager(page, 'admin/Users/Users.aspx');
           $.cookie('GenericCurrentPage', page, { expires: 7 });
       }
   });
}

function LoadGenericPager(page, url) {
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + url + "/LoadPager",
        data: "{'page':'" + page + "'}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('.Pager').html(msg.d);
        }
    });
    return false;
}

function LoadProfile() {
   var dto = { "id": Querystring('id') };
   $.ajax({
      url: SiteVars.ApplicationRelativeWebRoot + "admin/Users/Profile.aspx/GetProfile",
      data: JSON.stringify(dto),
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      beforeSend: onAjaxBeforeSend,
      success: function (msg) {
         $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/profile.htm', null, { filter_data: false });
         $('#Container').processTemplate(msg);

         $('#Container2').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/profile2.htm', null, { filter_data: false });
         $('#Container2').processTemplate(msg);
      }
   });
}

function LoadCustomFilters() {
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/Settings/Rules.aspx/GetCustomFilters",
        data: { },
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/customfilters.htm', null, { filter_data: false });
            $('#Container').processTemplate(msg);
        }
    });
}

//--------------    TRASH

function LoadTrash(obj, page) {
    $('.loader').show();
    var type = "All";

    if (obj != null) {
        $(".tableToolBox a").removeClass("current");
        $(obj).addClass("current");
        type = $(obj).attr("id");
    }
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/Trash.aspx/LoadTrash",
        data: "{'trashType':'" + type + "', 'page':" + page + "}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/trash.htm', null, { filter_data: false });
            $('#Container').processTemplate(msg);
            LoadTrashPager(page);
        }
    });
    return false;
}

function LoadTrashPager(page) {
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/Trash.aspx/LoadPager",
        data: "{'page':'" + page + "'}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('.Pager').html(msg.d);
        }
    });
    return false;
}

function ProcessTrash(action, scope) {
    $('#AjaxLoader').addClass('loader');

    var page = $('#current-page').html();
    var vals = [];   
    if (scope == 'Selected') {
        var checked = $('#TrashTable input[@type=checkbox]:checked');
        if (checked.length > 0) {
            checked.each(function () {
                var jThis = $(this);
                if (jThis.attr("id") != "selectall") {
                    var row = jThis.closest("tr");
                    var id = row.attr("id");
                    vals.push(id);
                }
            });
        }
    }
    else if (scope == 'All') {
        vals.push("All:All");
    }
    else {
        vals.push(scope);
    }

    var dto = { "action": action, "vals": vals };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/Trash.aspx/ProcessTrash",
        data: JSON.stringify(dto),
        type: "post",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            var rt = result.d;
            if (rt.Success) {
                LoadTrash(null, page);
                ShowStatus("success", rt.Message);
            }
            else {
                ShowStatus("warning", rt.Message);
            }
        }
    });
    //$('#AjaxLoader').removeClass('loader');
    return false;
}

//--------------    COMMENTS

var rowLoading = '<td colspan="8" style="text-align:center"><img src="../../pics/ajax-loader.gif" width="24" height="24" alt="Loading" /></td>';

function ProcessSelected(action, page) {
   var vals = [];

   // store the rows so they don't need to be queried for again.
   var commentsAndRows = [];
   // action: approve, reject, restore or delete
   var srv = SiteVars.ApplicationRelativeWebRoot + "api/Comments.asmx/" + action;

   // Gets all checkboxes inside the #Comments table to prevent selecting
   // checkboxes that aren't part of the comments list.
   var checkedComments = $('#Comments input[@type=checkbox]:checked');

   if(checkedComments.length > 0) {

      checkedComments.each(function () {
         var jThis = $(this);

         // Check for the selectall checkbox otherwise this will throw an error.
         if(jThis.attr("id") != "selectall") {

            var row = jThis.closest("tr");
            var id = row.attr("id");
            commentsAndRows.push({
               row: row,
               id: id
            });

            vals.push(id);
         }
      });

      if (vals.length > 0) {
          $('.loader').show();

          var dto = { "vals": vals };
          $.ajax({
              url: srv,
              data: JSON.stringify(dto),
              type: "post",
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              beforeSend: onAjaxBeforeSend,
              success: function (result) {

                  var rt = result.d;
                  if (rt.Success) {

                      // Reference the counters so they don't need to be requeried
                      // by each checkbox.
                      var comment_counter = $('#comment_counter');
                      var spam_counter = $('#spam_counter');
                      var pingback_counter = $('#pingback_counter');
                      var pending_counter = $('#pending_counter');


                      // parse the current counts
                      // Change these values before setting it to the element's text.
                      var com_cnt = parseInt(comment_counter.text(), 10);
                      var spm_cnt = parseInt(spam_counter.text(), 10);
                      var pbk_cnt = parseInt(pingback_counter.text(), 10);
                      var pnd_cnt = parseInt(pending_counter.text(), 10);


                      $.each(commentsAndRows, function (index, value) {

                          var row = value.row;
                          row.fadeOut(500, function () {
                              row.remove();
                          });

                          switch (action) {
                              case "Reject":
                                  spm_cnt += 1;

                                  switch (page) {
                                      case "Approved": (com_cnt -= 1); break;
                                      case "Pending": pending_counter.text((pnd_cnt - 1)); break;
                                  }
                                  break;

                              case "Approve":
                                  com_cnt += 1;

                                  switch (page) {
                                      case "Pending": (pnd_cnt -= 1); break;
                                      case "Spam": (spm_cnt -= 1); break;
                                  }
                                  break;

                              case "Delete":
                                  $("#recyclebin a").removeClass("empty");
                                  $("#recyclebin a").addClass("full");

                                  switch (page) {
                                      case "Approved": (com_cnt -= 1); break;
                                      case "Spam": (spm_cnt -= 1); break;
                                      case "Pingback": (pbk_cnt -= 1); break;
                                      case "Pending": (pnd_cnt -= 1); break;
                                  }
                                  break;

                              default:
                                  throw new Error("Unknown action: " + action);
                          }
                      });

                      spam_counter.text(spm_cnt);
                      comment_counter.text(com_cnt);
                      pending_counter.text(pnd_cnt);
                      pingback_counter.text(pbk_cnt);

                      LoadComments(0);

                      ShowStatus("success", "Updated");
                  }
                  else {
                      ShowStatus("warning", rt.Message);
                  }

                  $('.loader').hide();

              }
          });
      }

   }

   return false;
}

var editingComment = '';

function EditComment(id) {
   var oRow = $("[id$='" + id + "']");
   var hRow = oRow.html();
   editingComment = hRow;

   var dto = { "id": id };
   $.ajax({
      url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/GetComment",
      data: JSON.stringify(dto),
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      beforeSend: onAjaxBeforeSend,
      success: function (result) {
         oRow.setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/editcomment.htm', null, { filter_data: false });
         oRow.processTemplate(result);
      }
   });
   return false;

}

function SaveComment(obj) {
   var frm = document.forms.aspnetForm;
   $(frm).validate({
      rules: {
         txtAuthor: {
            required: true,
            maxlength: 30
         },
         txtComment: {
            required: true,
            maxlength: 2000
         },
         txtEmail: {
            required: true,
            email: true
         }
      }
   });

   var isValid = $(frm).valid();
   if(!isValid) { return false; }

   var oRow = $(obj).closest("tr");
   var vals = [];

   vals[0] = $(obj).closest("tr").attr("id");
   vals[1] = $("#txtAuthor").val();
   vals[2] = $("#txtEmail").val();
   vals[3] = $("#txtWebsite").val();
   vals[4] = $("#txtComment").val();

   var dto = { "vals": vals };
   $.ajax({
      url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/SaveComment",
      data: JSON.stringify(dto),
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      beforeSend: onAjaxBeforeSend,
      success: function (result) {
         oRow.setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/commentrow.htm', null, { filter_data: false });
         oRow.processTemplate(result);
         ShowStatus("success", "Updated");
      }
   });

   return false;
}

function CancelEditComment(obj) {
    var oRow = $(obj).closest("tr");
    $(oRow).html(editingComment);
}

function CommentAction(act, id) {
   var oRow = $("[id$='" + id + "']");
   var hRow = oRow.html();

   var rowLoader = '<td colspan="8" style="text-align:center"><img src="../../pics/ajax-loader.gif" width="24" height="24" alt="Loading" /></td>';
   oRow.html(rowLoader);

   var vals = [];
   vals[0] = id;
   var dto = { "vals": vals };
   $.ajax({
       url: SiteVars.ApplicationRelativeWebRoot + "api/Comments.asmx/" + act,
       data: JSON.stringify(dto),
       type: "POST",
       contentType: "application/json; charset=utf-8",
       dataType: "json",
       beforeSend: onAjaxBeforeSend,
       success: function (result) {
           var rt = result.d;
           if (rt.Success) {
               var com_cnt = $('#comment_counter').text();
               var spm_cnt = $('#spam_counter').text();
               var pbk_cnt = $('#pingback_counter').text();
               var pnd_cnt = $('#pending_counter').text();

               if (act == "Delete") {
                   if (location.href.indexOf('Comments\/Approved.aspx') > 0) { $('#comment_counter').text(parseInt(com_cnt, 10) - 1); }
                   if (location.href.indexOf('Comments\/Spam.aspx') > 0) { $('#spam_counter').text(parseInt(spm_cnt, 10) - 1); }
                   if (location.href.indexOf('Comments\/Pending.aspx') > 0) { $('#pending_counter').text(parseInt(pnd_cnt, 10) - 1); }
               }
               if (act == "Approve") {
                   $('#comment_counter').text(parseInt(com_cnt, 10) + 1);
                   // can approve from pending or spam
                   if (location.href.indexOf('Comments\/Pending.aspx') > 0)
                       $('#pending_counter').text(parseInt(pnd_cnt, 10) - 1);
                   else
                       $('#spam_counter').text(parseInt(spm_cnt, 10) - 1);
               }
               if (act == "Reject") {
                   $('#spam_counter').text(parseInt(spm_cnt, 10) + 1);

                   // can reject from pending or spam
                   if (location.href.indexOf('Comments\/Pending.aspx') > 0)
                       $('#pending_counter').text(parseInt(pnd_cnt, 10) - 1);
                   else
                       $('#comment_counter').text(parseInt(com_cnt, 10) - 1);
               }

               $(oRow).fadeOut(500, function () {
                   $(oRow).remove();
               });
               LoadComments(0);
               ShowStatus("success", rt.Message);
           }
           else {
               oRow.html(hRow);
               ShowStatus("warning", rt.Message);
           }
       }
   });
   return false;
}

function DeleteAllSpam() {
   $('.loader').show();
   $.ajax({
       url: SiteVars.ApplicationRelativeWebRoot + "api/Comments.asmx/DeleteAll",
       data: "{ }",
       type: "POST",
       contentType: "application/json; charset=utf-8",
       dataType: "json",
       beforeSend: onAjaxBeforeSend,
       success: function (result) {
           var rt = result.d;
           if (rt.Success) {
               $('.chk').each(function () {
                   var that = $("[id$='" + $(this).closest("tr").attr("id") + "']");
                   $(that).fadeOut(500, function () {
                       $(that).remove();
                   });
                   LoadComments(0);
                   ShowStatus("success", rt.Message);
                   $('#spam_counter').text('0');
               });
           }
           else {
               ShowStatus("warning", rt.Message);
           }
       }
   });
   $('.loader').hide();
   return false;

}

//--------------  POSTS AND PAGES

function ChangePostPageSize(select) {
    var pageSize = $(select).val();
    $.cookie('postPageSize', pageSize, { expires: 7 });
    $.cookie('postCurrentPage', 1, { expires: 7 });
    LoadPosts();
    return false;
}

// All, Draft or Published
function ChangePostFilterType(type) {
    $.cookie('postMainFilter', type, { expires: 7 });
    $.cookie('postSecondaryFilter', null, { expires: 7 });
    $.cookie('postSecondaryFilterId', null, { expires: 7 });
    $.cookie('postSecondaryFilterTitle', null, { expires: 7 });
    $.cookie('postCurrentPage', 1, { expires: 7 });
    LoadPosts();
    return false;
}
// Category, Tag or Author
function ChangePostFilter(filter, id, title) {
    $.cookie('postSecondaryFilter', filter, { expires: 7 });
    $.cookie('postSecondaryFilterId', id, { expires: 7 });
    $.cookie('postSecondaryFilterTitle', title, { expires: 7 });
    $.cookie('postCurrentPage', 1, { expires: 7 });
    LoadPosts();
}

function LoadPostsForPage(page) {
    if(page == null || page == 0)
        page = 1;

    $.cookie('postCurrentPage', page, { expires: 7 });
    LoadPosts();
    return false;
}

function LoadPosts() {
    if ($.cookie('postMainFilter') == null) {
        $.cookie('postMainFilter', 'All', { expires: 7 });
    }
    if ($.cookie('postSecondaryFilter') == null) {
        $.cookie('postSecondaryFilter', 'All', { expires: 7 });
    }
    if ($.cookie('postSecondaryFilterId') == null) {
        $.cookie('postSecondaryFilterId', '', { expires: 7 });
    }
    if ($.cookie('postSecondaryFilterTitle') == null) {
        $.cookie('postSecondaryFilterTitle', '', { expires: 7 });
    }
    if ($.cookie('postCurrentPage') == null) {
        $.cookie('postCurrentPage', 1, { expires: 7 });
    }
    if ($.cookie('postPageSize') == null) {
        $.cookie('postPageSize', $("#pageSizeTop").val(), { expires: 7 });
    }
    var ftr1 = $.cookie('postMainFilter');
    var ftr2 = $.cookie('postSecondaryFilter');
    var ftr2id = $.cookie('postSecondaryFilterId');
    var ftr2title = $.cookie('postSecondaryFilterTitle');
    var pg = $.cookie('postCurrentPage');
    var pageSize = $.cookie('postPageSize');

    // sync both dropdown lists.
    $("#pageSizeTop").val(pageSize);
    $("#pageSizeBottom").val(pageSize);

    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/LoadPosts",
        data: "{'page':'" + pg + "' , 'type':'" + ftr1 + "', 'filter':'" + ftr2 + "', 'title': '" + ftr2id + "', pageSize: '" + pageSize + "'}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/posts.htm', null, { filter_data: false });
            $('#Container').processTemplate(msg);

            LoadPostsPager(pg, pageSize, ftr1);

            var prefx = ftr1 + ' posts';
            if (ftr2 == "Category") {
                $("#filteredby").html(prefx + " in " + ftr2title + " category").show();
                //$(".tableToolBox a").removeClass("current");
            }
            else if (ftr2 == "Tag") {
                $("#filteredby").html(prefx + " tagged with " + ftr2id).show();
                //$(".tableToolBox a").removeClass("current");
            }
            else if (ftr2 == "Author") {
                $("#filteredby").html(prefx + " by author " + ftr2id).show();
                //$(".tableToolBox a").removeClass("current");
            }
            else
                $("#filteredby").hide();

            $(".tipsyhelp").tipsy({gravity: 's'});

        }
    });
    return false;
}

function LoadPages(type) {
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/LoadPages",
        data: "{'type':'" + type + "'}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/pages.htm', null, { filter_data: false });
            $('#Container').processTemplate(msg);
        }
    });
    return false;
}

function LoadPostsPager(pg, pageSize, type) {
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/LoadPostPager",
        data: "{'pageSize':'" + pageSize + "', 'page':'" + pg + "' , 'type':'" + type + "'}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('.Pager').html(msg.d);
        }
    });
    return false;
}

function DeletePost(obj) {
    var id = $(obj).closest("tr").attr("id");
    var srv = $(obj).closest("table").attr("id");
    var that = $("[id$='" + id + "']");
    var dto = { "id": id };
    var currPage = $('#PagerCurrentPage').html();

    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "Api/Posts.asmx/DeletePost",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            var rt = result.d;
            if (rt.Success) {
                $(that).fadeOut(500, function () {
                    $(that).remove();
                });
                ShowStatus("success", rt.Message);
                LoadPosts();
            }
            else {
                ShowStatus("warning", rt.Message);
            }
        }
    });
    return false;
}

function DeletePage(obj) {
    var id = $(obj).closest("tr").attr("id");
    var srv = $(obj).closest("table").attr("id");
    var that = $("[id$='" + id + "']");
    var dto = { "id": id };

    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "Api/Posts.asmx/DeletePage",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            var rt = result.d;
            if (rt.Success) {
                $(that).fadeOut(500, function () {
                    $(that).remove();
                });
                ShowStatus("success", rt.Message);
                LoadPages('All');
            }
            else {
                ShowStatus("warning", rt.Message);
            }
        }
    });
    return false;
}

function ChangePriority() {
    var dto = { "priority": $('#txtPriority').val(), "ext": $('#hdnExtensionName').val() };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/ChangePriority",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            var rt = result.d;
            if (rt == true) {
                window.location.reload();
            }
            else {
                closeOverlay();
                ShowStatus("warning", "Error changing priority");
            }
        }
    });
    return false;
}


//--------------  BLOGS

function ChangeBlogsPage(select) {
    var pageSize = $(select).val();
    $.cookie('blogsPageSize', pageSize, { expires: 7 });
    $.cookie('blogsCurrentPage', 1, { expires: 7 });
    LoadBlogs();
    return false;
}

function LoadBlogsForPage(page) {
    if (page == null || page == 0)
        page = 1;

    $.cookie('blogsCurrentPage', page, { expires: 7 });
    LoadBlogs();
    return false;
}

function LoadBlogs() {
    if ($.cookie('blogsCurrentPage') == null) {
        $.cookie('blogsCurrentPage', 1, { expires: 7 });
    }
    if ($.cookie('blogsPageSize') == null) {
        $.cookie('blogsPageSize', $("#pageSizeTop").val(), { expires: 7 });
    }
    var pg = $.cookie('blogsCurrentPage');
    var pageSize = $.cookie('blogsPageSize');

    // sync both dropdown lists.
    $("#pageSizeTop").val(pageSize);
    $("#pageSizeBottom").val(pageSize);

    // remove any select options from the dropdown list.
    $('#existingBlogToCreateNewBlogFrom').empty();

    var data = { page: pg, pageSize: pageSize };

    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/LoadBlogs",
        data: JSON.stringify(data),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/blogs.htm?v=2', null, { filter_data: false });
            $('#Container').processTemplate(msg);

            LoadBlogsPager(pg, pageSize);

            $(".tipsyhelp").tipsy({ gravity: 's' });

        }
    });
    return false;
}

function GetBlog(blogId, callback) {

    if (!blogId || blogId.length !== 36) {
        ShowStatus('warning', 'Invalid Blog ID.');
        return false;
    }

    var data = { blogId: blogId };

    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/GetBlog",
        data: JSON.stringify(data),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            callback(result.d);
        }
    });
    return false;
}

function LoadBlogsPager(pg, pageSize, type) {
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/LoadBlogsPager",
        data: "{'pageSize':'" + pageSize + "', 'page':'" + pg + "' , 'type':'" + type + "'}",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('.Pager').html(msg.d);
        }
    });
    return false;
}

function DeleteBlog(obj) {
    var id = $(obj).closest("tr").attr("id");
    var srv = $(obj).closest("table").attr("id");
    var that = $("[id$='" + id + "']");
    
    var deleteStorageContainer =
        confirm(
            'Also delete the storage container (files, tables, etc) for this blog?\n\n' +
            'Click OK to also delete the storage container.\n' +
            'Click Cancel to delete the blog without deleting the storage container.\n\n' +
            'Note, after you choose OK/Cancel, you will have an final opportunity to confirm ' +
            'the blog deletion.');

    if (!confirm("Final Confirmation: Delete the blog?")) { return false; }

    var dto = { "id": id, "deleteStorageContainer": deleteStorageContainer };

    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "Api/Blogs.asmx/DeleteBlog",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            var rt = result.d;
            if (rt.Success) {
                $(that).fadeOut(500, function () {
                    $(that).remove();
                });
                ShowStatus("success", rt.Message);
                LoadBlogs();
            }
            else {
                ShowStatus("warning", rt.Message);
            }
        }
    });
    return false;
}

//--------------GALLERY

function SetCurrentTheme(theme, mobile) {
    var dto = { "theme": theme, "mobile": mobile };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/SetCurrentTheme",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            var rt = result.d;
            if (rt == true) {
                window.location.reload();
            }
            else {
                ShowStatus("warning", "Error setting current theme");
            }
        }
    });
    return false;
}

function GalleryGetPackages(pg, pkgType) {
    $('.page-loader').show();
    var srt = $("#gallery-sort-order").val();
    var srch = $("#searchGallery").val();
    var dto = { "pkgType": pkgType, "page": pg, "sortOrder": srt, "searchVal": srch };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/LoadGalleryPage",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/packages.htm', null, { filter_data: false });
            $('#Container').processTemplate(msg);
            GalleryGetPager();
            $('.page-loader').hide();
        }
    });
    return false;
}

function GalleryGetPager() {
    var dto = {};
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/LoadGalleryPager",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (msg) {
            $('#Pager').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/pager.htm', null, { filter_data: false });
            $('#Pager').processTemplate(msg);
        }
    });
    return false;
}

function InstallPackage(pkgId) {
    var dto = { "pkgId": pkgId };
    var spinner = '<img class="loader2" src="../../pics/ajax-loader.gif" width="24" height="24" alt="loading..." />';
    var p = $("[id$='p_" + pkgId + "']");
    var backup = p.html();

    p.html(spinner);
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/InstallPackage",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            var rt = result.d;
            if (rt.Success) {
                p.removeClass("package-update");
                p.addClass("package-installed");
                p.closest("li").addClass("pkg-installed");
                p.html('Installed');
                ShowStatus("success", rt.Message);
            }
            else {
                p.html(backup);
                ShowStatus("warning", rt.Message);
            }
        }
    });
    return false;
}

function UpdatePackage(pkgId, location) {
    var spinner = '<img class="loader2" style="margin-left: 0" src="../../pics/ajax-loader.gif" width="24" height="24" alt="loader" />';
    var p = $("[id$='upd-" + pkgId + "']");
    var backup = p.html();
    p.css('background','none');
    p.html(spinner);

    var dto = { "pkgId": pkgId };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/InstallPackage",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            var rt = result.d;
            if (rt.Success) {
                if (location == 'G') {
                    p.css('background', 'url("../images/action-enable.png") no-repeat scroll left center transparent');
                    p.html('Installed');
                    p.removeClass("package-update");
                    p.addClass("package-installed");
                }
                else {
                    $(p).fadeOut(500, function () {
                        $(p).remove();
                    });
                }
                ShowStatus("success", rt.Message);
            }
            else {
                p.css('background', 'url("../images/action-enable.png") no-repeat scroll left center transparent');
                p.html(packup);
                ShowStatus("warning", rt.Message);
            }
        }
    });
    return false;
}

function UninstallPackage(pkgId) {
    var spinner = '<img class="loader2" style="margin-left: 0" src="../../pics/ajax-loader.gif" width="24" height="24" alt="loader" />';
    var p = $("[id$='del-" + pkgId + "']");
    var li = p.closest("li");
    var backup = p.html();
    p.html(spinner);

    var dto = { "pkgId": pkgId };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/UninstallPackage",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            var rt = result.d;
            if (rt.Success) {
                $(li).fadeOut(500, function () {
                    $(li).remove();
                });
                ShowStatus("success", rt.Message);
            }
            else {
                p.html(backup);
                ShowStatus("warning", rt.Message);
            }
        }
    });
    return false;
}

// extensions are special case
function UninstallExtension(pkgId) {
    var spinner = '<img class="loader2" style="margin-left: 70px" src="../../pics/ajax-loader.gif" width="24" height="24" alt="loader" />';
    var row = $("[id$='ext_" + pkgId + "']");
    var backup = row.html();
    row.html(spinner);
    
    var dto = { "pkgId": pkgId };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/AjaxHelper.aspx/UninstallPackage",
        data: JSON.stringify(dto),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onAjaxBeforeSend,
        success: function (result) {
            var rt = result.d;
            if (rt.Success) {
                $(row).fadeOut(500, function () {
                    $(row).remove();
                });
                ShowStatus("success", rt.Message);
            }
            else {
                row.html(backup);
                ShowStatus("warning", rt.Message);
            }
        }
    });
    return false;
}


//--------------HELPERS AND MISC

function onAjaxBeforeSend(jqXHR, settings) {

    // AJAX calls need to be made directly to the real physical location of the
    // web service/page method.  For this, SiteVars.ApplicationRelativeWebRoot is used.
    // If an AJAX call is made to a virtual URL (for a blog instance), although
    // the URL rewriter will rewrite these URLs, we end up with a "405 Method Not Allowed"
    // error by the web service.  Here we set a request header so the call to the server
    // is done under the correct blog instance ID.

    jqXHR.setRequestHeader('x-blog-instance', SiteVars.BlogInstanceId);
}

function colorboxDialogSubmitClicked(validationGroup, panelId) {

    // For file/image uploads, colorbox moves the file upload and submit buttons
    // outside the form tag.  This prevents submitting from working.  Before
    // a submit can work, need to move the dialog box containing the controls
    // back inside the form tag.
    // First check to make sure validation passes before closing colorbox.

    if (typeof Page_ClientValidate !== 'undefined') {
        if (!Page_ClientValidate(validationGroup)) {
            return true;
        }
    }

    $.colorbox.close();
    $("form").append($("#" + panelId));
    return true;
}

function toggleAllChecks(o) {
   if($(o).attr('checked')) {
      $('.chk').not(':disabled').attr('checked', 'checked');
   }
   else {
      $('.chk').attr('checked', '');
   }
   return false;
}

function formatJSONDate(jsonDate) {
   var d = new Date(parseInt(jsonDate.substr(6), 10));
   var nullDate = new Date(1001, 0, 1);

   if(d.getTime() <= nullDate.getTime()) {
      return "";
   }
   else {
      var m = d.getMonth() + 1;
      var s = m + "/" + d.getDate() + "/" + d.getFullYear();
      return s;
   }
}


function Querystring(key) {
   key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
   var regex = new RegExp("[\\?&]" + key + "=([^&#]*)");
   var qs = regex.exec(window.location.href);
   if(qs === null) {
      return "";
   }
   else {
      return decodeURIComponent(qs[1]);
   }
}

//--------------	STATUS AND MESSAGES

function ShowStatus(status, msg) {
   var adminStatus = $("[id$='AdminStatus']");
   adminStatus.removeClass("warning");
   adminStatus.removeClass("success");
   adminStatus.addClass(status);

   adminStatus.html('<a href="javascript:HideStatus()" class="closeStatus">close</a>' + msg);

   if(status == "success") {
       adminStatus.fadeIn(1000);
       window.setTimeout(function () {
           $("[id$='AdminStatus']").fadeOut(1000);
       }, 5000);
   }
   else {
      adminStatus.fadeIn(1000, function () { });
   }
}

function HideStatus() {
   $("[id$='AdminStatus']").slideUp('normal', function () { });
}

function Show(element) {
   $("[id$='" + element + "']").slideDown('slow', function () { });
   return false;
}

function Hide(element) {
   $("[id$='" + element + "']").slideUp('slow', function () { });
   return false;
}

function Toggle(element) {
    if ($("[id$='" + element + "']").is(':visible'))
        $("[id$='" + element + "']").slideUp('slow', function () { });
    else
        $("[id$='" + element + "']").slideDown('slow', function () { });
    return false;
}
