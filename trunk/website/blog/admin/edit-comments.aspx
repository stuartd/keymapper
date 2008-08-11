<%@ Page Language="C#" MasterPageFile="../KMBlogAdmin.Master" AutoEventWireup="true"
    CodeFile="edit-comments.aspx.cs" Inherits="edit_comments" Title="Edit Comments"  EnableEventValidation="false" %>

<%@ Import Namespace="KMBlog" %>
<%@ Register TagPrefix="comment" TagName="CommentEditor" Src="../Controls/comment-editor.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <asp:Label>Comments on the post '<asp:Label runat="server" ID="postname">(postname)</asp:Label>'
        <asp:Repeater ID="comments" runat="server">
            <HeaderTemplate>
                <table border="0" cellpadding="0" cellspacing="0" width="60%" id="adminposts">
                    <tr>
                        <th>
                            Comment
                        </th>
                        <th>
                            Poster
                        </th>
                        <th>
                            Date
                        </th>
                        <th>
                            Status
                        </th>
                        <th>
                            Approve
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
                            <%# DataBinder.Eval(Container.DataItem, "Text") %></a>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "Name") %>
                    <td>
                        <%# ((DateTime)DataBinder.Eval(Container.DataItem, "Posted")).ToLongDateString() %>
                    </td>
                    <td>
                        <%# (((bool)DataBinder.Eval(Container.DataItem, "Approved")) ? "Approved" : "Not approved") %>
                    </td>
                    <td>
                    <asp:Button runat="server" ID="ApproveComment" Text="Delete" OnCommand="ApproveComment"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID")%>' Enabled='<%# ((bool)DataBinder.Eval(Container.DataItem, "Approved")  && KMAuthentication.IsUserAdmin(Page.User))%>' />
                    </td>
                    <td>
                        <asp:Button runat="server" ID="DeleteComment" Text="Delete" Enabled="<%# KMAuthentication.IsUserAdmin(Page.User) %>"
                            OnCommand="DeleteComment" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID")%>' />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table></FooterTemplate>
        </asp:Repeater>
</asp:Content>
