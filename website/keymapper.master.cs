using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

public partial class KeyMapperMaster : System.Web.UI.MasterPage
{

    public void SetTitle(string title)
    {
        header.InnerText = title;
    }


   
}
