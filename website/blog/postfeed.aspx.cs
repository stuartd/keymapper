namespace KMBlog
{
    using System;
    using System.ServiceModel.Syndication;
    using System.Xml;

    public partial class BlogPostFeed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SyndicationFeed feed = Feeds.GetPostFeed();

            Response.Clear();
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/xml";

            using (XmlWriter rssWriter = XmlWriter.Create(Response.Output))
            {
                Rss20FeedFormatter rssFormatter = new Rss20FeedFormatter(feed);
                rssFormatter.WriteTo(rssWriter);
            }

            Response.End();
        }
    }
}