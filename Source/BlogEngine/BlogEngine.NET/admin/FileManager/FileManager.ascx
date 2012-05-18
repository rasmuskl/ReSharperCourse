<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FileManager.ascx.cs" Inherits="admin_FileManager" %>
<%@ Import Namespace="BlogEngine.Core" %>
<link href="<%= Utils.RelativeWebRoot %>admin/FileManager/FileManager.css" rel="Stylesheet" type="text/css" />
<link href="<%= Utils.RelativeWebRoot %>admin/uploadify/uploadify.css" rel="stylesheet" type="text/css" />
<script src="<%= Utils.RelativeWebRoot %>admin/uploadify/swfobject.js" type="text/javascript"></script>
<script src="<%= Utils.RelativeWebRoot %>admin/uploadify/jquery.uploadify.v2.1.4.min.js" type="text/javascript"></script>
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.14/jquery-ui.min.js"></script>
<link rel="Stylesheet" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.14/themes/base/jquery-ui.css" />
<script src="<%= Utils.RelativeWebRoot %>admin/FileManager/jquery.jeegoocontext.min.js" type="text/javascript"></script>
<link href="<%= Utils.RelativeWebRoot %>admin/FileManager/JCrop/css/jquery.Jcrop.css" rel="stylesheet" type="text/css" />
<script src="<%= Utils.RelativeWebRoot %>admin/FileManager/JCrop/js/jquery.Jcrop.min.js" type="text/javascript"></script>

<script type="text/javascript" src="<%= Utils.RelativeWebRoot %>admin/FileManager/FileManager-mini.js"></script>
            
        <div id="fmD" class="fmD" style="display:none;">
            <div class="fmDWrap">
            <div class="fmD-left">
                <h1>File Manager</h1>
                <ul>
                    <li>
                        <a href="javascript:;">
                            <img src="<%= Utils.RelativeWebRoot %>admin/filemanager/images/New-Document-icon.png" height="32px" width="32px" title="Upload a new file" />
                            Attach a new File.
                        </a>
                    </li>
                    <li class="fmDActive">
                        <a href="javascript:;">
                            <img src="<%= Utils.RelativeWebRoot %>admin/filemanager/images/archives.png" height="32px" width="32px" title="Upload a new file" />
                            File Manager
                        </a>
                    </li>
                    <li>
                        <a href="javascript:;" onclick="closefmD();">
                            <img src="<%= Utils.RelativeWebRoot %>admin/filemanager/images/logout.png" height="32px" width="32px" title="Upload a new file" />
                            Close
                        </a>
                    </li>
                </ul>
            </div>
            <div class="fmD-right">
                <div class="fmD-right-content" >
                    <div class="fmD-upload">
                        <h2>Upload File</h2>
                        <div class="fmD-upload-left">
                                &nbsp;
                        </div>
                        <div class="fmD-upload-right">
                            <div class="fmD-upload-right-wait">
                                <p>
                                    Select the files you wish to upload. When completed these files will be available in the File Manager.<br />
                                    
                                </p>
                                <strong>...ready</strong>
                            </div>
                            <div class="fmD-upload-right-uploading">
                                Uploading:<span id="fmD_UploadFile"></span>
                                <div id="fmD_UploadProgress"></div>
                            </div>
                        </div>
                        <div class="fmD-upload-right">
                            <div class="fmD-upload-ctrl">
                                <input type="file" id="fmD_upload_file" name="file_upload" />
                             </div>
                        </div>
                        <div class="clear"></div>
                        <div class="fmD-upload-options">
                            <input type="radio" id="fmdUpload_Append" checked="checked" name="fmdUpload_rdo" /><label for="fmdUpload_Append">Append to my document and close</label><br />
                            <input type="radio" id="fmdUpload_AppendContinue" name="fmdUpload_rdo" /><label for="fmdUpload_AppendContinue">Append to my document and continue</label><br />
                            <input type="radio" id="fmdUpload_FileManager" name="fmdUpload_rdo" /><label for="fmdUpload_FileManager">Upload and show file in the file manager</label>
                        </div>
                    </div>
                </div>
                <div class="fmD-right-content" style="display:block;">
                    File Manager&nbsp;<img src="<%= Utils.RelativeWebRoot %>admin/filemanager/images/design_wait.gif" height="16px" width="16px" id="dwait" style="border:none;" />
                    <br />
                    <div id="Container"></div>
                </div>
                <div class="fmD-right-content fmD-right-close">
                    <div align="center">
                        Close the file manager dialog
                    </div>
                </div>
            </div>
            <div class="clear"></div>
            </div>
        </div>

