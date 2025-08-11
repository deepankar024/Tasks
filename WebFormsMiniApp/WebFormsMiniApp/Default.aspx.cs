using System;
using System.Data;
using System.Web.UI;

namespace WebFormsMiniApp
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Email");

                dt.Rows.Add("Alice", "alice@example.com");
                dt.Rows.Add("Bob", "bob@example.com");

                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            lblRegResult.Text = "Registered: " + txtRegName.Text + " (" + txtRegEmail.Text + ")";
        }
    }
}
