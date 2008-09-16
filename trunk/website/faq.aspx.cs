using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

public partial class Faq : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((KeyMapperMaster)Page.Master).SetTitle("Key Mapper FAQ");
        this.LoadTOC();
    }

    private void LoadTOC()
    {
        // All controls belong to the master page: we need to recurse in until we find the questions,
        // then load them into the TOC
        this.RecurseControls(Page.Master.Controls);
    }

    private void RecurseControls(ControlCollection c)
    {
        // Only one of the passed controls will contain all the questions.
        int i = 0;
        foreach (Control con in c)
        {
            HtmlGenericControl hgc = con as HtmlGenericControl;
            if (hgc != null && hgc.TagName.ToUpperInvariant() == "H3")
            {
                if (hgc.Attributes["class"] == "question")
                {
                    string id = "q" + i.ToString();
                    hgc.Attributes.Add("id", id);

                    // Add to TOC
                    HtmlGenericControl tocItem = new HtmlGenericControl("li");
                    HtmlGenericControl anchor = new HtmlGenericControl("a");
                    anchor.Attributes.Add("href", "#" + id);
                    anchor.InnerText = hgc.InnerText;
                    tocItem.Controls.Add(anchor);
                    toc.Controls.Add(tocItem);

                    i++;
                }
            }

            this.RecurseControls(con.Controls);
        }
    }

    private bool GetContentsStatusFromCookie()
    {
        HttpCookie kmcookie = Request.Cookies["showfaqtoc"];
        if (kmcookie != null)
        {
            string cookieValue = kmcookie.Value;

            if (String.IsNullOrEmpty(cookieValue))
            {
                return false;
            }
            else
            {
                bool value;
                if (Boolean.TryParse(cookieValue, out value))
                {
                    return value;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }
}
