<%@ Page Language="C#" MasterPageFile="~/KMBlog.Master" AutoEventWireup="true" CodeBehind="admin.aspx.cs"
    Inherits="KMBlog.admin" Title="Key Mapper Blog Admin" %>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
<form id="pageform" runat="server">
    <h3 id="header">
        Key Mapper Blog Admin</h3>
        <ul class="topnav">
        <li><a href="post-edit.aspx">New Post</a></li>
        <li><a href="../default.aspx">View Blog</a></li>
        <li><a href="edit-categories.aspx">Categories</a></li>
        </ul>
<div id ="maindiv">
     <asp:Repeater ID="postsRepeater" runat="server">
        <HeaderTemplate>
            <table border="0" cellpadding="0" cellspacing="0" width="40%">
                <caption style="text-align: left">
                    Manage posts</caption>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <th style="text-align: left">
                    <a href="post-edit.aspx?p=<%# DataBinder.Eval(Container.DataItem, "ID")%>">
                        <%# DataBinder.Eval(Container.DataItem, "Title") %></a>
                </th>
                <th style="text-align: left">
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "Postdate")).ToLongDateString() %>
                </th>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table></FooterTemplate>
    </asp:Repeater>
    </div>
    </form>
</asp:Content>
