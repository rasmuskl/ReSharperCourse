<%@ Page Title="Register" Language="C#" MasterPageFile="account.master" AutoEventWireup="true"
    CodeFile="register.aspx.cs" Inherits="Account.Register" %>

<%@ MasterType VirtualPath="~/Account/account.master" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:CreateUserWizard ID="RegisterUser" runat="server" EnableViewState="false" OnCreatedUser="RegisterUser_CreatedUser"
        OnCreatingUser="RegisterUser_CreatingUser">
        <WizardSteps>
            <asp:CreateUserWizardStep ID="RegisterUserWizardStep" runat="server">
                <ContentTemplate>
                    <p><span id="CreateHdr">
                        <%=Resources.labels.createAccount %></span> <span>
                            <%=Resources.labels.alreadyHaveAccount %>
                            <a id="HeadLoginStatus" runat="server"><%=Resources.labels.loginNow %></a></span></p>
                    <div class="accountInfo">
                        <div class="login">
                            <p>
                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"><%=Resources.labels.userName %>:</asp:Label>
                                <div class="boxRound">
                                    <asp:TextBox ID="UserName" runat="server" CssClass="textEntry"></asp:TextBox>
                                </div>
                            </p>
                            <p>
                                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email"><%=Resources.labels.email %>:</asp:Label>
                                <div class="boxRound">
                                    <asp:TextBox ID="Email" runat="server" CssClass="textEntry"></asp:TextBox>
                                </div>
                            </p>
                            <p>
                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password"><%=String.Format(Resources.labels.passwordMinimumCharacters, Membership.MinRequiredPasswordLength) %></asp:Label>
                                <div class="boxRound">
                                    <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                                </div>
                            </p>
                            <p>
                                <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword"><%=Resources.labels.confirmPassword %>:</asp:Label>
                                <div class="boxRound">
                                    <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                                </div>
                            </p>
                        </div>
                        <p class="submitButton">
                            <asp:Button ID="CreateUserButton" runat="server" CommandName="MoveNext" Text="<%$Resources:labels,createUser %>"
                                OnClientClick="return ValidateNewUser()" />
                        </p>
                    </div>
                </ContentTemplate>
                <CustomNavigationTemplate>
                </CustomNavigationTemplate>
            </asp:CreateUserWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
    <asp:HiddenField ID="hdnPassLength" runat="server" />
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[name$='UserName']").focus();
        });
    </script>
</asp:Content>
