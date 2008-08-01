<%@ Page Language="C#" MasterPageFile="~/KMBlog.Master" AutoEventWireup="true" CodeBehind="edit-categories.aspx.cs"
    Inherits="KMBlog.edit_categories" Title="Edit Categories" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h1 id="header">
        Category Editor</h1>
    <form runat="server" id="editcategories">
        <asp:Repeater ID="rptCategories" runat="server">
            <HeaderTemplate>
                <table border="0" cellpadding="0" cellspacing="0" width="30%" id="admincategories">
                    <caption style="text-align: left">
                        <h4>
                            Manage Categories</h4>
                    </caption>
                    <tr>
                        <th>
                            Category
                        </th>
                        <th>
                            Delete
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <a href="category-edit.aspx?c=<%# DataBinder.Eval(Container.DataItem, "ID")%>">
                            <%# DataBinder.Eval(Container.DataItem, "Name") %></a>
                    </td>
                    <td>
                        <asp:Button runat="server" ID="DeleteCategory" Text="Delete" OnCommand="DeleteCategory"
                            CausesValidation="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID")%>' />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table></FooterTemplate>
        </asp:Repeater>
        
        <h4>Add A New Category</h4>
        <fieldset id="newcategory">
            <ol>
                <li>
                    <asp:Label ID="lblCategoryName" runat="server" Text="Name" AssociatedControlID="txtCategoryName"></asp:Label>
                    <asp:TextBox ID="txtCategoryName" runat="server" CssClass="input_textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtCategoryName"
                        ErrorMessage="The Category name can't be blank"></asp:RequiredFieldValidator></li>
                <li>
                    <asp:Label ID="lblCategorySlug" runat="server" Text="Slug" AssociatedControlID="txtCategorySlug"></asp:Label>
                    <asp:TextBox ID="txtCategorySlug" runat="server" CssClass="input_textbox"></asp:TextBox></li>
            </ol>
            <asp:Button ID="btnSaveCategory" runat="server" Text="Save" OnClick="SaveCategory" />
        </fieldset>
    </form>
</asp:Content>
