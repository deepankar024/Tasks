using FeedbackFormWebApp.Utils;
using System;

namespace FeedbackFormWebApp
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Log the logout action
            var currentUser = AuthHelper.GetCurrentUser();
            if (currentUser != null)
            {
                AppLogger.Info($"User logged out: {currentUser.Email}");
            }

            // Clear session
            AuthHelper.ClearCurrentUser();

            // Redirect to login page
            Response.Redirect("~/Login.aspx");
        }
    }
}
