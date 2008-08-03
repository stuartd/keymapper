<%@ Page Language="C#" MasterPageFile="~/KMBlog.Master" AutoEventWireup="true" CodeBehind="edit-comments.aspx.cs"
    Inherits="KMBlog.edit_comments" Title="Untitled Page" %>

<%@ Register TagPrefix="comment" TagName="comment_editor" Src="~/Controls/comment-editor.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <comment:comment_editor ID="editcomment" runat="server" />
</asp:Content>
