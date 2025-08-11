<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebFormsMiniApp._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>User List (GridView)</h3>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="true" />
    <hr />
    <h3>Registration Form</h3>

    Name: <asp:TextBox ID="txtRegName" runat="server" />
    <asp:RequiredFieldValidator ID="rfvName" runat="server"
        ControlToValidate="txtRegName" ErrorMessage="Name is required" ForeColor="Red" />
    <br /><br />

    Email: <asp:TextBox ID="txtRegEmail" runat="server" />
    <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
        ControlToValidate="txtRegEmail" ErrorMessage="Email is required" ForeColor="Red" />
    <asp:RegularExpressionValidator ID="revEmail" runat="server"
        ControlToValidate="txtRegEmail" ValidationExpression="^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$"
        ErrorMessage="Invalid Email Format" ForeColor="Red" />
    <br /><br />

    <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" />
    <asp:Label ID="lblRegResult" runat="server" ForeColor="Green" />
</asp:Content>
