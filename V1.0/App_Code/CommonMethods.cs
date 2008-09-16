using System;
using System.Text.RegularExpressions;


namespace KMBlog
{

	public class CommonMethods
	{
		public static string GetSlug(string seed)
		{
			// Rewrite spaces and full stops as hyphens, strip out everything else non-alpha
			// and remove consecutive hyphens
			string slug = Regex.Replace(Regex.Replace(seed, @"[^\w\-]", "-").ToLower(), @"(-)(\1)+", "$1");

			// If the last character is a hyphen, remove it:
			if (slug.EndsWith("-"))
				return slug.Substring(0, slug.Length - 1);
			else
				return slug;

		}
	}

}