<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="FeedbackFormWebApp.Login" Theme="Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Login - Feedback Form</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="ff-wrapper" style="max-width: 450px;">
            <div class="ff-header">Login to Your Account</div>

            <asp:Literal ID="litError" runat="server" />

            <div class="ff-row">
                <div class="ff-label">Email</div>
                <div class="ff-input">
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" />
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                        ControlToValidate="txtEmail" 
                        ErrorMessage="Email is required." 
                        CssClass="ff-error" 
                        Display="Dynamic" />
                </div>
            </div>

            <div class="ff-row">
                <div class="ff-label">Password</div>
                <div class="ff-input">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" 
                        ControlToValidate="txtPassword" 
                        ErrorMessage="Password is required." 
                        CssClass="ff-error" 
                        Display="Dynamic" />
                </div>
            </div>

            <div class="ff-actions">
                <asp:Button ID="btnLogin" runat="server" 
                    Text="Login" 
                    CssClass="ff-button ff-button-primary" 
                    OnClick="btnLogin_Click" />
            </div>

            <div style="text-align: center; margin-top: 20px; padding-top: 20px; border-top: 1px solid #e6e9ef;">
                <p style="color: #6b7280;">Don't have an account?</p>
                <a href="Register.aspx" style="color: #2563eb; text-decoration: none; font-weight: 600;">Create Account</a>
            </div>
        </div>
    </form>
</body>
</html>
