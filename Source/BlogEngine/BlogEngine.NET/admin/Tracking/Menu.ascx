<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Menu.ascx.cs" Inherits="Admin.Tracking.Menu" %>
<ul>
    <li <%=Current("Pingbacks.aspx")%>><a href="Pingbacks.aspx"><%=Resources.labels.pingbacksAndTrackbacks %></a></li>
    <li <%=Current("referrers.aspx")%>><a href="referrers.aspx"><%=Resources.labels.referrers %></a></li>
</ul>