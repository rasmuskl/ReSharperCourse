namespace Account
{
    using System;
    using System.Net.Mail;
    using System.Text;
    using System.Web.Security;

    using BlogEngine.Core;

    using Page = System.Web.UI.Page;

    /// <summary>
    /// The password retrieval.
    /// </summary>
    public partial class PasswordRetrieval : Page
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!BlogSettings.Instance.EnablePasswordReset)
            {
                Response.Redirect(Utils.RelativeWebRoot + "Account/login.aspx");
            }
        }

        /// <summary>
        /// Handles the Click event of the LoginButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void LoginButton_Click(object sender, EventArgs e)
        {
            var email = this.txtEmail.Text;

            if (string.IsNullOrEmpty(email))
            {
                this.Master.SetStatus("warning", "Email is required");
                return;
            }

            var userName = Membership.Provider.GetUserNameByEmail(email);

            if (string.IsNullOrEmpty(userName))
            {
                this.Master.SetStatus("warning", "Email does not exist in our system");
                return;
            }

            var pwd = Membership.Provider.ResetPassword(userName, string.Empty);

            if (!string.IsNullOrEmpty(pwd))
            {
                this.SendMail(email, userName, pwd);
            }
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="user">
        /// The user name.
        /// </param>
        /// <param name="pwd">
        /// The password.
        /// </param>
        private void SendMail(string email, string user, string pwd)
        {
            var mail = new MailMessage
                {
                    From = new MailAddress(BlogSettings.Instance.Email),
                    Subject = "Your password has been reset"
                };

            mail.To.Add(email);

            var sb = new StringBuilder();
            sb.Append("<div style=\"font: 11px verdana, arial\">");
            sb.AppendFormat("Dear {0}:", user);
            sb.AppendFormat("<br/><br/>Your password at \"{0}\" has been reset to: {1}", BlogSettings.Instance.Name, pwd);
            sb.Append(
                "<br/><br/>If it wasn't you who initiated the reset, please let us know immediately (use contact form on our site)");
            sb.AppendFormat("<br/><br/>Sincerely,<br/><br/><a href=\"{0}\">{1}</a> team.", Utils.AbsoluteWebRoot, BlogSettings.Instance.Name);
            sb.Append("</div>");

            mail.Body = sb.ToString();

            Utils.SendMailMessageAsync(mail);

            this.Master.SetStatus("success", "The password has been sent");
        }

        #endregion
    }
}