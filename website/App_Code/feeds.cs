using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Collections.ObjectModel;
using System;


public class Feeds
{

	public static SyndicationFeed GetFeed()
	{
		SyndicationFeed feed = new SyndicationFeed();

		feed.Copyright = new TextSyndicationContent("Copyright (C) Stuart Dunkeld 2008. All rights reserved.");
		feed.Generator = "ASP.Net 3.5 Syndication classes";

		SyndicationLink link = new SyndicationLink();
		link.Title = "Key Mapper Home";
		link.Uri = new Uri("http://justkeepswimming.net/keymapper");
		feed.Links.Add(link);

		return feed;
	}

	public static SyndicationFeed GetPostFeed()
	{
		SyndicationFeed postfeed = Feeds.GetFeed();
		postfeed.Title = new TextSyndicationContent("Key Mapper Blog Post RSS Feed");
		postfeed.Description = new TextSyndicationContent("A feed for posts to the Key Mapper Developer Blog");

		Collection<Post> posts = Post.GetAllPosts();
		List<SyndicationItem> items = new List<SyndicationItem>();

		foreach (Post p in posts)
		{
			SyndicationItem item = new SyndicationItem();
			item.Id = Guid.NewGuid().ToString();
			item.Title = new TextSyndicationContent(p.Title);
			item.Content = new TextSyndicationContent(p.Body, TextSyndicationContentKind.Html);

			SyndicationLink itemlink = new SyndicationLink();
			itemlink.Title = "Key Mapper Blog";
			itemlink.Uri = new Uri("http://justkeepswimming.net/keymapper/default.aspx?p=" + p.ID.ToString() + ".aspx");
			item.Links.Add(itemlink);

			SyndicationPerson person = new SyndicationPerson();
			person.Name = "Stuart Dunkeld";
			person.Email = "keymappersupport@gmail.com";
			item.Authors.Add(person);

			items.Add(item);

		}

		postfeed.Items = items;
		return postfeed;

	}
}
