<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edit.ascx.cs" Inherits="Widgets.TextBox.Edit" %>
<%@ Import Namespace="BlogEngine.Core" %>

<script type="text/javascript" src="<%=Utils.RelativeWebRoot %>editors/tiny_mce_3_4_3_1/tiny_mce.js"></script>
<script type="text/javascript">
	tinyMCE.init({
		// General options
		mode: "exact",
		elements : "<%=txtText.ClientID %>",
		theme: "advanced",
		plugins: "inlinepopups,fullscreen,contextmenu,emotions,table,iespell",
		convert_urls: false,
	  
	  // Theme options
		theme_advanced_buttons1: "fullscreen,code,|,cut,copy,paste,|,undo,redo,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,bullist,numlist,outdent,indent,|,iespell,link,unlink,sub,sup,removeformat,cleanup,charmap,emotions",
		theme_advanced_buttons2: "",
		theme_advanced_toolbar_location: "top",
		theme_advanced_toolbar_align: "left",
		theme_advanced_statusbar_location: "bottom",
		theme_advanced_resizing: true,
		theme_advanced_source_editor_height: 425,
		gecko_spellcheck : true,
		
		tab_focus : ":prev,:next"
	});
</script>

<asp:TextBox runat="server" ID="txtText" TextMode="multiLine" Columns="100" Rows="10" style="width:700px;height:372px" /><br />