<%@ Page Language="C#" AutoEventWireup="true" codefile="default.aspx.cs" Inherits="KMBlog.DefaultPage"
    MasterPageFile="KMBlog.Master" EnableViewState="false" Title="Key Mapper Developer Blog" %>

<%@ Import Namespace="System.Collections.ObjectModel" %>
<%@ Register TagPrefix="comment_edit" TagName="CommentEditor" Src="Controls/comment-editor.ascx" %>
<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <ul class="topnav" id="sitenav">
        <li><a href="../default.aspx">Key Mapper</a></li>
        <li><a href="http://code.google.com/p/keymapper/source/browse/trunk/website/blog">Blog
            Source</a></li>
        <li><a href="../blog">Blog Home</a></li>
    </ul>
    <div class="clearfloats">   </div>
    <div id="blog">
        <div id="sidebar">
            <div class="subheader">
                Categories</div>
            <div class="sidebarcontent">
                <ul>
                    <asp:Repeater ID="categoriesRepeater" runat="server">
                        <ItemTemplate>
                            <li><a href="?c=<%# DataBinder.Eval(Container.DataItem, "ID")%>">
                                <%# DataBinder.Eval(Container.DataItem, "Name") %></a></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
            <div class="subheader">
                Admin
            </div>
            <div class="sidebarcontent">
                <asp:LoginView runat="server">
                    <AnonymousTemplate>
                        <a href="admin/admin.aspx">Login</a></AnonymousTemplate>
                    <LoggedInTemplate>
                        <asp:LoginName ID="LoginName1" runat="server" FormatString="Logged in as {0}" />
                        <br />
                        <a href="admin/admin.aspx">Blog Admin</a><br />
                        <asp:LoginStatus runat="server" />
                    </LoggedInTemplate>
                </asp:LoginView>
            </div>
        </div>
        <div id="posts">
            <asp:Repeater ID="postsRepeater" runat="server">
                <ItemTemplate>
                    <div class="post">
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
                            <span class="categories">Categories:
                                <%# GetCategoriesForPost((Collection<Category>)DataBinder.Eval(Container.DataItem, "Categories")) %>
                            </span><span class="commentslink">
                                <%# GetCommentLinkText((int)DataBinder.Eval(Container.DataItem, "ID"), (int)DataBinder.Eval(Container.DataItem, "commentCount")) %></span>
                            <br />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div id="comments" runat="server">
                <asp:Repeater ID="commentsRepeater" runat="server">
                    <HeaderTemplate>
                        <asp:Label ID="commentsheader" runat="server">Comments:</asp:Label></HeaderTemplate>
                    <ItemTemplate>
                        <div class="comment">
                            <div class="commenthead">
                                <span class="commenter">
                                    <%# GetCommentLink(DataBinder.Eval(Container.DataItem, "name").ToString(),
                                      DataBinder.Eval(Container.DataItem, "url").ToString())%></span> </span>
                            </div>
                            <div class="commentbody">
                                <%# DataBinder.Eval(Container.DataItem, "text")%>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                If you want to leave a comment, all fields are optional except the text.<br />
                <comment_edit:CommentEditor ID="editcomment" runat="server" />
                <asp:CheckBox ID="chkRememberDetails" runat="server" Text="Remember my details" />
                <asp:Button ID="btnSaveComment" runat="server" Text="Save" OnClick="SaveComment" />
                <asp:Button ID="btnCancelComment" runat="server" Text="Cancel" OnClick="CancelComment" />
            </div>
        </div>
    </div>
</asp:Content>
