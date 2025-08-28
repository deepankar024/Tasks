using System;
using FeedbackFormWebApp.Controls;

namespace FeedbackFormWebApp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Event is already wired up through OnFeedbackSubmitted in markup
        }

        protected void FeedbackForm1_FeedbackSubmitted(object sender, FeedbackEventArgs e)
        {
            Session["LastFeedback"] = e;

            Utils.AppLogger.Info($"Feedback submitted - Name: {e.Name}, Email: {e.Email}, Category: {e.Category}");
        }
    }
}

//'e' parameter contains the data submitted from the form