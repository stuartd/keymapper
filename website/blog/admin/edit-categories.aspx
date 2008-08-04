<%@ Page Language="C#" MasterPageFile="~/KMBlogAdmin.Master" AutoEventWireup="true" CodeBehind="edit-categories.aspx.cs"
    Inherits="KMBlog.edit_categories" Title="Edit Categories" EnableEventValidation="false" %>

<%@ Register TagPrefix="category" TagName="category_editor" Src="~/Controls/category-editor.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h1 id="header">
        Category Editor</h1>
    <asp:Repeater ID="rptCategories" runat="server">
        <HeaderTemplate>
            <table border="0" cellpadding="0" cellspacing="0" width="30%" id="admincategories">
                <caption style="text-align: left">
                    <h4>
                        Manage Categories</h4>
                </caption>
                <tr>
                    <th>
                        Edit Category
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
                        <%# DataBinder.Eval(Container.DataItem, "Name") %></a> </td>
                <td>
                    <asp:Button runat="server" ID="DeleteCategory" Text="Delete" OnCommand="DeleteCategory"
                        CausesValidation="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID")%>'
                        Enabled="<%# AppController.IsUserAdmin(Page.User) %>" />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table></FooterTemplate>
    </asp:Repeater>
    <h4>
        Add A New Category</h4>
    <category:category_editor ID="newcategory" runat="server" />
 
</asp:Content>
