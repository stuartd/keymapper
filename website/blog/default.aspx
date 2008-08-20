<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="KMBlog.DefaultPage"
    MasterPageFile="KMBlog.Master" EnableViewState="false" Title="Key Mapper Developer Blog" ValidateRequest="false"%>

<%@ Import Namespace="KMBlog" %>
<%@ Import Namespace="System.Collections.ObjectModel" %>
<%@ Register TagPrefix="comment_edit" TagName="CommentEditor" Src="Controls/comment-editor.ascx" %>
<asp:Content ID="rss" ContentPlaceHolderID="head" runat="server">
    <link rel="alternate" type="application/rss+xml" title="RSS" href="postfeed.aspx" />
</asp:Content>
<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <div id="blog">
        <div id="sidebar">
            <div class="subheader">
                Navigation</div>
            <div class="sidebarcontent">
                <ul>
                    <li><a href="../default.aspx">Key Mapper home</a></li>
                    <li><a href="default.aspx">Blog home</a></li></ul>
            </div>
            <div class="subheader">
                Categories</div>
            <div class="sidebarcontent">
                <ul>
                    <asp:Repeater ID="categoriesRepeater" runat="server">
                        <ItemTemplate>
                            <li><a href="<%# GetCategoryLink(DataBinder.Eval(Container.DataItem, "Slug").ToString())%>">
                                <%# DataBinder.Eval(Container.DataItem, "Name") %></a></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
            <div class="subheader">
                Archives</div>
            <div class="sidebarcontent">
                <div id="archivelist" runat="server">
                    <asp:Repeater runat="server" ID="yearlistrepeater">
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li>
                                <%# DataBinder.Eval(Container.DataItem, "year").ToString() %>
                            </li>
                            <asp:Repeater runat="server" ID="monthlistrepeater">
                                <HeaderTemplate>
                                    <ul>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li>
                                        <%# DataBinder.Eval(Container.DataItem, "month").ToString() %>
                                    </li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul></FooterTemplate>
                            </asp:Repeater>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul></FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <div class="subheader">
                Admin
            </div>
            <div class="sidebarcontent">
                <asp:LoginView runat="server" ID="loginView">
                    <AnonymousTemplate>
                        <ul>
                            <li><a href="/keymapper/blog/admin">Login</a></li>
                        </ul>
                    </AnonymousTemplate>
                    <LoggedInTemplate>
                        <ul>
                            <li>
                                <div class="nonanchorsidebarcontent">
                                    <asp:LoginName ID="LoginName1" runat="server" FormatString="Logged in as {0}" />
                                </div>
                            </li>
                            <li><a href="/keymapper/blog/admin">Blog Admin</a></li>
                            <li>
                                <asp:LoginStatus runat="server" />
                            </li>
                        </ul>
                    </LoggedInTemplate>
                </asp:LoginView>
            </div>
            <div class="subheader">
                Source Code</div>
            <div class="sidebarcontent">
                <ul>
                    <li><a href="http://code.google.com/p/keymapper/source/browse/trunk/website/">Website
                        source code</a></li>
                    <li><a href="http://code.google.com/p/keymapper/source/browse/trunk/website/blog">Blog
                        source code</a></li>
                    <li><a href="http://code.google.com/p/keymapper/source/browse/trunk/keymapper">Key Mapper
                        source code</a></li>
                </ul>
            </div>
        </div>
        <div id="posts">
            <asp:Repeater ID="postsRepeater" runat="server">
                <ItemTemplate>
                    <div class="post">
                        <div class="subheader">
                            <span class="posttitle"><a href="<%# GetPostLink(DataBinder.Eval(Container.DataItem, "Slug").ToString())%>">
                                <%# DataBinder.Eval(Container.DataItem, "Title") %></a> </span><span class="postdate">
                                    Posted:
                                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "Postdate")).ToLongDateString() %></span>
                        </div>
                        <div class="postbody">
                            <%# DataBinder.Eval(Container.DataItem, "Body") %>
                        </div>
                        <div class="postfooter">
                            <span>Categories:
                                <%# FormatPostCategories((Collection<Category>)DataBinder.Eval(Container.DataItem, "Categories")) %>
                            </span><span class="commentslink">
                                <%# GetCommentLinkText(DataBinder.Eval(Container.DataItem, "Slug").ToString(), (int)DataBinder.Eval(Container.DataItem, "commentCount")) %></span>
                            <br />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div id="comments">
                <div id="comments_inner" runat="server">
                    <asp:Label ID="commentsheader" runat="server">Comments:</asp:Label>
                    <asp:Repeater ID="commentsRepeater" runat="server">
                        <ItemTemplate>
                            <div class="comment">
                                <div class="commenthead">
                                    <span class="commenter">
                                        <%# GetCommentNameLink(DataBinder.Eval(Container.DataItem, "name").ToString(),
                                      DataBinder.Eval(Container.DataItem, "url").ToString())%></span> <span class="commentposted">
                                          <%#  ((DateTime)DataBinder.Eval(Container.DataItem, "Posted")).ToLongDateString() %></span>
                                </div>
                                <div class="commentbody">
                                    <%# DataBinder.Eval(Container.DataItem, "text")%>
                                </div>
                                <a href="" class="commentlink">Link to this comment</a>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    If you want to leave a comment, all fields are optional except the text.<br />
                    Comments are moderated, so won't show up immediately.
                    <comment_edit:CommentEditor ID="editcomment" runat="server" />
                    <asp:CheckBox ID="chkRememberDetails" runat="server" Text="Remember my details" Visible="false" />
                    <asp:Button ID="btnSaveComment" runat="server" Text="Save" OnClick="SaveComment" />
                    <asp:Button ID="btnCancelComment" runat="server" Text="Cancel" OnClick="CancelComment" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
