<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="KMBlog._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link href="kmblog.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div id="maindiv">
        <form id="form1" runat="server">
            <div>
                <h1 id="header">
                    Key Mapper Developer Blog</h1>
                <div id="posts">
                    <div class="post">
                        <div class="postheader">
                            <span class="posttitle">Post Title</span>
                            <span class="postdate">Post Date</span>
                        </div>
                        <div class="postbody">
                            Lores Ipsum, Sic Onim, Alea Jacta Est Etc
                        </div>
                        <div class="postfooter">
                        <span class="creator"></span>Created by Himself</span>
                        <span class="tags"></span>Categories: foo, bar, foobar</span>
                        </div>
                    </div>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
