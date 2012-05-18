<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.PostViewBase" %>

<div class="post xfolkentry" id="post<%=Index %>">
    <h2><a class="postheader taggedlink" href="<%=Post.RelativeOrAbsoluteLink %>"><%=Server.HtmlEncode(Post.Title) %></a></h2>
    <span class="author">by <a href="<%=VirtualPathUtility.ToAbsolute("~/") + "author/" + BlogEngine.Core.Utils.RemoveIllegalCharacters(Post.Author) %>.aspx"><%=Post.AuthorProfile != null ? Post.AuthorProfile.DisplayName : Post.Author%></a></span>
    <span class="pubDate"><%=Post.DateCreated.ToShortDateString() %></span>
    <div class="entry"><asp:PlaceHolder ID="BodyContent" runat="server" /></div>

    <div class="postfooter">        
        Tags: <%=TagLinks(", ") %><br />
        Categories: <%=CategoryLinks(" | ") %><br />
    </div>
    
    <p>| <a href="#top">To the top </a> |</p>
</div>