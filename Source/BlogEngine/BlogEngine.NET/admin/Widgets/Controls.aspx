<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" ValidateRequest="False"
    AutoEventWireup="true" CodeFile="Controls.aspx.cs" Inherits="admin.Widgets.Controls"
    Title="Control settings" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">
	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu" runat="server" />
		</div>
		<div class="content-box-left">
            <div class="rightligned-top action_buttons">
                <asp:Button runat="server" ID="btnSave" CssClass="btn primary rounded" />
            </div>
            <h1><%=Resources.labels.recentPosts %></h1>
            <ul class="fl">
                <li>
                    <asp:Label runat="server" AssociatedControlID="txtNumberOfPosts" Text='<%$ Code: Resources.labels.numberOfPosts %>' CssClass="lbl" />
                    <asp:TextBox runat="server" ID="txtNumberOfPosts" Width="30" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumberOfPosts" ErrorMessage="Required" />
                    <asp:CompareValidator runat="Server" ControlToValidate="txtNumberOfPosts" Operator="dataTypeCheck"
                        Type="integer" ErrorMessage="<%$Resources:labels,enterValidNumber %>" /><br />
                </li>
                <li>
                    <asp:CheckBox runat="Server" ID="cbDisplayComments" />
                    <asp:Label runat="server" AssociatedControlID="cbDisplayComments" Text='<%$ Code: Resources.labels.displayCommentsOnRecentPosts %>' />
                    <asp:CheckBox runat="Server" ID="cbDisplayRating" />
                    <asp:Label runat="server" AssociatedControlID="cbDisplayRating" Text='<%$ Code: Resources.labels.displayRatingsOnRecentPosts %>' />
                </li>
            </ul>
            <h1><%=Resources.labels.recentComments %></h1>
            <ul class="fl">
                <li>
                    <asp:Label runat="server" AssociatedControlID="txtNumberOfPosts" Text='<%$ Code: Resources.labels.numberOfComments %>' CssClass="lbl" />
                    <asp:TextBox runat="server" ID="txtNumberOfComments" Width="30" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumberOfComments"
                        ErrorMessage="<%$Resources:labels,required %>" />
                    <asp:CompareValidator runat="Server" ControlToValidate="txtNumberOfComments" Operator="dataTypeCheck"
                        Type="integer" ErrorMessage="<%$Resources:labels,enterValidNumber %>" /><br />
                </li>
            </ul>
            <h1><%=Resources.labels.searchField %></h1>
            <ul class="fl">
                <li>
                    <asp:Label runat="server" AssociatedControlID="txtSearchButtonText" Text='<%$ Code: Resources.labels.buttonText %>' CssClass="lbl" />
                    <asp:TextBox runat="server" ID="txtSearchButtonText" Width="320" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSearchButtonText"
                        ErrorMessage="<%$Resources:labels,required %>" />
                </li>
                <li>
                    <asp:Label runat="server" AssociatedControlID="txtDefaultSearchText" Text='<%$ Code: Resources.labels.searchFieldText %>' CssClass="lbl" />
                    <asp:TextBox runat="server" ID="txtDefaultSearchText" Width="320" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDefaultSearchText"
                        ErrorMessage="<%$Resources:labels,required %>" />
                    <span class="belowHelp"><%=Resources.labels.defaultTextShownInSearchField %></span>
                </li>
                <li>
                    <asp:Label runat="server" AssociatedControlID="txtCommentLabelText" Text='<%$ Code: Resources.labels.commentLabelText %>' CssClass="lbl" />
                    <asp:TextBox runat="server" ID="txtCommentLabelText" Width="320" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCommentLabelText"
                        ErrorMessage="<%$Resources:labels,required %>" />
                </li>
                <li>
                    <asp:CheckBox runat="Server" ID="cbEnableCommentSearch" />
                    <asp:Label runat="server" AssociatedControlID="cbEnableCommentSearch" Text='<%$ Code: Resources.labels.enableCommentSearch %>' />
                </li>
                <li>
                    <asp:CheckBox runat="server" ID="cbShowIncludeCommentsOption" />
                    <asp:Label runat="server" AssociatedControlID="cbShowIncludeCommentsOption" Text="Show include comments option" />
                </li>
            </ul>

            <h1><%=Resources.labels.contactForm %></h1>
            <ul class="fl">
                <li>
                    <asp:Label runat="server" AssociatedControlID="txtFormMessage" Text='<%$ Code: Resources.labels.formMessage %>' CssClass="lbl" />
                    <asp:TextBox runat="server" ID="txtFormMessage" TextMode="multiLine" Rows="5" Columns="40" /><br />
                </li>
                <li>
                    <asp:Label runat="server" AssociatedControlID="txtThankMessage" Text='<%$ Code: Resources.labels.thankYouMessage %>' CssClass="lbl" />
                    <asp:TextBox runat="server" ID="txtThankMessage" TextMode="multiLine" Rows="5" Columns="40" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtThankMessage" ErrorMessage="<%$Resources:labels,required %>" />
                </li>
                <li>
                    <asp:CheckBox runat="Server" ID="cbEnableAttachments" />
                    <asp:Label runat="server" AssociatedControlID="cbEnableAttachments" Text='<%$ Code: Resources.labels.enableAttachments %>' />
                </li>
                <li>
                    <asp:CheckBox runat="Server" ID="cbEnableRecaptcha" />
                    <asp:Label runat="server" AssociatedControlID="cbEnableRecaptcha" Text='<%$ Code: Resources.labels.enableRecaptcha %>' />
                    <span class="belowHelp"><%=Resources.labels.recaptchaConfigureReminder%></span>
                </li>
            </ul>
            <div class="rightligned-bottom action_buttons">
                <asp:Button runat="server" ID="btnSave2" CssClass="btn primary rounded" />
            </div>
        </div>
    </div>
</asp:Content>
