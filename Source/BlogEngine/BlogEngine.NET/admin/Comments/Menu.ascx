<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Menu.ascx.cs" Inherits="Admin.Comments.Menu" %>
<%@ Import Namespace="BlogEngine.Core" %>
<ul>
    <% if(BlogSettings.Instance.EnableCommentsModeration){ %>
    <li <%=Current("Pending.aspx")%>><a href="Pending.aspx"><%=Resources.labels.pending %> (<span id="pending_counter"><%=PendingCount%></span>)</a></li>
    <% } %>
    <li <%=Current("Approved.aspx")%>><a href="Approved.aspx"><%=Resources.labels.approved %> (<span id="comment_counter"><%=CommentCount%></span>)</a></li>
    <li <%=Current("Spam.aspx")%>><a href="Spam.aspx"><%=Resources.labels.spam %> (<span id="spam_counter"><%=SpamCount%></span>)</a></li>
</ul>