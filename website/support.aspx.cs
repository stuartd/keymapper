using System;
using System.Web.UI;

public partial class KeyMapperSupport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((KeyMapperMaster)Page.Master).SetTitle("Key Mapper Quick Start");
    }
}
