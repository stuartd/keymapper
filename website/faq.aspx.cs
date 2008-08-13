using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

public partial class faq : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        ((KeyMapperMaster)Page.Master).SetTitle("Key Mapper FAQ");

        SetContentsLabelText();

        HtmlGenericControl body = (HtmlGenericControl)Page.Master.FindControl("masterbody");
        // body.Attributes.Add("onload", "load()");

        LoadTOC();
    }

    private void LoadTOC()
    {
        // All controls belong to the master page: we need to recurse in until we find the questions,
        // then load them into the TOC
        RecurseControls(Page.Master.Controls);
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
                    string href = "#" + id;
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    HtmlGenericControl anchor = new HtmlGenericControl("a");
                    anchor.Attributes.Add("href", href);
                    anchor.InnerText = hgc.InnerText;
                    li.Controls.Add(anchor);
                    toc.Controls.Add(li);

                    i++;

                    // Add 'Back to top' link
                    hgc.InnerHtml += " (<a href=" + (char)34 + "#faqtop" + (char)34 + ">back to top</a>)";


                }
            }

            RecurseControls(con.Controls);

        }
    }

    protected void SetContentsLabelText()
    {
        bool hidden = false; // GetContentsStatusFromCookie();

        string action = (hidden ? "Show" : "Hide");

        string text = "Contents (<a href='#' id='showhidetocanchor' onclick=" +
          (char)34 + "return ShowHidetoc(true)" + (char)34 +
          ">" + action + "</a>)";

        contents.InnerHtml = text;
    }

    protected bool GetContentsStatusFromCookie()
    {

        HttpCookie kmcookie = Request.Cookies["showfaqtoc"];
        if (kmcookie != null)
        {
            string cookieValue = kmcookie.Value;
    
            if (String.IsNullOrEmpty(cookieValue))
                return false;
            else
            {
                bool value;
                if (Boolean.TryParse(cookieValue, out value))
                    return value;
                else
                    return false;
            }
        }

        return false;

    }


}
