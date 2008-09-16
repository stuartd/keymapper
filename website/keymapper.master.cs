using System;

public partial class KeyMapperMaster : System.Web.UI.MasterPage
{
    public void SetTitle(string title)
    {
        header.InnerText = title;
    }
}
