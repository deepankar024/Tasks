using System;
using System.Collections.Generic;

namespace WebFormsMiniApp
{
    public partial class Contact : System.Web.UI.Page
    {
        protected void btnSend_Click(object sender, EventArgs e)
        {
            List<string> contacts = Session["Contacts"] as List<string>;
            if (contacts == null)
            {
                contacts = new List<string>();
            }

            string entry = txtContactName.Text + " - " + txtContactEmail.Text + " - " + txtContactMessage.Text;
            contacts.Add(entry);
            Session["Contacts"] = contacts;

            lblContactResult.Text = "Thank you for contacting us!";
        }
    }
}
