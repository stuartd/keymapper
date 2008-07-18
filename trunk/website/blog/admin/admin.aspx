<%@ Page Language="C#" MasterPageFile="~/KMBlog.Master" AutoEventWireup="true" CodeBehind="admin.aspx.cs" EnableEventValidation="false"
    Inherits="KMBlog.admin" Title="Key Mapper Blog Admin" %>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
<form id="pageform" runat="server">
    <h3 id="header">
        Key Mapper Blog Admin</h3>
        <ul class="topnav">
        <li><a href="post-edit.aspx">New Post</a></li>
        <li><a href="../default.aspx">View Blog</a></li>
        <li><a href="edit-categories.aspx">Categories</a></li>
        </ul><div class="clearfloats">   </div>
<div id ="maindiv">
     <asp:Repeater ID="postsRepeater" runat="server">
        <HeaderTemplate>
            <table border="0" cellpadding="0" cellspacing="0" width="40%" id="adminposts">
                <caption style="text-align: left">
                    <h4>Manage posts</h4></caption>
                    <tr><th>Post</th><th>Date</th><th>Status</th></tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <a href="post-edit.aspx?p=<%# DataBinder.Eval(Container.DataItem, "ID")%>">
                        <%# DataBinder.Eval(Container.DataItem, "Title") %></a>
                </td>
                <td>
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "Postdate")).ToLongDateString() %>
                </td>
                <td>
                    <%# (((int)DataBinder.Eval(Container.DataItem, "Published")) == 1 ? "Published" : "Draft") %>
                </td>
                <td>
                    <asp:Button runat="server" ID="DeletePost" Text="Delete" OnCommand="DeletePost" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID")%>' />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table></FooterTemplate>
    </asp:Repeater>
    </div>
    </form>
</asp:Content>
