using System;
using FeedbackFormWebApp.Controls;
using FeedbackFormWebApp.Utils;

namespace FeedbackFormWebApp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect to login if not authenticated
            AuthHelper.RedirectIfNotLoggedIn();

            // Display user info
            var currentUser = AuthHelper.GetCurrentUser();
            if (currentUser != null && !IsPostBack)
            {
                // You can display welcome message or user info here
                Page.Title = $"Feedback Form - Welcome {currentUser.FullName}";
            }
        }

        protected void FeedbackForm1_FeedbackSubmitted(object sender, FeedbackEventArgs e)
        {
            Session["LastFeedback"] = e;
            Utils.AppLogger.Info($"Feedback submitted - Name: {e.Name}, Email: {e.Email}, Category: {e.Category}");
        }
    }
}
