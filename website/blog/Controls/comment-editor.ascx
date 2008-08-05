<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="comment-editor.ascx.cs"
    Inherits="KMBlog.CommentEditor" %>
<div id="comment_editor">
    <asp:TextBox ID="txtName" runat="server" />
    <asp:Label ID="labelName" AssociatedControlID="txtName" runat="server" Text="Name" />
    <br />
    <asp:TextBox ID="txtURL" runat="server" Text="" />
    <asp:Label ID="labelWebsite" AssociatedControlID="txtURL" runat="server" Text="Website" />
    <br />
    <asp:Label ID="labelText" AssociatedControlID="txtText" runat="server" Text="Text" />
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtText"
        ErrorMessage="Comment text must be entered" />
    <br />
    <asp:TextBox ID="txtText" runat="server" TextMode="MultiLine" CssClass="comment_text" />
    <br />
</div>
