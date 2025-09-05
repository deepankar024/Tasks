using FeedbackFormWebApp.Models;
using FeedbackFormWebApp.Services;
using FeedbackFormWebApp.Utils;
using System;
using System.Web.UI;

namespace FeedbackFormWebApp
{
    public partial class Register : System.Web.UI.Page
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

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                // Check if email already exists
                if (_userRepo.EmailExists(txtEmail.Text.Trim()))
                {
                    ShowError("An account with this email already exists.");
                    return;
                }

                // Create new user
                var user = new User
                {
                    FullName = txtFullName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PasswordHash = UserRepository.HashPassword(txtPassword.Text),
                    Role = ddlRole.SelectedValue
                };

                _userRepo.Add(user);

                AppLogger.Info($"New user registered: {user.Email} ({user.Role})");

                // Auto-login the user after registration
                AuthHelper.SetCurrentUser(user);

                // Redirect based on role
                if (user.Role == UserRoles.Admin)
                {
                    Response.Redirect("~/ViewFeedback.aspx");
                }
                else
                {
                    Response.Redirect("~/WebForm1.aspx");
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Error during user registration", ex);
                ShowError("An error occurred during registration. Please try again.");
            }
        }

        private void ShowError(string message)
        {
            litError.Text = $"<div class='alert alert-error'>{message}</div>";
        }
    }
}
