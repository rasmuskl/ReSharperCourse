<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.PostViewBase" %>

<div class="post xfolkentry" id="post<%=Index %>">
    <h1><a class="postheader taggedlink" href="<%=Post.RelativeOrAbsoluteLink %>"><%=Server.HtmlEncode(Post.Title) %></a></h1>
    <div class="descr"><img id="Img1" src="~/themes/indigo/img/timeicon.gif" runat="server" alt="clock" /> <%=Post.DateCreated.ToString("MMMM d, yyyy HH:mm")%> by <img id="Img2" src="~/themes/indigo/img/author.gif" runat="server" alt="author" /> <a href="<%=VirtualPathUtility.ToAbsolute("~/") + "author/" + BlogEngine.Core.Utils.RemoveIllegalCharacters(Post.Author) %>.aspx"><%=Post.AuthorProfile != null ? Post.AuthorProfile.DisplayName : Post.Author%></a></div>
    <div class="postcontent"><asp:PlaceHolder ID="BodyContent" runat="server" /></div>
    <%=Rating %>
    <br />
    <div class="postfooter">
        Tags: <%=TagLinks(", ") %><br />
        Categories: <%=CategoryLinks(" | ") %><br />
        Actions: <%=AdminLinks %>
        <a rel="nofollow" href="mailto:?subject=<%=Server.UrlEncode(Post.Title) %>&amp;body=Thought you might like this: <%=Post.AbsoluteLink.ToString() %>">E-mail</a> | 
        <a rel="nofollow" href="http://www.dotnetkicks.com/submit?url=<%=Server.UrlEncode(Post.AbsoluteLink.ToString()) %>&amp;title=<%=Server.UrlEncode(Post.Title) %>">Kick it!</a> |
        <a href="<%=Post.PermaLink %>" rel="bookmark">Permalink</a> |
        <a rel="nofollow" href="<%=Post.RelativeOrAbsoluteLink %>#comment">
             <img id="Img4" runat="server" alt="comment" src="~/themes/indigo/img/comments.gif" /><%=Resources.labels.comments %> (<%=Post.ApprovedComments.Count %>)</a>
         |   
        <a rel="nofollow" href="<%=CommentFeed %>"><asp:Image ID="Image1" runat="Server" ImageUrl="~/pics/rssButton.png" AlternateText="RSS comment feed" style="margin-right:3px" />Comment RSS</a>
    </div>
    <br />
</div>