<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PostList.ascx.cs" EnableViewState="false" Inherits="UserControls.PostList" %>
<div runat="server" id="posts" class="posts" />

<div id="postPaging" style="display: none">
  <a runat="server" id="hlPrev" style="float:left">&lt;&lt; <%=Resources.labels.previousPosts %></a>
  <a runat="server" id="hlNext" style="float:right"><%=Resources.labels.nextPosts %> &gt;&gt;</a>
</div>

<div style="clear:both; display:block">
  <blog:PostPager ID="pager1" runat="server"></blog:PostPager>
</div>