<ul id="menu" class="jeegoocontext cm_default">
    <li class="icon" data-action="0">
        <span class="icon add"></span>
        Append To Post
    </li>
    <li class="icon" data-action="1">
        <span class="icon viewer"></span>
        Image Tools
    </li>
    <li class="separator"  data-action="-1"></li>
    <li class="icon" data-action="2">
        <span class="icon rename"></span>
        Rename
    </li>
    <li class="icon" data-action="3">
        <span class="icon page-delete"></span>
        Delete
    </li>
</ul>    
<div id="Div1" style="display:none;height:1px">
    <div id="ImagePanel" class="overlaypanel">
        <h2>Image Preview</h2>
        <div id="fmdNonResizeTools">
            <a href="javascript:;" class="fmDPreview-tools rotate-left fmdPreview-NonCropTools" onclick="imageChange(2);">Rotate Left</a>
            <a href="javascript:;" class="fmDPreview-tools rotate-right fmdPreview-NonCropTools" onclick="imageChange(3);">Rotate Right</a>
            <a href="javascript:;" class="fmDPreview-tools flip-horizontal fmdPreview-NonCropTools" onclick="imageChange(1);">Flip Horizontally</a>
            <a href="javascript:;" class="fmDPreview-tools flip-vertical fmdPreview-NonCropTools" onclick="imageChange(2);">Flip Vertically</a>
            <a href="javascript:;" class="fmDPreview-tools crop-image" onclick="setCrop()" id="fmD_CropStart">Crop Image</a>
            <a href="javascript:;" class="fmDPreview-tools crop-cancel" onclick="cancelCrop()" id="fmD_CropCancel" style="display:none;">Cancel Image Cropping</a>
            <a href="javascript:;" class="fmDPreview-tools crop-submit" onclick="submitCrop()" id="fmD_CropSubmit">Submit Cropped Image</a>
            <a href="javascript:;" class="fmDPreview-tools image-resize fmdPreview-NonCropTools" onclick="startResize()" id="fmD_ResizeStart">Resize Image</a>
        </div>
        <div class="resize-manager" id="fmdResizeTools">
            <strong>Resize Image</strong><input type="text" id="fmdPreview_ImageSliderTxt" style="border:none; background:transparent; color:#f6931f; font-weight:bold;" />
            <div id="fmdPreview_ImageSlider" class="fmdPreview-ImageSlider" style="width:600px" ></div>
            <div>
                <a href="javascript:;" class="fmDPreview-tools crop-cancel" onclick="cancelResizeImage();">Cancel Image Resizing</a>
                <a href="javascript:;" class="fmDPreview-tools crop-submit" onclick="submitResizeImage();">Submit Resized Image</a>
            </div>
            <div class="resize-text">
                Resizing is approximate, actual image dimensions will be redrawn to maintain aspect ration
            </div>
        </div>
        <div>
            <img class="fmDImgPrev" id="fmDImgPrev" src="" align="left" />
        </div>
        <div style="display:none;margin-left:5px;" id="fmD_CropCoords">
            X <input type="text" size="4" id="x1" name="x1" />
		    Y <input type="text" size="4" id="y1" name="y1" /><br />
		    W <input type="text" size="4" id="x2" name="x2" />
		    H <input type="text" size="4" id="y2" name="y2" />
        </div>
    </div>
</div>