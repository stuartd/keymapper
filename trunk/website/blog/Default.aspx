<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="KMBlog._Default"
    MasterPageFile="~/KMBlog.Master" EnableViewState="false" %>

<asp:Content ID="default_head" ContentPlaceHolderID="head" runat="server">
    <title>Key Mapper Developer Blog</title>
</asp:Content>
<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <form id="form1" runat="server">
    <div id="maindiv">
        <h1 id="header">
            Key Mapper Developer Blog</h1>
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
                            </span><span class="comments">
                                <%# GetCommentLink((int)DataBinder.Eval(Container.DataItem, "ID"), (int)DataBinder.Eval(Container.DataItem, "commentCount")) %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <br />
                <asp:Repeater ID="commentsRepeater" runat="server">
                    <HeaderTemplate>
                        Comments:</HeaderTemplate>
                    <ItemTemplate>
                        <div class="comment" id="comments">
                            Name:
                            <%# DataBinder.Eval(Container.DataItem, "commenter_name")%><br />
                            URL/Email:
                            <%# DataBinder.Eval(Container.DataItem, "commenter_url")%><br />
                            Comment:
                            <%# DataBinder.Eval(Container.DataItem, "comment")%><br />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    </form>
</asp:Content>
