<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RecaptchaLogViewer.aspx.cs"
    Inherits="Admin.Pages.RecaptchaLogViewer" EnableViewState="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Recaptcha Log</title>
    <style type="text/css">
        body
        {
            font: 11px verdana;
            margin: 0;
            overflow: hidden;
        }
        #title
        {
            background: #F1F1F1;
            border-bottom: 1px solid silver;
            padding: 10px;
        }
        label
        {
            font-weight: bold;
        }
        #phDetails
        {
            padding: 10px;
            height: 390px;
            overflow: auto;
            overflow-x: auto;
        }
        #bottom
        {
            background: #F1F1F1;
            border-top: 1px solid silver;
            padding: 10px;
            text-align: right;
        }
    </style>
    <script type="text/javascript">
        function ESCclose(evt) {
            if (!evt) evt = window.event;

            if (evt && evt.keyCode == 27) {
                closeRecaptchaLogViewer();
            }
        }
        function closeRecaptchaLogViewer() {
            var cDocument = parent.document;
            var cBody = cDocument.body;

            var iframe = cDocument.getElementById('RecaptchaLogDetails');
            var div = cDocument.getElementById('RecaptchaLogLayer');

            cBody.removeChild(iframe);
            cBody.removeChild(div);
            cBody.style.position = '';
            return false;
        }
    </script>
</head>
<body scroll="no" onkeypress="ESCclose(event)">
    <form id="form1" runat="server">
    <div id="title">
        <label>
            Recaptcha Log</label>
    </div>
    <div id="phDetails">
        <ul>
            <li><b>Attempts:</b> The number of tries before a successful captcha submission. This
                includes the successful attempt.</li>
            <li><b>Time to Post:</b> The time (in seconds) from the original page load to when the
                user has sucessfully posted a comment.</li>
            <li><b>Time to Solve:</b> The time to solve the captcha from either the original page
                load, or the last unsuccessful captcha attempt.</li>
        </ul>
        <asp:GridView runat="server" ID="RecaptchaLog" BorderColor="#f8f8f8" BorderStyle="solid"
            BorderWidth="1px" RowStyle-BorderWidth="0" RowStyle-BorderStyle="None" GridLines="None"
            Width="100%" AlternatingRowStyle-BackColor="#f8f8f8" AlternatingRowStyle-BorderColor="#f8f8f8"
            HeaderStyle-BackColor="#F1F1F1" CellPadding="3" ShowFooter="true" AutoGenerateColumns="False">
            <EmptyDataTemplate>
                There are no successful reCaptcha submissions to display.
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="24">
                    <ItemTemplate>
                        <%#Gravatar(DataBinder.Eval(Container.DataItem, "Email").ToString(), DataBinder.Eval(Container.DataItem, "Author").ToString())%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Date" HeaderText="<%$ Resources:labels, date %>" ItemStyle-HorizontalAlign="Center"
                    HtmlEncode="false" DataFormatString="{0:dd-MMM-yyyy HH:mm}" />
                <asp:TemplateField HeaderText="<%$ Resources:labels, author %>" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" NavigateUrl='<%# string.Format("mailto:{0}", DataBinder.Eval(Container.DataItem, "Email")) %>'
                            Text='<%#DataBinder.Eval(Container.DataItem, "Author")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:labels, website %>" ControlStyle-Width="200">
                    <ItemTemplate>
                        <asp:Label Text='<%# GetWebsite(DataBinder.Eval(Container.DataItem, "Website"))%>'
                            runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="RecaptchaAttempts" HeaderText="Attempts" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="CommentTime" HeaderText="Time to Post" HtmlEncode="false"
                    ItemStyle-HorizontalAlign="Center" DataFormatString="{0:N1}" />
                <asp:BoundField DataField="RecaptchaTime" HeaderText="Time to Solve" HtmlEncode="false"
                    ItemStyle-HorizontalAlign="Center" DataFormatString="{0:N1}" />
            </Columns>
        </asp:GridView>
    </div>
    <div id="bottom">
        <input type="button" value="Cancel" onclick="return closeRecaptchaLogViewer()" />
    </div>
    </form>
</body>
</html>
