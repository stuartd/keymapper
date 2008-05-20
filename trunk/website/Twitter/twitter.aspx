<%@ Page Language="C#" AutoEventWireup="true" CodeFile="twitter.aspx.cs" Inherits="Twitter_twitter" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <link href="http://justkeepswimming.net/Twitter/twitter.css" rel="stylesheet"
        type="text/css" />
    <title>Twitterer</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Repeater ID="tweets" runat="server">
         <ItemTemplate>
             <div><%# DataBinder.Eval(Container.DataItem, "Location") %> </div>
           
          </ItemTemplate>

        </asp:Repeater>
    </div>

    </form>
</body>
</html>
