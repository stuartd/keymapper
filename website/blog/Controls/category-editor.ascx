<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="category-editor.ascx.cs"
    Inherits="CategoryEditor" %>
<fieldset id="category-editor">
    <ol>
        <li>
            <asp:Label ID="lblCategoryName" runat="server" Text="Name" AssociatedControlID="txtCategoryName"></asp:Label>
            <asp:TextBox ID="txtCategoryName" runat="server" CssClass="input_textbox"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtCategoryName"
                ErrorMessage="The Category name can't be blank" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator runat="server" ID="CategoryNameExistsValidator" ErrorMessage="That category name is already used: please choose another"
                ControlToValidate="txtCategoryName" OnServerValidate="CategoryNameExistsValidator_ServerValidate"></asp:CustomValidator></li>
        <li>
            <asp:Label ID="lblCategorySlug" runat="server" Text="Slug" AssociatedControlID="txtCategorySlug"></asp:Label>
            <asp:TextBox ID="txtCategorySlug" runat="server" CssClass="input_textbox"></asp:TextBox></li>
        <li>The category slug will be generated automatically if left blank. It can only contain
            letters, numbers, and hyphens.</li>
    </ol>
    <input type="hidden" id="fldCategoryID" runat="server" />
    <asp:Button ID="btnSaveCategory" runat="server" Text="Save" OnClick="SaveCategory" />
</fieldset>
