<%@ Control Language="C#" AutoEventWireup="true" CodeFile="comment-editor.ascx.cs"
    Inherits="KMBlog.CommentEditor" %>
<fieldset id="comment-editor">
    <ol>
        <li>
            <asp:Label ID="labelName" AssociatedControlID="txtName" runat="server" Text="Name" />
            <asp:TextBox ID="txtName" runat="server" />
        </li>
        <li>
            <asp:Label ID="labelWebsite" AssociatedControlID="txtURL" runat="server" Text="Website" />
            <asp:TextBox ID="txtURL" runat="server" Text="" />
        </li>
        </li>
    </ol>
    <asp:Label ID="labelText" AssociatedControlID="txtText" runat="server" Text="Text" />
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtText"
        ErrorMessage="Comment text must be entered" />
    <br />
    <asp:TextBox ID="txtText" runat="server" TextMode="MultiLine" CssClass="comment_text" />
    <br />
</fieldset>
