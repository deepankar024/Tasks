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
            <div style="margin-top: 16px; text-align: right">
                <a href="ViewFeedback.aspx" style="text-decoration: none;">View all feedback →</a>
            </div>
        </div>
    </form>
</body>
</html>