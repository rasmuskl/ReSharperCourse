<%@ Page Title="Password Retrieval" Language="C#" MasterPageFile="~/Account/account.master" AutoEventWireup="true"
    CodeFile="password-retrieval.aspx.cs" Inherits="Account.PasswordRetrieval" %>
<%@ MasterType VirtualPath="~/Account/account.master" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1>
        <%=Resources.labels.passwordRetrieval %></h1>
    <p>
        <%=Resources.labels.passwordRetrievalInstructionMessage %>
    </p>
    <div class="accountInfo">
        <div class="login">
            <div>
                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="txtEmail"><%=Resources.labels.email %>:</asp:Label>
                <div class="boxRound">
                    <asp:TextBox ID="txtEmail" runat="server" AutoCompleteType="None" CssClass="textEntry"></asp:TextBox>
                </div>
            </div>
            <p class="submitButton">
                <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="<%$Resources:labels,send %>"
                    OnClick="LoginButton_Click" OnClientClick="return ValidatePasswordRetrieval()" />
                <span>
                    <%=Resources.labels.or %>
                    <a href="<%= Utils.RelativeWebRoot %>Account/login.aspx">
                        <%=Resources.labels.cancel %></a></span> </p>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[name$='txtEmail']").focus();
        });
    </script>
</asp:Content>
