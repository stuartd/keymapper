<%@ Page Language="C#" MasterPageFile="~/KMBlog.Master" AutoEventWireup="true" CodeBehind="edit-categories.aspx.cs"
    Inherits="KMBlog.edit_categories" Title="Edit Categories" %>

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
                        Manage posts</h4>
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
                        <%# DataBinder.Eval(Container.DataItem, "Name") %></a> </td>
                <td>
                    <asp:Button runat="server" ID="DeleteCategory" Text="Delete" OnCommand="DeleteCategory"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID")%>' />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table></FooterTemplate>
    </asp:Repeater>
    <h2>Add New Category</h2>
    
    </form>
</asp:Content>
