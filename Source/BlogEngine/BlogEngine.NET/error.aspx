<%@ Page Language="C#" AutoEventWireup="true" CodeFile="error.aspx.cs" Inherits="error_occurred" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" Runat="Server">
  <div class="post">
    <h1>Ooops! An unexpected error has occurred.</h1>
    
    <div>
      <p>
        This one's down to me! Please accept my apologies for this - I'll see to it
        that the developer responsible for this happening is given 20 lashes 
        (but only after he or she has fixed this problem).
      </p>
    </div>
    
    <div id="divErrorDetails" runat="server" visible="false">
        <h2>Error Details:</h2>
        <p id="pDetails" runat="server"></p>
    </div>
    
  </div>
</asp:Content>