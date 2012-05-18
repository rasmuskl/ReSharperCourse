var fmdOpen = false;
var iExts = ['.png', '.jpg', '.jpeg', '.gif', '.bmp'];
String.prototype.FileExtension = function () {
    return this.substring(this.lastIndexOf('.')).toLowerCase();
}
String.prototype.IsImage = function () {
    return $.inArray(String(this), iExts) != -1;
}

$(document).ready(function () {

    $(document).click(function (event) {
        if (fmdOpen) {
            var p = $(event.target);
            if (p.attr('id') == 'fmD') return;
            var cID = p.attr('id');
            while (cID != 'fmD' && cID !== undefined) {
                p = p.parent();
                cID = $(p).attr('id');
            }
            if (cID === undefined) {
                fmdOpen = false;
                $('#fmD').slideUp(150);
            }
        }
    });
    $('#fileManager').click(function (e) {
        e.stopPropagation();
        $('#fmD').slideToggle(300, function () {
            fmdOpen = $('#fmD').is(':visible');
        });
        $('.fmD-left ul li').removeClass('fmDActive');
        $('.fmD-right-content').hide();
        $('.fmD-left ul li:first').addClass('fmDActive');
        $('.fmD-right-content:eq(' + $('.fmD-left ul li:first').index() + ')').show();
        Files('');
    });
    $('.fmD-left ul li').click(function () {
        $('.fmD-left ul li').removeClass('fmDActive');
        $('.fmD-right-content').hide();
        $(this).addClass('fmDActive');
        $('.fmD-right-content:eq(' + $(this).index() + ')').show();
    });

    $('#fmD_upload_file').uploadify({
        'uploader': SiteVars.ApplicationRelativeWebRoot + 'admin/uploadify/uploadify.swf',
        'script': SiteVars.ApplicationRelativeWebRoot + 'admin/filemanager/FileUpload.ashx',
        'scriptData': { 'b': $('#bId').val() },
        'cancelImg': SiteVars.ApplicationRelativeWebRoot + 'admin/uploadify/cancel.png',
        'buttonImg': SiteVars.ApplicationRelativeWebRoot + 'admin/filemanager/images/uploadFMD.png',
        'fileDesc': 'All Files',
        'auto': true,
        onComplete: function (event, ID, fileObj, response, data) {
            var r = response.split(':');
            if (r[0] != '1') {
                ShowStatus("warning", r[1]);
                $('.fmD-upload-right-uploading').hide();
                $('.fmD-upload-right-wait').show();
                return false;
            }
            $('.fmD-upload-right-uploading').hide();
            $('.fmD-upload-right-wait').show();

            ShowStatus("success", 'File "' + r[2] + '" uploaded successfully.');
            Files(r[3]);
            if ($('#fmdUpload_Append').is(':checked')) {
                setMCE(r[1]);
                closefmD();
            }
            else if ($('#fmdUpload_AppendContinue').is(':checked')) {
                setMCE(r[1]);
            }
            else {
                $('.fmD-left ul li').removeClass('fmDActive');
                $('.fmD-right-content').hide();
                $('.fmD-left ul li:eq(1)').addClass('fmDActive');
                $('.fmD-right-content:eq(1)').show();
            }

        },
        onProgress: function (event, ID, fileObj, data) {
            $('#fmD_UploadProgress').progressbar("value", data.percentage);
            $('#' + $(event.target).attr('id') + ID).find('.percentage').text(' - ' + bytes + 'KB Uploaded');
            return false;
        },
        onOpen: function (event, ID, fileObj) {
            $('.fmD-upload-right-wait').hide();
            $('.fmD-upload-right-uploading').show();
            $('#fmD_UploadFile').text(fileObj.name);
            $('#fmD_UploadProgress').progressbar('destroy').progressbar({
                value: 0

            });
        }
    });
    $('tr.fmDRow td').live('dblclick', function () {
        var p = $(this).parent('tr');
        if (parseInt(p.attr('data-type')) == 0) {
            Files(p.attr('data-path'));
        }
    });
    $('tr.fmDRow td input[type=text]').live('click', function (e) {
        e.stopPropagation();
    });
    $('.fmdPathPiece').live('click', function () {
        Files($(this).attr('data-path'));

    });
    Files(' ');
    $(document).disableSelection();
});

function Files(Path) {
    hshow();
    var data = { path: Path };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/filemanager/AjaxFileManager.aspx/GetFiles",
        data: JSON.stringify(data),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (msg) {
            hhide();
            if (!msg.d.Success) {
                ShowStatus("warning", msg.d.Message);
                $('#Container').html('');
                return;
            }
            var a = {
                d: msg.d.Data
            }
            $('#Container').setTemplateURL(SiteVars.ApplicationRelativeWebRoot + 'Templates/FileManager.htm?v=2', null, { filter_data: false });
            $('#Container').processTemplate(a);
            bindContext();
        }
    });


    return false;
}

