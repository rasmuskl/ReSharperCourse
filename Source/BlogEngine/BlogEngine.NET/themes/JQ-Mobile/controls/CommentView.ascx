<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CommentView.ascx.cs" Inherits="MichaelJBaird.Themes.JQMobile.Controls.CommentView" %>
<%@ Import Namespace="BlogEngine.Core" %>

<% if (CommentCounter > 0) { %>
<div id="commentlist" data-role="collapsible" data-collapsed="false" data-theme="e" data-content-theme="c">
  <h3 id="comment"><%=Resources.labels.comments %> (<%=CommentCounter%>)</h3>
	<asp:PlaceHolder runat="server" ID="phComments" />  
</div>
<% } %>

<asp:PlaceHolder runat="server" ID="phTrckbacks"></asp:PlaceHolder>

<asp:PlaceHolder runat="Server" ID="phAddComment">
<p>&nbsp;</p>

<div id="comment-form" class="ui-body ui-body-a ui-corner-all">
	<img src="<%=Utils.RelativeWebRoot %>pics/ajax-loader.gif" alt="Saving the comment" style="display:none" id="ajaxLoader" />  
	<span id="status"></span>

	<div class="commentForm">
		<h3 id="addcomment"><%=Resources.labels.addComment %></h3>

		<% if (NestingSupported){ %>
		  <asp:HiddenField runat="Server" ID="hiddenReplyTo"  />
		  <p id="cancelReply" style="display:none;"><a href="javascript:void(0);" onclick="BlogEngine.cancelReply();"><%=Resources.labels.cancelReply %></a></p>
		<%} %>
			
    <div data-role="fieldcontain">
			<label for="<%=this.NameInputId %>" class="lbl-user"><%=Resources.labels.name %>*</label>
			<input type="text" class="txt-user" name="<%= this.NameInputId %>" id="<%= this.NameInputId %>" tabindex="2" value="<%= this.DefaultName %>" />
			<span id="spnNameRequired" style="color:Red;display:none;"> <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:labels, required %>"></asp:Literal></span>
			<span id="spnChooseOtherName" style="color:Red;display:none;"> <asp:Literal ID="Literal2" runat="server" Text="<%$Resources:labels, chooseOtherName %>"></asp:Literal></span>
		</div>
			
    <div data-role="fieldcontain">
			<label for="<%=txtEmail.ClientID %>" class="lbl-email"><%=Resources.labels.email %>*</label>
			<asp:TextBox runat="Server" CssClass="lbl-email" ID="txtEmail" TabIndex="3" ValidationGroup="AddComment" />
			<span id="gravatarmsg"></span>
			<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtEmail" ErrorMessage="<%$Resources:labels, required %>" Display="dynamic" ValidationGroup="AddComment" />
			<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail" ErrorMessage="<%$Resources:labels, enterValidEmail%>" Display="dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="AddComment" />
		</div>

		<% if (BlogSettings.Instance.EnableWebsiteInComments){ %>
		<div data-role="fieldcontain">
			<label for="<%=txtWebsite.ClientID %>" class="lbl-website"><%=Resources.labels.website%></label>
			<asp:TextBox runat="Server" CssClass="txt-website" ID="txtWebsite" TabIndex="4" ValidationGroup="AddComment" />
			<asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="Server" ControlToValidate="txtWebsite" ValidationExpression="(http://|https://|)([\w-]+\.)+[\w-]+(/[\w- ./?%&=;~]*)?" ErrorMessage="<%$Resources:labels, enterValidUrl %>" Display="Dynamic" ValidationGroup="AddComment" />
		</div>
		<%} %>

		<% if(BlogSettings.Instance.EnableCountryInComments){ %>
		<div data-role="fieldcontain">
			<label for="<%=ddlCountry.ClientID %>" class="lbl-country"><%=Resources.labels.country %></label>
			<asp:DropDownList runat="server" CssClass="txt-country" ID="ddlCountry" onchange="BlogEngine.setFlag(this.value)" TabIndex="5" EnableViewState="false" ValidationGroup="AddComment" />&nbsp;
			<%--<span class="CommentFlag"><asp:Image runat="server" ID="imgFlag" AlternateText="Country flag" Width="16" Height="11" EnableViewState="false" /></span>--%>
		</div>
		<%} %>

		<blog:SimpleCaptchaControl ID="simplecaptcha" runat="server" TabIndex="6" />

  	<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtContent" ErrorMessage="<%$Resources:labels, required %>" Display="dynamic" ValidationGroup="AddComment" /><br />

    <div id="commentCompose" data-role="fieldcontain">
		  <label for="<%=txtContent.ClientID %>" class="lbl-content"><%=Resources.labels.comment%></label>
		  <asp:TextBox runat="server" CssClass="txt-content" ID="txtContent" TextMode="multiLine" TabIndex="7" ValidationGroup="AddComment" />
		</div>

		<div id="commentPreview">
		  <img src="<%=Utils.RelativeWebRoot %>pics/ajax-loader.gif" style="display:none" alt="Loading" />  
		</div>
		
		<div data-role="fieldcontain">
	 	  <fieldset data-role="controlgroup">
        <input type="checkbox" name="checkbox-notify" id="cbNotify" class="custom" />
        <label for="cbNotify"><%=Resources.labels.notifyOnNewComments %></label>
      </fieldset>
		</div>

		<blog:RecaptchaControl ID="recaptcha" runat="server" TabIndex="9" />
			
		<p>
			<input type="button" id="btnSaveAjax" value="<%=Resources.labels.saveComment %>" data-theme="b" onclick="return BlogEngine.validateAndSubmitCommentForm()" tabindex="10" />
			<asp:HiddenField runat="server" ID="hfCaptcha" />
		</p>
	</div>

