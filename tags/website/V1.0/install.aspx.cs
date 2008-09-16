using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

public partial class KeyMapperInstall : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((KeyMapperMaster)Page.Master).SetTitle("Install Key Mapper");

        HtmlGenericControl body = (HtmlGenericControl)Page.Master.FindControl("masterbody");
        body.Attributes.Add("onload", "Initialize()");
    }
}
