<%@ Page Language="C#" MasterPageFile="../KMBlogAdmin.Master" AutoEventWireup="true"
    CodeFile="edit-comments.aspx.cs" Inherits="edit_comments" Title="Untitled Page" %>

<%@ Register TagPrefix="comment" TagName="CommentEditor" Src="../Controls/comment-editor.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <asp:Repeater ID="comments" runat="server">
        <HeaderTemplate>
            <table border="0" cellpadding="0" cellspacing="0" width="60%" id="adminposts">
                <caption style="text-align: left">
                    <h4>
                        Comments on the post '<asp:Label runat="server" ID="postname"></asp:Label>'</h4>
                </caption>
                <tr>
                    <th>
                        Comment
                    </th>
                    <th>
                        Date
                    </th>
                    <th>
                        Status
                    </th>
                    <th>
                        Delete
                    </th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <a href="comment-edit.aspx?p=<%# DataBinder.Eval(Container.DataItem, "ID")%>">
                        <%# DataBinder.Eval(Container.DataItem, "Title") %></a>
                </td>
                <td>
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "Postdate")).ToLongDateString() %>
                </td>
                <td>
                    <%# (((bool)DataBinder.Eval(Container.DataItem, "Published")) ? "Published" : "Draft") %>
                </td>
                <td>
                    <asp:Button runat="server" ID="DeletePost" Text="Delete" Enabled="<%# AppController.IsUserAdmin(Page.User) %>"
                        OnCommand="DeleteComment" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID")%>' />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table></FooterTemplate>
    </asp:Repeater>
</asp:Content>
