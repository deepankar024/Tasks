<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="WebFormsMiniApp.Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Contact Us</h3>
    Name: <asp:TextBox ID="txtContactName" runat="server" /><br /><br />
    Email: <asp:TextBox ID="txtContactEmail" runat="server" /><br /><br />
    Message: <asp:TextBox ID="txtContactMessage" runat="server" TextMode="MultiLine" Rows="4" Columns="30" /><br /><br />

    <asp:Button ID="btnSend" runat="server" Text="Send" OnClick="btnSend_Click" /><br /><br />
    <asp:Label ID="lblContactResult" runat="server" ForeColor="Green" />
</asp:Content>
