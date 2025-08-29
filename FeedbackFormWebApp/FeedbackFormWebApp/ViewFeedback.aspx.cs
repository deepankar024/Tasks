using FeedbackFormWebApp.Services;
using FeedbackFormWebApp.Utils;
using FeedbackFormWebApp.Models;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FeedbackFormWebApp
{
    public partial class ViewFeedback : System.Web.UI.Page
    {
        private readonly FeedbackRepository _repo = new FeedbackRepository();
        private const int DefaultPageSize = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            string eventArg = Request["__EVENTARGUMENT"];

            if (!string.IsNullOrEmpty(eventArg) && eventArg.StartsWith("Page$"))
            {
                int pageNumber = int.Parse(eventArg.Substring(5));
                GridView1.PageIndex = pageNumber - 1;
                BindGrid();
            }
            else if (!IsPostBack)
            {
                ViewState["SortField"] = "SubmittedAt";
                ViewState["SortDirection"] = "DESC";
                BindGrid();
            }
        }

        private void BindGrid()
        {
            try
            {
                int currentPage = GridView1.PageIndex + 1;
                string sortField = ViewState["SortField"]?.ToString() ?? "SubmittedAt";
                string sortDirection = ViewState["SortDirection"]?.ToString() ?? "DESC";

                var pagedResult = _repo.GetPagedFeedback(currentPage, DefaultPageSize, sortField, sortDirection);
                // 🚨 Optional: prevent EditIndex out of range
                if (GridView1.EditIndex >= pagedResult.Items.Count)
                {
                    GridView1.EditIndex = -1;
                }
                GridView1.DataSource = pagedResult.Items;
                GridView1.DataBind();

                UpdateSortArrows();

                ViewState["TotalRecords"] = pagedResult.TotalRecords;
                ViewState["CurrentPage"] = pagedResult.CurrentPage;
                ViewState["TotalPages"] = pagedResult.TotalPages;
                ViewState["HasPreviousPage"] = pagedResult.HasPreviousPage;
                ViewState["HasNextPage"] = pagedResult.HasNextPage;

                litPaginationInfo.Text = GetPaginationInfo(pagedResult);
                BuildCustomPager(pagedResult);
            }
            catch (Exception ex)
            {
                AppLogger.Error("Error loading feedback list", ex);
                ShowError("An error occurred while loading the feedback data.");
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.EditIndex = -1; // Cancel any editing when changing pages
            GridView1.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridView1.EditIndex = -1; // Cancel any editing when sorting

            string sortField = e.SortExpression;
            string currentField = ViewState["SortField"]?.ToString();
            string currentDirection = ViewState["SortDirection"]?.ToString();

            string newDirection = "ASC";
            if (currentField == sortField && currentDirection == "ASC")
            {
                newDirection = "DESC";
            }

            ViewState["SortField"] = sortField;
            ViewState["SortDirection"] = newDirection;

            GridView1.PageIndex = 0;
            BindGrid();
        }

        // NEW: Handle row editing
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.PageIndex = Convert.ToInt32(ViewState["CurrentPage"]) - 1;
            GridView1.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        // NEW: Handle row updating
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                // Validate the edit validation group
                Page.Validate("EditValidation");
                if (!Page.IsValid)
                    return;

                int feedbackId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
                GridViewRow row = GridView1.Rows[e.RowIndex];

                // Get the edited values
                var txtName = (TextBox)row.FindControl("txtEditName");
                var txtEmail = (TextBox)row.FindControl("txtEditEmail");
                var ddlCategory = (DropDownList)row.FindControl("ddlEditCategory");
                var txtMessage = (TextBox)row.FindControl("txtEditMessage");

                if (txtName != null && txtEmail != null && ddlCategory != null && txtMessage != null)
                {
                    // Get the existing feedback to preserve the original SubmittedAt
                    var existingFeedback = _repo.GetById(feedbackId);
                    if (existingFeedback != null)
                    {
                        existingFeedback.Name = txtName.Text.Trim();
                        existingFeedback.Email = txtEmail.Text.Trim();
                        existingFeedback.Category = ddlCategory.SelectedValue;
                        existingFeedback.Message = txtMessage.Text.Trim();

                        _repo.Update(existingFeedback);

                        AppLogger.Info($"Feedback updated successfully - ID: {feedbackId}, Email: {existingFeedback.Email}");
                        ShowSuccess("Feedback updated successfully!");
                    }
                }

                GridView1.EditIndex = -1;
                GridView1.PageIndex = Convert.ToInt32(ViewState["CurrentPage"]) - 1;
                BindGrid();
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Error updating feedback with ID: {GridView1.DataKeys[e.RowIndex].Value}", ex);
                ShowError("An error occurred while updating the feedback. Please try again.");
            }
        }

        // NEW: Handle row cancel editing
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GridView1.PageIndex = Convert.ToInt32(ViewState["CurrentPage"]) - 1;
            BindGrid();
        }

        // NEW: Handle row deleting
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int feedbackId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

                _repo.Delete(feedbackId);

                AppLogger.Info($"Feedback deleted successfully - ID: {feedbackId}");
                ShowSuccess("Feedback deleted successfully!");

                // If we're on the last page and it becomes empty, go to previous page
                int totalRecords = (int)(ViewState["TotalRecords"] ?? 0) - 1;
                int currentPage = GridView1.PageIndex + 1;
                int totalPages = (int)Math.Ceiling((double)totalRecords / DefaultPageSize);

                if (currentPage > totalPages && totalPages > 0)
                {
                    GridView1.PageIndex = totalPages - 1;
                }

                BindGrid();
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Error deleting feedback with ID: {GridView1.DataKeys[e.RowIndex].Value}", ex);
                ShowError("An error occurred while deleting the feedback. Please try again.");
            }
        }

        // NEW: Handle row data bound to set up edit controls
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && GridView1.EditIndex == e.Row.RowIndex)
            {
                // Row is in edit mode, you can perform additional setup here if needed
                var feedback = (Feedback)e.Row.DataItem;

                // Set the category dropdown value
                var ddlCategory = (DropDownList)e.Row.FindControl("ddlEditCategory");
                if (ddlCategory != null && feedback != null)
                {
                    ddlCategory.SelectedValue = feedback.Category;
                }
            }
        }

        protected string GetSortArrow(string columnName)
        {
            string currentField = ViewState["SortField"]?.ToString() ?? "SubmittedAt";
            string currentDirection = ViewState["SortDirection"]?.ToString() ?? "DESC";

            if (currentField.Equals(columnName, StringComparison.OrdinalIgnoreCase))
            {
                return currentDirection == "ASC" ?
                    "<i class='sort-arrow active'>▲</i>" :
                    "<i class='sort-arrow active'>▼</i>";
            }

            return "<i class='sort-arrow'>▼</i>";
        }

        private void UpdateSortArrows()
        {
            if (GridView1.HeaderRow != null)
            {
                string currentField = ViewState["SortField"]?.ToString() ?? "SubmittedAt";
                string currentDirection = ViewState["SortDirection"]?.ToString() ?? "DESC";

                UpdateColumnSortArrow("Id", currentField, currentDirection);
                UpdateColumnSortArrow("Name", currentField, currentDirection);
                UpdateColumnSortArrow("Email", currentField, currentDirection);
                UpdateColumnSortArrow("Category", currentField, currentDirection);
                UpdateColumnSortArrow("Message", currentField, currentDirection);
                UpdateColumnSortArrow("SubmittedAt", currentField, currentDirection);
            }
        }

        private void UpdateColumnSortArrow(string columnName, string currentField, string currentDirection)
        {
            var headerRow = GridView1.HeaderRow;
            if (headerRow != null)
            {
                var literal = headerRow.FindControl($"litArrow{columnName}") as Literal;
                if (literal != null)
                {
                    if (currentField.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        literal.Text = currentDirection == "ASC" ?
                            "<i class='sort-arrow active'>▲</i>" :
                            "<i class='sort-arrow active'>▼</i>";
                    }
                    else
                    {
                        literal.Text = "<i class='sort-arrow'>▼</i>";
                    }
                }
            }
        }

        private string GetPaginationInfo(PagedResult<Models.Feedback> result)
        {
            if (result.TotalRecords == 0)
                return "No feedback entries found.";

            int startRecord = (result.CurrentPage - 1) * result.PageSize + 1;
            int endRecord = Math.Min(result.CurrentPage * result.PageSize, result.TotalRecords);

            return $"Showing {startRecord} to {endRecord} of {result.TotalRecords} entries";
        }

        private void BuildCustomPager(PagedResult<Models.Feedback> result)
        {
            if (result.TotalPages <= 1)
            {
                litCustomPager.Text = "";
                return;
            }

            var pager = new StringBuilder();
            int currentPage = result.CurrentPage;
            int totalPages = result.TotalPages;

            pager.Append("<div class='ff-pager'>");

            // Previous button
            if (result.HasPreviousPage)
            {
                pager.AppendFormat("<a href=\"javascript:changePage({0})\" class=\"pager-btn\" title=\"Previous\">&laquo; Previous</a>",
                    currentPage - 1);
            }
            else
            {
                pager.Append("<span class=\"pager-btn disabled\">&laquo; Previous</span>");
            }

            // Page numbers
            int startPage = Math.Max(1, currentPage - 2);
            int endPage = Math.Min(totalPages, currentPage + 2);

            // First page and ellipsis
            if (startPage > 1)
            {
                pager.AppendFormat("<a href=\"javascript:changePage(1)\" class=\"pager-btn\">1</a>");
                if (startPage > 2)
                {
                    pager.Append("<span class=\"pager-btn disabled\">...</span>");
                }
            }

            // Page number links
            for (int i = startPage; i <= endPage; i++)
            {
                if (i == currentPage)
                {
                    pager.AppendFormat("<span class=\"pager-btn current-page\">{0}</span>", i);
                }
                else
                {
                    pager.AppendFormat("<a href=\"javascript:changePage({0})\" class=\"pager-btn\">{0}</a>", i);
                }
            }

            // Last page and ellipsis
            if (endPage < totalPages)
            {
                if (endPage < totalPages - 1)
                {
                    pager.Append("<span class=\"pager-btn disabled\">...</span>");
                }
                pager.AppendFormat("<a href=\"javascript:changePage({0})\" class=\"pager-btn\">{0}</a>", totalPages);
            }

            // Next button
            if (result.HasNextPage)
            {
                pager.AppendFormat("<a href=\"javascript:changePage({0})\" class=\"pager-btn\" title=\"Next\">Next &raquo;</a>",
                    currentPage + 1);
            }
            else
            {
                pager.Append("<span class=\"pager-btn disabled\">Next &raquo;</span>");
            }

            pager.Append("</div>");
            litCustomPager.Text = pager.ToString();
        }

        protected void ChangePage(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            if (Request.Form["__EVENTARGUMENT"] != null)
            {
                string arg = Request.Form["__EVENTARGUMENT"];
                if (arg.StartsWith("Page$"))
                {
                    int pageNumber = int.Parse(arg.Substring(5));
                    GridView1.PageIndex = pageNumber - 1;
                    BindGrid();
                }
            }
        }

        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            if (eventArgument != null && eventArgument.StartsWith("Page$"))
            {
                GridView1.EditIndex = -1;
                int pageNumber = int.Parse(eventArgument.Substring(5));
                GridView1.PageIndex = pageNumber - 1;
                BindGrid();
                return;
            }
            base.RaisePostBackEvent(sourceControl, eventArgument);
        }

        private void ShowError(string message)
        {
            litError.Text = $"<div class='alert alert-error'>{message}</div>";
        }

        private void ShowSuccess(string message)
        {
            litError.Text = $"<div class='alert alert-success'>{message}</div>";
        }
    }
}