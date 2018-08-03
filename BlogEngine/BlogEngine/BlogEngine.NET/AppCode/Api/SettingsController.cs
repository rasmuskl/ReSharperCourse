﻿using App_Code;
using BlogEngine.Core;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.ViewModels;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;

public class SettingsController : ApiController
{
    readonly ISettingsRepository repository;

    public SettingsController(ISettingsRepository repository)
    {
        if (!WebUtils.CheckRightsForAdminSettingsPage(true))
            throw new HttpResponseException(HttpStatusCode.Unauthorized);

        this.repository = repository;
    }

    public SettingsVM Get()
    {
        return repository.Get();
    }

    public HttpResponseMessage Put([FromBody]Settings settings, string action = "")
    {
        if (action == "testEmail")
        {
            var retMsg = TestEmail(settings);
            if (!string.IsNullOrEmpty(retMsg))
            {
                return Request.CreateResponse(HttpStatusCode.OK, retMsg);
            }
        }
        else if(action == "clearCache")
        {
            BlogEngine.Core.Blog.CurrentInstance.Cache.Reset();
        }
        else
        {
            repository.Update(settings);
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    string TestEmail(Settings settings)
    {
        string email = settings.Email;
        string smtpServer = settings.SmtpServer;
        string smtpServerPort = settings.SmtpServerPort.ToString();
        string smtpUserName = settings.SmtpUserName;
        string smtpPassword = settings.SmtpPassword;
        string enableSsl = settings.EnableSsl.ToString();

        var mail = new MailMessage
        {
            From = new MailAddress(email, smtpUserName),
            Subject = $"Test mail from {smtpUserName}",
            IsBodyHtml = true
        };
        mail.To.Add(mail.From);
        var body = new StringBuilder();
        body.Append("<div style=\"font: 11px verdana, arial\">");
        body.Append("Success");
        if (HttpContext.Current != null)
        {
            body.Append(
                "<br /><br />_______________________________________________________________________________<br /><br />");
            body.AppendFormat("<strong>IP address:</strong> {0}<br />", Utils.GetClientIP());
            body.AppendFormat("<strong>User-agent:</strong> {0}", HttpContext.Current.Request.UserAgent);
        }

        body.Append("</div>");
        mail.Body = body.ToString();

        return Utils.SendMailMessage(mail, smtpServer, smtpServerPort, smtpUserName, smtpPassword, enableSsl.ToString());
    }
}
