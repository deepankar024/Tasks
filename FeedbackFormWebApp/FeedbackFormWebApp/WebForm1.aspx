<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="FeedbackFormWebApp.WebForm1" Theme="Default" %>
<%@ Register TagPrefix="cc1" Namespace="FeedbackFormWebApp.Controls" Assembly="FeedbackFormWebApp" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Feedback Form Demo</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        
        <!-- User Navigation Bar -->
        <div style="background: #f8fafc; padding: 12px 24px; border-bottom: 1px solid #e6e9ef; margin-bottom: 20px;">
            <div style="max-width: 700px; margin: 0 auto; display: flex; justify-content: space-between; align-items: center;">
                <span style="color: #374151; font-weight: 500;">
                    Welcome, <%= FeedbackFormWebApp.Utils.AuthHelper.GetCurrentUser()?.FullName %>
                </span>
                <div>
                    <a href="ViewFeedback.aspx" style="color: #2563eb; text-decoration: none; margin-right: 15px;">My Feedback</a>
                    <a href="Logout.aspx" style="color: #dc2626; text-decoration: none;">Logout</a>
                </div>
            </div>
        </div>

        <div style="padding: 18px;">
            <cc1:FeedbackFormControl ID="FeedbackForm1" runat="server"
                ShowHeader="true"
                HeaderText="We value your feedback!"
                EnableEmailValidation="true"
                EnableAjax="true"
                OnFeedbackSubmitted="FeedbackForm1_FeedbackSubmitted">
                <HeaderTemplate>
                    <div class="ff-template-header">
                        <h2>We'd love your feedback</h2>
                        <p>Short answers are fine — tell us what you liked or what we should improve.</p>
                    </div>
                </HeaderTemplate>
                <FooterTemplate>
                    <div class="ff-template-footer">
                        <p>Thanks — your response helps us improve the product and experience.</p>
                    </div>
                </FooterTemplate>
            </cc1:FeedbackFormControl>
        </div>
    </form>
</body>
</html>
