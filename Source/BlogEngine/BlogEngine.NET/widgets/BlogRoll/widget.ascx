<%@ Import Namespace="BlogEngine.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="widget.ascx.cs" Inherits="Widgets.BlogRoll.Widget" %>
<blog:Blogroll ID="Blogroll1" runat="server" />
<a href="<%=Utils.AbsoluteWebRoot %>opml.axd" style="display: block; text-align: right"
    title="<%=Resources.labels.downloadOPML %>"><%=Resources.labels.downloadOPML %> <img src="<%=Utils.ApplicationRelativeWebRoot %>pics/opml.png" width="12" height="12"
        alt="OPML" /></a>