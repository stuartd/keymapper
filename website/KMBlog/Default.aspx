<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="KMBlog._Default"
    MasterPageFile="~/KMBlog.Master" %>

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
                    <br />
                    Key Mapper<br />
                    C-Sharp<br />
                    Keyboards<br />
                </div>
                <div id="posts">
                    <asp:Repeater ID="postsRepeater" runat="server" DataSourceID="SQLBlogData" 
                        ondatabinding="postsRepeater_DataBinding">
                        <ItemTemplate>
                            <div class="subheader">
                                <span class="posttitle"><a href="#"><%# DataBinder.Eval(Container.DataItem, "Title") %></a></span> <span class="postdate">Posted: <%# DataBinder.Eval(Container.DataItem, "Postdate") %></span>
                            </div>
                            <div class="postbody">
                            <%# DataBinder.Eval(Container.DataItem, "Body") %>
                            </div>
                            <div class="postfooter">
                                <span class="categories">
                                </span>Categories: Key Mapper</span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:SqlDataSource ID="SQLBlogData" runat="server" ConnectionString="<%$ ConnectionStrings:blogConnectionStringWork %>"
                        SelectCommand="pr_posts_SelectAll" SelectCommandType="StoredProcedure">
                    </asp:SqlDataSource>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