function setMCE(obj) {

    tinyMCE.activeEditor.execCommand("mceInsertContent", false, obj);

}
function closefmD() {
    $('#fmD').slideUp();
    fmdOpen = false;
}
function hhide() {
    $('#dwait').hide();
}
function hshow() {
    $('#dwait').show();
}

function appendFile(path) {
    hshow();
    var data = { path: path };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/filemanager/AjaxFileManager.aspx/AppendFile",
        data: JSON.stringify(data),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (msg) {
            hhide();
            if (!msg.d.Success) {
                ShowStatus("warning", msg.d.Message);
                return;
            }
            setMCE(msg.d.Data);
        }
    });
    return false;
}

function deleteFile(path) {
    if (!confirm('Are you sure you wish to delete the file "' + path + '"?\r\n\r\nThis operation cannot be undone.\r\n\r\nAny posts and pages that contain this file will contain broken links.'))
        return;
    hshow();
    var data = { path: path };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/filemanager/AjaxFileManager.aspx/DeleteFile",
        data: JSON.stringify(data),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (msg) {
            hhide();
            if (!msg.d.Success) {
                ShowStatus("warning", msg.d.Message);
                return;
            }
            Files(msg.d.Data);
            ShowStatus("success", msg.d.Message);
        }
    });
    return false;
}

function setupRename(obj) {
    obj.find('span:first').hide();
    obj.find('input[type=text]:first')
                .show()
                .focus()
                .keypress(function (event) {
                    if (event.which == 13) {
                        event.preventDefault();
                        $(this).blur();
                    }
                })
               .blur(function () {
                   bindContext();
                   $(this).parent('td').find('span:first').show();
                   $(this).hide();
                   var oldName = $(this).attr('data-name'),
                       newName = $(this).val();
                   if (oldName == newName)
                       return;
                   if (!confirm('Are you sure you wish to rename the file "' + oldName + '" to "' + newName + '"?\r\n\r\nAny posts and pages that contain this file will contain broken links.')) {
                       $(this).val($(this).attr('data-name'));
                       return;
                   }
                   else {
                       var data = { path: $(this).attr('data-path'), newname: newName };
                       $.ajax({
                           url: SiteVars.ApplicationRelativeWebRoot + "admin/filemanager/AjaxFileManager.aspx/RenameFile",
                           data: JSON.stringify(data),
                           type: "POST",
                           contentType: "application/json; charset=utf-8",
                           beforeSend: onAjaxBeforeSend,
                           dataType: "json",
                           success: function (msg) {
                               hhide();
                               if (!msg.d.Success) {
                                   ShowStatus("warning", msg.d.Message);
                                   return;
                               }
                               Files(msg.d.Data);
                               ShowStatus("success", msg.d.Message);
                           }
                       });
                   }
               });

}


function bindContext() {
    $("tr.fmDRow[data-type='1']").each(function () {
        $(this).jeegoocontext('menu', {
            widthOverflowOffset: 0,
            heightOverflowOffset: 3,
            submenuLeftOffset: 0,
            submenuTopOffset: 0,
            event: 'click',
            openBelowContext: true,
            onSelect: function (e, context) {
                var path = $(context).attr('data-path');
                switch (parseInt($(this).attr('data-action'))) {
                    case 0:
                        appendFile(path);
                        break;
                    case 3:
                        deleteFile(path);
                        break;
                    case 2:
                        $(context).nojeegoocontext();
                        setupRename($(context));
                        break;
                    case 1:
                        cancelCrop();
                        closefmD();
                        $('#ImagePanel').find('h2:first').html('Image Tools: ' + $(context).find('span:first').text());
                        $('#ImagePanel').find('img.fmDImgPrev:first').attr('src', SiteVars.ApplicationRelativeWebRoot + 'image.axd?picture=' + escape(path) + '&r=' + Math.random()).attr('data-path', path);
                        $(this).colorbox({ inline: true, height: '80%', width: '60%', href: "#ImagePanel" }).open();
                        break;
                }
            },
            onShow: function (e, context) {
                if ($(context).attr('data-path').FileExtension().IsImage())
                    $(this).find('li').show();
                else
                    $(this).find('li:eq(1)').hide();

            }
        });
    });
}
function imageChange(change) {
    var data = { path: $('#fmDImgPrev:first').attr('data-path'), change: change };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/filemanager/AjaxFileManager.aspx/ChangeImage",
        data: JSON.stringify(data),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (msg) {
            if (!msg.d.Success) {
                alert(msg.d.Message);
                return;
            }
            $('#fmDImgPrev:first').attr('src', $('#fmDImgPrev:first').attr('src') + '&m=' + Math.random()).css({ 'height': 'auto', 'width': 'auto' }); ;
        }
    });
}

