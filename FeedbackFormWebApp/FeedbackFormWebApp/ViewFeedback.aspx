<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewFeedback.aspx.cs" Inherits="FeedbackFormWebApp.ViewFeedback" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <title>All Feedback</title>
    <link href="App_Themes/Default/StyleSheet.css" rel="stylesheet" type="text/css" />
    <style>
        /* Actions column specific styles */
        .actions-cell {
            white-space: nowrap;
            text-align: center;
        }
        
        .action-btn {
            display: inline-block;
            padding: 4px 8px;
            margin: 0 2px;
            border: 1px solid;
            border-radius: 4px;
            text-decoration: none;
            font-size: 12px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.2s ease;
            min-width: 50px;
        }
        
        .btn-edit {
            background: #f0f9ff;
            border-color: #0ea5e9;
            color: #0369a1;
        }
        
        .btn-edit:hover {
            background: #0ea5e9;
            color: white;
        }
        
        .btn-delete {
            background: #fef2f2;
            border-color: #ef4444;
            color: #dc2626;
        }
        
        .btn-delete:hover {
            background: #ef4444;
            color: white;
        }
        
        .btn-save {
            background: #f0fdf4;
            border-color: #22c55e;
            color: #16a34a;
        }
        
        .btn-save:hover {
            background: #22c55e;
            color: white;
        }
        
        .btn-cancel {
            background: #f8fafc;
            border-color: #64748b;
            color: #475569;
        }
        
        .btn-cancel:hover {
            background: #64748b;
            color: white;
        }
        
        .edit-input {
            width: 100%;
            padding: 4px 6px;
            border: 1px solid #d1d5db;
            border-radius: 4px;
            font-size: 12px;
        }
        
        .edit-input:focus {
            outline: none;
            border-color: #2563eb;
            box-shadow: 0 0 0 2px rgba(37,99,235,0.1);
        }
        
        .edit-textarea {
            resize: vertical;
            min-height: 60px;
        }
        
        .edit-select {
            width: 100%;
            padding: 4px 6px;
            border: 1px solid #d1d5db;
            border-radius: 4px;
            font-size: 12px;
        }
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
                        OnRowEditing="GridView1_RowEditing"
                        OnRowUpdating="GridView1_RowUpdating"
                        OnRowCancelingEdit="GridView1_RowCancelingEdit"
                        OnRowDeleting="GridView1_RowDeleting"
                        OnRowDataBound="GridView1_RowDataBound"
                        AllowSorting="true"
                        ShowFooter="false"
                        PagerSettings-Visible="false"
                        DataKeyNames="Id">
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
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditName" runat="server" Text='<%# Bind("Name") %>' CssClass="edit-input" MaxLength="100" />
                                    <asp:RequiredFieldValidator ID="rfvEditName" runat="server" ControlToValidate="txtEditName" 
                                        ErrorMessage="Name is required" Text="*" CssClass="ff-error" ValidationGroup="EditValidation" />
                                </EditItemTemplate>
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
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditEmail" runat="server" Text='<%# Bind("Email") %>' CssClass="edit-input" MaxLength="150" />
                                    <asp:RequiredFieldValidator ID="rfvEditEmail" runat="server" ControlToValidate="txtEditEmail" 
                                        ErrorMessage="Email is required" Text="*" CssClass="ff-error" ValidationGroup="EditValidation" />
                                    <asp:RegularExpressionValidator ID="revEditEmail" runat="server" ControlToValidate="txtEditEmail"
                                        ErrorMessage="Invalid email format" Text="*" CssClass="ff-error" ValidationGroup="EditValidation"
                                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" />
                                </EditItemTemplate>
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
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlEditCategory" runat="server" SelectedValue='<%# Bind("Category") %>' CssClass="edit-select">
                                        <asp:ListItem Value="Suggestion">Suggestion</asp:ListItem>
                                        <asp:ListItem Value="Bug">Bug Report</asp:ListItem>
                                        <asp:ListItem Value="Complaint">Complaint</asp:ListItem>
                                        <asp:ListItem Value="Other">Other</asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate>
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
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditMessage" runat="server" Text='<%# Bind("Message") %>' 
                                        TextMode="MultiLine" CssClass="edit-input edit-textarea" MaxLength="1000" />
                                    <asp:RequiredFieldValidator ID="rfvEditMessage" runat="server" ControlToValidate="txtEditMessage" 
                                        ErrorMessage="Message is required" Text="*" CssClass="ff-error" ValidationGroup="EditValidation" />
                                </EditItemTemplate>
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
                                <EditItemTemplate>
                                    <%# Eval("SubmittedAt", "{0:dd-MMM-yyyy HH:mm}") %>
                                </EditItemTemplate>
                            </asp:TemplateField>

                          
                            <asp:TemplateField HeaderText="Actions">
                                <HeaderStyle Width="120px" />
                                <ItemTemplate>
                                    <div class="actions-cell">
                                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CssClass="action-btn btn-edit" 
                                            Text="Edit" ToolTip="Edit this feedback" />
                                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="action-btn btn-delete" 
                                            Text="Delete" ToolTip="Delete this feedback" 
                                            OnClientClick="return confirm('Are you sure you want to delete this feedback?');" />
                                    </div>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <div class="actions-cell">
                                        <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" CssClass="action-btn btn-save" 
                                            Text="Save" ToolTip="Save changes" ValidationGroup="EditValidation" />
                                        <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" CssClass="action-btn btn-cancel" 
                                            Text="Cancel" ToolTip="Cancel editing" />
                                    </div>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <HeaderStyle CssClass="ff-grid-header" />
                        <RowStyle CssClass="ff-grid-row" />
                        <AlternatingRowStyle CssClass="ff-grid-row-alt" />
                        <EditRowStyle CssClass="ff-grid-row" BackColor="#fffbeb" />
                    </asp:GridView>

                 
                    <asp:Literal ID="litCustomPager" runat="server" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="PageIndexChanging" />
                    <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="Sorting" />
                    <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="RowEditing" />
                    <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="RowUpdating" />
                    <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="RowCancelingEdit" />
                    <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="RowDeleting" />
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