<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Archive.aspx.cs" Inherits="MichaelJBaird.Themes.JQMobile.Archive" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" Runat="Server">
  <ul runat="server" id="ulArchive" data-role="listview" data-filter="true" />
  <br />
    
  <div id="totals">
    <h2>Total</h2>
    <span><asp:literal runat="server" id="ltPosts" /></span><br />
    <asp:literal runat="server" id="ltComments" />
    <span><asp:literal runat="server" id="ltRaters" /></span>
  </div>
</asp:Content>