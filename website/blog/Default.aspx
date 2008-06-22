<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="KMBlog._Default"
    MasterPageFile="~/KMBlog.Master" EnableViewState="false" Title="Key Mapper Developer Blog"%>

<asp:Content ID="default_head" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
    @import url('kmblog.css') ;
     </style>
</asp:Content>
<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <form id="form1" runat="server">
    <div id="maindiv">
        <h1 id="header"><a href="/">
            Key Mapper Developer Blog</a></h1>
        <div id="blog">
            <div id="sidebar">
                <div class="subheader">
                    Tags</div>
                <ul>
                    <asp:Repeater ID="categoriesRepeater" runat="server">
                        <ItemTemplate>
                            <li><a href="?c=<%# DataBinder.Eval(Container.DataItem, "ID")%>">
                                <%# DataBinder.Eval(Container.DataItem, "Name") %></a></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <div class="subheader">
                Admin
                </div>
                <a href="admin/admin.aspx">Log In</a>
            </div>
            <div id="posts">
                <asp:Repeater ID="postsRepeater" runat="server">
                    <ItemTemplate>
                        <div class="subheader">
                            <span class="posttitle"><a href="?p=<%# DataBinder.Eval(Container.DataItem, "ID")%>">
                                <%# DataBinder.Eval(Container.DataItem, "Title") %></a> </span><span class="postdate">
                                    Posted:
                                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "Postdate")).ToLongDateString() %></span>
                        </div>
                        <div class="postbody">
                            <%# DataBinder.Eval(Container.DataItem, "Body") %>
                        </div>
                        <div class="postfooter">
                            <span class="categories">Tagged:
                                <%# GetCategoriesForPost((int)DataBinder.Eval(Container.DataItem, "ID")) %>
                            </span><span class="commentslink">
                                <%# GetCommentLink((int)DataBinder.Eval(Container.DataItem, "ID"), (int)DataBinder.Eval(Container.DataItem, "commentCount")) %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <br />
                <asp:Repeater ID="commentsRepeater" runat="server">
                    <HeaderTemplate>
                        <asp:Label ID="commentsheader" runat="server">Comments:</asp:Label></HeaderTemplate>
                    <ItemTemplate>
                        <div class="comment" id="comments">
                                          <span class="commenthead"><span class="commenter">From: <%# DataBinder.Eval(Container.DataItem, "commenter_name")%></span>
                            <span class ="commenturl">URL/Email: 
                            <%# DataBinder.Eval(Container.DataItem, "commenter_url")%></span>
                            <span class="commentbody"><%# DataBinder.Eval(Container.DataItem, "comment")%></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    </form>
</asp:Content>
