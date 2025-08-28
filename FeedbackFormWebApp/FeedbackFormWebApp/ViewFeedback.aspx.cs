using FeedbackFormWebApp.Services;
using FeedbackFormWebApp.Utils;
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
            else if (!IsPostBack)  //first time
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
                //System.Diagnostics.Debug.WriteLine(currentPage);
                string sortField = ViewState["SortField"]?.ToString() ?? "SubmittedAt";
                string sortDirection = ViewState["SortDirection"]?.ToString() ?? "DESC";
                //foreach (string key in ViewState.Keys)
                //{
                //    var value = ViewState[key];
                //    System.Diagnostics.Debug.WriteLine($"{key}: {value}");
                //}
                //System.Diagnostics.Debug.WriteLine("hi");
                

                var pagedResult = _repo.GetPagedFeedback(currentPage, DefaultPageSize, sortField, sortDirection);

                GridView1.DataSource = pagedResult.Items; //Assign
                GridView1.DataBind();  //render

                // Update sort arrows after binding
                UpdateSortArrows();

                // Store pagination info in ViewState for custom pager
                ViewState["TotalRecords"] = pagedResult.TotalRecords;
                ViewState["CurrentPage"] = pagedResult.CurrentPage;
                ViewState["TotalPages"] = pagedResult.TotalPages;
                ViewState["HasPreviousPage"] = pagedResult.HasPreviousPage;
                ViewState["HasNextPage"] = pagedResult.HasNextPage;

                // Update pagination info
                litPaginationInfo.Text = GetPaginationInfo(pagedResult);

                // Build custom pager after data binding
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
            GridView1.PageIndex = e.NewPageIndex;
            BindGrid();
        }
        //headers LinkButton
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortField = e.SortExpression;
            string currentField = ViewState["SortField"]?.ToString();
            string currentDirection = ViewState["SortDirection"]?.ToString();

            // Toggle sort direction if same field, otherwise default to ASC ... to remember the current sorting state between postbacks
            string newDirection = "ASC";
            if (currentField == sortField && currentDirection == "ASC")
            {
                newDirection = "DESC";
            }

            ViewState["SortField"] = sortField;
            ViewState["SortDirection"] = newDirection;

            // Reset to first page when sorting
            GridView1.PageIndex = 0;
            BindGrid();
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

                // Update each column's sort arrow
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

        // Method to handle custom pagination
        protected void ChangePage(object sender, EventArgs e)
        {
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

        // Override to handle custom page events
        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            if (eventArgument != null && eventArgument.StartsWith("Page$"))
            {
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
    }
}