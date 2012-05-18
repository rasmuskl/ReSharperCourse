<%@ Page Language="C#" MasterPageFile="account.master" AutoEventWireup="true"
    CodeFile="change-password.aspx.cs" Inherits="Account.ChangePassword" %>

<%@ MasterType VirtualPath="~/Account/account.master" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        <%=Resources.labels.changePassword %></h2>
    <p>
        <%=String.Format(Resources.labels.requiredPasswordLength,Membership.MinRequiredPasswordLength) %>
    </p>
    <asp:ChangePassword ID="ChangeUserPassword" runat="server" CancelDestinationPageUrl="~/"
        EnableViewState="false" RenderOuterTable="false">
        <ChangePasswordTemplate>
            <div class="accountInfo">
                <div class="login">
                    <div class="field">
                        <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword"><%=Resources.labels.oldPassword %>:</asp:Label>
                        <div class="boxRound">
                            <asp:TextBox ID="CurrentPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                        </div>
                    </div>
                    <div class="field">
                        <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword"><%=Resources.labels.newPassword %>:</asp:Label>
                        <div class="boxRound">
                            <asp:TextBox ID="NewPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                        </div>
                    </div>
                    <div class="field">
                        <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword"><%=Resources.labels.confirmNewPassword %>:</asp:Label>
                        <div class="boxRound">
                            <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="submitButton">
                    <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword"
                        Text="<%$Resources:labels,changePassword %>" OnClick="ChangePasswordPushButton_Click"
                        OnClientClick="return ValidateChangePassword();" />
                </div>
            </div>
        </ChangePasswordTemplate>
    </asp:ChangePassword>
    <asp:HiddenField ID="hdnPassLength" runat="server" />
</asp:Content>