</div>

<script type="text/javascript">
<!--//
function registerCommentBox(){
	BlogEngine.comments.contentBox = BlogEngine.$("<%=txtContent.ClientID %>");
	BlogEngine.comments.moderation = <%=BlogSettings.Instance.EnableCommentsModeration.ToString().ToLowerInvariant() %>;
	BlogEngine.comments.checkName = <%=(!Security.IsAuthenticated).ToString().ToLowerInvariant() %>;
	BlogEngine.comments.postAuthor = "<%=Post.Author %>";
	BlogEngine.comments.nameBox = BlogEngine.$("<%= this.NameInputId %>");
	BlogEngine.comments.emailBox = BlogEngine.$("<%=txtEmail.ClientID %>");
	BlogEngine.comments.websiteBox = BlogEngine.$("<%=txtWebsite.ClientID %>");
	BlogEngine.comments.countryDropDown = BlogEngine.$("<%=ddlCountry.ClientID %>"); 
	BlogEngine.comments.captchaField = BlogEngine.$('<%=hfCaptcha.ClientID %>');
	BlogEngine.comments.controlId = '<%=this.UniqueID %>';
	BlogEngine.comments.replyToId = BlogEngine.$("<%=hiddenReplyTo.ClientID %>"); 
}
//-->
</script>

<% if (BlogSettings.Instance.IsCoCommentEnabled){ %>
<script type="text/javascript">
  // this ensures coComment gets the correct values
  coco =
{
  tool: "BlogEngine",
  siteurl: "<%=Utils.AbsoluteWebRoot %>",
  sitetitle: "<%=BlogSettings.Instance.Name %>",
  pageurl: location.href,
  pagetitle: "<%=this.Post.Title %>",
  author: "<%=this.Post.Title %>",
  formID: "<%=Page.Form.ClientID %>",
  textareaID: "<%=txtContent.UniqueID %>",
  buttonID: "btnSaveAjax"
}
</script>
<script id="cocomment-fetchlet" src="http://www.cocomment.com/js/enabler.js" type="text/javascript"></script>
<%} %>

</asp:PlaceHolder>

<script type="text/javascript">
  function toggle_visibility(id, id2) {
    var e = document.getElementById(id);
    var h = document.getElementById(id2);
    if (e.style.display == 'block') {
      e.style.display = 'none';
      h.innerHTML = "+";
    }
    else {
      e.style.display = 'block';
      h.innerHTML = "-";
    }
  }
</script>

<asp:label runat="server" id="lbCommentsDisabled" visible="false"><%=Resources.labels.commentsAreClosed %></asp:label>