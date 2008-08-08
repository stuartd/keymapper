using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace KMBlog
{
    public partial class CommentEditor : System.Web.UI.UserControl
    {

        public bool IsValid()
        {
            return !String.IsNullOrEmpty(Text);
        }

        public string Name
        {
            get
            {
                return txtName.Text;
            }
            set
            {
                txtName.Text = value;
            }
        }

        public string URL
        {
            get
            {
                return txtURL.Text;
            }
            set
            {
                txtURL.Text = value;
            }
        }

        public string Text
        {
            get
            {
                return txtText.Text;
            }
            set
            {
                txtText.Text = value;
            }
        }

        public void ClearValues()
        {
            txtName.Text = String.Empty;
            txtText.Text = String.Empty;
            txtURL.Text = String.Empty;
        }
    }
}