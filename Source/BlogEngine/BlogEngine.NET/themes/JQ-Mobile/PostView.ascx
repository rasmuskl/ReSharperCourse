<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="PostView.ascx.cs" Inherits="MichaelJBaird.Themes.JQMobile.PostView" %>
<%@ Import Namespace="BlogEngine.Core" %>
<%
  var postTitle = Location == ServingLocation.SinglePost ? Post.Title : string.Format("<a href=\"{0}\" class=\"taggedlink\">{1}</a>", Post.RelativeLink, Post.Title);
  var authorName = Post.AuthorProfile != null ? Post.AuthorProfile.DisplayName : Post.Author;
  var authorUrl = string.Format("<a href=\"{0}author/{1}.aspx\">{2}</a>", Utils.RelativeWebRoot, Utils.RemoveIllegalCharacters(Post.Author), authorName);
  
%>
<% if (Location == ServingLocation.PostList) { %>

  <li id="post<%=Index %>"><a class="postheader taggedlink" href="<%=Post.RelativeLink %>">
    <h3><%=Server.HtmlEncode(Post.Title) %></h3>
    <p>by <strong><%= authorName%></strong></p>
    <p><em><%=Post.DateCreated.ToShortDateString() %></em></p>
    <span class="ui-li-count"><%= Post.ApprovedComments.Count %></span>
    </a>
  </li>

<% } else { %>

  <div class="post xfolkentry" id="post<%=Index %>">
    <span class="author">by <%= authorUrl %></span>
    <span class="pubDate"><%=Post.DateCreated.ToShortDateString() %></span>
    <div class="entry"><asp:PlaceHolder ID="BodyContent" runat="server" /></div>

    <div class="postfooter">        
        Tags: <%=TagLinks(", ") %><br />
        Categories: <%=CategoryLinks(" | ") %><br />
    </div>
  </div>

<% } %>