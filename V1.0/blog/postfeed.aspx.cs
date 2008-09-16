using System.ServiceModel.Syndication;
using System.Xml;
using System;

namespace KMBlog
{

	public partial class blog_postfeed : System.Web.UI.Page
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