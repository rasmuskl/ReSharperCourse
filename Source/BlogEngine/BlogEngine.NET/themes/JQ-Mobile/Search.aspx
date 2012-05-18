<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="Search.aspx.cs" Inherits="MichaelJBaird.Themes.JQMobile.SearchPage" ValidateRequest="false" %>
<%@ Import Namespace="BlogEngine.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" Runat="Server">
  <div id="searchform" class="ui-body ui-body-a ui-corner-all">
    <h1 runat="server" id="h1Headline" />

    <input type="text" name="q" id="q" value="<%=Server.HtmlEncode(Request.QueryString["q"]) %>" onkeypress="if(event.keyCode==13) SearchPage()" />
    <input type="button" value="<%=Resources.labels.search %>" onclick="SearchPage()" onkeypress="SearchPage()" />
    <% if (BlogSettings.Instance.EnableCommentSearch && BlogSettings.Instance.ShowIncludeCommentsOption){ %>
      <input type="checkbox" name="comment" id="comment" class="custom" />
      <label for="comment"><%=BlogSettings.Instance.SearchCommentLabelText %></label>
    <%} %>
  </div>

    <script type="text/javascript">      
    //<![CDATA[ 
      var check = document.getElementById('comment');
      
      function SearchPage()
      {        
        var searchTerm = encodeURIComponent(document.getElementById('q').value);
        var include = check ? check.checked : false;
        var comment = '&comment=true';
        
        if (!include)
        {
          comment = ''
        }
        
        location.href = 'search.aspx?q=' + searchTerm + comment;
      }
      
      if (check != null)
      {
        check.checked = <%=(Request.QueryString["comment"] != null).ToString().ToLowerInvariant() %>;
      }

      //]]>
    </script>
  
    <br />
    <br />

    <ol data-role="listview">
       <asp:repeater runat="server" id="rep">
        <ItemTemplate>
          <li>
            <a href="<%# Eval("RelativeLink") %>"><%# Eval("Title") %>
              <p>&nbsp;</p>
              <p><%# GetContent((string)Eval("Description"), (string)Eval("Content")) %></p>
              <p>&nbsp;</p>
              <p id="type" runat="server"  />
              <p><em><%# ShortenUrl((String)Eval("RelativeLink")) %></em></p>
            </a>
          </li>
        </ItemTemplate>
      </asp:repeater>
    </ol>
    
    <asp:PlaceHolder ID="Paging" runat="server" />
  </div>
</asp:Content>