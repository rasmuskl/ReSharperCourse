
namespace admin.Settings
{
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Services;
    using Resources;
    using System.Net.Mail;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;
    using App_Code;
    using Page = System.Web.UI.Page;

    public partial class Email : Page
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            BindSettings();

            Page.MaintainScrollPositionOnPostBack = true;
            Page.Title = labels.settings;
            base.OnInit(e);
        }

        /// <summary>
        /// The bind settings.
        /// </summary>
        private void BindSettings()
        {
            // -----------------------------------------------------------------------
            // Bind Email settings
            // -----------------------------------------------------------------------
            txtEmail.Text = BlogSettings.Instance.Email;
            txtSmtpServer.Text = BlogSettings.Instance.SmtpServer;
            txtSmtpServerPort.Text = BlogSettings.Instance.SmtpServerPort.ToString();
            txtSmtpUsername.Text = BlogSettings.Instance.SmtpUserName;
            txtSmtpPassword.Attributes.Add("value", BlogSettings.Instance.SmtpPassword);
            cbComments.Checked = BlogSettings.Instance.SendMailOnComment;
            cbEnableSsl.Checked = BlogSettings.Instance.EnableSsl;
            txtEmailSubjectPrefix.Text = BlogSettings.Instance.EmailSubjectPrefix;
        }
		
        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="email"></param>
        /// <param name="smtpServer"></param>
        /// <param name="smtpServerPort"></param>
        /// <param name="smtpUserName"></param>
        /// <param name="smtpPassword"></param>
        /// <param name="sendMailOnComment"></param>
        /// <param name="enableSsl"></param>
        /// <param name="emailSubjectPrefix"></param>
        /// <returns></returns>
        [WebMethod]
        public static JsonResponse Save(
			string email, 
			string smtpServer,
			string smtpServerPort,
			string smtpUserName,
			string smtpPassword,
			string sendMailOnComment,
			string enableSsl,
			string emailSubjectPrefix)
        {
            var response = new JsonResponse {Success = false};

            if (!WebUtils.CheckRightsForAdminSettingsPage(true))
            {
                response.Message = "Not authorized";
                return response;
            }

            try
            {
                BlogSettings.Instance.Email = email;
				BlogSettings.Instance.SmtpServer = smtpServer;
				BlogSettings.Instance.SmtpServerPort = int.Parse(smtpServerPort);
				BlogSettings.Instance.SmtpUserName = smtpUserName;
				BlogSettings.Instance.SmtpPassword = smtpPassword;
				BlogSettings.Instance.SendMailOnComment = bool.Parse(sendMailOnComment);
				BlogSettings.Instance.EnableSsl = bool.Parse(enableSsl);
				BlogSettings.Instance.EmailSubjectPrefix = emailSubjectPrefix;
			
                BlogSettings.Instance.Save();
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("admin.Settings.Email.Save(): {0}", ex.Message));
                response.Message = string.Format("Could not save settings: {0}", ex.Message);
                return response;
            }

            response.Success = true;
            response.Message = "Settings saved";
            return response;
        }

        [WebMethod]
        public static JsonResponse TestSmtp(
            string email,
            string smtpServer,
            string smtpServerPort,
            string smtpUserName,
            string smtpPassword,
            string sendMailOnComment,
            string enableSsl,
            string emailSubjectPrefix
            )
        {
            var response = new JsonResponse { Success = false };

            var errorMsg = new StringBuilder();

            if (!WebUtils.CheckRightsForAdminSettingsPage(true))
            {
                response.Message = "Not authorized";
                return response;
            }
            try
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(email, smtpUserName),
                    Subject = string.Format("Test mail from {0}", smtpUserName),
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
                    body.AppendFormat("<strong>IP address:</strong> {0}<br />", HttpContext.Current.Request.UserHostAddress);
                    body.AppendFormat("<strong>User-agent:</strong> {0}", HttpContext.Current.Request.UserAgent);
                }

                body.Append("</div>");
                mail.Body = body.ToString();

                string error = Utils.SendMailMessage(mail);
                if (!string.IsNullOrEmpty(error))
                    errorMsg.Append(error);
            }
            catch (Exception ex)
            {
                Exception current = ex;

                while (current != null)
                {
                    if (errorMsg.Length > 0) { errorMsg.Append(" "); }
                    errorMsg.Append(current.Message);
                    current = current.InnerException;
                }
            }

            if (errorMsg.Length > 0)
            {
                response.Message = string.Format("Error: {0}", errorMsg);
                return response;
            }

            response.Success = true;
            response.Message = "Test successful";
            return response;
        }
    }
}