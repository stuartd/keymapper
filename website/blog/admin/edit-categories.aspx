<%@ Page Language="C#" MasterPageFile="../KMBlogAdmin.Master" AutoEventWireup="true" codefile="edit-categories.aspx.cs"
    Inherits="KMBlog.edit_categories" Title="Edit Categories" EnableEventValidation="false" %>

<%@ Import namespace="KMBlog" %>

<%@ Register TagPrefix="category" TagName="CategoryEditor" Src="../Controls/category-editor.ascx" %>

		<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
     <asp:Repeater ID="rptCategories" runat="server">
        <HeaderTemplate>
            <table border="0" cellpadding="0" cellspacing="0" width="30%" id="admincategories">
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
                    <Asp:Button runat="server" ID="DeleteCategory" Text="Delete" OnCommand="DeleteCategory"
                        CausesValidation="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID")%>'
                        Enabled="<%# KMAuthentication.IsUserAdmin(Page.User) %>" OnClientClick="return __doConfirm();" />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table></FooterTemplate>
    </asp:Repeater>
    <h4>
        Add A New Category</h4>
    <category:CategoryEditor ID="newcategory" runat="server" />
 
</asp:Content>
