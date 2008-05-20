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
using System.Net;
using System.IO;
using System.Collections.Generic;


public partial class Twitter_twitter : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Create the web request  
        HttpWebRequest request = WebRequest.Create
            ("http://twitter.com/statuses/public_timeline.xml") as HttpWebRequest;


        using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        {

            StreamReader reader = new StreamReader(response.GetResponseStream());

            // string twits = reader.ReadToEnd();

            XDocument xdoc = XDocument.Load(@"http://twitter.com/statuses/public_timeline.xml");

            IEnumerable<TwitterStatus> twits = from status in xdoc.Descendants("status")
                                               select new TwitterStatus
                                               {
                                                   Favorited = (status.Element("favorited").Value == "true" ? true : false),
                                                   statusID = status.Element("id").Value,
                                                   InReplyToStatusID = status.Element("in_reply_to_status_id").Value,
                                                   InReplyToUserID = status.Element("in_reply_to_user_id").Value,
                                                   Truncated = (status.Element("truncated").Value == "true" ? true : false),
                                                   Created = DateTime.ParseExact(status.Element("created_at").Value,
                                                       "ddd MMM dd HH:mm:ss zzz yyyy",
                                                       System.Globalization.CultureInfo.InvariantCulture),
                                                   Text = status.Element("text").Value,
                                                   Source = status.Element("source").Value,
                                                   User = new TwitterUser(status.Element("user").Element("id").Value,
                                                       status.Element("user").Element("name").Value,
                                                       status.Element("user").Element("screen_name").Value,
                                                       status.Element("user").Element("location").Value,
                                                       status.Element("user").Element("description").Value,
                                                       status.Element("user").Element("profile_image_url").Value,
                                                       status.Element("user").Element("url").Value,
                                                       System.Boolean.Parse(status.Element("user").Element("protected").Value),
                                                       System.Int32.Parse(status.Element("user").Element("followers_count").Value)
                                                       )
                                               };


            var datasource = from tweet in twits
                             select new
                             {
                                 tweet.Created,
                                 tweet.Favorited,
                                 tweet.statusID,
                                 tweet.InReplyToStatusID,
                                 tweet.InReplyToUserID,
                                 tweet.Source,
                                 tweet.Text,
                                 tweet.Truncated,
                                 tweet.User.Description,
                                 tweet.User.Followers,
                                 userID = tweet.User.UserID,
                                 tweet.User.Location,
                                 tweet.User.Name,
                                 ProfileUmageURL = tweet.User.ProfileImageURL,
                                 tweet.User.Protected,
                                 tweet.User.ScreenName,
                                 tweet.User.URL

                             };
            tweets.DataSource = datasource.ToList();
            tweets.DataBind();
        }

    }
}
public class TwitterStatus
{
    public DateTime Created { get; set; }
    public string statusID { get; set; }
    public string Text { get; set; }
    public string Source { get; set; }
    public bool Truncated { get; set; }
    public string InReplyToStatusID { get; set; }
    public string InReplyToUserID { get; set; }
    public bool Favorited { get; set; }
    public TwitterUser User { get; set; }
}

public class TwitterUser
{
    public TwitterUser(string twitterUserID, string userName, string userScreenName, string userLocation,
        string userDescription, string userProfileUmageURL, string userURL, bool userProtected, int userFollowers)
    {
        UserID = twitterUserID;
        Name = userName;
        ScreenName = userScreenName;
        Location = userLocation;
        Description = userDescription;
        ProfileImageURL = userProfileUmageURL;
        URL = userURL;
        Protected = userProtected;
        Followers = userFollowers;


    }

    public string UserID { get; set; }
    public string Name { get; set; }
    public string ScreenName { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public string ProfileImageURL { get; set; } 
    public string URL { get; set; }
    public bool Protected { get; set; }
    public int Followers { get; set; }
}