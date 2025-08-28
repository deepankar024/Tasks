<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewFeedback.aspx.cs" Inherits="FeedbackFormWebApp.ViewFeedback" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <title>All Feedback</title>
    <link href="App_Themes/Default/StyleSheet.css" rel="stylesheet" type="text/css" />
    <style>
        /* Any page-specific overrides can go here if needed */
    </style>
    
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

        <div class="ff-wrapper view-feedback">
            <div class="ff-header">All Feedback</div>

            <asp:Literal ID="litError" runat="server" />

            <div class="pagination-info">
                <asp:Literal ID="litPaginationInfo" runat="server" />
            </div>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:GridView ID="GridView1" runat="server"
                        AllowPaging="true" PageSize="5"
                        AutoGenerateColumns="false"
                        CssClass="ff-grid"
                        OnPageIndexChanging="GridView1_PageIndexChanging"
                        OnSorting="GridView1_Sorting"
                        AllowSorting="true"
                        ShowFooter="false"
                        PagerSettings-Visible="false">
                        <Columns>
                            <asp:TemplateField HeaderText="ID" SortExpression="Id">
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lnkSortId" runat="server" CommandName="Sort" CommandArgument="Id" CssClass="sort-header">
                                        ID
                                        <asp:Literal ID="litArrowId" runat="server" />
                                    </asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("Id") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lnkSortName" runat="server" CommandName="Sort" CommandArgument="Name" CssClass="sort-header">
                                        Name
                                        <asp:Literal ID="litArrowName" runat="server" />
                                    </asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("Name") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Email" SortExpression="Email">
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lnkSortEmail" runat="server" CommandName="Sort" CommandArgument="Email" CssClass="sort-header">
                                        Email
                                        <asp:Literal ID="litArrowEmail" runat="server" />
                                    </asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("Email") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Category" SortExpression="Category">
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lnkSortCategory" runat="server" CommandName="Sort" CommandArgument="Category" CssClass="sort-header">
                                        Category
                                        <asp:Literal ID="litArrowCategory" runat="server" />
                                    </asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("Category") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Message" SortExpression="Message">
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lnkSortMessage" runat="server" CommandName="Sort" CommandArgument="Message" CssClass="sort-header">
                                        Message
                                        <asp:Literal ID="litArrowMessage" runat="server" />
                                    </asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="message-cell" title="<%# Eval("Message") %>">
                                        <%# Eval("Message") %>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Submitted On" SortExpression="SubmittedAt">
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lnkSortSubmittedAt" runat="server" CommandName="Sort" CommandArgument="SubmittedAt" CssClass="sort-header">
                                        Submitted On
                                        <asp:Literal ID="litArrowSubmittedAt" runat="server" />
                                    </asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("SubmittedAt", "{0:dd-MMM-yyyy HH:mm}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <HeaderStyle CssClass="ff-grid-header" />
                        <RowStyle CssClass="ff-grid-row" />
                        <AlternatingRowStyle CssClass="ff-grid-row-alt" />
                    </asp:GridView>

                    <!-- Custom Pager string builder ka use kiya-->
                    <asp:Literal ID="litCustomPager" runat="server" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="PageIndexChanging" />
                    <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="Sorting" />
                </Triggers>
            </asp:UpdatePanel>

            <div>
                <a href="WebForm1.aspx" class="back-link">← Back to form</a>
            </div>
        </div>
        <script type="text/javascript">
            function changePage(pageNumber) {
                __doPostBack('<%= GridView1.UniqueID %>', 'Page$' + pageNumber);
            }
        </script>

    </form>
</body>
</html>
<!--'__doPostBack' is a special ASP.NET function that submits the form.-->