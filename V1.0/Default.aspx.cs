using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

public partial class KeyMapperDefault : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
             ((KeyMapperMaster)Page.Master).SetTitle("Key Mapper");
    }
}
