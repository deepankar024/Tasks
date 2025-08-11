using System;
using System.Collections.Generic;
using System.Data;

namespace WebFormsMiniApp
{
    public partial class Submissions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Session["Contacts"] != null)
            {
                List<string> contacts = (List<string>)Session["Contacts"];
                DataTable dt = new DataTable();
                dt.Columns.Add("Submission");

                foreach (string entry in contacts)
                {
                    dt.Rows.Add(entry);
                }

                gvSubmissions.DataSource = dt;
                gvSubmissions.DataBind();
            }
        }
    }
}
