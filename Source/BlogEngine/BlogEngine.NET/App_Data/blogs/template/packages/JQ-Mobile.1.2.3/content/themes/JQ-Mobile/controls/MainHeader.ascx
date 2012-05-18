<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MainHeader.ascx.cs" Inherits="MichaelJBaird.Themes.JQMobile.Controls.MainHeader" %>
<%@ Import Namespace="BlogEngine.Core" %>

<div data-role="header">
  <h1><%=BlogSettings.Instance.Name %> - <%= BlogSettings.Instance.Description %></h1>
  <a href="<%=Utils.RelativeWebRoot%>default.aspx" data-icon="home" data-iconpos="notext"><%= Resources.labels.home %></a>
  <div data-role="navbar" data-iconpos="top">
	  <ul>
      <li><a href="<%=Utils.RelativeWebRoot%>archive.aspx" data-icon="grid"><%= Resources.labels.archive %></a></li>
		  <li><a href="<%=Utils.RelativeWebRoot%>contact.aspx" id="contact" data-icon="custom"><%= Resources.labels.contact %></a></li>
		  <li><a href="<%=Utils.RelativeWebRoot%>search.aspx" data-icon="search"><%= Resources.labels.search %></a></li>
		  <li><a href="<%=Utils.FeedUrl%>" id="rss" data-icon="custom"><%= Resources.labels.feed %></a></li>
	  </ul>
  </div>
</div>