<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Comments.aspx.cs" Inherits="admin.Comments.Settings" %>
<%@ Import Namespace="Resources"%>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server">

    <script type="text/javascript">
        function ConfirmReset() {
            return confirm('<%=labels.confirmResetCounters%>');  
        } 
    </script>
  
	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu1" runat="server" />
		</div>
		<div class="content-box-left">
            <div class="rightligned-top action_buttons">
                <asp:Button runat="server" class="btn primary" ID="btnSave" Text="<%$Resources:labels, saveSettings %>" />&nbsp;
                <span class="loader">&nbsp;</span>
            </div>
            <h1 style="border:none;"><%=labels.commentSettings %></h1>
            <h2><%=labels.basic %></h2>
            <ul class="fl leftaligned">
                <li>
                    <label class="lbl" for="<%=ddlCloseComments.ClientID %>"> <%=labels.closeCommetsAfter %></label>
                    <asp:DropDownList runat="server" ID="ddlCloseComments">
                        <asp:ListItem Text="<%$ Resources:labels, never %>" Value="0" />
                        <asp:ListItem Text="1" />
                        <asp:ListItem Text="2" />
                        <asp:ListItem Text="3" />
                        <asp:ListItem Text="7" />
                        <asp:ListItem Text="10" />
                        <asp:ListItem Text="14" />
                        <asp:ListItem Text="21" />
                        <asp:ListItem Text="30" />
                        <asp:ListItem Text="60" />
                        <asp:ListItem Text="90" />
                        <asp:ListItem Text="180" />
                        <asp:ListItem Text="365" />
                    </asp:DropDownList>
                    <%=labels.days%>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbEnableComments" />
                    <label for="<%=cbEnableComments.ClientID %>"><%=labels.enableComments %></label>
                    <span class="insetHelp">(<%=labels.enableCommentsDescription %>)</span>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbEnableCommentsModeration" /> 
                    <label for="<%=cbEnableCommentsModeration.ClientID %>"><%=labels.enableCommentsModeration%></label>
                    <span class="insetHelp">(<%=labels.pendingApproval%>)</span>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbEnableCommentNesting" />
                    <label for="<%=cbEnableCommentNesting.ClientID %>"><%=labels.enableCommentNesting %></label>
                    <span class="insetHelp">(<%=labels.enableCommentNestingDescription%>)</span>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbEnableCoComment" />
                    <label for="<%=cbEnableCoComment.ClientID %>"><%=labels.enableCoComments %></label>
                </li>
            </ul>

            <h2><%=labels.appearance %></h2>
            <ul class="fl leftaligned">
                <li>
                    <div class="avatar-list">
                        <img src="../images/avatars/monsterid.png" alt="None" />
                        <img src="../images/avatars/wavatar.png" alt="None" />
                        <img src="../images/avatars/identicon.png" alt="None" />
                        <img src="../../pics/noavatar.jpg" alt="None" />
                    </div>
                    <label class="lbl" for="<%=rblAvatar.ClientID %>"><%=labels.avatars %></label>
                    <asp:RadioButtonList runat="Server" ID="rblAvatar" RepeatLayout="flow" RepeatDirection="horizontal">
                        <asp:ListItem Text="MonsterID" Value="monster" />
                        <asp:ListItem Text="Wavatar" Value="wavatar" />
                        <asp:ListItem Text="Identicon" Value="identicon" />
                        <asp:ListItem Text="<%$ Resources:labels, none %>" Value="none" />
                    </asp:RadioButtonList>
                </li>
                <li>
                    <label class="lbl" for=""><%=Resources.labels.enableTrackbacks %></label>
                        <asp:CheckBox runat="server" ID="cbEnableTrackBackSend" /><label><%=Resources.labels.send %></label>
                        &nbsp;&nbsp;
                        <asp:CheckBox runat="server" ID="cbEnableTrackBackReceive" /><label><%=Resources.labels.receive %></label>
                </li>
                <li>
                    <label for="" class="lbl"><%=Resources.labels.enablePingbacks %></label>
                        <asp:CheckBox runat="server" ID="cbEnablePingBackSend" /><label><%=Resources.labels.send %></label>
                        &nbsp;&nbsp;
                        <asp:CheckBox runat="server" ID="cbEnablePingBackReceive" /><label><%=Resources.labels.receive %></label>
                </li>
                <li>
                    <label class="lbl" for="<%=txtThumbProvider.ClientID %>"><%=labels.thumbnailServiceProvider %></label>
                    <asp:TextBox runat="server" ID="txtThumbProvider" Width="250" MaxLength="550" />
                    <span class="insetHelp">(<%=labels.thumbnailServiceProviderHelp%>)</span>
                </li>
                <li>
                    <label class="lbl" for="<%=ddlCommentsPerPage.ClientID %>" style="position: relative; top: 4px">
                        <%=labels.commentsPerPage%>
                    </label>
                    <asp:DropDownList runat="server" ID="ddlCommentsPerPage">
                        <asp:ListItem Text="5" />
                        <asp:ListItem Text="10" />
                        <asp:ListItem Text="15" />
                        <asp:ListItem Text="20" />
                        <asp:ListItem Text="50" />
                    </asp:DropDownList>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbEnableCountryInComments" />
                    <label for="<%=cbEnableCountryInComments.ClientID %>"><%=labels.showCountryChooser %></label>
                    <span class="insetHelp">(<%=labels.showCountryChooserDescription %>)</span>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbEnableWebsiteInComments" />
                    <label for="<%=cbEnableWebsiteInComments.ClientID %>"><%=labels.showEnableWebsiteInComments %></label>
                    <span class="insetHelp">(<%=labels.showEnableWebsiteInCommentsDescription %>)</span>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbShowLivePreview" />
                    <label for="<%=cbShowLivePreview.ClientID %>"><%=labels.showLivePreview %></label>
                </li>
            </ul>

            <h2><%=labels.disqusSettings %></h2>       
            <div class="info rounded" style="max-width:600px;">
                <%=labels.disqusSignupMessage %>
            </div>  
            <ul class="fl leftaligned">
                <li>
                    <label class="lbl"><%=labels.turnDisqusOnOff %></label>
                    <asp:CheckBox runat="server" ID="cbEnableDisqus" />
                    <label for="<%=cbEnableDisqus.ClientID %>"><%=labels.useDisqusAsCommentProvider %></label>
                </li>
                <li>
                    <label class="lbl" for="<%=txtDisqusName.ClientID %>"><%=labels.disqusShortName %></label>
                    <asp:TextBox runat="server" ID="txtDisqusName" Width="250" MaxLength="250" />
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbDisqusDevMode" />
                    <label for="<%=cbDisqusDevMode.ClientID %>"><%=labels.developmentMode %></label>
                    <span class="insetHelp">(<%=labels.developmentModeCheckboxMessage %>)</span>
                </li>
                <li>
                    <span class="filler"></span>
                    <asp:CheckBox runat="server" ID="cbDisqusAddToPages" />
                    <label for="<%=cbDisqusAddToPages.ClientID %>"><%=labels.addCommentsToPages %></label>
                    <span class="insetHelp">(<%=labels.addToPages %>)</span>
                </li>
            </ul>
    
            <div class="rightligned-bottom action_buttons">
                <asp:Button runat="server" class="btn primary" ID="btnSave2" Text="<%$Resources:labels, saveSettings %>" />&nbsp;
                <span class="loader">&nbsp;</span>
            </div>
            
		</div>
	</div> 

</asp:Content>