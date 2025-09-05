<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="FeedbackFormWebApp.Register" Theme="Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Register - Feedback Form</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="ff-wrapper" style="max-width: 450px;">
            <div class="ff-header">Create Your Account</div>

            <asp:Literal ID="litError" runat="server" />

            <div class="ff-row">
                <div class="ff-label">Full Name</div>
                <div class="ff-input">
                    <asp:TextBox ID="txtFullName" runat="server" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvFullName" runat="server" 
                        ControlToValidate="txtFullName" 
                        ErrorMessage="Full name is required." 
                        CssClass="ff-error" 
                        Display="Dynamic" />
                </div>
            </div>

            <div class="ff-row">
                <div class="ff-label">Email</div>
                <div class="ff-input">
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" MaxLength="150" />
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                        ControlToValidate="txtEmail" 
                        ErrorMessage="Email is required." 
                        CssClass="ff-error" 
                        Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                        ControlToValidate="txtEmail"
                        ErrorMessage="Please enter a valid email address." 
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
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
                    <asp:RegularExpressionValidator ID="revPassword" runat="server"
                        ControlToValidate="txtPassword"
                        ErrorMessage="Password must be at least 6 characters long."
                        ValidationExpression=".{6,}"
                        CssClass="ff-error"
                        Display="Dynamic" />
                </div>
            </div>

            <div class="ff-row">
                <div class="ff-label">Confirm Password</div>
                <div class="ff-input">
                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" />
                    <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" 
                        ControlToValidate="txtConfirmPassword" 
                        ErrorMessage="Please confirm your password." 
                        CssClass="ff-error" 
                        Display="Dynamic" />
                    <asp:CompareValidator ID="cvPasswords" runat="server"
                        ControlToValidate="txtConfirmPassword"
                        ControlToCompare="txtPassword"
                        ErrorMessage="Passwords do not match."
                        CssClass="ff-error"
                        Display="Dynamic" />
                </div>
            </div>

            <div class="ff-row">
                <div class="ff-label">Role</div>
                <div class="ff-input">
                    <asp:DropDownList ID="ddlRole" runat="server">
                        <asp:ListItem Value="User" Selected="True">User</asp:ListItem>
                        <asp:ListItem Value="Admin">Admin</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="ff-actions">
                <asp:Button ID="btnRegister" runat="server" 
                    Text="Create Account" 
                    CssClass="ff-button ff-button-primary" 
                    OnClick="btnRegister_Click" />
            </div>

            <div style="text-align: center; margin-top: 20px; padding-top: 20px; border-top: 1px solid #e6e9ef;">
                <p style="color: #6b7280;">Already have an account?</p>
                <a href="Login.aspx" style="color: #2563eb; text-decoration: none; font-weight: 600;">Sign In</a>
            </div>
        </div>
    </form>
</body>
</html>
