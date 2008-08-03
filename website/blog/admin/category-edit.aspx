<%@ Page Language="C#" MasterPageFile="~/KMBlog.Master" AutoEventWireup="true" CodeBehind="category-edit.aspx.cs"
    Inherits="KMBlog.category_edit" Title="Edit A Category" %>

<%@ Register TagPrefix="category" TagName="category_editor" Src="~/Controls/category-editor.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h4>
        Edit Category</h4>
    <asp:Label runat="server" ID="lblCategoryDoesNotExist"></asp:Label>
    <category:category_editor ID="editcategory" runat="server" />
</asp:Content>
