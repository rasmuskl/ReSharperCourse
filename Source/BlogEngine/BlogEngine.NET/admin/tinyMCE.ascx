<%@ Control Language="C#" AutoEventWireup="true" CodeFile="tinyMCE.ascx.cs" Inherits="Admin.TinyMce" %>
<%@ Import Namespace="BlogEngine.Core" %>

<script type="text/javascript" src="<%=Utils.RelativeWebRoot %>editors/tiny_mce_3_4_3_1/tiny_mce.js"></script>
<script type="text/javascript">
	tinyMCE.init({
		// General options
		mode: "exact",
		elements : "<%=txtContent.ClientID %>",
		theme: "advanced",
		plugins: "inlinepopups,fullscreen,contextmenu,emotions,table,iespell,advlink,insertcode",
		convert_urls: false,
		
	  // Theme options
		theme_advanced_buttons1: "fullscreen,code,|,cut,copy,paste,pastetext,pasteword,|,undo,redo,|,bold,italic,underline,strikethrough,|,blockquote,sub,sup,|,justifyleft,justifycenter,justifyright,|,bullist,numlist,outdent,indent",
		theme_advanced_buttons2: "iespell,link,unlink,removeformat,cleanup,charmap,emotions,|,formatselect,fontselect,fontsizeselect,|,forecolor,backcolor,insertcode",
        theme_advanced_buttons3: "",
		theme_advanced_toolbar_location: "top",
		theme_advanced_toolbar_align: "left",
		theme_advanced_statusbar_location: "bottom",
		theme_advanced_resizing: true,
        theme_advanced_resize_horizontal : false,
		tab_focus : ":prev,:next",
		gecko_spellcheck : true,
        
        //Character count        
        theme_advanced_path : false,
        setup : function(ed) {
            ed.onKeyUp.add(function (ed, e) {  
                
                var strip = (tinyMCE.activeEditor.getContent()).replace(/(<([^>]+)>)/ig,"");
                var text = strip.split(' ').length + " Words, " +  strip.length + " Characters"
                tinymce.DOM.setHTML(tinymce.DOM.get(tinyMCE.activeEditor.id + '_path_row'), text);   
            });
        },
        auto_focus: "<%=txtContent.ClientID %>"
	});
</script>

<asp:TextBox runat="Server" ID="txtContent" CssClass="post" Width="100%" Height="250px" TextMode="MultiLine" />