var jCropApi = null;
function setCrop() {
    var $i = $('#fmDImgPrev:first'),
            aTo = [20, 20, $i.width() - 20, $i.height() - 20];
    jCropApi = $.Jcrop($('#fmDImgPrev'), { setSelect: aTo, onChange: setC });
    $('#fmD_CropStart').hide();
    $('#fmD_CropCancel').show();
    $('#fmD_CropSubmit').show();
    $('.fmdPreview-NonCropTools').hide();
}
function setC(c) {
    $('#x1').val(c.x);
    $('#y1').val(c.y);
    $('#x2').val(c.w);
    $('#y2').val(c.h);
};

function cancelCrop() {
    if (jCropApi != null) {
        jCropApi.destroy();
        jCropApi = null;
    }
    $('.jcrop-holder').remove();
    //$('#fmDImgPrev').Jcrop('destroy');
    $('#fmD_CropStart').show();
    $('#fmD_CropCancel').hide();
    $('#fmD_CropSubmit').hide();
    $('.fmdPreview-NonCropTools').show();
}

function submitCrop() {
    if (jCropApi == null) {
        cancelCrop();
        return;
    }
    var data = { path: $('#fmDImgPrev:first').attr('data-path'), x: parseInt($('#x1').val()), y: parseInt($('#y1').val()), width: parseInt($('#x2').val()), height: parseInt($('#y2').val()) };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/filemanager/AjaxFileManager.aspx/CropImage",
        data: JSON.stringify(data),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (msg) {
            if (!msg.d.Success) {
                alert(msg.d.Message);
                return;
            }
            $('#fmDImgPrev:first').attr('src', $('#fmDImgPrev:first').attr('src') + '&m=' + Math.random()).css({ 'height': 'auto', 'width': 'auto' }); ;
        }
    });
    cancelCrop();
};
var startsize = { width: 0, height: 0 };
function startResize() {
    startsize.width = $('#fmDImgPrev:first').width();
    startsize.height = $('#fmDImgPrev:first').height();
    $('#fmdNonResizeTools').hide();
    $('#fmdResizeTools').show();
    $('#fmdPreview_ImageSlider').slider({
        max: 100,
        min: 1,
        value: 100,
        slide: function (event, ui) {
            $("#fmdPreview_ImageSliderTxt").val(ui.value + '%');
        },
        change: function (event, ui) {
            resizeImage(ui.value / 100);
        }
    });
    $("#fmdPreview_ImageSliderTxt").val($("#fmdPreview_ImageSlider").slider("value") + '%');
}

function resizeImage(perc) {
    var size = { height: startsize.height * perc, width: startsize.height * perc };
    $('#fmDImgPrev:first').css({ 'width': size.width + 'px', 'height': size.height + 'px' });
}

function cancelResizeImage() {
    $('#fmDImgPrev:first').css({ 'width': startsize.width + 'px', 'height': startsize.height + 'px' });
    $('#fmdNonResizeTools').show();
    $('#fmdResizeTools').hide();
}

function submitResizeImage() {
    var percent = $("#fmdPreview_ImageSlider").slider("value");
    $('#fmdResizeTools').hide();
    $('#fmdNonResizeTools').show();
    var data = { path: $('#fmDImgPrev:first').attr('data-path'), percent: parseInt(percent) };
    $.ajax({
        url: SiteVars.ApplicationRelativeWebRoot + "admin/filemanager/AjaxFileManager.aspx/ResizeImage",
        data: JSON.stringify(data),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        beforeSend: onAjaxBeforeSend,
        dataType: "json",
        success: function (msg) {
            if (!msg.d.Success) {
                alert(msg.d.Message);
                return;
            }
            $('#fmDImgPrev:first').attr('src', $('#fmDImgPrev:first').attr('src') + '&m=' + Math.random()).css({ 'height': 'auto', 'width': 'auto' });
        }
    });
};

$.fn.disableSelection = function () {
    $(this).attr('unselectable', 'on')
           .css('-moz-user-select', 'none')
           .each(function () {
               this.onselectstart = function () { return false; };
           });
};

function onAjaxBeforeSend(jqXHR, settings) {

    // AJAX calls need to be made directly to the real physical location of the
    // web service/page method.  For this, SiteVars.ApplicationRelativeWebRoot is used.
    // If an AJAX call is made to a virtual URL (for a blog instance), although
    // the URL rewriter will rewrite these URLs, we end up with a "405 Method Not Allowed"
    // error by the web service.  Here we set a request header so the call to the server
    // is done under the correct blog instance ID.

    jqXHR.setRequestHeader('x-blog-instance', SiteVars.BlogInstanceId);
}