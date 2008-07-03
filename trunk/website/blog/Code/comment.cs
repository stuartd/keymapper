using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;


	public class Comment
	{
		public String Commenter { get; set; }
		public String CommenterURL { get; set; }
		public String CommenterEmail { get; set; }
		public String CommentBody { get; set; }
	}

