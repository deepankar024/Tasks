<%@ Page Title="Submissions" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Submissions.aspx.cs" Inherits="WebFormsMiniApp.Submissions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Submitted Messages</h3>
    <asp:GridView ID="gvSubmissions" runat="server" />
</asp:Content>
