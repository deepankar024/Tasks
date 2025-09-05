using FeedbackFormWebApp.Services;
using FeedbackFormWebApp.Utils;
using System;
using System.Web.UI;

namespace FeedbackFormWebApp
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly UserRepository _userRepo = new UserRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            // If user is already logged in, redirect them
            if (AuthHelper.IsLoggedIn())
            {
                if (AuthHelper.IsAdmin())
                {
                    Response.Redirect("~/ViewFeedback.aspx");
                }
                else
                {
                    Response.Redirect("~/WebForm1.aspx");
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                var user = _userRepo.ValidateUser(txtEmail.Text.Trim(), txtPassword.Text);

                if (user != null)
                {
                    AuthHelper.SetCurrentUser(user);
                    AppLogger.Info($"User logged in successfully: {user.Email} ({user.Role})");

                    // Redirect based on role
                    if (user.Role == Models.UserRoles.Admin)
                    {
                        Response.Redirect("~/ViewFeedback.aspx");
                    }
                    else
                    {
                        Response.Redirect("~/WebForm1.aspx");
                    }
                }
                else
                {
                    ShowError("Invalid email or password.");
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Error during login", ex);
                ShowError("An error occurred during login. Please try again.");
            }
        }

        private void ShowError(string message)
        {
            litError.Text = $"<div class='alert alert-error'>{message}</div>";
        }
    }
}
