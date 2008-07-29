<%@ Page Language="C#" MasterPageFile="~/KMBlog.Master" AutoEventWireup="true" CodeBehind="edit-categories.aspx.cs"
    Inherits="KMBlog.edit_categories" Title="Edit Categories" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
<h1 id="header">Category Editor</h1>
    <asp:Repeater ID="rptCategories" runat="server">
        <ItemTemplate>
            <div class="catlistitem">
                <a href="category-edit.aspx?c=<%# DataBinder.Eval(Container.DataItem, "ID")%>">
                    <%# DataBinder.Eval(Container.DataItem, "Name") %></a>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
