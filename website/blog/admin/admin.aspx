<%@ Page Language="C#" MasterPageFile="~/KMBlog.Master" 
AutoEventWireup="true" CodeBehind="admin.aspx.cs"
    Inherits="KMBlog.admin" Title="Key Mapper Blog Admin"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        @import url('../kmblog.css');
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

<h3 id="header">Key Mapper Blog Admin</h3>
<a class="buttonlink" href="post-edit.aspx">New post</a>
    <asp:Repeater ID="postsRepeater" runat="server">
    <HeaderTemplate>Posts</HeaderTemplate>
    <ItemTemplate>
    
    
    
    </ItemTemplate>
    </asp:Repeater>

</asp:Content>
