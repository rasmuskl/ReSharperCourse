<%@ Page Language="C#" MasterPageFile="account.master" AutoEventWireup="true" ClientIDMode="Static"
    CodeFile="login.aspx.cs" Inherits="Account.Login" %>

<%@ MasterType VirtualPath="~/Account/account.master" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false" OnAuthenticate="LoginUser_OnAuthenticate">
        <LayoutTemplate>
            <div class="accountInfo">
                <div class="login">
                    <h1><asp:Label runat="server" ID="lblTitle" Text="<%$Resources:labels,login %>"></asp:Label></h1>
                    <div class="field">
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"><%=Resources.labels.userName %>:</asp:Label>
                        <div class="boxRound">
                            <asp:TextBox ID="UserName" runat="server" AutoCompleteType="None" CssClass="textEntry"></asp:TextBox>
                        </div>
                    </div>
                    <div class="field">
                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password"><%=Resources.labels.password %>:</asp:Label>
                        <div class="boxRound">
                            <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                        </div>
                    </div>
                    <div class="field">
                        <asp:CheckBox ID="RememberMe" runat="server" />
                        <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline"><%=Resources.labels.rememberMe %></asp:Label>
                    </div>
                    <div class="submitButton">
                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="<%$Resources:labels,login %>" OnClientClick="return ValidateLogin();" />
                        <asp:PlaceHolder ID="phResetPassword" runat="server">
                            <span>
                                <asp:HyperLink runat="server" ID="linkForgotPassword" Text="<%$ Resources:labels,forgotPassword %>" />
                            </span>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </LayoutTemplate>
    </asp:Login>
    <% if (BlogEngine.Core.BlogSettings.Instance.EnableSelfRegistration)
       { %>
    <div id="LoginRegister">
        <%=Resources.labels.dontHaveAccount %>
        <asp:HyperLink ID="RegisterHyperLink" runat="server" EnableViewState="false" />
    </div>
    <% } %>
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[name$='UserName']").focus();
        });
    </script>
</asp:Content>
